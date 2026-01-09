using System;

using Newtonsoft.Json;

using Shift.Common;

namespace InSite.Common.Web.UI
{
    public class DateSelector : BaseDateTimeSelector<DateTime?>
    {
        [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
        protected class JsonDateTimeOutput : JsonOutput
        {
            protected override string DateInternal => Value.HasValue ? Value.Value.ToString("yyyy-MM-dd") : null;
        }

        [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
        private class JsonDateTimeInput : JsonInput
        {
            public override DateTime? Value
            {
                get
                {
                    if (StringValue.IsEmpty())
                        return null;

                    var separator = StringValue.IndexOf('T');
                    var text = separator != -1 ? StringValue.Substring(0, separator) : StringValue;

                    return !DateTime.TryParse(text, out var value) ? (DateTime?)null : value;
                }
            }

            public override bool IsChanged(DateTime? value) => Value != value;
        }

        public override DateTime? Value
        {
            get
            {
                return (DateTime?)ViewState[nameof(Value)];
            }
            set
            {
                ViewState[nameof(Value)] = value.HasValue
                    ? new DateTime(value.Value.Year, value.Value.Month, value.Value.Day)
                    : (DateTime?)null;
            }
        }

        public override bool HasValue => Value.HasValue;

        protected override JsonOptions GetJsonOptions()
        {
            var result = base.GetJsonOptions();

            result.ShowDate = true;

            return result;
        }

        protected override JsonInput GetInputData(string json)
        {
            return json.IsEmpty() ? null : JsonConvert.DeserializeObject<JsonDateTimeInput>(json);
        }

        protected override JsonOutput GetOutputData()
        {
            return new JsonDateTimeOutput()
            {
                FullDateFormat = "MMM D, YYYY",
                ShortDateFormat = "MMM D, YYYY"
            };
        }
    }
}