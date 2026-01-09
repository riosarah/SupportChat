#if GENERATEDCODE_ON
namespace SupportChat.WebApi.Controllers.App
{
    using TModel = SupportChat.WebApi.Models.App.ChatMessage;
    using TEntity = SupportChat.Logic.Entities.App.ChatMessage;
    using TContract = SupportChat.Common.Contracts.App.IChatMessage;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Text.Json;
    using System.Text;

    /// <summary>
    /// Custom ChatMessages controller with n8n webhook integration.
    /// </summary>
    public sealed partial class ChatMessagesController
    { 
        /// <summary>
        /// Creates a new chat message and triggers n8n workflow for AI processing.
        /// </summary>
        /// <param name="model">The chat message to create.</param>
        /// <returns>The created chat message.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public override async Task<ActionResult<TModel>> PostAsync([FromBody] TModel model)
        {
            try
            {
                // 1. Speichere User-Message in DB
                var entity = ToEntity(model, null);
                await EntitySet.AddAsync(entity);
                await Context.SaveChangesAsync();

                var createdMessage = ToModel(entity);

                // 2. Nur bei User-Messages: Trigger n8n Workflow
                if (entity.IsUserMessage)
                {
                    await TriggerN8nWorkflowAsync(entity);
                }

                return CreatedAtAction("Get", new { id = entity.Id }, createdMessage);
            }
            catch (Exception ex)
            {
                return BadRequest(GetErrorMessage(ex));
            }
        }

        /// <summary>
        /// Triggers the n8n workflow and processes the AI response.
        /// Returns the ChatResponse object with the bot's answer.
        /// </summary>
        /// <param name="userMessage">The user's chat message.</param>
        /// <returns>ChatResponse with bot's answer, or null if failed.</returns>
        private async Task<Logic.Entities.App.ChatResponse?> TriggerN8nWorkflowAsync(TEntity userMessage)
        {
            try
            {
                // Load SupportTicket
                var ticket = await Context.SupportTicketSet.GetByIdAsync(userMessage.SupportTicketId);
                if (ticket == null)
                {
                    Console.WriteLine($"⚠️  [n8n Workflow] Ticket {userMessage.SupportTicketId} not found");
                    return null;
                }

                // Webhook URL aus AppSettings
                var appSettings = Common.Modules.Configuration.AppSettings.Instance;
                var webhookUrl = appSettings["n8n:WebHook:ChatMessageTestUrl"];

                if (string.IsNullOrEmpty(webhookUrl))
                {
                    Console.WriteLine("⚠️  [n8n Workflow] Webhook URL not configured");
                    return null;
                }

                // Payload erstellen
                var payload = new
                {
                    action = "Added",
                    id = userMessage.Id,
                    content = userMessage.Content,
                    isUserMessage = userMessage.IsUserMessage,
                    timestamp = userMessage.Timestamp,
                    supportTicketId = userMessage.SupportTicketId,
                    sessionId = $"ticket-{userMessage.SupportTicketId}",
                    ticketProblemDescription = ticket.ProblemDescription,
                    ticketStatus = ticket.Status.ToString(),
                    ticketPriority = ticket.Priority.ToString(),
                    ticketIdentityId = ticket.IdentityId,
                    sentAt = DateTime.UtcNow
                };

                using var httpClient = new HttpClient();
                httpClient.Timeout = TimeSpan.FromSeconds(30);

                var jsonContent = JsonSerializer.Serialize(payload, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                Console.WriteLine($"📤 [n8n Workflow] Sending webhook for message {userMessage.Id}...");
                var response = await httpClient.PostAsync(webhookUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"✓ [n8n Workflow] Webhook successful");

                    // Parse AI Response
                    using var jsonDoc = JsonDocument.Parse(responseBody);
                    if (jsonDoc.RootElement.TryGetProperty("message", out var messageElement))
                    {
                        var aiResponse = messageElement.GetString();

                        if (!string.IsNullOrWhiteSpace(aiResponse) && aiResponse != "OK")
                        {
                            // Speichere Bot-Message
                            var botMessage = new TEntity
                            {
                                Content = aiResponse,
                                IsUserMessage = false,
                                Timestamp = DateTime.UtcNow,
                                SupportTicketId = userMessage.SupportTicketId
                            };

                            await Context.ChatMessageSet.AddAsync(botMessage);
                            await Context.SaveChangesAsync();

                            // Erstelle ChatResponse mit Metadaten
                            var chatResponse = new Logic.Entities.App.ChatResponse
                            {
                                ResponseContent = aiResponse,
                                ModelUsed = "gpt-4.1-mini",
                                PromptTokens = 0,
                                CompletionTokens = 0,
                                TotalTokens = 0,
                                ResponseTimeMs = 0,
                                IsSuccessful = true,
                                SessionId = userMessage.SupportTicketId,
                                CreatedAt = DateTime.UtcNow,
                                SupportTicketId = ticket.Id,
                                UserMessageId = userMessage.Id,
                                BotMessageId = botMessage.Id
                            };

                            await Context.ChatResponseSet.AddAsync(chatResponse);
                            await Context.SaveChangesAsync();

                            Console.WriteLine($"✓ [n8n Workflow] Bot message saved (ID: {botMessage.Id})");
                            Console.WriteLine($"✓ [n8n Workflow] ChatResponse created (ID: {chatResponse.Id})");
                            
                            return chatResponse; // Return ChatResponse to controller
                        }
                    }
                }
                else
                {
                    var errorBody = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"✗ [n8n Workflow] Failed: {response.StatusCode}");
                    Console.WriteLine($"   Error: {errorBody}");
                }
            }
            catch (TaskCanceledException)
            {
                Console.WriteLine($"✗ [n8n Workflow] Timeout");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ [n8n Workflow] Error: {ex.Message}");
            }
            
            return null; // Return null if failed
        }
    }
}
#endif