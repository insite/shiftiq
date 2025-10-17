using System;
using System.Collections.Generic;

namespace Shift.Sdk.UI
{
    [Serializable]
    public class ModuleSerialized
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public List<CourseContentSerialized> Content { get; set; }

        public List<ActivitySerialized> Activities { get; set; }
        public List<PrerequisiteSerialized> Prerequisites { get; set; }
        public List<string> PrivacyGroups { get; set; }
    }
}