using System.Collections.Generic;

using InSite.Application.Attempts.Read;
using InSite.Domain.Banks;

using Shift.Common;

namespace InSite.UI.Portal.Assessments.Attempts.Utilities
{
    public class AttemptSectionInfo
    {
        public MultilingualDictionary BankSectionContent { get; set; }
        public QAttemptSection AttemptSection { get; set; }
        public List<QAttemptQuestion> Questions { get; set; } = new List<QAttemptQuestion>();

        public AttemptSectionInfo()
        {

        }

        public AttemptSectionInfo(Section section)
        {
            Set(section);
        }

        public AttemptSectionInfo(Criterion criterion)
        {
            Set(criterion);
        }

        public void Set(Section section)
        {
            BankSectionContent = section.Content ?? new MultilingualDictionary();
        }

        public void Set(Criterion criterion)
        {
            BankSectionContent = criterion.Content ?? new MultilingualDictionary();
        }
    }
}
