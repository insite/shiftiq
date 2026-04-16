using System;

namespace Shift.Contract
{
    public partial class FormWorkshop
    {
        public class Questions
        {
            public bool IsQuestionOrderMatch { get; set; }
            public DateTimeOffset? StaticQuestionOrderVerified { get; set; }
            public FormWorkshop.StaticQuestionOrder[] VerifiedQuestions { get; set; }
        }
    }
}
