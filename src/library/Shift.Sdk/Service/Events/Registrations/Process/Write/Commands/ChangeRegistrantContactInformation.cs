using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Registrations;

namespace InSite.Application.Registrations.Write
{
    public class ChangeRegistrantContactInformation : Command
    {
        public RegistrantChangedField[] ChangedFields { get; set; }

        public ChangeRegistrantContactInformation(Guid registration, RegistrantChangedField[] changedFields)
        {
            AggregateIdentifier = registration;
            ChangedFields = changedFields;
        }
    }
}
