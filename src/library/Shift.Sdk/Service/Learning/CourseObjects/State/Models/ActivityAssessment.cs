using System;

namespace InSite.Domain.CourseObjects
{
    [Serializable]
    public class ActivityAssessment : BaseObject
    {
        public BaseObject Bank { get; set; }
        public string Name { get; set; }
        public string Instructions { get; set; }
        public string Disclosure { get; set; }
    }
}