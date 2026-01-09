using InSite.Common.Web.UI;
using InSite.Persistence;

namespace InSite.UI.Admin.Records.AchievementLayouts
{
    public partial class SearchCriteria : SearchCriteriaController<TCertificateLayoutFilter>
    {
        public override TCertificateLayoutFilter Filter
        {
            get
            {
                var filter = new TCertificateLayoutFilter
                {
                    OrganizationIdentifier = Organization.Identifier,
                    Code = CertificateLayoutCode.Text,
                    Data = CertificateLayoutData.Text,
                };

                GetCheckedShowColumns(filter);

                return filter;
            }
            set
            {
                CertificateLayoutCode.Text = value.Code;
                CertificateLayoutData.Text = value.Data;
            }
        }

        public override void Clear()
        {
            CertificateLayoutCode.Text = null;
            CertificateLayoutData.Text = null;
        }
    }
}