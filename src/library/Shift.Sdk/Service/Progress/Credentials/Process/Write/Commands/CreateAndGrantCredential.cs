using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Credentials.Write
{
    public class CreateAndGrantCredential : Command
    {
        public CreateAndGrantCredential(
            Guid credential,
            Guid tenant,
            Guid achievement,
            Guid user,
            DateTimeOffset granted,
            string description,
            decimal? score,
            Guid? employerGroup,
            string employerGroupStatus
            )
        {
            AggregateIdentifier = credential;
            Tenant = tenant;
            Achievement = achievement;
            User = user;
            Granted = granted;
            Description = description;
            Score = score;
            EmployerGroup = employerGroup;
            EmployerGroupStatus = employerGroupStatus;
        }

        public Guid Tenant { get; private set; }
        public Guid Achievement { get; private set; }
        public Guid User { get; private set; }
        public DateTimeOffset Granted { get; private set; }
        public string Description { get; private set; }
        public decimal? Score { get; private set; }
        public Guid? EmployerGroup { get; private set; }
        public string EmployerGroupStatus { get; private set; }
    }
}
