using InSite.Common.Web.UI;

namespace InSite.UI.Admin.Events.Classes.Controls
{
    public partial class ClassTabNav : BaseUserControl
    {
        public ClassSetup ClassSetup => ClassSetupControl;

        public ClassSettings ClassSettings => ClassSettingsControl;
    }
}