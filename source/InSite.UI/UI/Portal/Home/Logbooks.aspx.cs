using System;

using InSite.Persistence;
using InSite.UI.Layout.Admin;
using InSite.UI.Layout.Portal;

using Shift.Constant;

namespace InSite.UI.Portal.Home
{
    public partial class Logbooks : PortalBasePage
    {
        public bool HideHeading { get; set; }

        private Guid GetUserIdentifier()
        {
            if (Guid.TryParse(Request.QueryString["user"], out Guid user))
                return user;
            return User.UserIdentifier;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            PageHelper.AutoBindHeader(this);

            PortalMaster.ShowAvatar();
            PortalMaster.EnableSidebarToggle(true);
            
            BindModelToControls(GetUserIdentifier());
        }

        private void BindModelToControls(Guid user)
        {
            var journals = ServiceLocator.JournalSearch.GetLearnerJournals(Organization.OrganizationIdentifier, user, CurrentLanguage);

            if (journals.Count > 0)
            {
                JournalRepeater.DataSource = journals;
                JournalRepeater.DataBind();
            }
            else
            {
                StatusAlert.AddMessage(AlertType.Information, GetDisplayText("You have not been assigned a Logbook."));
            }
        }
    }
}