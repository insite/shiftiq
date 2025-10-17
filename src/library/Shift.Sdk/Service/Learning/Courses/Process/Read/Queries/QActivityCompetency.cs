using System;

namespace InSite.Application.Courses.Read
{
    public class QActivityCompetency
    {
        public Guid ActivityIdentifier { get; set; }
        public Guid CompetencyStandardIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }

        public string CompetencyCode { get; set; }
        public string RelationshipType { get; set; }

        public virtual QActivity Activity { get; set; }
    }
}
