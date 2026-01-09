////@AiCode
//#if GENERATEDCODE_ON
//namespace SupportChat.Logic.DataContext.App
//{
//    using TEntity = Entities.App.ChatMessage;
//    using System.Net.Http;
//    using System.Text.Json;
//    using Microsoft.EntityFrameworkCore;

//    /// <summary>
//    /// Partial class for ChatMessageSet with n8n webhook functionality.
//    /// Sends notifications to n8n when ChatMessages are added, modified, or deleted.
//    /// </summary>
//    internal sealed partial class ChatMessageSet
//    {
//        /// <summary>
//        /// Called after saving changes to the database.
//        /// Sends webhook notification to n8n for ChatMessage changes.
//        /// </summary>
//        /// <param name="changedEntries">A read-only list of entity entries that have been saved.</param>
//        internal override void AfterSaveChanges(IReadOnlyList<ChangedEntry> changedEntries)
//        {
//            Task.Run(async () =>
//            {
//                await SendWebhookNotificationAsync(Context, changedEntries).ConfigureAwait(false);
//            }).Wait();
//        }

//        /// <summary>
//        /// Sends webhook notification to n8n after saving ChatMessage entries.
//        /// </summary>
//        /// <param name="context">The database context.</param>
//        /// <param name="changedEntries">The list of changed entries.</param>
//        private static async Task SendWebhookNotificationAsync(ProjectDbContext context, IReadOnlyList<ChangedEntry> changedEntries)
//        {
//            // Filter for ChatMessage entities with relevant state changes
//            var relevantEntries = changedEntries
//                .Where(e => e.Entry.Entity is TEntity &&
//                           (e.State == EntityState.Added || 
//                            e.State == EntityState.Modified || 
//                            e.State == EntityState.Deleted))
//                .ToList();

//            if (!relevantEntries.Any())
//                return;

//            var appSettings = Common.Modules.Configuration.AppSettings.Instance;
//            //var webhookUrl = appSettings["n8n:WebHook:ChatMessageUrl"];
//            var webhookUrl = appSettings["n8n:WebHook:ChatMessageTestUrl"];


//            if (string.IsNullOrEmpty(webhookUrl))
//            {
//                Console.WriteLine("[n8n Webhook] ChatMessage webhook URL not configured in appsettings.");
//                return;
//            }

//            using var httpClient = new HttpClient();
//            httpClient.Timeout = TimeSpan.FromSeconds(10);

//            foreach (var changedEntry in relevantEntries)
//            {
//                if (changedEntry.Entry.Entity is TEntity entity)
//                {
//                    // ✅ WICHTIG: Nur User-Messages senden!
//                    if (!entity.IsUserMessage)
//                    {
//                        Console.WriteLine($"⏭️  [n8n Webhook] Skipping bot message (ID: {entity.Id})");
//                        continue;
//                    }

//                    try
//                    {
//                        // Load related SupportTicket information
//                        var supportTicket = await LoadSupportTicketByIdAsync(context, entity.SupportTicketId).ConfigureAwait(false);

//                        // Determine action based on entity state
//                        var action = changedEntry.State switch
//                        {
//                            EntityState.Added => "Added",
//                            EntityState.Modified => "Modified",
//                            EntityState.Deleted => "Deleted",
//                            _ => "Unknown"
//                        };

//                        // Create DTO payload with Action field
//                        // Only send current message + sessionId, n8n loads history from Postgres
//                        var payload = new ChatMessageWebhookDto
//                        {
//                            Action = action,
//                            Id = entity.Id,
//                            Content = entity.Content,
//                            IsUserMessage = entity.IsUserMessage,
//                            Timestamp = entity.Timestamp,
//                            SupportTicketId = entity.SupportTicketId,
//                            SessionId = $"ticket-{entity.SupportTicketId}", // For n8n Postgres Chat Memory
//                            TicketProblemDescription = supportTicket?.ProblemDescription ?? "N/A",
//                            TicketStatus = supportTicket?.Status.ToString() ?? "N/A",
//                            TicketPriority = supportTicket?.Priority.ToString() ?? "N/A",
//                            TicketIdentityId = supportTicket?.IdentityId ?? 0,
//                            SentAt = DateTime.UtcNow
//                        };

//                        var jsonContent = JsonSerializer.Serialize(payload, new JsonSerializerOptions
//                        {
//                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
//                        });

//                        var content = new StringContent(
//                            jsonContent,
//                            System.Text.Encoding.UTF8,
//                            "application/json");

//                        var response = await httpClient.PostAsync(webhookUrl, content).ConfigureAwait(false);
                        
//                        if (response.IsSuccessStatusCode)
//                        {
//                            // ✅ RESPONSE BODY LESEN
//                            var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                            
//                            Console.WriteLine($"✓ [n8n Webhook] ChatMessage webhook successfully sent");
//                            Console.WriteLine($"   Message ID: {entity.Id}");
//                            Console.WriteLine($"   User message: {entity.Content}");
//                            Console.WriteLine($"   Response: {responseBody}");

//                            // ✅ AI-ANTWORT PARSEN UND SPEICHERN
//                            try
//                            {
//                                using var jsonDoc = JsonDocument.Parse(responseBody);
//                                if (jsonDoc.RootElement.TryGetProperty("message", out var messageElement))
//                                {
//                                    var aiMessage = messageElement.GetString();
                                    
//                                    if (!string.IsNullOrWhiteSpace(aiMessage) && aiMessage != "OK")
//                                    {
//                                        var botMessage = new TEntity
//                                        {
//                                            Content = aiMessage,
//                                            IsUserMessage = false, // ← Bot-Antwort
//                                            Timestamp = DateTime.UtcNow,
//                                            SupportTicketId = entity.SupportTicketId
//                                        };

//                                        await context.ChatMessageSet.AddAsync(botMessage).ConfigureAwait(false);
//                                        await context.SaveChangesAsync().ConfigureAwait(false);

//                                        Console.WriteLine($"✓ [n8n Webhook] Bot message saved (ID: {botMessage.Id})");
//                                    }
//                                }
//                            }
//                            catch (JsonException jsonEx)
//                            {
//                                Console.WriteLine($"⚠️  [n8n Webhook] Failed to parse AI response: {jsonEx.Message}");
//                            }
//                        }
//                        else
//                        {
//                            var errorBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
//                            Console.WriteLine($"✗ [n8n Webhook] Failed with {response.StatusCode}");
//                            Console.WriteLine($"   Message ID: {entity.Id}");
//                            Console.WriteLine($"   Error: {errorBody}");
//                        }
//                    }
//                    catch (TaskCanceledException)
//                    {
//                        Console.WriteLine($"? [n8n Webhook] ChatMessage webhook timed out");
//                        Console.WriteLine($"   Message ID: {entity.Id}");
//                    }
//                    catch (Exception ex)
//                    {
//                        Console.WriteLine($"? [n8n Webhook] Error calling ChatMessage webhook:");
//                        Console.WriteLine($"   Error: {ex.Message}");
//                        Console.WriteLine($"   Message ID: {entity.Id}");
//                        if (ex.InnerException != null)
//                        {
//                            Console.WriteLine($"   Inner Exception: {ex.InnerException.Message}");
//                        }
//                    }
//                }
//            }
//        }

//        /// <summary>
//        /// Loads a SupportTicket by ID.
//        /// </summary>
//        /// <param name="context">The database context.</param>
//        /// <param name="supportTicketId">The SupportTicket ID.</param>
//        /// <returns>The SupportTicket or null if not found.</returns>
//        private static async Task<Entities.App.SupportTicket?> LoadSupportTicketByIdAsync(ProjectDbContext context, IdType supportTicketId)
//        {
//            var tickets = await context.SupportTicketSet.GetAllAsync().ConfigureAwait(false);
//            return tickets.FirstOrDefault(t => t.Id == supportTicketId);
//        }
//    }

//    /// <summary>
//    /// DTO for ChatMessage webhook payload sent to n8n.
//    /// n8n loads chat history from Postgres using the sessionId.
//    /// </summary>
//    internal class ChatMessageWebhookDto
//    {
//        /// <summary>
//        /// The action that triggered the webhook: Added, Modified, or Deleted.
//        /// </summary>
//        public string Action { get; set; } = string.Empty;

//        /// <summary>
//        /// The unique identifier of the chat message.
//        /// </summary>
//        public IdType Id { get; set; }

//        /// <summary>
//        /// The content of the chat message.
//        /// </summary>
//        public string Content { get; set; } = string.Empty;

//        /// <summary>
//        /// Indicates whether this message was sent by the user (true) or the chatbot (false).
//        /// </summary>
//        public bool IsUserMessage { get; set; }

//        /// <summary>
//        /// Timestamp when the message was sent.
//        /// </summary>
//        public DateTime Timestamp { get; set; }

//        /// <summary>
//        /// Foreign key reference to the parent SupportTicket.
//        /// </summary>
//        public IdType SupportTicketId { get; set; }

//        /// <summary>
//        /// Session ID for n8n Postgres Chat Memory (format: "ticket-{id}").
//        /// n8n uses this to load the complete chat history from the database.
//        /// </summary>
//        public string SessionId { get; set; } = string.Empty;

//        /// <summary>
//        /// Problem description from the related SupportTicket.
//        /// </summary>
//        public string TicketProblemDescription { get; set; } = string.Empty;

//        /// <summary>
//        /// Status of the related SupportTicket.
//        /// </summary>
//        public string TicketStatus { get; set; } = string.Empty;

//        /// <summary>
//        /// Priority of the related SupportTicket.
//        /// </summary>
//        public string TicketPriority { get; set; } = string.Empty;

//        /// <summary>
//        /// Identity ID from the related SupportTicket.
//        /// </summary>
//        public IdType TicketIdentityId { get; set; }

//        /// <summary>
//        /// Timestamp when the webhook was sent.
//        /// </summary>
//        public DateTime SentAt { get; set; }
//    }
//}
//#endif
