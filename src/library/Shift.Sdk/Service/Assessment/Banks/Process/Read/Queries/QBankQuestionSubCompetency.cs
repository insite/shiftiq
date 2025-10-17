using System;

using InSite.Application.Standards.Read;

namespace InSite.Application.Banks.Read
{
    public class QBankQuestionSubCompetency
    {
        public Guid QuestionIdentifier { get; set; }
        public Guid SubCompetencyIdentifier { get; set; }

        public virtual QBankQuestion Question { get; set; }
        public virtual VCompetency SubCompetency { get; set; }
    }
}
