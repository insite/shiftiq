using System;
using System.Collections.Generic;
using System.ComponentModel;

using Newtonsoft.Json;

using Shift.Common;

namespace InSite.Common.Web.UI
{
    public enum DatePreset
    {
        None,
        SinceStartOfDay,   
        BeforeEndOfDay     
    }


    public class DateTimeOffsetSelector : BaseDateTimeSelector<DateTimeOffset?>
    {
        #region Classes

        [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
        private class JsonTimeZone
        {
            #region Properties

            [JsonProperty(PropertyName = "title")]
            public string DisplayName { get; set; }

            [JsonProperty(PropertyName = "abbrv")]
            public string Abbreviation { get; set; }

            [JsonProperty(PropertyName = "moment")]
            public string MomentName { get; set; }

            #endregion
        }

        [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
        protected class JsonDateTimeOffsetOutput : JsonOutput
        {
            #region Properties

            protected override string DateInternal => Value.HasValue ? Value.Value.ToString("yyyy-MM-dd'T'HH:mm:ss") : null;

            [JsonProperty(PropertyName = "timeZone")]
            private string TimeZone
            {
                get
                {
                    var info = Value.GetTimeZone();

                    if (info == null)
                        info = TimeZones.GetInfo(_defaultTimeZone);

                    if (info == null)
                        throw new ApplicationError("Time zone not found: " + (Value.HasValue ? Value.Value.ToString() : _defaultTimeZone));

                    return info.GetAbbreviation().Generic;
                }
            }

            #endregion

            #region Fields

            private string _defaultTimeZone;

            #endregion

            #region Construction

            public JsonDateTimeOffsetOutput(string defaultTimeZone)
            {
                if (defaultTimeZone.IsEmpty())
                    throw new ArgumentNullException(nameof(defaultTimeZone));

                _defaultTimeZone = defaultTimeZone;
            }

            #endregion
        }

        [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
        private class JsonDateTimeOffsetInput : JsonInput
        {
            public override DateTimeOffset? Value =>
                StringValue.IsEmpty() || !DateTimeOffset.TryParse(StringValue, out var value) ? (DateTimeOffset?)null : value;

            [JsonProperty(PropertyName = "timeZone")]
            public string TimeZone { get; set; }

            public override bool IsChanged(DateTimeOffset? value) => Value != value;
        }

        #endregion

        #region Properties

        public string DefaultTimeZone
        {
            get { return (string)(ViewState[nameof(DefaultTimeZone)] ?? CurrentSessionState.Identity?.User?.TimeZone.Id ?? TimeZones.Utc.Id); }
            set { ViewState[nameof(DefaultTimeZone)] = value; }
        }

        public bool ApplyUserTimezone
        {
            get => (bool?)ViewState[nameof(ApplyUserTimezone)] ?? false;
            set => ViewState[nameof(ApplyUserTimezone)] = value;
        }

        [Browsable(true)]
        [DefaultValue(DatePreset.SinceStartOfDay)]
        public DatePreset Preset
        {
            get => (DatePreset?)ViewState[nameof(Preset)] ?? DatePreset.SinceStartOfDay;
            set => ViewState[nameof(Preset)] = value;
        }

        public override DateTimeOffset? Value
        {
            get
            {
                return (DateTimeOffset?)ViewState[nameof(Value)];
            }
            set
            {
                var newValue = value.HasValue
                    ? new DateTimeOffset(
                        value.Value.Year,
                        value.Value.Month,
                        value.Value.Day,
                        value.Value.Hour,
                        value.Value.Minute,
                        0,
                        value.Value.Offset)
                    : (DateTimeOffset?)null;

                if (newValue.HasValue && ApplyUserTimezone)
                {
                    newValue = TimeZones.ConvertFromUtc(
                        newValue.Value,
                        CurrentSessionState.Identity?.User?.TimeZone ?? throw new ArgumentNullException("CurrentSessionState.Identity.User")
                    );
                }

                ViewState[nameof(Value)] = newValue;
            }
        }

        public override bool HasValue => Value.HasValue;

        #endregion

        #region Methods

        protected override JsonOptions GetJsonOptions()
        {
            var result = base.GetJsonOptions();

            result.ShowDate = true;
            result.ShowTime = true;
            result.ShowTimeZone = true;


            var presetExplicitlySet = ViewState[nameof(Preset)] != null;
            var effective = Preset;

            if (!presetExplicitlySet && ID != null)
            {
                var id = ID.ToLowerInvariant();
                if (id.Contains("before"))
                    effective = DatePreset.BeforeEndOfDay;
                else if (id.Contains("since"))
                    effective = DatePreset.SinceStartOfDay;
            }

            result.Preset = (effective == DatePreset.BeforeEndOfDay) ? "before" : "since";
            return result;
        }

        protected override JsonInput GetInputData(string json)
        {
            var result = json.IsEmpty() ? null : JsonConvert.DeserializeObject<JsonDateTimeOffsetInput>(json);

            if (result != null)
            {
                if (result.TimeZone.IsNotEmpty())
                    DefaultTimeZone = result.TimeZone;
            }

            return result;
        }

        protected override JsonOutput GetOutputData()
        {
            return new JsonDateTimeOffsetOutput(DefaultTimeZone)
            {
                FullDateFormat = "MMM D, YYYY h:mm A z",
                ShortDateFormat = "MMM D, YYYY",
            };
        }

        // DON'T REMOVE!
        private static string SetupTimeZones()
        {
            var timeZones = new List<JsonTimeZone>();

            foreach (var tz in TimeZones.Supported)
            {
                var abbrv = tz.GetAbbreviation();

                timeZones.Add(new JsonTimeZone
                {
                    DisplayName = tz.DisplayName,
                    Abbreviation = abbrv.Generic,
                    MomentName = abbrv.Moment
                });
            }

            var timeZonesJson = JsonHelper.SerializeJsObject(timeZones);

            return $"inSite.common.dateTimeOffsetSelector.tz({timeZonesJson});";
        }

        #endregion
    }
}