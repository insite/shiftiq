using System;
using System.Linq;

using Shift.Common.Timeline.Changes;

using Shift.Common;

namespace InSite.Domain.Attempts
{
    public partial class AttemptStarted2 : Change
    {
        public Guid OrganizationIdentifier { get; set; }
        public Guid AssessorUserIdentifier { get; set; }
        public Guid LearnerUserIdentifier { get; set; }
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

        public AttemptStarted2(Guid organization, Guid assessor, Guid learner, Guid form, QuestionHandle[] questions, Guid? registration, int? timeLimit, string language, string userAgent, bool sectionsAsTabsEnabled, bool tabNavigationEnabled, bool singleQuestionPerTabEnabled, int? formSectionsCount, int? activeSectionIndex, int? activeQuestionIndex)
        {
            OrganizationIdentifier = organization;
            AssessorUserIdentifier = assessor;
            LearnerUserIdentifier = learner;
            FormIdentifier = form;
            Questions = questions.Select(x => x.Clone()).ToArray();
            RegistrationIdentifier = registration;

            TimeLimit = timeLimit;
            Language = language;
            UserAgent = userAgent;

            SectionsAsTabsEnabled = sectionsAsTabsEnabled;
            TabNavigationEnabled = tabNavigationEnabled;
            SingleQuestionPerTabEnabled = singleQuestionPerTabEnabled;
            FormSectionsCount = formSectionsCount;
            ActiveSectionIndex = activeSectionIndex;
            ActiveQuestionIndex = activeQuestionIndex;
        }

        [Serializable]
        public class QuestionHandle
        {
            public Guid Question { get; set; }
            public int? Section { get; set; }
            public int[] Options { get; set; }
            public Guid[] LikertRows { get; set; }
            public Guid[] LikertColumns { get; set; }

            public QuestionHandle Clone()
            {
                var clone = new QuestionHandle();

                this.ShallowCopyTo(clone);

                clone.Options = Options?.ToArray();
                clone.LikertRows = LikertRows?.ToArray();
                clone.LikertColumns = LikertColumns?.ToArray();

                return clone;
            }
        }
    }
}
