using System;
using System.Linq;
using System.Web.UI;

using InSite.UI.Admin.Reports.Changes.Models;
using InSite.UI.Layout.Admin;

using Shift.Sdk.UI;

namespace InSite.Admin.Reports.Changes.Forms
{
    public partial class ViewHistory : AdminBasePage
    {
        private Guid? EntityID => Guid.TryParse(Request.QueryString["id"], out var result) ? result : (Guid?)null;

        private string EntityType => Request.QueryString["type"];

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            if (EntityID == null)
            {
                CloseWindow();
                return;
            }

            HistoryCollection history;

            if (EntityType == "user")
            {
                history = HistoryReader.ReadUser(EntityID.Value, Organization.Identifier);
            }
            else if (EntityType == "user_membership")
            {
                history = HistoryReader.ReadUserMembership(EntityID.Value, Organization.Identifier);
            }
            else if (EntityType == "group_membership")
            {
                history = HistoryReader.ReadGroupMembership(EntityID.Value, Organization.Identifier);
            }
            else
            {
                CloseWindow();
                return;
            }

            HistoryRepeater.DataSource = history.Select(x => new HtmlDataItem(x, User.TimeZone));
            HistoryRepeater.DataBind();
        }

        private void CloseWindow()
        {
            ScriptManager.RegisterStartupScript(
                Page,
                typeof(ViewHistory),
                "close_window",
                "modalManager.closeModal();",
                true
            );
        }
    }
}