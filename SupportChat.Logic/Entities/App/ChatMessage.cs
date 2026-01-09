//@AiCode
namespace SupportChat.Logic.Entities.App
{
    /// <summary>
    /// Represents a single message in a support chat session.
    /// </summary>
    [Table("ChatMessages")]
    [Index(nameof(SupportTicketId), nameof(Timestamp), Name = "ChatMessage_Ticket_Time_Index")]
    public partial class ChatMessage : EntityObject
    {
        #region properties

        /// <summary>
        /// The content of the chat message.
        /// </summary>
        [MaxLength(4000)]
        [Required]
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// Indicates whether this message was sent by the user (true) or the chatbot (false).
        /// </summary>
        public bool IsUserMessage { get; set; }

        /// <summary>
        /// Timestamp when the message was sent.
        /// </summary>
        [Required]
        public DateTime Timestamp { get; set; }

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

        #endregion navigationalProperties

        #region constructor

        /// <summary>
        /// Initializes a new instance of the ChatMessage class.
        /// </summary>
        public ChatMessage()
        {
        }

        #endregion constructor

        #region overrides

        /// <summary>
        /// Returns a string representation of the chat message.
        /// </summary>
        public override string ToString()
        {
            var sender = IsUserMessage ? "User" : "Bot";
            return $"[{Timestamp:HH:mm:ss}] {sender}: {Content.Substring(0, Math.Min(50, Content.Length))}...";
        }

        #endregion overrides
    }
}
