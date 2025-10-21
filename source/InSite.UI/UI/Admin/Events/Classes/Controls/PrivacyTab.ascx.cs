using System;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;
using InSite.Persistence;

namespace InSite.UI.Admin.Events.Classes.Controls
{
    public partial class PrivacyTab : BaseUserControl
    {
        public void BindControls(Guid eventId, bool canEdit)
        {
            BindGroups(eventId);

            ChangeLink.Visible = canEdit;
            ChangeLink.NavigateUrl = $"/ui/admin/events/classes/change-privacy?event={eventId}";
        }

        private void BindGroups(Guid eventId)
        {
            var groupPermissions = TGroupPermissionSearch
                .Bind(
                    x => new
                    {
                        Name = x.Group.GroupName,
                        Type = x.Group.GroupType
                    },
                    x => x.ObjectIdentifier == eventId
                )
                .OrderBy(x => x.Name)
                .ToList();

            GroupDataRepeaterFooter.Visible = groupPermissions.Count == 0;

            GroupDataRepeater.Visible = groupPermissions.Count > 0;
            GroupDataRepeater.DataSource = groupPermissions;
            GroupDataRepeater.DataBind();
        }
    }
}