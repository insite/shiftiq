using System;
using System.Linq;

using InSite.Common.Web.UI;

using Shift.Constant;

namespace InSite.UI.Admin.Reports.Controls
{
    public partial class PrivacyGroupManager : BaseUserControl
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            FindGroup.Filter.OrganizationIdentifier = Organization.Identifier;
            FindGroup.Filter.MembershipUserIdentifier = User.UserIdentifier;
        }

        public void SetFilter(string reportType)
        {
            FindGroup.Filter.OrganizationIdentifier = Organization.Identifier != OrganizationIdentifiers.Global || reportType != "Shared"
                ? (Guid?)Organization.Identifier
                : null;
        }

        public void LoadData(Guid reportId)
        {
            var ids = ServiceLocator.ContentSearch.SelectPrivacyGroup(x => x.ObjectIdentifier == reportId && x.ObjectType == "Report")
                .Select(x => x.GroupIdentifier)
                .ToArray();

            FindGroup.Values = ids;
        }

        public void SaveData(Guid reportId)
        {
            var groupIds = FindGroup?.Values;

            var existing = ServiceLocator.ContentSearch.SelectPrivacyGroup(x => x.ObjectIdentifier == reportId);

            var deletePrivacyIds = groupIds != null
                ? existing.Where(x => !groupIds.Contains(x.GroupIdentifier)).Select(x => x.PermissionIdentifier).ToArray()
                : existing.Select(x => x.PermissionIdentifier).ToArray();

            var insertGroupIds = groupIds != null
                ? groupIds.Where(x => !existing.Any(y => y.GroupIdentifier == x)).ToArray()
                : new Guid[0];

            foreach (var privacyId in deletePrivacyIds)
                ServiceLocator.ContentStore.DeletePrivacyGroup(privacyId);

            foreach (var groupId in insertGroupIds)
                ServiceLocator.ContentStore.InsertPrivacyGroup(reportId, "Report", groupId);
        }
    }
}