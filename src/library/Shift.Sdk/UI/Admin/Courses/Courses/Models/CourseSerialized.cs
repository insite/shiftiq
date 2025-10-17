using System;
using System.Collections.Generic;

namespace Shift.Sdk.UI
{
    [Serializable]
    public class CourseSerialized
    {
        public string Code { get; set; }
        public string Name { get; set; }

        public string Framework { get; set; }

        public string Icon { get; set; }
        public string Image { get; set; }
        public string Label { get; set; }
        public string Slug { get; set; }
        public string Style { get; set; }
        public string FlagText { get; set; }
        public string FlagColor { get; set; }
        public string Url { get; set; }
        public bool AllowMultipleUnits { get; set; }
        public bool IsProgressReportEnabled { get; set; }
        public int? OutlineWidth { get; set; }

        public List<CourseContentSerialized> Content { get; set; }
        public List<UnitSerialized> Units { get; set; }
        public List<string> PrivacyGroups { get; set; }
    }
}