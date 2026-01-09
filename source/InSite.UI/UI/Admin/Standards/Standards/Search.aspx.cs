using System;

using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Contract;

namespace InSite.UI.Admin.Standards.Standards
{
    public partial class Search : SearchPage<StandardFilter>
    {
        public override string EntityName => "Standard";

        private string DefaultSubType => Request["type"];

        private bool IsOverrideCriteria => Request.QueryString["override-criteria"] == "1";

        protected override bool LoadSavedFilter()
        {
            var result = base.LoadSavedFilter();

            var filter = new StandardFilter
            {
                OrganizationIdentifier = Request["owner"].IsNotEmpty() ? Guid.Parse(Request["owner"]) : (Guid?)null,
            };

            if (!string.IsNullOrEmpty(DefaultSubType))
                filter.StandardTypes = new[] { DefaultSubType };

            if (filter.OrganizationIdentifier == null && filter.StandardTypes.IsEmpty())
                return result;

            if (result)
            {
                var savedFilter = SearchResults.Filter;

                foreach (var columnName in savedFilter.ShowColumns)
                    filter.ShowColumns.Add(columnName);

                filter.AdvancedOptionsVisible = savedFilter.AdvancedOptionsVisible;
            }

            SearchResults.SaveSearch(filter, false);

            SearchResults.Filter = filter;

            return true;
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (!IsPostBack && IsOverrideCriteria)
                LoadSearchedResults();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PageHelper.AutoBindHeader(this, new BreadcrumbItem("Add New Standard", "/ui/admin/standards/create", null, null));
        }
    }
}