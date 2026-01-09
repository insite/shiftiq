using InSite.Common.Web.UI;

using InSite.Persistence;

namespace InSite.Admin.Accounts.Organizations.Controls
{
    public partial class SearchCriteria : SearchCriteriaController<OrganizationFilter> 
    {
        public override OrganizationFilter Filter
        {
            get
            {
                var filter = new OrganizationFilter
                {
                    CompanyDomain = DomainName.Text,
                    OrganizationCode = Code.Text,
                    CompanyName = CompanyName.Text,
                    IsClosed = IsClosed.ValueAsBoolean
                };

                GetCheckedShowColumns(filter);

                return filter;
            }
            set
            {
                DomainName.Text = value.CompanyDomain;
                Code.Text = value.OrganizationCode;
                CompanyName.Text = value.CompanyName;
                IsClosed.ValueAsBoolean = value.IsClosed;
            }
        }

        public override void Clear()
        {
            DomainName.Text = null;
            Code.Text = null;
            CompanyName.Text = null;
            IsClosed.ClearSelection();
        }
    }
}