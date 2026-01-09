using InSite.Application.Contacts.Read;

using PersonModel = InSite.Domain.Foundations.Person;

namespace InSite.Persistence
{
    public static class PersonAdapter
    {
        public static PersonModel CreatePersonPacket(QPerson person)
        {
            var packet = new PersonModel
            {
                IsAdministrator = person.IsAdministrator,
                IsDeveloper = person.IsDeveloper,
                IsLearner = person.IsLearner,
                IsOperator = person.IsOperator,
                Organization = person.OrganizationIdentifier,
                User = person.UserIdentifier,
            };

            return packet;
        }
    }
}
