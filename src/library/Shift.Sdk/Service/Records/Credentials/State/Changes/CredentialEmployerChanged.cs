using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class CredentialEmployerChanged : Change
    {
        public Guid? EmployerGroup { get; private set; }
        public string EmployerGroupStatus { get; private set; }

        public CredentialEmployerChanged(Guid? employerGroup, string employerGroupStatus)
        {
            EmployerGroup = employerGroup;
            EmployerGroupStatus = employerGroupStatus;
        }
    }
}
