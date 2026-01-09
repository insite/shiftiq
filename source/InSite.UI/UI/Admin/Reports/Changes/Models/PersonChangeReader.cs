using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Shift.Common.Timeline.Changes;

using InSite.Application.Standards.Read;
using InSite.Domain;
using InSite.Domain.Contacts;

using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.UI.Admin.Reports.Changes.Models
{
    static class PersonChangeReader
    {
        class ChangeDifference
        {
            public IChange Change { get; set; }
            public PersonComparer.StateFieldDifference[] StateFields { get; set; }
            public PersonComparer.AddressFieldDifference[] HomeAddressFields { get; set; }
            public PersonComparer.AddressFieldDifference[] WorkAddressFields { get; set; }
            public PersonComparer.AddressFieldDifference[] BillingAddressFields { get; set; }
            public PersonComparer.AddressFieldDifference[] ShippingAddressFields { get; set; }
            public PersonComparer.CommentDifference[] CommentDifferences { get; set; }
        }

        public static void Read(Guid userId, Guid organizationId, DateTimeOffset? after, HistoryCollection history)
        {
            var person = ServiceLocator.PersonSearch.GetPerson(userId, organizationId);
            if (person == null)
                return;

            var changes = ServiceLocator.SnapshotRepository.GetStatesAndChanges(person.PersonIdentifier);
            if (changes.Length == 0)
                return;

            var firstChangeIndex = Skip(changes, after);
            if (firstChangeIndex == -1)
                return;

            var changeDifferences = new List<ChangeDifference>();

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

        private static void AddDifferences(IChange prev, IChange cur, List<ChangeDifference> changeDifferences)
        {
            var prevState = (PersonState)prev.AggregateState;
            var curState = (PersonState)cur.AggregateState;

            var stateFields = PersonComparer.CompareStateFields(prevState, curState)
                .OrderBy(x => x.Field.ToString())
                .ToArray();

            var homeAddressFields = PersonComparer.CompareAddressFields(prevState.HomeAddress, curState.HomeAddress);
            var workAddressFields = PersonComparer.CompareAddressFields(prevState.WorkAddress, curState.WorkAddress);
            var billingAddressFields = PersonComparer.CompareAddressFields(prevState.BillingAddress, curState.BillingAddress);
            var shippingAddressFields = PersonComparer.CompareAddressFields(prevState.ShippingAddress, curState.ShippingAddress);
            var commentDiffs = PersonComparer.CompareCommentFields(curState.User, prevState.Comments, curState.Comments);

            if (stateFields.Length == 0
                && homeAddressFields.Length == 0
                && workAddressFields.Length == 0
                && billingAddressFields.Length == 0
                && shippingAddressFields.Length == 0
                && commentDiffs.Length == 0
                )
            {
                return;
            }

            changeDifferences.Add(new ChangeDifference
            {
                Change = cur,
                StateFields = stateFields,
                HomeAddressFields = homeAddressFields,
                WorkAddressFields = workAddressFields,
                BillingAddressFields = billingAddressFields,
                ShippingAddressFields = shippingAddressFields,
                CommentDifferences = commentDiffs,
            });
        }

        private static void AddChanges(List<ChangeDifference> changeDifferences, HistoryCollection history)
        {
            Dictionary<Guid, string> users, collection;

            MappIDs(changeDifferences, 
                out users, 
                out collection);

            var buffer = new StringBuilder();

            foreach (var changeDiff in changeDifferences)
            {
                buffer.Clear();

                AddChangesForStateFields(changeDiff.StateFields, collection, buffer);
                AddChangesForAddressFields("Home Address", changeDiff.HomeAddressFields, buffer);
                AddChangesForAddressFields("Work Address", changeDiff.WorkAddressFields, buffer);
                AddChangesForAddressFields("Billing Address", changeDiff.BillingAddressFields, buffer);
                AddChangesForAddressFields("Shipping Address", changeDiff.ShippingAddressFields, buffer);
                AddChangesForComments(changeDiff.CommentDifferences, buffer);

                if (!users.TryGetValue(changeDiff.Change.OriginUser, out var userFullName))
                    userFullName = "Someone";

                history.Add(changeDiff.Change.ChangeTime.UtcDateTime, userFullName, buffer.ToString());
            }
        }

        private static void MappIDs(List<ChangeDifference> changeDifferences, 
            out Dictionary<Guid, string> users, 
            out Dictionary<Guid, string> collection)
        {
            var userIds = changeDifferences.Select(x => x.Change.OriginUser).ToHashSet();
            users = ReaderHelper.GetUsers(userIds);

            var employerIds = GetFieldIds(changeDifferences, PersonField.EmployerGroupIdentifier);
            Dictionary<Guid, string> employers = GetEmployers(employerIds);

            var accountStatusesIds = GetFieldIds(changeDifferences, PersonField.MembershipStatusItemIdentifier);
            Dictionary<Guid, string> accountStatuses = GetAccountStatuses(accountStatusesIds);

            var occupationStandardIds = GetFieldIds(changeDifferences, PersonField.OccupationStandardIdentifier);
            Dictionary<Guid, string> occupationStandards = GetOccupationStandards(occupationStandardIds);

            var merged = Enumerable.Empty<KeyValuePair<Guid, string>>();

            if (employers != null)
                merged = merged.Concat(employers);

            if (accountStatuses != null)
                merged = merged.Concat(accountStatuses);

            if (occupationStandards != null)
                merged = merged.Concat(occupationStandards);

            collection = merged.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        private static void AddChangesForStateFields(PersonComparer.StateFieldDifference[] fields, Dictionary<Guid, string> collection, StringBuilder buffer)
        {
            var mappedFields = new HashSet<PersonField>
            {
                PersonField.EmployerGroupIdentifier,
                PersonField.MembershipStatusItemIdentifier,
                PersonField.OccupationStandardIdentifier
            };

            foreach (var field in fields)
            {
                if (mappedFields.Contains(field.Field))
                {
                    var field1 = GetName((Guid?)field.Value1, collection);
                    var field2 = GetName((Guid?)field.Value2, collection);

                    ReaderHelper.AddLine(field.Field.ToString(), StateFieldType.Text, field1, field2, buffer);
                }
                else
                {
                    ReaderHelper.AddLine(field.Field.ToString(), field.Type, field.Value1, field.Value2, buffer);
                }
            }
        }

        private static void AddChangesForComments(PersonComparer.CommentDifference[] commentChanges, StringBuilder buffer)
        {
            if (commentChanges == null || commentChanges.Length == 0)
                return;

            foreach (var diff in commentChanges)
            {
                if (buffer.Length > 0)
                    buffer.AppendLine();

                string header;
                switch (diff.Action)
                {
                    case CommentActionType.Author:
                        header = $"### [Comment](/ui/admin/contacts/comments/revise?contact={diff.OwnerId}&comment={diff.CommentId}) Added";
                        break;
                    case CommentActionType.Revise:
                        header = $"### [Comment](/ui/admin/contacts/comments/revise?contact={diff.OwnerId}&comment={diff.CommentId}) Revised";
                        break;
                    case CommentActionType.Delete:
                        header = $"### Comment Deleted";
                        break;
                    default:
                        header = $"### [Comment](/ui/admin/contacts/comments/revise?contact={diff.OwnerId}&comment={diff.CommentId}) Changed";
                        break;
                }

                buffer.AppendLine(header);

                foreach (var field in diff.Fields)
                {
                    var label = field.Field.ToString();

                    switch (diff.Action)
                    {
                        case CommentActionType.Author:
                            if (field.Value2 != null && !IsBlank(field.Value2))
                                ReaderHelper.AddAddedLine(label, field.Type, field.Value2, buffer);
                            break;

                        case CommentActionType.Delete:
                            if (field.Value1 != null && !IsBlank(field.Value1))
                                ReaderHelper.AddLine(label, field.Type, field.Value1, null, buffer);
                            break;

                        case CommentActionType.Revise:
                        default:
                            if (!Equals(field.Value1, field.Value2))
                                ReaderHelper.AddLine(label, field.Type, field.Value1, field.Value2, buffer);
                            break;
                    }
                }
            }
        }

        private static bool IsBlank(object value)
            => value == null || (value is string s && string.IsNullOrWhiteSpace(s));

        private static void AddChangesForAddressFields(string title, PersonComparer.AddressFieldDifference[] fields, StringBuilder buffer)
        {
            if (fields.Length == 0)
                return;

            if (buffer.Length > 0)
                buffer.AppendLine();

            buffer.AppendLine($"### {title}");

            foreach (var field in fields)
                ReaderHelper.AddLine(field.Field.ToString(), StateFieldType.Text, field.Value1, field.Value2, buffer);
        }

        private static string GetName(Guid? id, Dictionary<Guid, string> collection)
        {
            if (id == null)
                return "(blank)";

            return collection.TryGetValue(id.Value, out var name)
                ? name
                : "Unknown";
        }

        private static Dictionary<Guid, string> GetEmployers(HashSet<Guid> employerIDs)
        {
            return ServiceLocator.GroupSearch.BindGroups(
                x => new
                {
                    x.GroupIdentifier,
                    x.GroupName,
                },
                x => employerIDs.Contains(x.GroupIdentifier)
            ).ToDictionary(x => x.GroupIdentifier, x => x.GroupName);
        }

        private static Dictionary<Guid, string> GetAccountStatuses(HashSet<Guid> accountStatusIDs)
            => ServiceLocator.CollectionSearch.Select(accountStatusIDs);

        private static Dictionary<Guid, string> GetOccupationStandards(HashSet<Guid> occupationStandardIDs)
        {
            return ServiceLocator.StandardSearch.GetStandards(new QStandardFilter
            {
                OrganizationIdentifier = CurrentSessionState.Identity.Organization.Identifier,
                StandardIdentifiers = occupationStandardIDs.ToArray()
            }).ToDictionary(x => x.StandardIdentifier, x => x.ContentTitle);
        }

        private static HashSet<Guid> GetFieldIds(List<ChangeDifference> changeDifferences, PersonField type)
        {
            var ids = new HashSet<Guid>();

            foreach (var changeDiff in changeDifferences)
            {
                var field = changeDiff.StateFields.Where(x => x.Field == type).FirstOrDefault();
                if (field == null)
                    continue;

                if (field.Value1 != null)
                    ids.Add((Guid)field.Value1);

                if (field.Value2 != null)
                    ids.Add((Guid)field.Value2);
            }

            return ids;
        }
        
    }
}