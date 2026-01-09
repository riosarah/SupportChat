//@AiCode
namespace SupportChat.Logic.Entities.App
{
    /// <summary>
    /// Represents a single step taken during a support session.
    /// </summary>
    [Table("TicketSteps")]
    public partial class TicketStep : EntityObject
    {
        #region properties

        /// <summary>
        /// The order number of this step in the sequence.
        /// </summary>
        [Required]
        public int StepNumber { get; set; }

        /// <summary>
        /// Description of the step taken.
        /// </summary>
        [MaxLength(500)]
        [Required]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Timestamp when this step was executed.
        /// </summary>
        public DateTime? ExecutedAt { get; set; }

        /// <summary>
        /// Indicates whether this step was successful.
        /// </summary>
        public bool WasSuccessful { get; set; }

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
        /// Initializes a new instance of the TicketStep class.
        /// </summary>
        public TicketStep()
        {
        }

        #endregion constructor

        #region overrides

        /// <summary>
        /// Returns a string representation of the ticket step.
        /// </summary>
        public override string ToString()
        {
            return $"Step {StepNumber}: {Description}";
        }

        #endregion overrides
    }
}
