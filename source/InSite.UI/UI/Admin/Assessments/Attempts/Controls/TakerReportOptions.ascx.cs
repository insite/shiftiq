using System.Web.UI;

namespace InSite.UI.Admin.Assessments.Attempts.Controls
{
    public partial class TakerReportOptions : UserControl
    {
        public TakerReportControl.Language Language
        {
            get
            {
                return EnglishLanguage.Checked ? TakerReportControl.Language.English : TakerReportControl.Language.French;
            }
        }
    }
}