using System;

using Shift.Constant;

namespace InSite.Domain.CourseObjects
{
    [Serializable]
    public class Prerequisite
    {
        public PrerequisiteType Type { get; set; }
        public PrerequisiteCondition Condition { get; set; }
        public BaseObject Lock { get; set; }

        public Prerequisite()
        {
            Condition = new PrerequisiteCondition();
            Lock = new BaseObject();
        }
    }

    [Serializable]
    public class PrerequisiteCondition : BaseObject
    {
        public int? ScoreFrom { get; set;}
        public int? ScoreThru { get; set; }
    }
}