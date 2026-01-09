using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Shift.Common.Timeline.Changes;

using InSite.Domain;
using InSite.Domain.Contacts;

using Shift.Common;
using Shift.Sdk.UI;

namespace InSite.UI.Admin.Reports.Changes.Models
{
    static class MembershipChangeReader
    {
        #region Classes

        private class ChangeInfo
        {
            public Guid ChangeUserId { get; set; }
            public DateTime ChangeDate { get; set; }
            public string ChangeHeader { get; set; }
            public string ChangeDescription { get; set; }

            public Guid UserId { get; set; }
            public Guid GroupId { get; set; }
        }

        private class MembershipHistoryEntity : HistoryEntity
        {
            public Guid GroupId { get; }
            public string Header { get; private set; }

            private List<MembershipHistoryEntity> _thisGroupMerged = new List<MembershipHistoryEntity>();
            private List<MembershipHistoryEntity> _otherGroupMerged = new List<MembershipHistoryEntity>();

            public MembershipHistoryEntity(DateTime date, string userName, Guid groupId, string header, string description)
                : base(date, userName, description)
            {
                GroupId = groupId;
                Header = header;
            }

            public override void MergeWith(HistoryEntity other)
            {
                var source = (MembershipHistoryEntity)other;
                if (source.GroupId == GroupId)
                {
                    source.Header = null;
                    _thisGroupMerged.Add(source);
                }
                else
                {
                    var otherMerge = _otherGroupMerged.FirstOrDefault(x => x.GroupId == source.GroupId);
                    if (otherMerge == null)
                        _otherGroupMerged.Add(source);
                    else
                        otherMerge.MergeWith(source);
                }
            }

            public override string BuildDescription()
            {
                var result = Description;

                if (Header.IsNotEmpty())
                    result = Header + Environment.NewLine + result;

                foreach (var m in _thisGroupMerged)
                    result = MergeDescription(result, m.BuildDescription());

                foreach (var m in _otherGroupMerged)
                    result = MergeDescription(result, m.BuildDescription());

                return result;
            }
        }

        #endregion

        #region User

        private class GroupInfo
        {
            public Guid GroupId { get; set; }
            public Guid OrganizationId { get; set; }
            public string GroupName { get; set; }
        }

        public static void ReadUser(Guid userId, Guid organizationId, HistoryCollection history)
        {
            var changes = GetUserChanges(userId);
            var users = ReaderHelper.GetUsers(changes.Select(x => x.ChangeUserId).Distinct());
            var groups = GetGroups(changes.Select(x => x.GroupId).Distinct().ToArray());

            foreach (var change in changes)
            {
                var group = groups[change.GroupId];
                if (group != null && group.OrganizationId != organizationId)
                    continue;

                change.ChangeHeader = change.ChangeHeader.Replace("{GROUP_NAME}", group?.GroupName ?? "Unknown");

                if (!users.TryGetValue(change.ChangeUserId, out var userFullName))
                    userFullName = "Someone";

                history.Add(new MembershipHistoryEntity(change.ChangeDate, userFullName, change.GroupId, change.ChangeHeader, change.ChangeDescription));
            }
        }

        private static Dictionary<Guid, GroupInfo> GetGroups(Guid[] groupIds)
        {
            var result = new Dictionary<Guid, GroupInfo>();

            var groups = ServiceLocator.GroupSearch
                .BindGroups(
                    x => new GroupInfo
                    {
                        GroupId = x.GroupIdentifier,
                        OrganizationId = x.OrganizationIdentifier,
                        GroupName = x.GroupName,
                    },
                    x => groupIds.Contains(x.GroupIdentifier)
                )
                .ToDictionary(x => x.GroupId, x => x);

            foreach (var id in groupIds)
            {
                var info = groups.GetOrDefault(id);

                if (info == null)
                {
                    var aggregate = ServiceLocator.SnapshotRepository.Get<GroupAggregate>(id);
                    if (aggregate != null)
                    {
                        var state = aggregate.Data;
                        info = new GroupInfo
                        {
                            GroupId = id,
                            GroupName = state.Name,
                            OrganizationId = state.Tenant
                        };
                    }
                }

                result.Add(id, info);
            }

            return result;
        }

        private static ChangeInfo[] GetUserChanges(Guid userId)
        {
            var result = new List<ChangeInfo>();

            var membershipIds = ServiceLocator.MembershipSearch.GetUserAllMembershipIds(userId);
            foreach (var membershipId in membershipIds)
                DescribeUserChanges(membershipId, result);

            return result.ToArray();
        }

        private static void DescribeUserChanges(Guid membershipId, List<ChangeInfo> result)
        {
            var data = ServiceLocator.SnapshotRepository.GetStatesAndChanges(membershipId);
            var groupId = Guid.Empty;
            MembershipState prevState = null;

            foreach (var item in data)
            {
                var change = DescribeUserChange(item);
                var state = (MembershipState)item.AggregateState;
                var description = DescribeFieldChanges(prevState, state);

                if (change.GroupId.HasValue)
                    groupId = change.GroupId.Value;

                if (change.Header != null || description.Length > 0)
                    result.Add(new ChangeInfo
                    {
                        GroupId = groupId,
                        ChangeUserId = item.OriginUser,
                        ChangeDate = item.ChangeTime.UtcDateTime,
                        ChangeHeader = change.Header ?? "- Modified membership for **{GROUP_NAME}**",
                        ChangeDescription = description
                    });

                prevState = state;
            }
        }

        private static (Guid? GroupId, string Header) DescribeUserChange(IChange change)
        {
            if (change is MembershipStarted started)
                return (started.Group, "- Added to **{GROUP_NAME}**");
            else if (change is MembershipEnded ended)
                return (null, "- Removed from **{GROUP_NAME}**");
            else if (change is MembershipResumed resumed)
                return (resumed.Group, "- Restored membership in **{GROUP_NAME}**");

            return (null, null);
        }

        #endregion

        #region Group

        private class UserInfo
        {
            public Guid UserId { get; set; }
            public string FullName { get; set; }
            public bool HasPerson { get; set; }
        }

        public static void ReadGroup(Guid groupId, Guid organizationId, HistoryCollection history)
        {
            var changes = GetGroupChanges(groupId);
            var changeUsers = ReaderHelper.GetUsers(changes.Select(x => x.ChangeUserId).Distinct());
            var membershipUsers = GetUsers(organizationId, changes.Select(x => x.UserId).Distinct().ToArray());

            foreach (var change in changes)
            {
                var membershipUser = membershipUsers[change.UserId];
                if (membershipUser?.HasPerson == false)
                    continue;

                change.ChangeHeader = change.ChangeHeader.Replace("{USER_NAME}", membershipUser?.FullName ?? "Unknown");

                if (!changeUsers.TryGetValue(change.ChangeUserId, out var changeUserName))
                    changeUserName = "Someone";

                history.Add(new MembershipHistoryEntity(change.ChangeDate, changeUserName, change.GroupId, change.ChangeHeader, change.ChangeDescription));
            }
        }

        private static Dictionary<Guid, UserInfo> GetUsers(Guid organizationId, Guid[] userIds)
        {
            var result = new Dictionary<Guid, UserInfo>();

            var groups = ServiceLocator.ContactSearch
                .Bind(
                    x => new UserInfo
                    {
                        UserId = x.UserIdentifier,
                        HasPerson = x.Persons.Any(y => y.OrganizationIdentifier == organizationId),
                        FullName = x.UserFullName
                    },
                    x => userIds.Contains(x.UserIdentifier)
                )
                .ToDictionary(x => x.UserId, x => x);

            foreach (var id in userIds)
            {
                var info = groups.GetOrDefault(id);

                if (info == null)
                {
                    var aggregate = ServiceLocator.SnapshotRepository.Get<UserAggregate>(id);
                    if (aggregate != null)
                    {
                        var state = aggregate.Data;
                        info = new UserInfo
                        {
                            UserId = id,
                            HasPerson = true,
                            FullName = state.GetTextValue(UserField.FullName)
                        };
                    }
                }

                result.Add(id, info);
            }

            return result;
        }

        private static ChangeInfo[] GetGroupChanges(Guid groupId)
        {
            var result = new List<ChangeInfo>();

            var membershipIds = ServiceLocator.MembershipSearch.GetGroupAllMembershipIds(groupId);
            foreach (var membershipId in membershipIds)
                DescribeGroupChanges(membershipId, result);

            return result.ToArray();
        }

        private static void DescribeGroupChanges(Guid membershipId, List<ChangeInfo> result)
        {
            var data = ServiceLocator.SnapshotRepository.GetStatesAndChanges(membershipId);
            var userId = Guid.Empty;
            MembershipState prevState = null;

            foreach (var item in data)
            {
                var change = DescribeGroupChange(item);
                var state = (MembershipState)item.AggregateState;
                var description = DescribeFieldChanges(prevState, state);

                if (change.UserId.HasValue)
                    userId = change.UserId.Value;

                if (change.Header != null || description.Length > 0)
                    result.Add(new ChangeInfo
                    {
                        UserId = userId,
                        ChangeUserId = item.OriginUser,
                        ChangeDate = item.ChangeTime.UtcDateTime,
                        ChangeHeader = change.Header ?? "- Modified membership for **{USER_NAME}**",
                        ChangeDescription = description
                    });

                prevState = state;
            }
        }

        private static (Guid? UserId, string Header) DescribeGroupChange(IChange change)
        {
            if (change is MembershipStarted started)
                return (started.User, "- Added **{USER_NAME}**");
            else if (change is MembershipEnded ended)
                return (null, "- Removed **{USER_NAME}**");
            else if (change is MembershipResumed resumed)
                return (resumed.User, "- Restored **{USER_NAME}**");

            return (null, null);
        }

        #endregion

        #region Field

        private static string DescribeFieldChanges(MembershipState prevState, MembershipState state)
        {
            var description = new StringBuilder();

            DescribeFieldChanges(prevState, state, description);

            return description.ToString();
        }

        private static void DescribeFieldChanges(MembershipState prevState, MembershipState state, StringBuilder buffer)
        {
            DescribeFieldChange("Effective", prevState?.Effective, state.Effective, buffer);
            DescribeFieldChange("Function", prevState?.Function, state.Function, buffer);
            DescribeFieldChange("Membership Expiry", prevState?.Expiry, state.Expiry, buffer);
        }

        private static void DescribeFieldChange(string field, string valueBefore, string valueAfter, StringBuilder buffer)
        {
            if (valueBefore.NullIfEmpty() != valueAfter.NullIfEmpty())
                ReaderHelper.AddLine(field, StateFieldType.Text, valueBefore, valueAfter, buffer, "    ");
        }

        private static void DescribeFieldChange(string field, DateTimeOffset? valueBefore, DateTimeOffset? valueAfter, StringBuilder buffer)
        {
            if (valueBefore != valueAfter)
                ReaderHelper.AddLine(field, StateFieldType.DateOffset, valueBefore, valueAfter, buffer, "    ");
        }

        #endregion
    }
}