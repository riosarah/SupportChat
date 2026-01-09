//@AiCode
namespace SupportChat.Logic.Entities.App
{
    /// <summary>
    /// Represents an AI-generated response from the chatbot with associated metadata.
    /// Tracks AI model performance, token usage, and response quality.
    /// </summary>
    [Table("ChatResponses")]
    [Index(nameof(SupportTicketId), nameof(CreatedAt), Name = "ChatResponse_Ticket_Time_Index")]
    public partial class ChatResponse : EntityObject
    {
        #region properties

        /// <summary>
        /// The AI-generated response content.
        /// </summary>
        [MaxLength(4000)]
        [Required]
        public string ResponseContent { get; set; } = string.Empty;

        /// <summary>
        /// The AI model used to generate this response (e.g., "gpt-4", "gpt-3.5-turbo").
        /// </summary>
        [MaxLength(100)]
        public string ModelUsed { get; set; } = string.Empty;

        /// <summary>
        /// Number of tokens used in the prompt (input).
        /// </summary>
        public int PromptTokens { get; set; }

        /// <summary>
        /// Number of tokens used in the completion (output).
        /// </summary>
        public int CompletionTokens { get; set; }

        /// <summary>
        /// Total tokens used (prompt + completion).
        /// </summary>
        public int TotalTokens { get; set; }

        /// <summary>
        /// Response time in milliseconds from n8n workflow.
        /// </summary>
        public int ResponseTimeMs { get; set; }

        /// <summary>
        /// Indicates whether the response was successfully generated.
        /// </summary>
        public bool IsSuccessful { get; set; } = true;

        /// <summary>
        /// Error message if response generation failed.
        /// </summary>
        [MaxLength(500)]
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// Session ID reference to the SupportTicket (same as SupportTicketId for compatibility).
        /// </summary>
        public IdType SessionId { get; set; }

        /// <summary>
        /// Timestamp when this response was created.
        /// </summary>
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Optional: User feedback rating (1-5 stars).
        /// </summary>
        public int? UserRating { get; set; }

        /// <summary>
        /// Optional: User feedback comment.
        /// </summary>
        [MaxLength(1000)]
        public string? UserFeedback { get; set; }

        #endregion properties

        #region navigationalProperties

        /// <summary>
        /// Foreign key reference to the parent SupportTicket.
        /// </summary>
        [ForeignKey(nameof(SupportTicket))]
        public IdType SupportTicketId { get; set; }

        /// <summary>
        /// Navigation property to the parent SupportTicket.
        /// </summary>
        public SupportChat.Logic.Entities.App.SupportTicket? SupportTicket { get; set; }

        /// <summary>
        /// Foreign key reference to the user's ChatMessage that triggered this response.
        /// </summary>
        [ForeignKey(nameof(UserMessage))]
        public IdType UserMessageId { get; set; }

        /// <summary>
        /// Navigation property to the user's ChatMessage.
        /// </summary>
        public SupportChat.Logic.Entities.App.ChatMessage? UserMessage { get; set; }

        /// <summary>
        /// Foreign key reference to the bot's ChatMessage (the actual response).
        /// </summary>
        [ForeignKey(nameof(BotMessage))]
        public IdType? BotMessageId { get; set; }

        /// <summary>
        /// Navigation property to the bot's ChatMessage.
        /// </summary>
        public SupportChat.Logic.Entities.App.ChatMessage? BotMessage { get; set; }

        #endregion navigationalProperties

        #region constructor

        /// <summary>
        /// Initializes a new instance of the ChatResponse class.
        /// </summary>
        public ChatResponse()
        {
        }

        #endregion constructor

        #region overrides

        /// <summary>
        /// Returns a string representation of the chat response.
        /// </summary>
        public override string ToString()
        {
            return $"ChatResponse #{Id}: {ModelUsed} - {TotalTokens} tokens ({ResponseTimeMs}ms)";
        }

        #endregion overrides
    }
}
