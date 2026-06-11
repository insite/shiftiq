using System;

namespace InSite.Domain.CourseObjects
{
    [Serializable]
    public class Link
    {
        public string Url { get; set; }
        public string Target { get; set; }
        public string Type { get; set; }
    }
}