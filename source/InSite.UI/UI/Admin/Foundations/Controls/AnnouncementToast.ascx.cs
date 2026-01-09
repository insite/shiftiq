using System;

using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.Admin.Programs.Controls
{
    public partial class AnnouncementToast : AdminBaseControl
    {
        public bool Enabled
            => !string.IsNullOrEmpty(Organization.AccountWarning);

        protected void Page_Load(object sender, EventArgs e)
        {
            AnnouncementHtml.InnerHtml = Markdown.ToHtml(Organization.AccountWarning);

            Visible = Enabled;
            if (Visible)
                System.Web.UI.ScriptManager.RegisterStartupScript(Page, typeof(AnnouncementToast), "announcementToast", "showAnnouncementToast();", true);
        }
    }
}