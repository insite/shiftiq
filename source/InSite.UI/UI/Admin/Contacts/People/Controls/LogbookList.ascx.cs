using System;
using System.Linq;
using System.Web.UI;

using InSite.Application.Records.Read;

using Shift.Constant;

namespace InSite.UI.Admin.Contacts.People.Controls
{
    public partial class LogbookList : UserControl
    {
        public int LoadData(Guid userIdentifier)
        {
            var isAdmin = CurrentSessionState.Identity.IsGranted(PermissionIdentifiers.Admin_Records);
            var isValidator = !isAdmin && CurrentSessionState.Identity.IsGranted(PermissionIdentifiers.Portal_Logbooks);

            var filter = new VJournalSetupUserFilter
            {
                OrganizationIdentifier = CurrentSessionState.Identity.Organization.OrganizationIdentifier,
                UserIdentifier = userIdentifier,
                Role = JournalSetupUserRole.Learner,
                OrderBy = "JournalCreated"
            };

            var data = ServiceLocator.JournalSearch
                .GetJournalSetupUsersExtended(filter)
                .Select(x => new
                {
                    x.JournalIdentifier,
                    x.JournalSetupIdentifier,
                    x.JournalSetupName,
                    x.UserIdentifier,
                    EntryCount = x.ExperienceCount,
                    IsAdmin = isAdmin,
                    IsValidator = isValidator
                })
                .ToList();

            NoLogbooks.Visible = data.Count == 0;

            LogbookRepeater.Visible = LogbookPanel.Visible = data.Count > 0;
            LogbookRepeater.DataSource = data;
            LogbookRepeater.DataBind();

            return data.Count;
        }
    }
}