//@AiCode
using SupportChat.Logic.Contracts;
using SupportChat.Logic.Modules.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace SupportChat.Logic.Entities.App
{
    /// <summary>
    /// Validation logic for the SupportTicket entity.
    /// </summary>
    public partial class SupportTicket : IValidatableEntity
    {
        /// <summary>
        /// Validates the SupportTicket entity.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="entityState">The current state of the entity.</param>
        public void Validate(IContext context, EntityState entityState)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(ProblemDescription))
            {
                errors.Add($"{nameof(ProblemDescription)} must not be empty.");
            }

            if (ProblemDescription.Length > 2000)
            {
                errors.Add($"{nameof(ProblemDescription)} must not exceed 2000 characters.");
            }

            if (SystemInfo.Length > 1000)
            {
                errors.Add($"{nameof(SystemInfo)} must not exceed 1000 characters.");
            }

            if (Recommendation.Length > 2000)
            {
                errors.Add($"{nameof(Recommendation)} must not exceed 2000 characters.");
            }

            if (StartTime == default)
            {
                errors.Add($"{nameof(StartTime)} must be set.");
            }

            if (EndTime.HasValue && EndTime.Value < StartTime)
            {
                errors.Add($"{nameof(EndTime)} must be after {nameof(StartTime)}.");
            }

            if (IdentityId <= 0)
            {
                errors.Add($"{nameof(IdentityId)} reference must be a valid positive number.");
            }

            if (errors.Any())
            {
                throw new ValidationException(string.Join(" | ", errors));
            }
        }
    }
}
