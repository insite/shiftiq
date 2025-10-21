using System;
using System.Collections.Generic;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using InSite.Domain.Foundations;
using InSite.Persistence.Content;
using InSite.Web.Helpers;

using Shift.Common;

namespace InSite.UI.Portal.Controls
{
    public partial class LaunchCardRepeater : Common.Web.UI.BaseUserControl
    {
        public int ColumnSize
        {
            get => (int?)ViewState[nameof(ColumnSize)] ?? 4;
            set => ViewState[nameof(ColumnSize)] = value;
        }

        public Guid? PortalIdentifier
        {
            get => (Guid?)ViewState[nameof(PortalIdentifier)];
            set => ViewState[nameof(PortalIdentifier)] = value;
        }

        public string PortalBodyHtml
        {
            get => (string)ViewState[nameof(PortalBodyHtml)];
            set => ViewState[nameof(PortalBodyHtml)] = value;
        }

        public string PortalSlug
        {
            get => (string)ViewState[nameof(PortalSlug)];
            set => ViewState[nameof(PortalSlug)] = value;
        }

        public List<LaunchCard> Cards { get; set; }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            CardRepeater.ItemDataBound += CardRepeater_ItemDataBound;
        }

        public void BindModelToControls(List<LaunchCard> cards, ISecurityFramework identity)
        {
            Cards = cards;
            CardRepeater.DataSource = Cards;
            CardRepeater.DataBind();
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            SummaryPanel.Visible = PortalBodyHtml.IsNotEmpty();
            SummaryPanel.InnerHtml = PortalBodyHtml;
        }

        private void CardRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!RepeaterHelper.IsContentItem(e.Item))
                return;

            // Get the UI controls required to render a Bootstrap card...

            var flag = (Literal)e.Item.FindControl("Flag");
            var image = (Literal)e.Item.FindControl("Image");
            var icon = (Literal)e.Item.FindControl("Icon");
            var title = (HtmlGenericControl)e.Item.FindControl("Title");
            var summary = (HtmlGenericControl)e.Item.FindControl("Summary");
            var progress = (HtmlGenericControl)e.Item.FindControl("Progress");

            // ... then get the data object and bind its properties to the UI.

            var card = (LaunchCard)e.Item.DataItem;

            title.InnerText = card.Title;

            summary.InnerText = card.Summary;
            summary.Visible = !string.IsNullOrEmpty(card.Summary);

            if (card.HasImage())
                image.Text = card.GetImageHtml();

            else if (card.HasIcon())
                icon.Text = card.GetIconHtml();

            if (card.HasFlag())
                flag.Text = card.GetFlagHtml();

            if (card.HasProgress())
                progress.InnerHtml = card.GetProgressHtml();
        }
    }
}