using System;

using Shift.Common;

namespace InSite.Application.Contacts.Read
{
    [Serializable]
    public class QUserFilter : Filter
    {
        public Guid[] OrganizationIdentifiers { get; set; }
        public Guid[] AttemptFormOrganizationIdentifiers { get; set; }
        public Guid[] UserIdentifiers { get; set; }
        public Guid? AttemptBankIdentifier { get; set; }
        public Guid? AttemptFormIdentifier { get; set; }
        public string Code { get; set; }
        public string FullName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string NameFilterType { get; set; }
        public string NameOrCode { get; set; }
        public string EmailOrAlternate { get; set; }
        public bool? MustHaveAttempts { get; set; }
        public Guid? RegistrationEventIdentifier { get; set; }
        public Guid? ExcludeGradebookIdentifier { get; set; }
        public Guid? ExcludeLearnerJournalSetupIdentifier { get; set; }

        public QUserFilter Clone()
        {
            return (QUserFilter)MemberwiseClone();
        }
    }
}
