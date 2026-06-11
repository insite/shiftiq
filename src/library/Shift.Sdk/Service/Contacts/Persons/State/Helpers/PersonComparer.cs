using System;
using System.Collections.Generic;
using System.Linq;

using Shift.Constant;

namespace InSite.Domain.Contacts
{
    public static class PersonComparer
    {
        private static readonly HashSet<PersonField> _excludeFields = new HashSet<PersonField>
        {
            PersonField.Created,
            PersonField.FullName
        };

        public class StateFieldDifference
        {
            public PersonField Field { get; }
            public StateFieldType Type { get; }
            public object Value1 { get; }
            public object Value2 { get; }

            public StateFieldDifference(PersonField field, StateFieldType type, object value1, object value2)
            {
                Field = field;
                Type = type;
                Value1 = value1;
                Value2 = value2;
            }
        }

        public class AddressFieldDifference
        {
            public PersonAddressField Field { get; }
            public string Value1 { get; }
            public string Value2 { get; }

            public AddressFieldDifference(PersonAddressField field, string value1, string value2)
            {
                Field = field;
                Value1 = value1;
                Value2 = value2;
            }
        }

        public class CommentFieldDifference
        {
            public PersonCommentField Field { get; }
            public StateFieldType Type {  get; }
            public object Value1 { get; }
            public object Value2 { get; }

            public CommentFieldDifference(PersonCommentField field, StateFieldType type, object value1, object value2)
            {
                Field = field;
                Type = type;
                Value1 = value1;
                Value2 = value2;
            }
        }

        public static StateFieldDifference[] CompareStateFields(PersonState state1, PersonState state2)
        {
            if (state1 == null)
                throw new ArgumentNullException("state1");

            if (state2 == null)
                throw new ArgumentNullException("state2");

            var result = new List<StateFieldDifference>();

            foreach (var fieldAndMeta in PersonFieldList.GetAllFields())
            {
                var field = fieldAndMeta.Key;
                if (_excludeFields.Contains(field))
                    continue;

                var value1 = state1.GetValue(field);
                var value2 = state2.GetValue(field);

                if (fieldAndMeta.Value.FieldType == StateFieldType.Text)
                {
                    var s1 = value1?.ToString() ?? "";
                    var s2 = value2?.ToString() ?? "";

                    if (!string.Equals(s1, s2))
                        result.Add(new StateFieldDifference(field, fieldAndMeta.Value.FieldType, value1, value2));
                }
                else if (!Equals(value1, value2))
                    result.Add(new StateFieldDifference(field, fieldAndMeta.Value.FieldType, value1, value2));
            }

            return result.ToArray();
        }

        public static AddressFieldDifference[] CompareAddressFields(PersonAddress address1, PersonAddress address2)
        {
            var result = new List<AddressFieldDifference>();

            if (!string.Equals(address1?.City ?? "", address2?.City ?? ""))
                result.Add(new AddressFieldDifference(PersonAddressField.City, address1?.City, address2?.City));

            if (!string.Equals(address1?.Country ?? "", address2?.Country ?? ""))
                result.Add(new AddressFieldDifference(PersonAddressField.Country, address1?.Country, address2?.Country));

            if (!string.Equals(address1?.Description ?? "", address2?.Description ?? ""))
                result.Add(new AddressFieldDifference(PersonAddressField.Description, address1?.Description, address2?.Description));

            if (!string.Equals(address1?.PostalCode ?? "", address2?.PostalCode ?? ""))
                result.Add(new AddressFieldDifference(PersonAddressField.PostalCode, address1?.PostalCode, address2?.PostalCode));

            if (!string.Equals(address1?.Province ?? "", address2?.Province ?? ""))
                result.Add(new AddressFieldDifference(PersonAddressField.Province, address1?.Province, address2?.Province));

            if (!string.Equals(address1?.Street1 ?? "", address2?.Street1 ?? ""))
                result.Add(new AddressFieldDifference(PersonAddressField.Street1, address1?.Street1, address2?.Street1));

            if (!string.Equals(address1?.Street2 ?? "", address2?.Street2 ?? ""))
                result.Add(new AddressFieldDifference(PersonAddressField.Street2, address1?.Street2, address2?.Street2));

            return result.ToArray();
        }

        public class CommentDifference
        {
            public Guid CommentId { get; set; }
            public Guid OwnerId { get; set; }
            public CommentActionType Action { get; set; }
            public List<CommentFieldDifference> Fields { get; set; } = new List<CommentFieldDifference>();
        }

        public static CommentDifference[] CompareCommentFields(
            Guid ownerId,
            Dictionary<Guid, PersonComment> comments1,
            Dictionary<Guid, PersonComment> comments2)
        {
            var result = new List<CommentDifference>();

            var allKeys = new HashSet<Guid>(comments1.Keys);
            allKeys.UnionWith(comments2.Keys);

            foreach (var commentId in allKeys)
            {
                comments1.TryGetValue(commentId, out var comment1);
                comments2.TryGetValue(commentId, out var comment2);

                if (comment1 == null && comment2 != null)
                {
                    result.Add(new CommentDifference
                    {
                        CommentId = commentId,
                        OwnerId = ownerId,
                        Action = CommentActionType.Author,
                        Fields = new List<CommentFieldDifference>
                        {
                            new CommentFieldDifference(PersonCommentField.Text, StateFieldType.Text, null, comment2.Text),
                            new CommentFieldDifference(PersonCommentField.Flag, StateFieldType.Text, null, comment2.Flag),
                            new CommentFieldDifference(PersonCommentField.Resolved, StateFieldType.DateOffset, null, comment2.Resolved),
                        }
                    });
                }
                else if (comment1 != null && comment2 == null)
                {
                    result.Add(new CommentDifference
                    {
                        CommentId = commentId,
                        OwnerId = ownerId,
                        Action = CommentActionType.Delete,
                        Fields = new List<CommentFieldDifference>
                        {
                            new CommentFieldDifference(PersonCommentField.Text, StateFieldType.Text, comment1.Text, null),
                            new CommentFieldDifference(PersonCommentField.Flag, StateFieldType.Text, comment1.Flag, null),
                            new CommentFieldDifference(PersonCommentField.Resolved, StateFieldType.DateOffset, comment1.Resolved, null),
                        }
                    });
                }
                else if (comment1 != null && comment2 != null)
                {
                    var fieldDiffs = CompareCommentFields(comment1, comment2);

                    if (fieldDiffs.Length > 0)
                    {
                        result.Add(new CommentDifference
                        {
                            CommentId = commentId,
                            OwnerId = ownerId,
                            Action = CommentActionType.Revise,
                            Fields = fieldDiffs.ToList()
                        });
                    }
                }
            }

            return result.ToArray();
        }

        public static CommentFieldDifference[] CompareCommentFields(PersonComment comment1, PersonComment comment2)
        {
            var result = new List<CommentFieldDifference>();

            if (!string.Equals(comment1?.Text, comment2?.Text))
                result.Add(new CommentFieldDifference(PersonCommentField.Text, StateFieldType.Text, comment1?.Text, comment2?.Text));

            if (!string.Equals(comment1?.Flag, comment2?.Flag))
                result.Add(new CommentFieldDifference(PersonCommentField.Flag, StateFieldType.Text, comment1?.Flag, comment2?.Flag));

            if (comment1?.Resolved != comment2?.Resolved)
                result.Add(new CommentFieldDifference(PersonCommentField.Resolved, StateFieldType.DateOffset, comment1?.Resolved, comment2?.Resolved));

            return result.ToArray();
        }
    }
}
