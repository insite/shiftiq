using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Credentials.Write
{
    public class ChangeCredentialEmployer : Command
    {
        public Guid? EmployerGroup { get; private set; }
        public string EmployerGroupStatus { get; private set; }

        public ChangeCredentialEmployer(Guid credential, Guid? employerGroup, string employerGroupStatus)
        {
            AggregateIdentifier = credential;
            EmployerGroup = employerGroup;
            EmployerGroupStatus = employerGroupStatus;
        }
    }
}
