using System;
using System.ComponentModel;
using System.Web.UI;

using InSite.Common.Web.UI;
using InSite.Domain.Messages;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Admin.Messages.Mailouts.Controls
{
    public partial class SearchResults : SearchResultsGridViewController<MailoutFilter>
    {
        protected override int SelectCount(MailoutFilter filter)
        {
            return ServiceLocator.MessageSearch.CountMailouts(filter);
        }

        protected override IListSource SelectData(MailoutFilter filter)
        {
            return ServiceLocator.MessageSearch.GetMailouts(filter).ToSearchResult();
        }

        protected string GetLocalTime(string name)
        {
            var dataItem = Page.GetDataItem();
            var value = (DateTimeOffset?)DataBinder.Eval(dataItem, name);
            return value.Format(User.TimeZone, true);
        }
    }
}