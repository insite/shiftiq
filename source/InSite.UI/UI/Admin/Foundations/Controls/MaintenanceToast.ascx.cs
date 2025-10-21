using System;

using InSite.UI.Layout.Admin;
using InSite.Web.Optimization;

using Shift.Common;

namespace InSite.Admin.Programs.Controls
{
    public partial class MaintenanceToast : AdminBaseControl
    {
        public bool ShowOnEachRequest
        {
            get => (bool)(ViewState[nameof(ShowOnEachRequest)] ?? false);
            set => ViewState[nameof(ShowOnEachRequest)] = value;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!ShowOnEachRequest && IsPostBack)
                return;

            var monitor = new MaintenanceMonitor();

            if (monitor.Lockouts == null)
                return;

            Visible = monitor.Lockouts.State != LockoutState.Closed;

            if (!Visible)
                return;

            Description.Text = Markdown.ToHtml(monitor.Lockouts.Description);

            System.Web.UI.ScriptManager.RegisterStartupScript(Page, typeof(MaintenanceToast), "maintenanceToast", "showMaintenanceToast();", true);
        }
    }
}