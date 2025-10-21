using System;

using InSite.Common.Web.UI;

namespace InSite.UI.Admin.Courses.Outlines.Controls
{
    public partial class PrivacySettings : BaseUserControl
    {
        public void BindModelToControls(Guid container, string containerType)
        {
            PrivacySettingsGroups.LoadData(container, containerType);
        }
    }
}