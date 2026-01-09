using System;

using Shift.Common;

namespace Shift.Contract
{
    public class CountRegistrationInstructors : Query<int>, IRegistrationInstructorCriteria
    {
        public Guid? OrganizationIdentifier { get; set; }
    }
}