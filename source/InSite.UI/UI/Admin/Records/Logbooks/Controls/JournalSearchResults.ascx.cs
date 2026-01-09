using System.ComponentModel;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Application.Records.Read;
using InSite.Common.Web.UI;

using Shift.Common.Linq;

namespace InSite.Admin.Records.Logbooks.Controls
{
    public partial class JournalSearchResults : SearchResultsGridViewController<VJournalSetupUserFilter>
    {
        public bool IsValidator { get; set; }

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
                    JournalIdentifier = x.JournalIdentifier,
                    JournalSetupIdentifier = x.JournalSetupIdentifier,
                    JournalSetupName = x.JournalSetupName,
                    UserIdentifier = x.UserIdentifier,
                    UserFullName = x.UserFullName,
                    UserEmail = x.UserEmail,
                    EntryCount = x.ExperienceCount,
                    LearnerUrl = IsValidator
                        ? returnUrl.GetRedirectUrl($"/ui/admin/records/logbooks/validators/user-journal?user={x.UserIdentifier}")
                        : $"/ui/admin/contacts/people/edit?contact={x.UserIdentifier}",
                    JournalUrl = IsValidator
                        ? $"/ui/admin/records/logbooks/validators/outline-journal?journalsetup={x.JournalSetupIdentifier}&user={x.UserIdentifier}"
                        : $"/ui/admin/records/logbooks/outline-journal?journalsetup={x.JournalSetupIdentifier}&user={x.UserIdentifier}",
                    DeleteUrl = IsValidator
                        ? returnUrl.GetRedirectUrl($"/admin/records/logbooks/validators/delete-user?journalsetup={x.JournalSetupIdentifier}&user={x.UserIdentifier}")
                        : returnUrl.GetRedirectUrl($"/ui/admin/records/logbooks/delete-user?journalsetup={x.JournalSetupIdentifier}&user={x.UserIdentifier}")
                })
                .ToList()
                .ToSearchResult();
        }
    }
}