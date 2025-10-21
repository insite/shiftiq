using System.Web.UI;

using InSite.Persistence;

namespace InSite.Admin.Standards.Collections.Controls
{
    public partial class DetailInfo : UserControl
    {
        #region Properties

        public string DefaultLanguage => "en";

        #endregion

        #region Getting and setting input values

        public void GetInputValues(Standard asset)
        {
            asset.ContentTitle = Title.Text;
            asset.StandardLabel = Label.Text;
        }

        public void SetInputValues(Standard asset)
        {
            Title.Text = asset.ContentTitle;
            Label.Text = asset.StandardLabel;
        }

        #endregion
    }
}