using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Application.Registrations.Read;
using InSite.Common.Web.UI;

using Newtonsoft.Json;

using Shift.Common;

namespace InSite.Admin.Events.Reports.Controls
{
    public partial class StatisticsAccommodationsPivotTable : BaseUserControl
    {
        #region Constants

        protected static readonly Dictionary<string, int> DimensionsAccommodationMapping;
        protected static readonly Dictionary<string, int> DimensionsAttendanceMapping;
        protected static readonly Dictionary<string, int> DimensionsFormatMapping;
        protected static readonly Dictionary<string, int> DimensionsExamTypeMapping;
        protected static readonly Dictionary<string, int> DimensionsLevelTypeMapping;

        #endregion

        #region Classes

        [Serializable, JsonObject(MemberSerialization = MemberSerialization.OptIn)]
        protected class DimensionDataItem
        {
            #region Properties

            [JsonProperty(PropertyName = "accommodation")]
            public DimensionsDataOption[] Accommodation { get; set; }

            [JsonProperty(PropertyName = "attendance")]
            public DimensionsDataOption[] Attendance { get; set; }

            [JsonProperty(PropertyName = "format")]
            public DimensionsDataOption[] Format { get; set; }

            [JsonProperty(PropertyName = "examType")]
            public DimensionsDataOption[] ExamType { get; set; }

            [JsonProperty(PropertyName = "levelType")]
            public DimensionsDataOption[] LevelType { get; set; }

            [JsonProperty(PropertyName = "venue")]
            public DimensionsDataOption[] Venue { get; set; }

            #endregion
        }

        [Serializable, JsonObject(MemberSerialization = MemberSerialization.OptIn)]
        protected class DimensionsDataOption
        {
            #region Properties

            [JsonProperty(PropertyName = "value")]
            public int Value { get; set; }

            [JsonProperty(PropertyName = "text")]
            public string Text { get; set; }

            #endregion
        }

        [Serializable, JsonObject(MemberSerialization = MemberSerialization.OptIn)]
        protected class TableDataItem
        {
            #region Properties

            [JsonProperty(PropertyName = "accommodation")]
            public int? Accommodation { get; set; }

            [JsonProperty(PropertyName = "attendance")]
            public int? Attendance { get; set; }

            [JsonProperty(PropertyName = "format")]
            public int? Format { get; set; }

            [JsonProperty(PropertyName = "examType")]
            public int? ExamType { get; set; }

            [JsonProperty(PropertyName = "levelType")]
            public int? LevelType { get; set; }

            [JsonProperty(PropertyName = "venue")]
            public int? Venue { get; set; }

            [JsonProperty(PropertyName = "count")]
            public int Count { get; set; }

            #endregion
        }

        #endregion

        #region Properties

        protected TableDataItem[] TableData
        {
            get => (TableDataItem[])ViewState[nameof(TableData)];
            set => ViewState[nameof(TableData)] = value;
        }

        private DimensionsDataOption[] VenueData
        {
            get => (DimensionsDataOption[])ViewState[nameof(VenueData)];
            set => ViewState[nameof(VenueData)] = value;
        }

        #endregion

        #region Fields

        private static readonly DimensionDataItem _predefinedDimensions = null;

        protected DimensionDataItem DimensionsData = null;

        #endregion

        #region Construction

        static StatisticsAccommodationsPivotTable()
        {
            _predefinedDimensions = new DimensionDataItem
            {
                Accommodation = new[]
                {
                    new DimensionsDataOption { Text = "Reader", Value = 1 },
                    new DimensionsDataOption { Text = "Time Extension", Value = 2 },
                    new DimensionsDataOption { Text = "Translator", Value = 3 },
                    new DimensionsDataOption { Text = "Translation Dictionary", Value = 4 },
                    new DimensionsDataOption { Text = "Scribe", Value = 5 },
                    new DimensionsDataOption { Text = "Split Exam", Value = 6 }
                },
                Attendance = new[]
                {
                    new DimensionsDataOption { Text = "Pending", Value = 1 },
                    new DimensionsDataOption { Text = "Present", Value = 2 },
                    new DimensionsDataOption { Text = "Absent", Value = 3 }
                },
                Format = new[]
                {
                    new DimensionsDataOption { Text = EventExamFormat.Online.Value, Value = 1 },
                    new DimensionsDataOption { Text = EventExamFormat.Paper.Value, Value = 2 }
                },
                ExamType = new[]
                {
                    new DimensionsDataOption { Text = EventExamType.Class.Value, Value = 1 },
                    new DimensionsDataOption { Text = EventExamType.IndividualA.Value, Value = 2 },
                    new DimensionsDataOption { Text = EventExamType.IndividualN.Value, Value = 3 },
                    new DimensionsDataOption { Text = EventExamType.Sitting.Value, Value = 4 },
                    new DimensionsDataOption { Text = EventExamType.Test.Value, Value = 5 }
                },
                LevelType = new[]
                {
                    new DimensionsDataOption { Text = "CofQ", Value = 1 },
                    new DimensionsDataOption { Text = "EE", Value = 2 },
                    new DimensionsDataOption { Text = "FE", Value = 3 },
                    new DimensionsDataOption { Text = "IPSE", Value = 4 },
                    new DimensionsDataOption { Text = "SLE", Value = 5 }
                }
            };

            DimensionsAccommodationMapping = _predefinedDimensions.Accommodation.ToDictionary(x => x.Text, x => x.Value);
            DimensionsAttendanceMapping = _predefinedDimensions.Attendance.ToDictionary(x => x.Text, x => x.Value);
            DimensionsFormatMapping = _predefinedDimensions.Format.ToDictionary(x => x.Text, x => x.Value);
            DimensionsExamTypeMapping = _predefinedDimensions.ExamType.ToDictionary(x => x.Text, x => x.Value);
            DimensionsLevelTypeMapping = _predefinedDimensions.LevelType.ToDictionary(x => x.Text, x => x.Value);
        }

        #endregion

        #region Initialization and loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PivotSave.Click += PivotSave_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                PivotState.Value = PersonalizationRepository.GetValue<string>(
                    Organization.OrganizationIdentifier,
                    User.UserIdentifier,
                    "ui/admin/events:AccommodationsPivotState",
                    false);
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            DimensionsData = new DimensionDataItem
            {
                Accommodation = _predefinedDimensions.Accommodation,
                Attendance = _predefinedDimensions.Attendance,
                Format = _predefinedDimensions.Format,
                ExamType = _predefinedDimensions.ExamType,
                LevelType = _predefinedDimensions.LevelType,
                Venue = VenueData
            };

            base.OnPreRender(e);
        }

        #endregion

        #region Event handlers

        private void PivotSave_Click(object sender, EventArgs e)
        {
            PersonalizationRepository.SetValue(
                Organization.OrganizationIdentifier,
                User.UserIdentifier,
                "ui/admin/events:AccommodationsPivotState",
                PivotState.Value);
        }

        #endregion

        #region Methods (data binding)

        public bool LoadData(IEnumerable<QRegistration> registrations)
        {
            TableData = null;

            if (!registrations.Any())
                return false;

            try
            {
                VenueData = registrations
                    .Where(x => x.Event?.VenueLocation != null)
                    .Select(x => x.Event.VenueLocation)
                    .GroupBy(x => x.GroupIdentifier)
                    .Select(g => new DimensionsDataOption
                    {
                        Value = g.Key.GetHashCode(),
                        Text = g.First().GroupName
                    })
                    .OrderBy(x => x.Text)
                    .ToArray();
            }
            catch (Exception ex)
            {
                var source = typeof(StatisticsAccommodationsPivotTable).FullName + "." + nameof(LoadData);
                var error = "An unexpected error occurred building statistics for venue data. " + ex.Message;
                AppSentry.SentryError(error, source);
                PivotError.InnerText = error;
                PivotError.Visible = true;
                return false;
            }

            try
            {
                TableData = registrations
                    .Where(x => x.Accommodations.Count > 0 && x.Attempt != null && x.Attempt.AttemptGraded.HasValue)
                    .SelectMany(r => r.Accommodations.Select(a => new
                    {
                        Accommodation = DimensionsAccommodationMapping.TryGetValue(a.AccommodationType ?? string.Empty, out var accommodationKey) ? accommodationKey : (int?)null,
                        Attendance = DimensionsAttendanceMapping.TryGetValue(r.AttendanceStatus ?? string.Empty, out var attendanceKey) ? attendanceKey : (int?)null,
                        Format = DimensionsFormatMapping.TryGetValue(r.Event?.EventFormat ?? string.Empty, out var formatKey) ? formatKey : (int?)null,
                        ExamType = DimensionsExamTypeMapping.TryGetValue(r.Event?.ExamType ?? string.Empty, out var examTypeKey) ? examTypeKey : (int?)null,
                        LevelType = DimensionsLevelTypeMapping.TryGetValue(r.Form?.BankLevelType ?? string.Empty, out var levelTypeKey) ? levelTypeKey : (int?)null,
                        Venue = r.Event?.VenueLocation?.GroupIdentifier
                    }))
                    .GroupBy(x => new { x.Accommodation, x.Attendance, x.Format, x.ExamType, x.LevelType, x.Venue })
                    .Select(x => new TableDataItem
                    {
                        Accommodation = x.Key.Accommodation,
                        Attendance = x.Key.Attendance,
                        Format = x.Key.Format,
                        ExamType = x.Key.ExamType,
                        LevelType = x.Key.LevelType,
                        Venue = x.Key.Venue?.GetHashCode(),
                        Count = x.Count()
                    })
                    .ToArray();
            }
            catch (Exception ex)
            {
                var source = typeof(StatisticsAccommodationsPivotTable).FullName + "." + nameof(LoadData);
                var error = "An unexpected error occurred building statistics for accommodations data. " + ex.Message;
                AppSentry.SentryError(error, source);
                PivotError.InnerText = error;
                PivotError.Visible = true;
                return false;
            }

            return TableData.Any();
        }

        #endregion
    }
}