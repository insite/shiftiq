using System;
using System.ComponentModel;

using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;

namespace InSite.Admin.Integrations.Controls
{
    public partial class SearchResults : SearchResultsGridViewController<ApiRequestFilter>
    {
        protected override int SelectCount(ApiRequestFilter filter)
        {
            return ApiRequestSearch.Count(filter);
        }

        protected override IListSource SelectData(ApiRequestFilter filter)
        {
            return ApiRequestSearch.SelectByFilter(filter);
        }

        protected static string GetLocalTime(DateTimeOffset requestStarted)
        {
            return requestStarted.Format(User.TimeZone, true, false, true);
        }

        protected static string GetRelativeURL(string requestUri)
        {
            var uri = new Uri(requestUri);
            return uri.AbsolutePath;
        }
    }
}