using System;
using System.Collections.Generic;

using Shift.Common;

using Shift.Constant;

namespace InSite.Domain.CourseObjects
{
    [Serializable]
    public class Unit : BaseObject
    {
        public int Asset { get; set; }

        public Course Course { get; set; }
        public BaseObject GradeItem { get; set; }

        public List<Module> Modules { get; set; }
        public List<Prerequisite> Prerequisites { get; set; }
        public List<PrivacyGroup> PrivacyGroups { get; set; }

        public ContentContainer Content { get; set; }

        public bool HasPrerequisites => Prerequisites.Count > 0;
        public bool IsAdaptive { get; set; }

        public DeterminerType PrerequisiteDeterminer { get; set; }

        public Unit()
        {
            Modules = new List<Module>();
            Prerequisites = new List<Prerequisite>();
            PrivacyGroups = new List<PrivacyGroup>();
        }
    }
}
