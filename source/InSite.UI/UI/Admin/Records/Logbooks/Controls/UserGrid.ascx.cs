using System;
using System.ComponentModel;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Application.Records.Read;
using InSite.Common.Web.UI;

using Shift.Common.Linq;
using Shift.Constant;

namespace InSite.Admin.Records.Logbooks.Controls
{
    public partial class UserGrid : SearchResultsGridViewController<VJournalSetupUserFilter>
    {
        public class LoadResult
        {
            public bool HasRows { get; set; }
            public bool HasExperiences { get; set; }
        }

        private bool _hasExperiences;

        protected override bool IsFinder => false;

        protected Guid JournalSetupIdentifier
        {
            get => (Guid?)ViewState[nameof(JournalSetupIdentifier)] ?? Guid.Empty;
            set => ViewState[nameof(JournalSetupIdentifier)] = value;
        }

        public bool IsValidator { get; set; }

        public LoadResult LoadData(Guid journalSetupIdentifier, bool logbookHasAchievement, string keyword)
        {
            JournalSetupIdentifier = journalSetupIdentifier;

            var filter = new VJournalSetupUserFilter
            {
                JournalSetupIdentifier = JournalSetupIdentifier,
                Role = JournalSetupUserRole.Learner,
                UserKeyword = keyword
            };

            Search(filter);

            return new LoadResult { HasRows = RowCount > 0, HasExperiences = _hasExperiences };
        }

        protected override int SelectCount(VJournalSetupUserFilter filter)
        {
            return ServiceLocator.JournalSearch.CountJournalSetupUsers(filter);
        }

        protected override IListSource SelectData(VJournalSetupUserFilter filter)
        {
            var returnUrl = new ReturnUrl();

            var data = ServiceLocator.JournalSearch
                .GetJournalSetupUsersExtended(filter)
                .Select(x => new
                {
                    JournalIdentifier = x.JournalIdentifier,
                    JournalSetupIdentifier = x.JournalSetupIdentifier,
                    UserIdentifier = x.UserIdentifier,
                    UserFullName = x.UserFullName,
                    Email = x.UserEmail,
                    EmailAlternate = x.UserEmailAlternate,
                    PersonCode = x.PersonCode,
                    EmployerIdentifier = x.EmployerGroupIdentifier,
                    Employer = x.EmployerGroupName,
                    ExperienceCount = x.ExperienceCount,
                    HasAchievement = x.HasAchievement,
                    Validated = !x.HasAchievement && x.ExperienceCount > 0 && x.ValidatedExperienceCount == x.ExperienceCount,
                    NotValidated = !x.HasAchievement && x.ExperienceCount > 0 && x.ValidatedExperienceCount < x.ExperienceCount,
                    LearnerUrl = IsValidator
                        ? returnUrl.GetRedirectUrl($"/ui/admin/records/logbooks/validators/user-journal?user={x.UserIdentifier}&journalsetup={JournalSetupIdentifier}")
                        : $"/ui/admin/contacts/people/edit?contact={x.UserIdentifier}",
                    OutlineUrl = IsValidator
                        ? $"/ui/admin/records/logbooks/validators/outline-journal?journalsetup={JournalSetupIdentifier}&user={x.UserIdentifier}"
                        : $"/ui/admin/records/logbooks/outline-journal?journalsetup={JournalSetupIdentifier}&user={x.UserIdentifier}",
                    DeleteUrl = IsValidator
                        ? $"/admin/records/logbooks/validators/delete-user?journalsetup={JournalSetupIdentifier}&user={x.UserIdentifier}"
                        : $"/ui/admin/records/logbooks/delete-user?journalsetup={JournalSetupIdentifier}&user={x.UserIdentifier}"
                })
                .ToList();

            _hasExperiences = data.Any(x => x.ExperienceCount > 0);

            return data.ToSearchResult();
        }
    }
}