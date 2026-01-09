using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Credentials.Write
{
    public class GrantCredential : Command
    {
        public GrantCredential(
            Guid credential,
            DateTimeOffset granted,
            string description,
            decimal? score,
            Guid? employerGroup,
            string employerGroupStatus
            )
        {
            AggregateIdentifier = credential;
            Granted = granted;
            Description = description;
            Score = score;
            EmployerGroup = employerGroup;
            EmployerGroupStatus = employerGroupStatus;
        }

        public DateTimeOffset Granted { get; private set; }
        public string Description { get; private set; }
        public decimal? Score { get; private set; }
        public Guid? EmployerGroup { get; private set; }
        public string EmployerGroupStatus { get; private set; }
    }
}