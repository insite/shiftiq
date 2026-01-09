using System;

using Shift.Common;
using Shift.Sdk.UI;

namespace InSite.UI.Admin.Reports.Changes.Models
{
    static class HistoryReader
    {
        public static HistoryCollection ReadUser(Guid userId, Guid organizationId)
        {
            var history = new HistoryCollection();

            UserChangeReader.Read(userId, null, history);
            PersonChangeReader.Read(userId, organizationId, null, history);

            return history;
        }

        public static HistoryCollection ReadUserMembership(Guid userId, Guid organizationId)
        {
            var history = new HistoryCollection();

            MembershipChangeReader.ReadUser(userId, organizationId, history);

            return history;
        }

        public static HistoryCollection ReadGroupMembership(Guid groupId, Guid organizationId)
        {
            var history = new HistoryCollection();

            MembershipChangeReader.ReadGroup(groupId, organizationId, history);

            return history;
        }

        public static string ReadUserLatestAsHtml(Guid userId, Guid organizationId, DateTimeOffset after)
        {
            var history = new HistoryCollection();

            UserChangeReader.Read(userId, after, history);
            PersonChangeReader.Read(userId, organizationId, after, history);

            var description = history.First?.BuildDescription();

            return !string.IsNullOrEmpty(description)
                ? Markdown.ToHtml(description)
                : null;
        }
    }
}