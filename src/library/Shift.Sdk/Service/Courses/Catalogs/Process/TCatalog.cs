using System;
using System.Collections.Generic;

namespace InSite.Application.Courses.Read
{
    public class TCatalog
    {
        public Guid CatalogIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }

        public string CatalogName { get; set; }

        public bool IsHidden { get; set; }

        public virtual ICollection<QCourse> Courses { get; set; } = new HashSet<QCourse>();
    }
}
