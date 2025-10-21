using System;
using System.Web;

using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Contract;

namespace InSite.UI.Admin.Jobs.Opportunities
{
    public partial class Search : SearchPage<TOpportunityFilter>
    {
        private string DefaultType
        {
            get
            {
                var type = Request.QueryString["type"];
                if (!string.IsNullOrEmpty(type))
                    return HttpUtility.UrlDecode(type);
                return null;
            }
        }

        public bool HasDefaultCriteria => !string.IsNullOrEmpty(DefaultType);
        public override string EntityName => "Job opportunities";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            string addUrl, addTitle;

            if (HasDefaultCriteria)
            {
                addUrl = $"/ui/admin/jobs/opportunities/create?type={DefaultType}";
                addTitle = $"Add New {DefaultType}";
            }
            else
            {
                addUrl = $"/ui/admin/jobs/opportunities/create";
                addTitle = $"Add New Job Opportunity";
            }

            PageHelper.AutoBindHeader(this, new BreadcrumbItem(addTitle, addUrl, null, null));
        }
    }
}