using InSite.Persistence;

namespace InSite.Admin.Standards.Standards.Controls
{
    public partial class StandardInfo : System.Web.UI.UserControl
    {
        public void BindStandard(Standard Asset)
        {
            StandardLink.HRef = $"/ui/admin/standards/edit?id={Asset.StandardIdentifier}";
            StandardTitle.Text = Asset.ContentTitle;
            StandardCode.Text = !string.IsNullOrEmpty(Asset.Code) ? Asset.Code : "None";
            StandardLabel.Text = !string.IsNullOrEmpty(Asset.StandardLabel) ? Asset.StandardLabel : "None";
        }
    }
}