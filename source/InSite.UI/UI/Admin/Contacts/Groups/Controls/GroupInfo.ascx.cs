using System.Web.UI;

using InSite.Application.Contacts.Read;
using InSite.Persistence;

using Shift.Common;

namespace InSite.Admin.Contacts.Groups.Controls
{
    public partial class GroupInfo : UserControl
    {
        public void BindGroup(QGroup group)
        {
            GroupName.Text = group.GroupName;
            GroupLink.HRef = $"/ui/admin/contacts/groups/edit?contact={group.GroupIdentifier}";
            GroupLabel.Text = group.GroupLabel.IfNullOrEmpty("-");
            GroupType.Text = group.GroupType;
            GroupCode.Text = group.GroupCode.IfNullOrEmpty("-");
        }

        public void BindGroup(VGroupDetail group)
        {
            GroupName.Text = group.GroupName;
            GroupLink.HRef = $"/ui/admin/contacts/groups/edit?contact={group.GroupIdentifier}";
            GroupLabel.Text = group.GroupLabel.IfNullOrEmpty("-");
            GroupType.Text = group.GroupType;
            GroupCode.Text = group.GroupCode.IfNullOrEmpty("-");
        }
    }
}