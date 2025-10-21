using InSite.Persistence;

using Shift.Common;
using Shift.Constant;

namespace InSite.Common.Web.UI
{
    public class ReportComboBox : ComboBox
    {
        public VReportFilter ListFilter => (VReportFilter)(ViewState[nameof(ListFilter)]
            ?? (ViewState[nameof(ListFilter)] = new VReportFilter
            {
                OrganizationIdentifier = CurrentSessionState.Identity.Organization.Identifier,
                UserIdentifier = CurrentSessionState.Identity.User.UserIdentifier,
                ReportTypes = new[] { ReportType.Custom }
            }));

        protected override ListItemArray CreateDataSource()
        {
            var list = new ListItemArray();

            foreach (var item in VReportSearch.Select(ListFilter))
                list.Add(item.ReportIdentifier.ToString(), item.ReportTitle);

            return list;
        }
    }
}