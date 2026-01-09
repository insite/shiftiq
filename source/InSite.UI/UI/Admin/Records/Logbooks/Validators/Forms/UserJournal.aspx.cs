using System;

using InSite.Common.Web;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.UI.Admin.Records.Validators.Forms
{
    public partial class UserJournal : AdminBasePage, IOverrideWebRouteParent, IHasParentLinkParameters
    {
        private Guid UserIdentifier => Guid.TryParse(Request.QueryString["user"], out var id) ? id : Guid.Empty;

        private Guid JournalSetupIdentifier => Guid.TryParse(Request.QueryString["journalsetup"], out var id) ? id : Guid.Empty;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                LoadData();
        }

        private void LoadData()
        {
            var person = PersonSearch.Select(Organization.Identifier, UserIdentifier, x => x.User);
            if (person == null)
            {
                HttpResponseHelper.Redirect("/ui/admin/records/logbooks/validators/search");
                return;
            }

            PageHelper.AutoBindHeader(this, null, person.User.FullName);

            var hasJournals = UserJournalTree.LoadData(Organization.Identifier, UserIdentifier, User.UserIdentifier);

            NoJournalPanel.Visible = !hasJournals;
            UserJournalTree.Visible = hasJournals;

            CompetenciesPanel.Visible = hasJournals;

            if (hasJournals)
                Competencies.LoadData(Organization.Identifier, UserIdentifier, User.UserIdentifier);
        }

        IWebRoute IOverrideWebRouteParent.GetParent() =>
            GetParent();

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"journalsetup={JournalSetupIdentifier}"
                : GetParentLinkParameters(parent, null);
        }
    }
}
