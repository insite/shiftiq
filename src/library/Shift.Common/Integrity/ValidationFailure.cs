using System.Collections.Generic;

namespace Shift.Common
{
    public class ValidationFailure
    {
        public List<Problem> Errors { get; set; }

        public bool IsFailed() => !IsPassed();

        public bool IsPassed() => Errors == null || Errors.Count == 0;

        public ValidationFailure()
        {
            Errors = new List<Problem>();
        }

        public ValidationFailure(string summary)
        {
            AddError(summary);
        }

        public void AddError(string detail)
        {
            Errors.Add(new Problem(422, detail));
        }
    }
}