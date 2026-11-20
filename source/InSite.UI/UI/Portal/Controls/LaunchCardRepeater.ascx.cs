using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;
using InSite.Domain.Foundations;
using InSite.Persistence.Content;

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

            CommonScript.ContentKey = typeof(LaunchCardRepeater).FullName;
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
            if (!IsContentItem(e))
                return;

            // Get the UI controls required to render a Bootstrap card...

            var flag = (ITextControl)e.Item.FindControl("Flag");
            var image = (ITextControl)e.Item.FindControl("Image");
            var icon = (ITextControl)e.Item.FindControl("Icon");
            var title = (HtmlGenericControl)e.Item.FindControl("Title");
            var inlineFlag = (HtmlGenericControl)e.Item.FindControl("InlineFlag");
            var summary = (HtmlGenericControl)e.Item.FindControl("Summary");
            var progress = (HtmlGenericControl)e.Item.FindControl("Progress");

            // ... then get the data object and bind its properties to the UI.

            var card = (LaunchCard)e.Item.DataItem;

            title.InnerText = card.Title;

            summary.InnerText = card.Summary;
            summary.Visible = !string.IsNullOrEmpty(card.Summary);

            var hasImage = card.HasImage();
            var hasFlag = card.HasFlag();

            if (hasImage)
                image.Text = card.GetImageHtml();

            else if (card.HasIcon())
                icon.Text = card.GetIconHtml();

            if (hasFlag)
            {
                // A floating badge sits over the card image. Without an image it lands on the
                // title instead, so a fee, which the learner has to be able to read, moves into
                // the card body below the title. Other flags keep their floating position.

                var floating = hasImage || !card.FlagIsFee;

                if (floating)
                {
                    flag.Text = card.GetFlagHtml(true);
                }
                else
                {
                    inlineFlag.InnerHtml = card.GetFlagHtml(false);
                    inlineFlag.Visible = true;
                }
            }

            if (card.HasProgress())
                progress.InnerHtml = card.GetProgressHtml();

            if (card.IsOverview)
            {
                var modal = (Modal)e.Item.FindControl("ModalOverview");
                var modalBody = (HtmlGenericControl)modal.FindContentControl("ModalOverviewBody");
                var content = ServiceLocator.ContentSearch.GetBlock(card.Identifier);

                modal.Visible = true;
                modal.Title = content.Title.GetText(Identity.Language, true).IfNullOrEmpty("Overview").MaxLength(30, true);
                modalBody.InnerHtml = content.Body.GetHtml(Identity.Language).IfNullOrEmpty(() => content.Body.GetHtml());
            }
        }
    }
}