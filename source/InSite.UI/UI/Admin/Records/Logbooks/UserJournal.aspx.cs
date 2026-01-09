using System;

using InSite.Common.Web;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

namespace InSite.Admin.Records.Logbooks
{
    public partial class UserJournal : AdminBasePage
    {
        private Guid UserIdentifier => Guid.TryParse(Request.QueryString["user"], out var userIdentifier) ? userIdentifier : Guid.Empty;

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
                HttpResponseHelper.Redirect("/ui/admin/records/home");
                return;
            }

            PageHelper.AutoBindHeader(this, null, person.User.FullName);

            var hasJournals = UserJournalTree.LoadData(Organization.Identifier, UserIdentifier, null);
            
            NoJournalPanel.Visible = !hasJournals;
            UserJournalTree.Visible = hasJournals;

            CompetenciesPanel.Visible = hasJournals;

            if (hasJournals)
                Competencies.LoadData(Organization.Identifier, UserIdentifier, null);
        }
    }
}
