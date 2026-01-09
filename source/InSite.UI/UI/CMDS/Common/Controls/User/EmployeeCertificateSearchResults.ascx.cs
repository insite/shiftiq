using System;
using System.ComponentModel;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;

namespace InSite.Cmds.Controls.Profiles.EmployeeCertificates
{
    public partial class EmployeeCertificateSearchResults : SearchResultsGridViewController<EmployeeCertificateFilter>
    {
        protected override void OnRowCommand(GridViewRow row, string name, object argument)
        {
            if (name == "Grant")
            {
                var parts = ((string)argument).Split(';');
                Guid userKey = Guid.Parse(parts[0]);
                Guid profileStandardIdentifier = Guid.Parse(parts[1]);

                var entity = TCollegeCertificateSearch.Select(userKey, profileStandardIdentifier);
                entity.DateGranted = DateTime.UtcNow;

                TCollegeCertificateStore.Update(entity);

                SearchWithCurrentPageIndex(Filter);
            }
            else
            {
                base.OnRowCommand(row, name, argument);
            }
        }

        protected override int SelectCount(EmployeeCertificateFilter filter)
        {
            return ProfileCertificationRepository.CountByFilter(filter);
        }

        protected override IListSource SelectData(EmployeeCertificateFilter filter)
        {
            return ProfileCertificationRepository.SelectByFilter(filter);
        }
    }
}