using System;

using Shift.Constant;

namespace InSite.Domain.CourseObjects
{
    [Serializable]
    public class GradeItem : BaseObject
    {
        public GradeItemFormat Format { get; set; }
        public Guid? Achievement { get; set; }
        public string Name { get; set; }
        public decimal? PassPercent { get; set; }
    }
}