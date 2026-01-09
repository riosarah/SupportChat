//@AiCode
namespace SupportChat.Common.Models.App
{
    /// <summary>
    /// Represents the priority level of a support ticket.
    /// </summary>
    public enum TicketPriority
    {
        /// <summary>
        /// Low priority - non-urgent issues.
        /// </summary>
        Low = 0,

        /// <summary>
        /// Medium priority - standard issues.
        /// </summary>
        Medium = 1,

        /// <summary>
        /// High priority - urgent issues.
        /// </summary>
        High = 2,

        /// <summary>
        /// Critical priority - system-critical issues requiring immediate attention.
        /// </summary>
        Critical = 3
    }
}
