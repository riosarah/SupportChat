//@AiCode
using SupportChat.Logic.Contracts;
using SupportChat.Logic.Modules.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace SupportChat.Logic.Entities.App
{
    /// <summary>
    /// Validation logic for the TicketStep entity.
    /// </summary>
    public partial class TicketStep : IValidatableEntity
    {
        /// <summary>
        /// Validates the TicketStep entity.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="entityState">The current state of the entity.</param>
        public void Validate(IContext context, EntityState entityState)
        {
            var errors = new List<string>();

            if (StepNumber < 1)
            {
                errors.Add($"{nameof(StepNumber)} must be a positive number starting from 1.");
            }

            if (string.IsNullOrWhiteSpace(Description))
            {
                errors.Add($"{nameof(Description)} must not be empty.");
            }

            if (Description.Length > 500)
            {
                errors.Add($"{nameof(Description)} must not exceed 500 characters.");
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
