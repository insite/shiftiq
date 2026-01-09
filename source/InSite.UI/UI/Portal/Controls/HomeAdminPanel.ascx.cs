using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Individual
{
    public partial class HomeAdminPanel : BaseUserControl
    {
        private class TileInfo
        {
            public Guid Identifier { get; set; }

            public int CardKey { get; set; }

            public string Url { get; set; }
            public string Target { get; set; }
            public string Title { get; set; }
            public string Indicator { get; set; }
            public string Icon { get; set; }
            public string Description { get; set; }

            public string OnClick { get; set; }
        }

        private AdminHomeSettings _adminSettings;

        private AdminHomeSettings AdminSettings =>
            _adminSettings ?? (_adminSettings =
                PersonalizationRepository.GetValue<AdminHomeSettings>(Guid.Empty, CurrentSessionState.Identity.User.UserIdentifier,
                    PersonalizationName.AdminHome, false) ?? new AdminHomeSettings());

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            AdminRepeater.DataBinding += AdminRepeater_DataBinding;
            AdminRepeater.ItemDataBound += AdminRepeater_ItemDataBound;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            AdminRepeater.DataBind();
            ((NavItem)Parent).Visible = AdminRepeater.Items.Count > 0;
        }

        private void AdminRepeater_DataBinding(object sender, EventArgs e)
        {
            var tiles = GetAdminTiles();

            foreach (var tile in tiles)
                tile.Title = Translate(tile.Title);

            AdminRepeater.DataSource = tiles;
        }

        private IList<TileInfo> GetAdminTiles()
        {
            var identity = CurrentSessionState.Identity;

            var isE03 = ServiceLocator.Partition.IsE03();

            var routes = TActionSearch.Search(
                x => x.IsToolkitHomePage
                 && (!x.ActionUrl.StartsWith("portal/") || x.ActionUrl == "portal/programs/home" || x.ActionUrl == RelativeUrl.PortalHomeUrl))
                .Select(x => new TileInfo
                {
                    Url = $"/{x.ActionUrl}",
                    Title = x.ActionNameShort ?? x.ActionName,
                    Icon = x.ActionIcon
                })
                .OrderBy(x => x.Title)
                .ThenBy(x => x.Title);

            var tiles = routes
                .Where(tile =>
                    identity.IsActionAuthorized(tile.Url) &&
                    (tile.Url != "cmds/user/modules/search" || isE03))
                .ToList();

            return AdminSettings == null
                ? tiles
                : tiles.OrderBy(x => x.Title).ToList();
        }

        private void AdminRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var tile = (TileInfo)e.Item.DataItem;

                var icon = $"<i class='{tile.Icon} fa-3x mb-3'></i>";
                var summary = $"<p class='fs-sm text-body-secondary mb-2'>{tile.Description}</p>";

                var cardIcon = (Label)e.Item.FindControl("CardIcon");
                cardIcon.Text = icon;
                // if (tile.Indicator != null)
                //    cardIcon.CssClass = tile.Indicator;

                var cardTitle = (ITextControl)e.Item.FindControl("CardTitle");
                cardTitle.Text = $"<h3 class='h5 nav-heading mb-2 {cardIcon.CssClass}'>{tile.Title}</h3>";

                var cardSummary = e.Item.FindControl("CardSummary");
                ((ITextControl)cardSummary).Text = summary;
                cardSummary.Visible = tile.Description.HasValue();
            }
        }

        public static NavItem CreatePortalPanel(Page page)
        {
            var panel = new NavItem { Title = "Admin" };
            panel.Icon = "fas fa-cog";

            var content = (HomeAdminPanel)page.LoadControl("~/UI/Portal/Controls/HomeAdminPanel.ascx");
            panel.Controls.Add(content);

            return panel;
        }
    }
}