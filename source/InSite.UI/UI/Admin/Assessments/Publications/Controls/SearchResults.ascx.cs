using System;
using System.ComponentModel;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Application.Sites.Read;
using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common.Linq;

namespace InSite.UI.Admin.Assessments.Publications.Controls
{
    public partial class SearchResults : SearchResultsGridViewController<VAssessmentPageFilter>
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Grid.RowDataBound += Grid_RowDataBound;
        }

        private void Grid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (!IsContentItem(e.Row.RowType))
                return;

            var pageId = (Guid)DataBinder.Eval(e.Row.DataItem, "PageIdentifier");

            var groups = TGroupPermissionSearch
                .GetByObjectId(pageId, x => x.Group)
                .Select(x => new
                {
                    GroupName = x.Group.GroupName,
                    GroupType = x.Group.GroupType
                })
                .OrderBy(x => x.GroupName)
                .ToList();

            if (groups.Count == 0)
                return;

            var groupRepeater = (Repeater)e.Row.FindControl("GroupRepeater");
            groupRepeater.DataSource = groups;
            groupRepeater.DataBind();
        }

        protected override int SelectCount(VAssessmentPageFilter filter)
        {
            return ServiceLocator.PageSearch.Count(filter);
        }

        protected override IListSource SelectData(VAssessmentPageFilter filter)
        {
            var pages = ServiceLocator.PageSearch.Select(filter);
                
            return pages.ToSearchResult();
        }

        private ReturnUrl _returnUrl;

        protected string GetRedirectUrl(string url)
        {
            if (_returnUrl == null)
                _returnUrl = new ReturnUrl();

            return _returnUrl.GetRedirectUrl(url, "panel=results");
        }

        protected static string GetFormLink(object o)
        {
            var page = (VAssessmentPage)o;
            return $"/ui/admin/assessments/banks/outline?bank={page.BankIdentifier}&form={page.FormIdentifier}";
        }
    }
}