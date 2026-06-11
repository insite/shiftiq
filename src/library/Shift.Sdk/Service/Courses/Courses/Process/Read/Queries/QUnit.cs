using System;
using System.Collections.Generic;

namespace InSite.Application.Courses.Read
{
    public class QUnit
    {
        public Guid CourseIdentifier { get; set; }
        public Guid UnitIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }

        public Guid CreatedBy { get; set; }
        public Guid ModifiedBy { get; set; }
        public Guid? SourceIdentifier { get; set; }

        public string PrerequisiteDeterminer { get; set; }
        public string UnitCode { get; set; }
        public string UnitName { get; set; }

        public bool UnitIsAdaptive { get; set; }

        public int UnitAsset { get; set; }
        public int UnitSequence { get; set; } = -1;

        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Modified { get; set; }

        public virtual QCourse Course { get; set; }
        public virtual ICollection<QModule> Modules { get; set; } = new HashSet<QModule>();
    }
}
