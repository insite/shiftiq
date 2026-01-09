using System;

namespace InSite.Admin.Events.Candidates.Controls
{
    public partial class CandidatePanel
    {
        private class ExamCandidate
        {
            public Guid RegistrationIdentifier { get; set; }

            public string CandidateCode { get; set; }
            public Guid CandidateIdentifier { get; set; }
            public string CandidateName { get; set; }

            public string FormCode { get; set; }
            public Guid? FormIdentifier { get; set; }
            public string FormTitle { get; set; }
            public string FormName { get; set; }

            public decimal? AttemptScore { get; set; }
        }
    }
}