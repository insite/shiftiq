using System;

namespace InSite.Domain.Contacts
{
    public class PersonOrganizationListDataItem
    {
        public Guid OrganizationIdentifier { get; set; }

        public string OrganizationCode { get; set; }
        public string OrganizationDomain { get; set; }
        public string OrganizationName { get; set; }
        public string OrganizationStatus { get; set; }
        public string OrganizationUrl { get; set; }

        public bool PersonIsAdministrator { get; set; }
        public bool PersonIsDeveloper { get; set; }
        public bool PersonIsLearner { get; set; }
        public bool PersonIsOperator { get; set; }

        public bool PersonIsGrantedAccess { get; set; }
    }
}
