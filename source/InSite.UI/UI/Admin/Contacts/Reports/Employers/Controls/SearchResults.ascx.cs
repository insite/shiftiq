using System.ComponentModel;

using InSite.Common.Web.UI;
using InSite.Persistence;

namespace InSite.Admin.Contacts.Reports.Employers.Controls
{
    public partial class SearchResults : SearchResultsGridViewController<EmployerFilter>
    {
        protected override int SelectCount(EmployerFilter filter)
        {
            return EmployerRepository.CountByEmployerFilter(filter);
        }

        protected override IListSource SelectData(EmployerFilter filter)
        {
            return EmployerRepository.SelectByEmployerFilter(filter);
        }
    }
}