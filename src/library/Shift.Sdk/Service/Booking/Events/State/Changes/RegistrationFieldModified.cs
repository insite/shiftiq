using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Events
{
    public class RegistrationFieldModified : Change
    {
        public RegistrationField Field { get; }

        public RegistrationFieldModified(RegistrationField field)
        {
            Field = field ?? throw new ArgumentNullException(nameof(field));
        }
    }
}
