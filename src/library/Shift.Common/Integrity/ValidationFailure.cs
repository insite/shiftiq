using System.Collections.Generic;

namespace Shift.Common
{
    public class ValidationFailure
    {
        public List<ValidationError> Errors { get; set; }

        public bool IsFailed() => !IsPassed();

        public bool IsPassed() => Errors == null || Errors.Count == 0;

        public ValidationFailure()
        {
            Errors = new List<ValidationError>();
        }

        public ValidationFailure(string summary)
        {
            AddError(summary);
        }

        public void AddError(string summary, string description = null)
        {
            Errors.Add(new ValidationError { Summary = summary, Description = description });
        }
    }
}