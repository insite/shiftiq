using System;
using System.ComponentModel;

using InSite.Common.Web.UI;
using InSite.Persistence.Plugin.CMDS;

namespace InSite.Custom.CMDS.User.Progressions.Controls
{
    public partial class EmployeeCertificateGrid : SearchResultsGridViewController<EmployeeCertificateFilter>
    {
        public void LoadData(Guid employeeID)
        {
            EmployeeCertificateFilter filter = new EmployeeCertificateFilter
            {
                UserIdentifier = employeeID
            };

            Search(filter);
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