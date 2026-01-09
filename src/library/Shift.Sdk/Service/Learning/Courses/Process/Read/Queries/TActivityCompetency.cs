using System;

namespace InSite.Application.Courses.Read
{
    public class TActivityCompetency
    {
        public Guid ActivityIdentifier { get; set; }
        public Guid CompetencyIdentifier { get; set; }
        
        public string CompetencyCode { get; set; }
        public string RelationshipType { get; set; }
    }
}
