using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.People.Write.Old
{
    public class ChangePersonAuthorization : Command
    {
        public bool IsAdministrator { get; set; }
        public bool IsUser { get; set; }
        public bool IsEmployee { get; set; }
        public bool IsCustomer { get; set; }

        public ChangePersonAuthorization(Guid person, bool isAdministrator, bool isUser, bool isEmployee, bool isCustomer)
        {
            AggregateIdentifier = person;
            IsAdministrator = isAdministrator;
            IsUser = isUser;
            IsEmployee = isEmployee;
            IsCustomer = isCustomer;
        }
    }

    public class ChangePersonIdentification : Command
    {
        public string EmployeeUnion { get; set; }
        public string SocialInsuranceNumber { get; set; }

        public ChangePersonIdentification(Guid person, string employeeUnion, string socialInsuranceNumber)
        {
            AggregateIdentifier = person;
            EmployeeUnion = employeeUnion;
            SocialInsuranceNumber = socialInsuranceNumber;
        }
    }

    public class ConnectPerson : Command
    {
        public Guid User { get; set; }
        public Guid Tenant { get; set; }

        public ConnectPerson(Guid person, Guid user, Guid tenant)
        {
            AggregateIdentifier = person;
            User = user;
            Tenant = tenant;
        }
    }

    public class DisconnectPerson : Command
    {
        public string Reason { get; set; }

        public DisconnectPerson(Guid person, string reason)
        {
            AggregateIdentifier = person;
            Reason = reason;
        }
    }
}