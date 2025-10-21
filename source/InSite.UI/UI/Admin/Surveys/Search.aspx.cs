using System;

using InSite.Application.Surveys.Read;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

using Shift.Contract;

namespace InSite.UI.Admin.Surveys
{
    public partial class Search : SearchPage<QSurveyFormFilter>
    {
        public override string EntityName => "Survey Form";

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (!IsPostBack && !SearchResults.HasRows && SearchCriteria.DefaultStatusID.HasValue)
                SearchResults.Search(SearchCriteria.Filter);
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PageHelper.AutoBindHeader(this, new BreadcrumbItem("Add New Survey", "/ui/admin/surveys/forms/create-survey-form", null, null));
        }
    }
}