using System;
using System.Collections.Generic;

namespace Shift.Sdk.UI
{
    [Serializable]
    public class ActivitySerialized
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }

        public List<CourseContentSerialized> Content { get; set; }

        public string Url { get; set; }
        public string UrlType { get; set; }
        public string UrlTarget { get; set; }

        public string ContentDeliveryPlatform { get; set; }

        public int? DurationMinutes { get; set; }

        public bool IsAdaptive { get; set; }

        public string Requirement { get; set; }

        public string Assessment { get; set; }
        public string Survey { get; set; }

        public List<PrerequisiteSerialized> Prerequisites { get; set; }
        public List<string> PrivacyGroups { get; set; }
        public List<CompetencySerialized> Competencies { get; set; }
    }
}