using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Application.Records.Read;
using InSite.Common.Web.UI;

using Shift.Common.Linq;
using Shift.Constant;

namespace InSite.UI.Admin.Records.Logbooks.Achievements.Controls
{
    public partial class UserGrantGrid : SearchResultsGridViewController<VJournalSetupUserFilter>
    {
        protected override bool IsFinder => false;

        public HashSet<Guid> SelectedUsers
            => (HashSet<Guid>)(ViewState[nameof(SelectedUsers)] ?? (ViewState[nameof(SelectedUsers)] = new HashSet<Guid>()));

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                SaveSelections();
        }

        private void SaveSelections()
        {
            foreach (GridViewRow row in Grid.Rows)
            {
                var id = Guid.Parse(((ITextControl)row.FindControl("UserIdentifier")).Text);
                var selected = ((ICheckBoxControl)row.FindControl("Selected")).Checked;

                if (selected)
                    SelectedUsers.Add(id);
                else
                    SelectedUsers.Remove(id);
            }
        }

        public int LoadData(Guid journalSetupIdentifier, Guid achievementIdentifier)
        {
            var filter = new VJournalSetupUserFilter
            {
                JournalSetupIdentifier = journalSetupIdentifier,
                ExcludeAchievementIdentifier = achievementIdentifier,
                Role = JournalSetupUserRole.Learner
            };

            Search(filter);

            return RowCount;
        }

        protected override int SelectCount(VJournalSetupUserFilter filter)
        {
            return ServiceLocator.JournalSearch.CountJournalSetupUsers(filter);
        }

        protected override IListSource SelectData(VJournalSetupUserFilter filter)
        {
            var returnUrl = new ReturnUrl();

            return ServiceLocator.JournalSearch
                .GetJournalSetupUsersExtended(filter)
                .Select(x => new
                {
                    UserIdentifier = x.UserIdentifier,
                    UserFullName = x.UserFullName,
                    Email = x.UserEmail,
                    EmailAlternate = x.UserEmailAlternate,
                    PersonCode = x.PersonCode,
                    EmployerIdentifier = x.EmployerGroupIdentifier,
                    Employer = x.EmployerGroupName,
                    ExperienceCount = x.ExperienceCount,
                    LearnerUrl = $"/ui/admin/contacts/people/edit?contact={x.UserIdentifier}",
                    Validated = !x.HasAchievement && x.ExperienceCount > 0 && x.ValidatedExperienceCount == x.ExperienceCount,
                    NotValidated = !x.HasAchievement && x.ExperienceCount > 0 && x.ValidatedExperienceCount < x.ExperienceCount,
                    NoEntries = !x.HasAchievement && x.ExperienceCount == 0,
                })
                .ToList()
                .ToSearchResult();
        }

        protected bool IsSelected()
        {
            var item = Page.GetDataItem();
            var id = (Guid)DataBinder.Eval(item, "UserIdentifier");

            return SelectedUsers.Contains(id);
        }
    }
}