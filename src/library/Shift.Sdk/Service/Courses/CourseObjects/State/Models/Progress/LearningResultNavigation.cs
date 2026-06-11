using System;

namespace InSite.Domain.CourseObjects
{
    [Serializable]
    public class LearningResultNavigation
    {
        public bool IsSelected { get; set; }
        public Guid ActivityIdentifier { get; set; }
        public int PageNumber { get; set; }
        public string Url { get; set; }
    }
}