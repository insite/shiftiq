using System;

namespace InSite.Persistence
{
    public class TCourseCategory
    {
        public Guid CourseIdentifier { get; set; }
        public Guid ItemIdentifier { get; set; }
        public Guid? OrganizationIdentifier { get; set; }

        public int? CategorySequence { get; set; }

        public virtual TCollectionItem Category { get; set; }
    }
}
