using System;

using InSite.UI.Layout.Admin;
using InSite.UI.Layout.Portal;

using Shift.Common;

namespace InSite.UI.Portal.Events
{
    public partial class Calendar : PortalBasePage
    {
        protected string CalendarDate
        {
            get
            {
                if (!DateTime.TryParse(Request.QueryString["date"], out var date))
                    date = TimeZones.ConvertTimeFromUtc(DateTime.UtcNow, User.TimeZone);

                return date.ToShortDateString();
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                LoadData();
        }

        private void LoadData()
        {
            PageHelper.AutoBindHeader(this, qualifier: Translate("Calendar"));

            MainPanel.Visible = true;
        }
    }
}
