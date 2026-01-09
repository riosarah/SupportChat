//@AiCode
using SupportChat.Common.Models.App;

namespace SupportChat.Logic.Entities.App
{
    /// <summary>
    /// Represents a support ticket created from an IT support chatbot session.
    /// </summary>
    [Table("SupportTickets")]
    public partial class SupportTicket : EntityObject
    {
        #region properties

        /// <summary>
        /// Description of the problem reported by the user.
        /// </summary>
        [MaxLength(2000)]
        [Required]
        public string ProblemDescription { get; set; } = string.Empty;

        /// <summary>
        /// System information provided by the user (OS, browser, etc.).
        /// </summary>
        [MaxLength(1000)]
        public string SystemInfo { get; set; } = string.Empty;

        /// <summary>
        /// Current status of the ticket (Resolved or Unresolved).
        /// </summary>
        public SupportChat.Common.Models.App.TicketStatus Status { get; set; } = SupportChat.Common.Models.App.TicketStatus.Unresolved;

        /// <summary>
        /// Priority level of the ticket.
        /// </summary>
        public SupportChat.Common.Models.App.TicketPriority Priority { get; set; } = SupportChat.Common.Models.App.TicketPriority.Medium;

        /// <summary>
        /// Recommendation provided by the chatbot for resolving the issue.
        /// </summary>
        [MaxLength(2000)]
        public string Recommendation { get; set; } = string.Empty;

        /// <summary>
        /// Timestamp when the chat session started.
        /// </summary>
        [Required]
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Timestamp when the chat session ended.
        /// </summary>
        public DateTime? EndTime { get; set; }

        #endregion properties

        #region navigationalProperties

        /// <summary>
        /// Foreign key reference to the Identity who created the ticket.
        /// </summary>
        public IdType IdentityId { get; set; }

        /// <summary>
        /// Collection of steps taken during the support session.
        /// </summary>
        public List<TicketStep> TicketSteps { get; set; } = [];

        /// <summary>
        /// Collection of chat messages from the support session.
        /// </summary>
        public List<ChatMessage> ChatMessages { get; set; } = [];

        /// <summary>
        /// Collection of AI-generated responses with metadata from the support session.
        /// </summary>
        public List<ChatResponse> ChatResponses { get; set; } = [];

        #endregion navigationalProperties

        #region constructor

        /// <summary>
        /// Initializes a new instance of the SupportTicket class.
        /// </summary>
        public SupportTicket()
        {
        }

        #endregion constructor

        #region overrides

        /// <summary>
        /// Returns a string representation of the support ticket.
        /// </summary>
        public override string ToString()
        {
            return $"Ticket #{Id}: {Status} - {ProblemDescription.Substring(0, Math.Min(50, ProblemDescription.Length))}...";
        }

        #endregion overrides
    }
}
