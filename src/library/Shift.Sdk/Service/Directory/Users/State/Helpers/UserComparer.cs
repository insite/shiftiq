using System;
using System.Collections.Generic;

namespace InSite.Domain.Contacts
{
    public static class UserComparer
    {
        public class Difference
        {
            public UserField Field { get; }
            public StateFieldType Type { get; }
            public object Value1 { get; }
            public object Value2 { get; }

            public Difference(UserField field, StateFieldType type, object value1, object value2)
            {
                Field = field;
                Type = type;
                Value1 = value1;
                Value2 = value2;
            }
        }

        public static Difference[] Compare(UserState state1, UserState state2)
        {
            if (state1 == null)
                throw new ArgumentNullException("state1");

            if (state2 == null)
                throw new ArgumentNullException("state2");

            var result = new List<Difference>();

            foreach (var fieldAndMeta in UserFieldList.GetAllFields())
            {
                var field = fieldAndMeta.Key;
                var value1 = state1.GetValue(field);
                var value2 = state2.GetValue(field);

                if (fieldAndMeta.Value.FieldType == StateFieldType.Text)
                {
                    var s1 = value1?.ToString() ?? "";
                    var s2 = value2?.ToString() ?? "";

                    if (!string.Equals(s1, s2))
                        result.Add(new Difference(field, fieldAndMeta.Value.FieldType, value1, value2));
                }
                else if (!Equals(value1, value2))
                    result.Add(new Difference(field, fieldAndMeta.Value.FieldType, value1, value2));
            }

            return result.ToArray();
        }
    }
}
