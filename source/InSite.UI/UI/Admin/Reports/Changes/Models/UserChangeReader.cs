using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Shift.Common.Timeline.Changes;

using InSite.Domain.Contacts;

using Shift.Sdk.UI;

namespace InSite.UI.Admin.Reports.Changes.Models
{
    static class UserChangeReader
    {
        private static readonly HashSet<UserField> DiffExclusions = new HashSet<UserField>
        {
            UserField.DefaultPassword,
            UserField.UserPasswordHash,
            UserField.OldUserPasswordHash,
            UserField.UserPasswordExpired
        };

        public static void Read(Guid userId, DateTimeOffset? after, HistoryCollection history)
        {
            var changes = ServiceLocator.SnapshotRepository.GetStatesAndChanges(userId);
            if (changes.Length == 0)
                return;

            var firstChangeIndex = Skip(changes, after);
            if (firstChangeIndex == -1)
                return;

            var changeDifferences = new List<(IChange, List<UserComparer.Difference>)>();

            for (int i = firstChangeIndex; i < changes.Length; i++)
                AddDifferences(changes[i - 1], changes[i], changeDifferences);

            if (changeDifferences.Count > 0)
                AddChanges(changeDifferences, history);
        }

        private static int Skip(IChange[] changes, DateTimeOffset? after)
        {
            const int defaultSkipTimeMs = 3000;

            // We do not want to include changes related to the creation of the Person
            var afterCreation = changes[0].ChangeTime.AddMilliseconds(defaultSkipTimeMs);

            if (after == null || after.Value < afterCreation)
                after = afterCreation;

            for (int i = 1; i < changes.Length; i++)
            {
                if (changes[i].ChangeTime > after.Value)
                    return i;
            }

            return -1;
        }

        private static void AddDifferences(IChange prev, IChange cur, List<(IChange, List<UserComparer.Difference>)> changeDifferences)
        {
            var prevState = (UserState)prev.AggregateState;
            var curState = (UserState)cur.AggregateState;
            var differences = UserComparer.Compare(prevState, curState);

            if (differences.Length == 0)
                return;

            var ordered = differences
                .Where(x => !DiffExclusions.Contains(x.Field))
                .OrderBy(x => x.Field.ToString())
                .ToList();

            if (ordered.Count > 0)
                changeDifferences.Add((cur, ordered));
        }

        private static void AddChanges(List<(IChange Change, List<UserComparer.Difference> List)> changeDifferences, HistoryCollection history)
        {
            var userIds = changeDifferences.Select(x => x.Change.OriginUser).ToHashSet();
            var users = ReaderHelper.GetUsers(userIds);

            var buffer = new StringBuilder();

            foreach (var (change, list) in changeDifferences)
            {
                buffer.Clear();

                foreach (var diff in list)
                    ReaderHelper.AddLine(diff.Field.ToString(), diff.Type, diff.Value1, diff.Value2, buffer);

                if (!users.TryGetValue(change.OriginUser, out var userFullName))
                    userFullName = "Someone";

                history.Add(change.ChangeTime.UtcDateTime, userFullName, buffer.ToString());
            }
        }
    }
}