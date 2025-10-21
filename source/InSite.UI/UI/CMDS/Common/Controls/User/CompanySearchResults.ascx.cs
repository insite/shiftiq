using System.ComponentModel;

using InSite.Common.Web.UI;
using InSite.Persistence.Plugin.CMDS;

namespace InSite.Cmds.Controls.Contacts.Companies
{
    public partial class CompanySearchResults : SearchResultsGridViewController<CompanyFilter>
    {
        protected override int SelectCount(CompanyFilter filter)
        {
            return ContactRepository3.CountSearchResults(filter);
        }

        protected override IListSource SelectData(CompanyFilter filter)
        {
            return ContactRepository3.SelectSearchResults(filter);
        }
    }
}