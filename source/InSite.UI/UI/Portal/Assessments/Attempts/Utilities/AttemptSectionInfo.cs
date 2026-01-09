using System.Collections.Generic;

using InSite.Application.Attempts.Read;
using InSite.Domain.Banks;

namespace InSite.UI.Portal.Assessments.Attempts.Utilities
{
    public class AttemptSectionInfo
    {
        public Section BankSection { get; set; }
        public QAttemptSection AttemptSection { get; set; }
        public List<QAttemptQuestion> Questions { get; set; } = new List<QAttemptQuestion>();
    }
}