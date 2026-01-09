using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Shift.Common.Timeline.Changes;

using InSite.Domain.Issues;
using InSite.Persistence;

using Newtonsoft.Json;

namespace InSite.Web.Change
{
    public class CaseChangeDescriptor
    {
        class CustomAction<T> where T : IChange
        {
            public string Key { get; }
            public Func<T, string> Transform { get; }

            public CustomAction(string key, Func<T, string> transform)
            {
                Key = key;
                Transform = transform;
            }
        }

        class ChangeHolder
        {
            public IChange DeSerializeChange { get; set; }
            public SerializedChange Change { get; set; }
        }

        private string Current { get; set; }

        private Dictionary<string, (string prev, string curr)> Differences { get; set; } = null;

        private readonly string[] _excludedProperties;

        public CaseChangeDescriptor(Guid aggregate, int version, string[] excludedProperties)
        {
            _excludedProperties = excludedProperties;

            var changes = ServiceLocator.ChangeStore.GetSerializedChangesPaged(aggregate, "", false, 0, int.MaxValue);
            if (changes == null || changes.Count == 0)
                return;

            var change = changes.FirstOrDefault(x => x.AggregateVersion == version);

            Current = SerializeChange(change);

            if (change != null)
            {
                Differences = CompareIssueChanges(changes, change);
            }
        }

        public string GetDescription()
        {
            if (Current is null)
                return "NOT FOUND";

            if (Differences is null)
                return Current;

            var diffJson = JsonConvert.SerializeObject(Differences.ToDictionary(x => x.Key,
                x => string.IsNullOrWhiteSpace(x.Value.prev) ? (object)x.Value.curr : new
                {
                    From = x.Value.prev,
                    To = x.Value.curr
                }));

            return diffJson;
        }

        private (List<ChangeHolder> prevs, CaseOpened initial, CaseOpened2 initial2, CaseCommentPosted initialComment) GroupChanges(
            List<SerializedChange> changes,
            SerializedChange change
            )
        {
            var prevs = changes.Where(x => x.ChangeType == change.ChangeType
                        && x.AggregateVersion < change.AggregateVersion
                        && x.ChangeTime <= change.ChangeTime)
                .OrderByDescending(x => x.AggregateVersion)
                .ThenByDescending(x => x.ChangeTime)
                .Select(x => new ChangeHolder { DeSerializeChange = DeSerializeChange(x), Change = x })
                .ToList();

            var initial = changes.Where(x => x.ChangeType == typeof(CaseOpened).Name)
                .OrderByDescending(x => x.AggregateVersion)
                .ThenByDescending(x => x.ChangeTime)
                .Select(x => DeSerializeChange(x))
                .FirstOrDefault() as CaseOpened;

            var initial2 = changes.Where(x => x.ChangeType == typeof(CaseOpened2).Name)
               .OrderByDescending(x => x.AggregateVersion)
               .ThenByDescending(x => x.ChangeTime)
               .Select(x => DeSerializeChange(x))
               .FirstOrDefault() as CaseOpened2;

            var initialComment = changes.Where(x => x.ChangeType == typeof(CaseCommentPosted).Name)
                .OrderByDescending(x => x.AggregateVersion)
                .ThenByDescending(x => x.ChangeTime)
                .Select(x => DeSerializeChange(x))
                .FirstOrDefault() as CaseCommentPosted;

            return (prevs, initial, initial2, initialComment);

        }

        private Dictionary<string, (string prev, string curr)> ProcessGroupAssigned(List<ChangeHolder> prevs, GroupAssigned typedChange)
        {
            foreach (var item in prevs)
            {
                if (item.DeSerializeChange is GroupAssigned typedChange2
                    && typedChange2.Role == typedChange.Role)
                {
                    var differences = new Dictionary<string, (string prev, string curr)>
                        {
                            { nameof(typedChange.Role), ("", typedChange.Role) },
                        };
                    InsertIntoDiffDictionary(differences, nameof(typedChange.Group), GetGroupName(typedChange2.Group), GetGroupName(typedChange.Group));
                    return differences;
                }
            }

            return ProcessNoDiff(typedChange,
                new CustomAction<GroupAssigned>(nameof(typedChange.Group), x => GetGroupName(x.Group))
            );
        }

        private Dictionary<string, (string prev, string curr)> ProcessGroupUnAssigned(List<ChangeHolder> prevs, GroupUnassigned typedChange)
        {
            foreach (var item in prevs)
            {
                if (item.DeSerializeChange is GroupUnassigned typedChange2
                    && typedChange2.Role == typedChange.Role)
                {
                    var differences = new Dictionary<string, (string prev, string curr)>
                    {
                        { nameof(typedChange.Role), ("", typedChange.Role) },
                    };
                    InsertIntoDiffDictionary(differences, nameof(typedChange.Group), GetGroupName(typedChange2.Group), GetGroupName(typedChange.Group));
                    return differences;
                }
            }

            return ProcessNoDiff(typedChange,
                new CustomAction<GroupUnassigned>(nameof(typedChange.Group), x => GetGroupName(x.Group))
            );
        }

        private Dictionary<string, (string prev, string curr)> ProcessIssueCommentDeleted(
            List<SerializedChange> changes,
            CaseCommentPosted initialComment
            )
        {
            var comment = changes.Where(x => x.ChangeType == typeof(CaseCommentModified).Name)
                .OrderByDescending(x => x.AggregateVersion)
                .ThenByDescending(x => x.ChangeTime)
                .FirstOrDefault();
            var differences = new Dictionary<string, (string prev, string curr)>();
            if (comment != null)
            {
                var itemDictionary = DeSerializeChange(SerializeChange(comment));
                InsertIntoDiffDictionary(differences, "Status", "", "Deleted");
                foreach (var item in itemDictionary)
                {
                    if (item.Key == "AssignedTo" || item.Key == "Revisor" || item.Key == "ResolvedBy")
                        InsertIntoDiffDictionary(differences, item.Key, "", GetUserName(item.Value));
                    else if (item.Key == "Revisor")
                        continue;
                    else if (item.Key == "Revised")
                        continue;
                    else
                        InsertIntoDiffDictionary(differences, item.Key, "", item.Value);
                }
            }
            else
            {
                if (initialComment != null)
                {
                    InsertIntoDiffDictionary(differences, nameof(initialComment.Comment), "", initialComment.Comment.ToString());
                    InsertIntoDiffDictionary(differences, nameof(initialComment.Text), "", initialComment.Text);
                    InsertIntoDiffDictionary(differences, nameof(initialComment.Tag), "", initialComment.Tag);
                    InsertIntoDiffDictionary(differences, nameof(initialComment.Category), "", initialComment.Category);
                    InsertIntoDiffDictionary(differences, nameof(initialComment.SubCategory), "", initialComment.SubCategory);
                    InsertIntoDiffDictionary(differences, nameof(initialComment.Flag), "", initialComment.Flag);
                    InsertIntoDiffDictionary(differences, nameof(initialComment.AssignedTo), "", GetUserName(initialComment.AssignedTo));
                    InsertIntoDiffDictionary(differences, nameof(initialComment.Author), "", GetUserName(initialComment.Author));
                    InsertIntoDiffDictionary(differences, nameof(initialComment.ResolvedBy), "", GetUserName(initialComment.ResolvedBy));
                    InsertIntoDiffDictionary(differences, nameof(initialComment.Authored), "", initialComment.Authored.ToString());
                    InsertIntoDiffDictionary(differences, nameof(initialComment.Flagged), "", initialComment.Flagged.ToString());
                    InsertIntoDiffDictionary(differences, nameof(initialComment.Submitted), "", initialComment.Submitted.ToString());
                    InsertIntoDiffDictionary(differences, nameof(initialComment.Resolved), "", initialComment.Resolved.ToString());
                    InsertIntoDiffDictionary(differences, "Status", "", "Deleted");
                }
            }
            return differences;
        }

        private Dictionary<string, (string prev, string curr)> ProcessIssueCommentModified(
            List<ChangeHolder> prevs,
            CaseCommentModified typedChange,
            SerializedChange change,
            CaseCommentPosted initialComment
            )
        {
            foreach (var item in prevs)
            {
                if (item.DeSerializeChange is CaseCommentModified)
                {
                    var differences = new Dictionary<string, (string prev, string curr)>();
                    var changeDictionary = DeSerializeChange(SerializeChange(change));
                    var itemDictionary = DeSerializeChange(SerializeChange(item.Change));
                    foreach (var item2 in changeDictionary.Keys)
                    {
                        if (!itemDictionary.ContainsKey(item2))
                            itemDictionary.Add(item2, "");
                    }
                    foreach (var item2 in itemDictionary.Keys)
                    {
                        if (!changeDictionary.ContainsKey(item2))
                            changeDictionary.Add(item2, "");
                    }
                    foreach (var item2 in changeDictionary.Keys)
                    {
                        if (item2 == "AssignedTo" || item2 == "ResolvedBy")
                            InsertIntoDiffDictionary(differences, item2, GetUserName(itemDictionary[item2]), GetUserName(changeDictionary[item2]));
                        else if (item2 == "Revisor")
                            InsertIntoDiffDictionary(differences, item2, "", GetUserName(changeDictionary[item2]));
                        else if(item2 == "Revised")
                            InsertIntoDiffDictionary(differences, item2, "", changeDictionary[item2]);
                        else
                            InsertIntoDiffDictionary(differences, item2, itemDictionary[item2], changeDictionary[item2]);
                    }
                    return differences;
                }
            }

            if (initialComment != null)
            {
                var differences = new Dictionary<string, (string prev, string curr)>();
                InsertIntoDiffDictionary(differences, nameof(typedChange.Comment), initialComment.Comment.ToString(), typedChange.Comment.ToString());
                InsertIntoDiffDictionary(differences, nameof(typedChange.Text), initialComment.Text, typedChange.Text);
                InsertIntoDiffDictionary(differences, nameof(typedChange.Tag), initialComment.Tag, typedChange.Tag);
                InsertIntoDiffDictionary(differences, nameof(typedChange.Category), initialComment.Category, typedChange.Category);
                InsertIntoDiffDictionary(differences, nameof(typedChange.SubCategory), initialComment.SubCategory, typedChange.SubCategory);
                InsertIntoDiffDictionary(differences, nameof(typedChange.Flag), initialComment.Flag, typedChange.Flag);
                InsertIntoDiffDictionary(differences, nameof(typedChange.AssignedTo), GetUserName(initialComment.AssignedTo), GetUserName(typedChange.AssignedTo));
                InsertIntoDiffDictionary(differences, nameof(typedChange.Revisor), "", GetUserName(typedChange.Revisor));
                InsertIntoDiffDictionary(differences, nameof(typedChange.ResolvedBy), GetUserName(initialComment.ResolvedBy), GetUserName(typedChange.ResolvedBy));
                InsertIntoDiffDictionary(differences, nameof(typedChange.Revised), "", typedChange.Revised.ToString());
                InsertIntoDiffDictionary(differences, nameof(typedChange.Flagged), initialComment.Flagged.ToString(), typedChange.Flagged.ToString());
                InsertIntoDiffDictionary(differences, nameof(typedChange.Submitted), initialComment.Submitted.ToString(), typedChange.Submitted.ToString());
                InsertIntoDiffDictionary(differences, nameof(typedChange.Resolved), initialComment.Resolved.ToString(), typedChange.Resolved.ToString());
                return differences;
            }

            return ProcessNoDiff(typedChange,
                new CustomAction<CaseCommentModified>(nameof(typedChange.AssignedTo), x => GetUserName(x.AssignedTo)),
                new CustomAction<CaseCommentModified>(nameof(typedChange.Revisor), x => GetUserName(x.Revisor)),
                new CustomAction<CaseCommentModified>(nameof(typedChange.ResolvedBy), x => GetUserName(x.ResolvedBy))
            );
        }

        private Dictionary<string, (string prev, string curr)> ProcessIssueDescribed(List<ChangeHolder> prevs, CaseDescribed typedChange, CaseOpened initial, CaseOpened2 initial2)
        {
            foreach (var item in prevs)
            {
                if (item.DeSerializeChange is CaseDescribed typedChange2)
                {
                    var differences = new Dictionary<string, (string prev, string curr)>();
                    InsertIntoDiffDictionary(differences, nameof(typedChange.Description), typedChange2.Description, typedChange.Description);
                    return differences;
                }
            }

            if (initial != null)
            {
                var differences = new Dictionary<string, (string prev, string curr)>();
                InsertIntoDiffDictionary(differences, nameof(typedChange.Description), initial.Description, typedChange.Description);
                return differences;
            }

            if (initial2 != null)
            {
                var differences = new Dictionary<string, (string prev, string curr)>();
                InsertIntoDiffDictionary(differences, nameof(typedChange.Description), initial2.Description, typedChange.Description);
                return differences;
            }

            return null;
        }

        private Dictionary<string, (string prev, string curr)> ProcessIssueStatusChanged(List<ChangeHolder> prevs, CaseStatusChanged typedChange, CaseOpened initial, CaseOpened2 initial2)
        {
            foreach (var item in prevs)
            {
                if (item.DeSerializeChange is CaseStatusChanged typedChange2)
                {
                    var differences = new Dictionary<string, (string prev, string curr)>();
                    InsertIntoDiffDictionary(differences, nameof(typedChange.Status), GetStatusName(typedChange2.Status), GetStatusName(typedChange.Status));
                    InsertIntoDiffDictionary(differences, nameof(typedChange.Effective), typedChange2.Effective.ToString(), typedChange.Effective.ToString());
                    return differences;
                }
            }

            return ProcessNoDiff(typedChange,
                new CustomAction<CaseStatusChanged>(nameof(typedChange.Status), x => GetStatusName(x.Status))
            );
        }

        private Dictionary<string, (string prev, string curr)> ProcessIssueTitleChanged(List<ChangeHolder> prevs, CaseTitleChanged typedChange, CaseOpened initial, CaseOpened2 initial2)
        {
            foreach (var item in prevs)
            {
                if (item.DeSerializeChange is CaseTitleChanged typedChange2)
                {
                    var differences = new Dictionary<string, (string prev, string curr)>();
                    InsertIntoDiffDictionary(differences, nameof(typedChange.IssueTitle), typedChange2.IssueTitle, typedChange.IssueTitle);
                    return differences;
                }
            }
            if (initial != null)
            {
                var differences = new Dictionary<string, (string prev, string curr)>();
                InsertIntoDiffDictionary(differences, nameof(typedChange.IssueTitle), initial.Title, typedChange.IssueTitle);
                return differences;
            }
            if (initial2 != null)
            {
                var differences = new Dictionary<string, (string prev, string curr)>();
                InsertIntoDiffDictionary(differences, nameof(typedChange.IssueTitle), initial2.Title, typedChange.IssueTitle);
                return differences;
            }
            return null;
        }

        private Dictionary<string, (string prev, string curr)> ProcessIssueTypeChanged(List<ChangeHolder> prevs, CaseTypeChanged typedChange, CaseOpened initial, CaseOpened2 initial2)
        {
            foreach (var item in prevs)
            {
                if (item.DeSerializeChange is CaseTypeChanged typedChange2)
                {
                    var differences = new Dictionary<string, (string prev, string curr)>();
                    InsertIntoDiffDictionary(differences, nameof(typedChange.IssueType), typedChange2.IssueType, typedChange.IssueType);
                    return differences;
                }
            }
            if (initial != null)
            {
                var differences = new Dictionary<string, (string prev, string curr)>();
                InsertIntoDiffDictionary(differences, nameof(typedChange.IssueType), initial.Type, typedChange.IssueType);
                return differences;
            }
            if (initial2 != null)
            {
                var differences = new Dictionary<string, (string prev, string curr)>();
                InsertIntoDiffDictionary(differences, nameof(typedChange.IssueType), initial2.Type, typedChange.IssueType);
                return differences;
            }
            return null;
        }

        private Dictionary<string, (string prev, string curr)> ProcessUserAssigned(List<ChangeHolder> prevs, UserAssigned typedChange, CaseOpened initial, CaseOpened2 initial2)
        {
            foreach (var item in prevs)
            {
                if (item.DeSerializeChange is UserAssigned typedChange2)
                {
                    if (typedChange2.Role == typedChange.Role)
                    {
                        var differences = new Dictionary<string, (string prev, string curr)>
                                    {
                                        { nameof(typedChange.Role), ("", typedChange.Role) },
                                    };
                        InsertIntoDiffDictionary(differences, nameof(typedChange.User), GetUserName(typedChange2.User), GetUserName(typedChange.User));
                        return differences;
                    }
                }
            }

            return ProcessNoDiff(typedChange,
                new CustomAction<UserAssigned>(nameof(typedChange.User), x => GetUserName(x.User))
            );
        }

        private Dictionary<string, (string prev, string curr)> ProcessUserUnAssigned(List<ChangeHolder> prevs, UserUnassigned typedChange, CaseOpened initial, CaseOpened2 initial2)
        {
            foreach (var item in prevs)
            {
                if (item.DeSerializeChange is UserAssigned typedChange2)
                {
                    if (typedChange2.Role == typedChange.Role)
                    {
                        var differences = new Dictionary<string, (string prev, string curr)>
                                    {
                                        { nameof(typedChange.Role), ("", typedChange.Role) },
                                    };
                        InsertIntoDiffDictionary(differences, nameof(typedChange.User), GetUserName(typedChange2.User), GetUserName(typedChange.User));
                        return differences;
                    }
                }
            }

            return ProcessNoDiff(typedChange,
                new CustomAction<UserUnassigned>(nameof(typedChange.User), x => GetUserName(x.User))
            );
        }

        private Dictionary<string, (string prev, string curr)> ProcessCommentPrivacyChanged(CaseAttachmentAdded change)
        {
            return ProcessNoDiff(change,
                new CustomAction<CaseAttachmentAdded>(nameof(change.Posted), x => GetUserName(x.Poster))
            );
        }

        private Dictionary<string, (string prev, string curr)> ProcessCommentPosted(CaseCommentPosted change)
        {
            return ProcessNoDiff(change,
                new CustomAction<CaseCommentPosted>(nameof(change.Author), x => GetUserName(x.Author)),
                new CustomAction<CaseCommentPosted>(nameof(change.AssignedTo), x => GetUserName(x.AssignedTo)),
                new CustomAction<CaseCommentPosted>(nameof(change.ResolvedBy), x => GetUserName(x.ResolvedBy))
            );
        }

        private Dictionary<string, (string prev, string curr)> ProcessNoDiff<T>(T change, params CustomAction<T>[] actions) where T : IChange
        {
            var differences = new Dictionary<string, (string prev, string curr)>();

            var type = typeof(T);
            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var p in props)
            {
                if (_excludedProperties.Any(x => string.Equals(x, p.Name)))
                    continue;

                var action = actions.FirstOrDefault(x => string.Equals(x.Key, p.Name));
                var value = action != null ? action.Transform.Invoke(change) : p.GetValue(change)?.ToString();

                InsertIntoDiffDictionary(differences, p.Name, string.Empty, value);
            }

            return differences;
        }

        private Dictionary<string, (string prev, string curr)> CompareIssueChanges(List<SerializedChange> changes, SerializedChange change)
        {
            var (prevs, initial, initial2, initialComment) = GroupChanges(changes, change);
            if (prevs.Count != 0 || initial != null || initial2 != null)
            {
                var deSerializeChange = DeSerializeChange(change);
                switch (deSerializeChange)
                {
                    case CommentPrivacyChanged typedChange:
                        break;
                    case GroupAssigned typedChange:
                        return ProcessGroupAssigned(prevs, typedChange);
                    case GroupUnassigned typedChange:
                        return ProcessGroupUnAssigned(prevs, typedChange);
                    case CaseAttachmentAdded typedChange:
                        return ProcessCommentPrivacyChanged(typedChange);
                    case CaseAttachmentDeleted typedChange:
                        break;
                    case CaseClosed typedChange:
                        break;
                    case CaseCommentDeleted typedChange:
                        return ProcessIssueCommentDeleted(changes, initialComment);
                    case CaseCommentModified typedChange:
                        return ProcessIssueCommentModified(prevs, typedChange, change, initialComment);
                    case CaseCommentPosted typedChange:
                        return ProcessCommentPosted(typedChange);
                    case CaseConnectedToSurveyResponse typedChange:
                        break;
                    case CaseDeleted typedChange:
                        break;
                    case CaseDescribed typedChange:
                        return ProcessIssueDescribed(prevs, typedChange, initial, initial2);
                    case CaseOpened typedChange:
                        break;
                    case CaseOpened2 typedChange:
                        break;
                    case CaseReopened typedChange:
                        break;
                    case CaseStatusChanged typedChange:
                        return ProcessIssueStatusChanged(prevs, typedChange, initial, initial2);
                    case CaseTitleChanged typedChange:
                        return ProcessIssueTitleChanged(prevs, typedChange, initial, initial2);
                    case CaseTypeChanged typedChange:
                        return ProcessIssueTypeChanged(prevs, typedChange, initial, initial2);
                    case UserAssigned typedChange:
                        return ProcessUserAssigned(prevs, typedChange, initial, initial2);
                    case UserUnassigned typedChange:
                        return ProcessUserUnAssigned(prevs, typedChange, initial, initial2);
                    default:
                        break;
                }
            }
            return null;
        }

        private IChange DeSerializeChange(SerializedChange change)
        {
            if (change is null)
                return null;
            return change.Deserialize();
        }

        private string SerializeChange(SerializedChange change)
        {
            var deserializeChange = DeSerializeChange(change);
            if (deserializeChange is null)
                return null;

            return ServiceLocator.Serializer.Serialize(deserializeChange, _excludedProperties, false);
        }

        private static void InsertIntoDiffDictionary(Dictionary<string, (string prev, string curr)> dictionary, string key, string prev, string current)
        {
            if (!string.IsNullOrWhiteSpace(current))
            {
                if (!string.IsNullOrWhiteSpace(prev))
                {
                    if (current != prev)
                    {
                        dictionary.Add(key, (prev, current));
                        return;
                    }
                }
                dictionary.Add(key, ("", current));

            }
        }

        private Dictionary<string, string> DeSerializeChange(string change)
        {
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(change);
        }

        private static string GetUserName(Guid? userIdentifier)
        {
            if (userIdentifier == null)
                return string.Empty;

            return UserSearch.Select(userIdentifier.Value)?.FullName ?? userIdentifier.ToString();
        }

        private static string GetUserName(string userIdentifier)
        {
            if (string.IsNullOrWhiteSpace(userIdentifier))
                return string.Empty;
            if (!Guid.TryParse(userIdentifier, out var userId))
                return string.Empty;

            return GetUserName(userId);
        }

        private static string GetGroupName(Guid groupIdentifier)
        {
            return ServiceLocator.GroupSearch.GetGroup(groupIdentifier)?.GroupName ?? groupIdentifier.ToString();
        }

        private static string GetStatusName(Guid statusIdentifier)
        {
            return ServiceLocator.IssueSearch.GetStatus(statusIdentifier)?.StatusName ?? statusIdentifier.ToString();
        }
    }
}