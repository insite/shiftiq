using System;
using System.Collections.Generic;

using Newtonsoft.Json;

using Shift.Common;

namespace InSite.Persistence
{
    public class DepartmentProfileUser : IHasTimestamp
    {
        public Guid CreatedBy { get; set; }
        public Guid ModifiedBy { get; set; }

        public bool IsInProgress { get; set; }
        public bool IsPrimary { get; set; }
        public bool IsRecommended { get; set; }
        public bool IsRequired { get; set; }

        public Guid DepartmentIdentifier { get; set; }
        public Guid ProfileStandardIdentifier { get; set; }
        public Guid UserIdentifier { get; set; }

        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Modified { get; set; }

        public virtual Department Department { get; set; }
        public virtual Standard Profile { get; set; }
        public virtual User User { get; set; }

        public string GetEntityData()
        {
            var clone = new DepartmentProfileUser();

            this.ShallowCopyTo(clone);

            return JsonConvert.SerializeObject(clone);
        }

        public string GetEntityName()
            => "DepartmentProfileUser";

        public static readonly ICollection<string> DiffExclusions = new HashSet<string>
        {
            "CreatedBy",
            "ModifiedBy",
            "Created",
            "Modified"
        };
    }
}
