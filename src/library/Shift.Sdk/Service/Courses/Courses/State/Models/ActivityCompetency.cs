using System;

namespace InSite.Domain.Courses
{
    public class ActivityCompetency
    {
        public Guid CompetencyStandardIdentifier { get; set; }
        public string CompetencyCode { get; set; }
        public string RelationshipType { get; set; }

        public ActivityCompetency Clone()
        {
            return (ActivityCompetency)MemberwiseClone();
        }

        public bool Equal(ActivityCompetency c)
        {
            return CompetencyStandardIdentifier == c.CompetencyStandardIdentifier
                && CompetencyCode == c.CompetencyCode
                && RelationshipType == c.RelationshipType
                ;
        }
    }
}
