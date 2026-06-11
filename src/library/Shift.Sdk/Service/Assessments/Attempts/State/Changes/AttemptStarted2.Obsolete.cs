using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Attempts
{
    public partial class AttemptStarted2
    {
        private class AttemptStarted1 : Change
        {
            public Guid Tenant { get; set; }
            public Guid CandidateIdentifier { get; set; }
            public Guid FormIdentifier { get; set; }
            public QuestionHandle[] Questions { get; set; }
            public Guid? RegistrationIdentifier { get; set; }

            public int? TimeLimit { get; set; }
            public string Language { get; set; }
            public string UserAgent { get; set; }

            public bool SectionsAsTabsEnabled { get; set; }
            public bool TabNavigationEnabled { get; set; }
            public bool SingleQuestionPerTabEnabled { get; set; }
            public int? FormSectionsCount { get; set; }
            public int? ActiveSectionIndex { get; set; }
            public int? ActiveQuestionIndex { get; set; }
        }

        public static AttemptStarted2 Upgrade(SerializedChange serializedChange)
        {
            var v1 = serializedChange.Deserialize<AttemptStarted1>();

            var v2 = new AttemptStarted2(v1.Tenant, v1.CandidateIdentifier, v1.CandidateIdentifier, v1.FormIdentifier, v1.Questions, v1.RegistrationIdentifier, v1.TimeLimit, v1.Language, v1.UserAgent, v1.SectionsAsTabsEnabled, v1.TabNavigationEnabled, v1.SingleQuestionPerTabEnabled, v1.FormSectionsCount, v1.ActiveSectionIndex, v1.ActiveQuestionIndex)
            {
                AggregateState = serializedChange.AggregateState,
                AggregateIdentifier = v1.AggregateIdentifier,
                AggregateVersion = v1.AggregateVersion,
                OriginOrganization = v1.OriginOrganization,
                OriginUser = v1.OriginUser,
                ChangeTime = v1.ChangeTime
            };

            return v2;
        }
    }
}
