using InSite.Common.Web.UI;
using InSite.Persistence;

namespace InSite.UI.Admin.Contacts.Reports.Employers
{
    public partial class Search : SearchPage<EmployerFilter>
    {
        public override string EntityName => "Employer";
    }
}