using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using Shift.Common;
using Shift.Constant;

namespace InSite.Application.Contacts.Read
{
    public interface IGroupSearch
    {
        // Groups

        QGroup GetGroup(Guid groupIdentifier, params Expression<Func<QGroup, object>>[] includes);

        bool GroupExists(QGroupFilter filter);
        int CountGroups(QGroupFilter filter);
        List<QGroup> GetGroups(QGroupFilter filter, params Expression<Func<QGroup, object>>[] includes);
        List<GroupSearchResult> SearchGroups(QGroupFilter filter);
        List<GroupExportResult> ExportGroups(QGroupFilter filter, bool empty);
        List<GroupDetail> SearchGroupDetails(QGroupFilter filter);
        List<T> BindGroups<T>(Expression<Func<QGroup, T>> binder, Expression<Func<QGroup, bool>> filter, string orderBy = null);

        int CountSelectorGroups(QGroupSelectorFilter filter);
        List<GroupSelectorItem> GetSelectorGroups(QGroupSelectorFilter filter, bool includeShippingAddress);

        List<SubgroupDataItem> GetSubgroups(Guid group);
        List<SupergroupDataItem> GetSupergroups(Guid organization, Guid group);
        List<GroupOutlineItem> GetGroupOutlineItems(Guid organization, string keyword);

        GroupRelationshipInfo[] GetGroupDescendentRelationships(Guid root);
        bool CausesCycle(Guid child, Guid parent);

        ICollection<CountModel> CountPerLabel(Guid organizationId);
        ICollection<CountModel> CountPerType(Guid organizationId);

        List<UserRoleItem> GetUserRoles(Guid organization, Guid[] groups, bool isPlatformAdministrator, Guid user);
        List<UserRoleItem> GetUserRoles(Guid organization, Guid user);

        List<string> GetGroupLabels(Guid organization, string groupType);
        List<string> GetGroupTags(Guid groupIdentifier);

        // Addresses

        QGroupAddress GetAddress(Guid groupIdentifier, AddressType type);
        List<QGroupAddress> GetAddresses(Guid groupIdentifier);

        // Connections

        int CountParentConnections(Guid childGroupIdentifier);
        List<QGroupConnection> GetParentConnections(Guid childGroupIdentifier, params Expression<Func<QGroupConnection, object>>[] includes);

        int CountChildConnections(Guid parentGroupIdentifier);
        List<QGroupConnection> GetChildConnections(Guid parentGroupIdentifier, params Expression<Func<QGroupConnection, object>>[] includes);

        string GetOrganizationCode(Guid organization);
    }
}
