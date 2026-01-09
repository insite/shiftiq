using System;
using System.Linq;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Shift.Common.Timeline.Registries;

using InSite.Application.Logs.Read;
using InSite.UI.Layout.Admin;

namespace InSite.UI.Admin.Logs
{
    public partial class Home : AdminBasePage
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            AggregateRepeater.ItemDataBound += AggregateRepeater_ItemDataBound;
        }

        private void AggregateRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var item = (IAggregateTypeInfo)e.Item.DataItem;

            var commands = (Repeater)e.Item.FindControl("CommandRepeater");
            commands.DataSource = item.Commands;
            commands.DataBind();

            var changes = (Repeater)e.Item.FindControl("ChangeRepeater");
            changes.DataSource = item.Changes;
            changes.DataBind();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!IsPostBack)
                BindModelToControls();
        }

        protected void BindModelToControls()
        {
            PageHelper.AutoBindHeader(this);

            var insite = CurrentSessionState.Identity.IsOperator;

            var commandCount = ServiceLocator.CommandSearch.Count(new CommandFilter { OriginOrganization = Organization.OrganizationIdentifier });
            LoadCounter(CommandCounter, CommandCount, insite, commandCount, CommandLink, "/ui/admin/logs/commands/search");

            var aggregates = TypeRegistry.GetAggregates();
            AggregateRepeater.DataSource = aggregates;
            AggregateRepeater.DataBind();

            var commands = TypeRegistry.GetCommands();
            var unregisteredCommands = commands.Where(c => !aggregates.Any(a => a.Commands.Contains(c))).ToList();
            UnregisteredCommandRepeater.DataSource = unregisteredCommands;
            UnregisteredCommandRepeater.DataBind();
            WarningPanel.Visible = unregisteredCommands.Count > 0;
        }

        public static void LoadCounter(HtmlGenericControl card, Literal counter, bool visible, int count, HtmlAnchor link, string action)
        {
            card.Visible = visible;
            link.HRef = action;
            counter.Text = $@"{count:n0}";
        }
    }
}