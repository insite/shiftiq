using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using InSite.Domain;
using InSite.Persistence;

namespace InSite.UI.Admin.Reports.Changes.Models
{
    static class ReaderHelper
    {
        public static void AddLine(string field, StateFieldType type, object value1, object value2, StringBuilder buffer, string prefix = null)
        {
            if (prefix != null)
                buffer.Append(prefix);

            buffer.Append("- ")
                .Append(field)
                .Append(" changed from *").Append(GetStringValue(type, value1))
                .Append("* to **").Append(GetStringValue(type, value2)).Append("**")
                .AppendLine();
        }

        public static void AddAddedLine(string field, StateFieldType type, object value, StringBuilder buffer, string prefix = null)
        {
            if (value == null || type == StateFieldType.Text && string.IsNullOrEmpty(value.ToString()))
                return;

            if (prefix != null)
                buffer.Append(prefix);

            buffer.Append("- ")
                .Append(field)
                .Append(" added: **")
                .Append(GetStringValue(type, value))
                .Append("**")
                .AppendLine();
        }

        public static Dictionary<Guid, string> GetUsers(IEnumerable<Guid> userIds)
        {
            return UserSearch
                .Bind(
                    x => new { x.UserIdentifier, x.FullName },
                    new UserFilter { IncludeUserIdentifiers = userIds.ToArray() }
                )
                .ToDictionary(x => x.UserIdentifier, x => x.FullName);
        }

        private static string GetStringValue(StateFieldType type, object value)
        {
            if (value == null || type == StateFieldType.Text && string.IsNullOrEmpty((string)value))
                return "(blank)";

            switch (type)
            {
                case StateFieldType.Date:
                case StateFieldType.DateOffset:
                    return $"{value:MMM d, yyyy}";
                default:
                    return value.ToString();
            }
        }
    }
}