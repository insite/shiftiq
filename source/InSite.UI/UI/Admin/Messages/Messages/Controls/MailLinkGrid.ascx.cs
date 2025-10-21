using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Application.Messages.Write;
using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common.Linq;

namespace InSite.Admin.Messages.Messages.Controls
{
    public partial class MailLinkGrid : SearchResultsGridViewController<MessageLinkFilter>
    {
        protected override bool IsFinder => false;

        public event EventHandler Refreshed;
        private void OnRefreshed() => Refreshed?.Invoke(this, EventArgs.Empty);

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            Grid.RowDataBound += Grid_ItemDataBound;
            Grid.RowDeleting += Grid_DeleteCommand;
        }

        private void Grid_ItemDataBound(object sender, GridViewRowEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var testLink = (HyperLink)e.Row.FindControl("TestLink");
            testLink.NavigateUrl = MessageHelper.GetLinkUrl(
                (Guid)DataBinder.Eval(e.Row.DataItem, "LinkIdentifier"),
                User.UserIdentifier.ToString(),
                Common.Web.HttpRequestHelper.CurrentRootUrl);
        }

        private void Grid_DeleteCommand(object sender, GridViewDeleteEventArgs e)
        {
            var grid = (Grid)sender;
            var linkId = grid.GetDataKey<Guid>(e);

            var email = ServiceLocator.MessageSearch.FindLink(linkId);

            ServiceLocator.SendCommand(new ResetLinkCounter(email.MessageIdentifier, linkId));

            SearchWithCurrentPageIndex(Filter);

            OnRefreshed();
        }

        protected override void OnDataStateChanged(bool hasData)
        {
            base.OnDataStateChanged(hasData);

            NoRecordMessage.Visible = !hasData;
        }

        public void LoadData(Guid messageIdentifier, bool allowEdit)
        {
            Grid.Columns.FindByName("ActionColumn").Visible = allowEdit;

            Search(new MessageLinkFilter { MessageIdentifier = messageIdentifier });
        }

        protected override int SelectCount(MessageLinkFilter filter)
        {
            return ServiceLocator.MessageSearch
                .CountLinks(filter.MessageIdentifier.Value);
        }

        protected override IListSource SelectData(MessageLinkFilter filter)
        {
            return ServiceLocator.MessageSearch
                .FindLinks(filter.MessageIdentifier.Value)
                .ToSearchResult();
        }
    }
}