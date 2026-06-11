using System;
using System.Drawing;
using System.Linq;
using System.Web.UI;

using InSite.Persistence;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace InSite.UI.Admin.Records.AchievementLayouts
{
    public partial class Details : UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
                return;

            string[] fontNames = FontFamily.Families
                .Select(family => family.Name)
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .OrderBy(name => name)
                .ToArray();

            AvailableFonts.Text = string.Join(Environment.NewLine, fontNames);
        }

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
