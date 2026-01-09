using System;

namespace Shift.Sdk.UI
{
    public class ResponseSessionNavigatorStep
    {
        public int PageNumber { get; set; }
        public int QuestionNumber { get; set; }
        public Guid QuestionIdentifier { get; set; }
        public bool IsHidden { get; set; }
    }
}
