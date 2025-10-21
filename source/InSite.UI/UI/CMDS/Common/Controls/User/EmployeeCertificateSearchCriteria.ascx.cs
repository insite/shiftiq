using InSite.Common.Web.UI;
using InSite.Persistence.Plugin.CMDS;

namespace InSite.Cmds.Controls.Profiles.EmployeeCertificates
{
    public partial class EmployeeCertificateSearchCriteria : SearchCriteriaController<EmployeeCertificateFilter>
    {
        public override EmployeeCertificateFilter Filter
        {
            get
            {
                var filter = new EmployeeCertificateFilter
                {
                    EmployeeName = EmployeeName.Text,
                    ProfileTitle = ProfileTitle.Text
                };

                GetCheckedShowColumns(filter);

                return filter;
            }
            set
            {
                EmployeeName.Text = value.EmployeeName;
                ProfileTitle.Text = value.ProfileTitle;
            }
        }

        public override void Clear()
        {
            EmployeeName.Text = null;
            ProfileTitle.Text = null;
        }
    }
}