//@AiCode
#if GENERATEDCODE_ON
namespace SupportChat.WebApi.Controllers.App
{
    using Microsoft.AspNetCore.Mvc;
    using System.Net.Http;
    using System.Text;
    using System.Text.Json;
    using TModel = SupportChat.WebApi.Models.App.SupportTicket;
    using TEntity = SupportChat.Logic.Entities.App.SupportTicket;
    using TChatMessage = SupportChat.Logic.Entities.App.ChatMessage;
    using TChatResponse = SupportChat.Logic.Entities.App.ChatResponse;

    /// <summary>
    /// Custom SupportTickets controller with chat message handling.
    /// </summary>
    public sealed partial class SupportTicketsController
    {
        /// <summary>
        /// DTO for sending a message to a ticket and receiving bot response.
        /// </summary>
        public class SendMessageRequest
        {
            /// <summary>
            /// The user's message content.
            /// </summary>
            public string Content { get; set; } = string.Empty;

            /// <summary>
            /// Optional: System information for new tickets.
            /// </summary>
            public string? SystemInfo { get; set; }

            /// <summary>
            /// Optional: Priority for new tickets (Low=0, Medium=1, High=2, Critical=3).
            /// </summary>
            public int? Priority { get; set; }

            /// <summary>
            /// Optional: Problem description for new tickets.
            /// </summary>
            public string? ProblemDescription { get; set; }

            /// <summary>
            /// Required: Identity ID of the user.
            /// </summary>
            public IdType IdentityId { get; set; }
        }

        /// <summary>
        /// Sends a message to a ticket and returns the bot's response.
        /// Creates the ticket if it doesn't exist.
        /// </summary>
        /// <param name="ticketId">The ticket ID.</param>
        /// <param name="request">The message request.</param>
        /// <returns>The ChatResponse with bot's answer.</returns>
        [HttpPut("{ticketId:int}/messages")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Models.App.ChatResponse>> SendMessageAsync(
            [FromRoute] IdType ticketId,
            [FromBody] SendMessageRequest request)
        {
            try
            {
                // Validate request
                if (string.IsNullOrWhiteSpace(request.Content))
                {
                    return BadRequest("Message content cannot be empty.");
                }

                if (request.IdentityId <= 0)
                {
                    return BadRequest("Valid IdentityId is required.");
                }

                // 1. Check if ticket exists
                var ticket = await Context.SupportTicketSet.GetByIdAsync(ticketId);

                // 2. Create ticket if it doesn't exist
                if (ticket == null)
                {
                    Console.WriteLine($"?? [Ticket API] Creating new ticket with ID {ticketId}...");

                    ticket = new TEntity
                    {
                        Id = ticketId,
                        ProblemDescription = request.ProblemDescription ?? request.Content,
                        SystemInfo = request.SystemInfo ?? "N/A",
                        Status = SupportChat.Common.Models.App.TicketStatus.Unresolved,
                        Priority = request.Priority.HasValue
                            ? (SupportChat.Common.Models.App.TicketPriority)request.Priority.Value
                            : SupportChat.Common.Models.App.TicketPriority.Medium,
                        StartTime = DateTime.UtcNow,
                        IdentityId = request.IdentityId,
                        Recommendation = string.Empty
                    };

                    await Context.SupportTicketSet.AddAsync(ticket);
                    await Context.SaveChangesAsync();

                    Console.WriteLine($"? [Ticket API] Ticket {ticketId} created successfully");
                }

                // 3. Create user's chat message
                var userMessage = new TChatMessage
                {
                    Content = request.Content,
                    IsUserMessage = true,
                    Timestamp = DateTime.UtcNow,
                    SupportTicketId = ticketId
                };

                await Context.ChatMessageSet.AddAsync(userMessage);
                await Context.SaveChangesAsync();

                Console.WriteLine($"? [Ticket API] User message saved (ID: {userMessage.Id})");

                // 4. Trigger n8n workflow and wait for bot response
                var chatResponse = await TriggerN8nWorkflowForTicketAsync(ticket, userMessage);

                if (chatResponse == null)
                {
                    return StatusCode(500, new { error = "Failed to get AI response from n8n workflow." });
                }

                // 5. Convert to WebApi Model and return
                var responseModel = Models.App.ChatResponse.Create(chatResponse);
                return Ok(responseModel);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"? [Ticket API] Error: {ex.Message}");
                return StatusCode(500, new { error = GetErrorMessage(ex) });
            }
        }

        /// <summary>
        /// Triggers n8n workflow and returns the ChatResponse.
        /// </summary>
        private async Task<TChatResponse?> TriggerN8nWorkflowForTicketAsync(TEntity ticket, TChatMessage userMessage)
        {
            try
            {
                // Webhook URL aus AppSettings
                var appSettings = Common.Modules.Configuration.AppSettings.Instance;
                var webhookUrl = appSettings["n8n:WebHook:ChatMessageUrl"];

                if (string.IsNullOrEmpty(webhookUrl))
                {
                    Console.WriteLine("??  [n8n Workflow] Webhook URL not configured");
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

                Console.WriteLine($"?? [n8n Workflow] Sending webhook for ticket {ticket.Id}, message {userMessage.Id}...");
                var response = await httpClient.PostAsync(webhookUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"? [n8n Workflow] Webhook successful");

                    // Parse AI Response
                    using var jsonDoc = JsonDocument.Parse(responseBody);
                    if (jsonDoc.RootElement.TryGetProperty("message", out var messageElement))
                    {
                        var aiResponse = messageElement.GetString();

                        if (!string.IsNullOrWhiteSpace(aiResponse) && aiResponse != "OK")
                        {
                            // Speichere Bot-Message
                            var botMessage = new TChatMessage
                            {
                                Content = aiResponse,
                                IsUserMessage = false,
                                Timestamp = DateTime.UtcNow,
                                SupportTicketId = userMessage.SupportTicketId
                            };

                            await Context.ChatMessageSet.AddAsync(botMessage);
                            await Context.SaveChangesAsync();

                            // Erstelle ChatResponse mit Metadaten
                            var chatResponse = new TChatResponse
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

                            Console.WriteLine($"? [n8n Workflow] Bot message saved (ID: {botMessage.Id})");
                            Console.WriteLine($"? [n8n Workflow] ChatResponse created (ID: {chatResponse.Id})");

                            return chatResponse;
                        }
                    }
                }
                else
                {
                    var errorBody = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"? [n8n Workflow] Failed: {response.StatusCode}");
                    Console.WriteLine($"   Error: {errorBody}");
                }
            }
            catch (TaskCanceledException)
            {
                Console.WriteLine($"? [n8n Workflow] Timeout");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"? [n8n Workflow] Error: {ex.Message}");
            }

            return null;
        }
    }
}
#endif
