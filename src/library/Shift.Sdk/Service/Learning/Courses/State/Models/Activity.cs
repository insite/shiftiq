using System;
using System.Collections.Generic;

using Newtonsoft.Json;

using Shift.Common;

namespace InSite.Domain.Courses
{
    [Serializable]
    public class Activity
    {
        public static class Defaults
        {
            public const bool IsMultilingual = false;
            public const bool IsAdaptive = false;
            public const bool IsDispatch = false;
        }

        [JsonProperty, JsonConverter(typeof(ActivityStateDictionaryConverter))]
        private Dictionary<ActivityField, object> Values = new Dictionary<ActivityField, object>();

        [JsonIgnore]
        public Module Module { get; set; }

        public Guid Identifier { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public ActivityType ActivityType { get; set; }

        public ContentContainer Content { get; set; }
        public List<Prerequisite> Prerequisites { get; set; }
        public List<ActivityCompetency> Competencies { get; set; }

        public string GetTextValue(ActivityField activityField) => Values.TryGetValue(activityField, out var value) ? (string)value : null;
        public bool? GetBoolValue(ActivityField activityField) => Values.TryGetValue(activityField, out var value) ? (bool?)value : null;
        public int? GetIntValue(ActivityField activityField) => Values.TryGetValue(activityField, out var value) ? (int?)value : null;
        public DateTimeOffset? GetDateOffsetValue(ActivityField activityField) => Values.TryGetValue(activityField, out var value) ? (DateTimeOffset?)value : null;
        public DateTime? GetDateValue(ActivityField activityField) => Values.TryGetValue(activityField, out var value) ? (DateTime?)value : null;
        public Guid? GetGuidValue(ActivityField activityField) => Values.TryGetValue(activityField, out var value) ? (Guid?)value : null;

        internal void SetValue<T>(ActivityField activityField, T value, bool directlyModified)
        {
            var field = ActivityFieldList.GetField(activityField);
            if (value != null && field.FieldType != StateFieldHelper.GetFieldType<T>())
                throw new ArgumentException($"Invalid activity field: {activityField}");

            if (directlyModified && !field.DirectlyModifiable)
                throw new ArgumentException($"The field cannot be modified directly field: {activityField}");

            if (value == null)
            {
                if (field.Required)
                    throw new ArgumentNullException($"Field {activityField} is a required field");

                Values.Remove(activityField);
            }
            else
                Values[activityField] = value;
        }
    }
}
