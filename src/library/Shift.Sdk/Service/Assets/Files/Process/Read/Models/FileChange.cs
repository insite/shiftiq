using System;
using System.Globalization;

using Newtonsoft.Json;

namespace InSite.Application.Files.Read
{
    public class FileChange
    {
        public string FieldName { get; private set; }
        public bool IsDate { get; private set; }
        public string OldValue { get; private set; }
        public string NewValue { get; private set; }

        [JsonIgnore]
        public DateTimeOffset? OldDate
            => IsDate && DateTimeOffset.TryParse(OldValue, CultureInfo.InvariantCulture, DateTimeStyles.None, out var value) ? value : (DateTimeOffset?)null;

        [JsonIgnore]
        public DateTimeOffset? NewDate
            => IsDate && DateTimeOffset.TryParse(NewValue, CultureInfo.InvariantCulture, DateTimeStyles.None, out var value) ? value : (DateTimeOffset?)null;

        [JsonConstructor]
        public FileChange(string fieldName, bool isDate, string oldValue, string newValue)
        {
            FieldName = fieldName;
            IsDate = isDate;
            OldValue = oldValue;
            NewValue = newValue;
        }

        public FileChange(string fieldName, string oldValue, string newValue)
        {
            FieldName = fieldName;
            IsDate = false;
            OldValue = oldValue;
            NewValue = newValue;
        }

        public FileChange(string fieldName, DateTimeOffset? oldValue, DateTimeOffset? newValue)
        {
            FieldName = fieldName;
            IsDate = true;
            OldValue = FormatDate(oldValue);
            NewValue = FormatDate(newValue);
        }

        public FileChange(string fieldName, bool oldValue, bool newValue)
        {
            FieldName = fieldName;
            IsDate = false;
            OldValue = oldValue ? "Yes" : "No";
            NewValue = newValue ? "Yes" : "No";
        }

        private static string FormatDate(DateTimeOffset? value)
        {
            return value.HasValue ? value.Value.ToString(CultureInfo.InvariantCulture) : null;
        }

        // Used by Newtonsoft.Json

        public bool ShouldSerializeIsDate() { return IsDate; }
    }
}
