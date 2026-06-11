using System;

namespace InSite.Application.Courses.Read
{
    public class VActivityCompetency
    {
        public Guid ActivityIdentifier { get; set; }
        public Guid CompetencyIdentifier { get; set; }
        public Guid CourseIdentifier { get; set; }

        public string CompetencyCode { get; set; }
        public string CompetencyLabel { get; set; }
        public string CompetencyTitle { get; set; }
        public string CompetencyType { get; set; }
        public string RelationshipType { get; set; }

        public int? CompetencyAsset { get; set; }
    }
}
