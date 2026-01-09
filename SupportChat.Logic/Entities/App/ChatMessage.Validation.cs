//@AiCode
using SupportChat.Logic.Contracts;
using SupportChat.Logic.Modules.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace SupportChat.Logic.Entities.App
{
    /// <summary>
    /// Validation logic for the ChatMessage entity.
    /// </summary>
    public partial class ChatMessage : IValidatableEntity
    {
        /// <summary>
        /// Validates the ChatMessage entity.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="entityState">The current state of the entity.</param>
        public void Validate(IContext context, EntityState entityState)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(Content))
            {
                errors.Add($"{nameof(Content)} must not be empty.");
            }

            if (Content.Length > 4000)
            {
                errors.Add($"{nameof(Content)} must not exceed 4000 characters.");
            }

            if (Timestamp == default)
            {
                errors.Add($"{nameof(Timestamp)} must be set.");
            }

            if (SupportTicketId <= 0)
            {
                errors.Add($"{nameof(SupportTicketId)} reference must be a valid positive number.");
            }

            if (errors.Any())
            {
                throw new ValidationException(string.Join(" | ", errors));
            }
        }
    }
}
