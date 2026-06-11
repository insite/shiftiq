using System;
using System.Collections.Generic;

namespace InSite.Application.Courses.Read
{
    public class QModule
    {
        public Guid ModuleIdentifier { get; set; }
        public Guid UnitIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }

        public Guid CreatedBy { get; set; }
        public Guid ModifiedBy { get; set; }
        public Guid? SourceIdentifier { get; set; }

        public string PrerequisiteDeterminer { get; set; }
        public string ModuleCode { get; set; }
        public string ModuleImage { get; set; }
        public string ModuleName { get; set; }

        public bool ModuleIsAdaptive { get; set; }

        public int ModuleAsset { get; set; }
        public int ModuleSequence { get; set; } = -1;

        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Modified { get; set; }

        public virtual QUnit Unit { get; set; }
        public virtual ICollection<QActivity> Activities { get; set; } = new HashSet<QActivity>();
    }
}
