using System;

namespace InSite.Persistence
{
    public class ContactExperience
    {
        public Guid ExperienceIdentifier { get; set; }
        public String AuthorityCity { get; set; }
        public String AuthorityCountry { get; set; }
        public String AuthorityName { get; set; }
        public String AuthorityProvince { get; set; }
        public String ContactExperienceType { get; set; }
        public String Description { get; set; }
        public String Status { get; set; }
        public String Title { get; set; }
        public Boolean IsSuccess { get; set; }
        public Int32? LifetimeMonths { get; set; }
        public Guid UserIdentifier { get; set; }
        public Decimal? CreditHours { get; set; }
        public Decimal? Score { get; set; }
        public DateTime? Completed { get; set; }
        public DateTime? Expired { get; set; }

        public virtual User User { get; set; }
    }
}
