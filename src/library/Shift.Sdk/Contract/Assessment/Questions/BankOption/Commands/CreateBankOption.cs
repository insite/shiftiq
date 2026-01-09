using System;

namespace Shift.Contract
{
    public class CreateBankOption
    {
        public Guid BankIdentifier { get; set; }
        public Guid? CompetencyIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid QuestionIdentifier { get; set; }
        public Guid SetIdentifier { get; set; }

        public string OptionText { get; set; }

        public int OptionKey { get; set; }
    }
}