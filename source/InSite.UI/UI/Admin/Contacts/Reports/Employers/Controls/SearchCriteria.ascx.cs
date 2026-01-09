using InSite.Common.Web.UI;
using InSite.Persistence;

namespace InSite.Admin.Contacts.Reports.Employers.Controls
{
    public partial class SearchCriteria : SearchCriteriaController<EmployerFilter>
    {
        public override EmployerFilter Filter
        {
            get
            {
                var filter = new EmployerFilter
                {
                    OrganizationIdentifier = Organization.OrganizationIdentifier,

                    EmployerName = EmployerName.Text
                };

                GetCheckedShowColumns(filter);

                filter.SortByColumn = SortColumns.Value;

                return filter;
            }
            set
            {
                EmployerName.Text = value.EmployerName;

                SortColumns.Value = value.SortByColumn;
            }
        }

        public override void Clear()
        {
            EmployerName.Text = null;
        }
    }
}