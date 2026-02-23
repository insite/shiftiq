using System;

using InSite.Application.Standards.Read;

namespace InSite.Application.Banks.Read
{
    public class QBankOption
    {
        public Guid OrganizationIdentifier { get; set; }
        public Guid BankIdentifier { get; set; }
        public Guid SetIdentifier { get; set; }
        public Guid QuestionIdentifier { get; set; }
        public int OptionKey { get; set; }
        public Guid? CompetencyIdentifier { get; set; }
        public string OptionText { get; set; }

        public virtual QBankQuestion Question { get; set; }
        public virtual VCompetency Competency { get; set; }
    }
}
