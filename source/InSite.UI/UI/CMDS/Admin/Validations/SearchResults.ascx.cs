using System;
using System.ComponentModel;
using System.Text;

using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;

namespace InSite.Custom.CMDS.Admin.Standards.Validations.Controls
{
    public partial class SearchResults : SearchResultsGridViewController<StandardValidationFilter>
    {
        protected override int SelectCount(StandardValidationFilter filter)
            => StandardValidationSearch.Count(filter);

        protected override IListSource SelectData(StandardValidationFilter filter)
        {
            return StandardValidationSearch.Select(filter);
        }

        protected string GetStatusHtml(bool isValidated)
        {
            var html = new StringBuilder();

            if (isValidated)
                html.AppendLine("<span class='badge bg-custom-default'>Validated</span>");

            return html.ToString();
        }

        protected string GetLocalTime(DateTimeOffset? date)
            => date.Format(User.TimeZone, true, true);
    }
}