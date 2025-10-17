using System;
using System.Collections.Generic;

namespace InSite.Domain.CourseObjects
{
    [Serializable]
    public class Gradebook
    {
        public Guid Identifier { get; set; }
        public Guid? Achievement { get; set; }

        public bool IsLocked { get; set; }

        public List<GradeItem> Items { get; set; }

        public Gradebook()
        {
            Items = new List<GradeItem>();
        }
    }
}