using System;

namespace InSite.Domain.CourseObjects
{
    [Serializable]
    public class BaseObject
    {
        public Guid Identifier { get; set; }

        public string Code { get; set; }
        public string Type { get; set; }
    }
}