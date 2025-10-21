using System.Web.UI;

using InSite.Persistence;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace InSite.UI.Admin.Records.AchievementLayouts
{
    public partial class Details : UserControl
    {
        public void SetInputValues(TCertificateLayout layout)
        {
            CertificateLayoutCode.Text = layout.CertificateLayoutCode;
            CertificateLayoutData.Text = JToken.Parse(layout.CertificateLayoutData).ToString(Formatting.Indented);
        }

        public bool GetInputValues(TCertificateLayout layout)
        {
            try
            {
                _ = JToken.Parse(CertificateLayoutData.Text);
            }
            catch
            {
                return false;
            }
            layout.CertificateLayoutCode = CertificateLayoutCode.Text;
            layout.CertificateLayoutData = CertificateLayoutData.Text;
            return true;
        }
    }
}