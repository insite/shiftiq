using System;

using InSite.UI.Layout.Admin;
using InSite.UI.Layout.Portal;

using Shift.Constant;

namespace InSite.UI.Portal.Records.Logbooks
{
    public partial class Search : PortalBasePage
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                LoadData();
        }

        private void LoadData()
        {
            PageHelper.AutoBindHeader(this);

            var journals = ServiceLocator.JournalSearch.GetEnrolledJournals(Organization.OrganizationIdentifier, User.UserIdentifier, CurrentLanguage);

            MainAccordion.Visible = journals.Count > 0;

            if (journals.Count > 0)
            {
                JournalRepeater.DataSource = journals;
                JournalRepeater.DataBind();
            }
            else
            {
                StatusAlert.AddMessage(AlertType.Warning, GetDisplayText("You have no logbooks.", null));
            }
        }
    }
}