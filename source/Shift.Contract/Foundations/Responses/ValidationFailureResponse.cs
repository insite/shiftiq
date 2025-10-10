using System.Collections.Generic;

namespace Shift.Contract.Responses
{
    public class ValidationFailureResponse
    {
        public IEnumerable<ValidationResponse> Errors { get; set; }
    }
}