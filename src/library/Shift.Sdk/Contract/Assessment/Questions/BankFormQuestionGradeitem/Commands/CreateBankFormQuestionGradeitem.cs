using System;

namespace Shift.Contract
{
    public class CreateBankFormQuestionGradeitem
    {
        public Guid FormIdentifier { get; set; }
        public Guid GradeItemIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid QuestionIdentifier { get; set; }
    }
}