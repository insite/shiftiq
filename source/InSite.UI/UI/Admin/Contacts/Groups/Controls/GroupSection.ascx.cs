using System.Linq;
using System.Web.UI;

using InSite.Application.Contacts.Read;
using InSite.Persistence;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Admin.Contacts.Groups.Controls
{
    public partial class GroupSection : UserControl
    {
        private const string None = "<i>None</i>";

        public void SetInputValues(QGroup group)
        {
            var isRole = group.GroupType == GroupTypes.Role;

            AddNewUsersAutomatically.Visible = CurrentSessionState.Identity.IsOperator;
            OfficePhonePanel.Visible = !isRole;

            GroupType.Text = group.GroupType;
            GroupLabel.Text = group.GroupLabel.IfNullOrEmpty(None);
            GroupName.Text = group.GroupName;
            GroupDescription.Text = group.GroupDescription.IfNullOrEmpty(None);
            GroupCode.Text = group.GroupCode.IfNullOrEmpty(None);
            GroupRegion.Text = group.GroupRegion.IfNullOrEmpty(None);
            Capacity.Text = group.GroupCapacity.HasValue ? group.GroupCapacity.ToString() : None;
            AddNewUsersAutomatically.Checked = group.AddNewUsersAutomatically;
            AllowSelfSubscription.Checked = group.AllowSelfSubscription;

            GroupCategory.Text = group.GroupCategory.IfNullOrEmpty(None);
            GroupStatus.Text = TCollectionItemCache.GetName(group.GroupStatusItemIdentifier).IfNullOrEmpty(None);

            GroupOffice.Text = group.GroupOffice.IfNullOrEmpty(None);
            GroupPhone.Text = group.GroupPhone.IfNullOrEmpty(None);

            var parent = group.ParentGroupIdentifier.HasValue
                ? ServiceLocator.GroupSearch.GetGroup(group.ParentGroupIdentifier.Value)
                : null;

            if (parent != null)
            {
                ParentLink.NavigateUrl = $"/ui/admin/contacts/groups/edit?contact={parent.GroupIdentifier}";
                ParentLink.Text = parent?.GroupName;
            }
            else
            {
                ParentLink.Visible = false;
                NoParent.Text = None;
            }

            ParentLink.Visible = group != null;
            OrganizeLink.NavigateUrl = $"/ui/admin/contacts/groups/organize?group={group.GroupIdentifier}";

            BindParentConnections(group);
            BindChildren(group);
            BindChildConnections(group);
        }

        private void BindParentConnections(QGroup group)
        {
            var parentContainments = ServiceLocator.GroupSearch
                .GetParentConnections(group.GroupIdentifier, x => x.ParentGroup)
                .Select(x => new { x.ParentGroup.GroupIdentifier, x.ParentGroup.GroupName })
                .OrderBy(x => x.GroupName)
                .ToArray();

            var hasParentContainments = parentContainments.Length > 0;

            ParentContainmentRepeater.Visible = hasParentContainments;
            ParentContainmentRepeater.DataSource = parentContainments;
            ParentContainmentRepeater.DataBind();

            if (!hasParentContainments)
            {
                ParentContainmentMessage.Visible = true;
                ParentContainmentMessage.Text = $"<p>There are no parent groups contains the {group.GroupType}.</p>";
            }
        }

        private void BindChildren(QGroup group)
        {
            var childrenGroups = ServiceLocator.GroupSearch
                .GetGroups(new QGroupFilter { ParentGroupIdentifier = group.GroupIdentifier })
                .Select(x => new { x.GroupIdentifier, x.GroupName })
                .OrderBy(x => x.GroupName)
                .ToArray();

            ChildrenRepeater.DataSource = childrenGroups;
            ChildrenRepeater.DataBind();

            ChildrenField.Visible = childrenGroups.Length > 0;
        }

        private void BindChildConnections(QGroup group)
        {
            var childContainments = ServiceLocator.GroupSearch
                .GetChildConnections(group.GroupIdentifier, x => x.ChildGroup)
                .Select(x => new { x.ChildGroup.GroupIdentifier, x.ChildGroup.GroupName })
                .OrderBy(x => x.GroupName)
                .ToArray();

            ChildrenContainmentRepeater.DataSource = childContainments;
            ChildrenContainmentRepeater.DataBind();
            ChildrenContainmentField.Visible = childContainments.Length > 0;
        }
    }
}