using System;

using InSite.Application.Events.Read;
using InSite.Application.Registrations.Read;
using InSite.UI.Layout.Admin;

namespace InSite.UI.Admin.Events.Reports
{
    public partial class Dashboard : AdminBasePage
    {
        private QEventFilter CreateEventFilter(string eventType)
            => new QEventFilter { OrganizationIdentifier = Organization.OrganizationIdentifier, EventType = eventType };

        private QRegistrationFilter CreateRegistrationFilter()
            => new QRegistrationFilter { OrganizationIdentifier = Organization.OrganizationIdentifier };

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                PageHelper.AutoBindHeader(this);
            }
        }
    }
}