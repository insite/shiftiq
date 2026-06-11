
using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Registrations
{
    public class RegistrationDeleted : Change
    {
        public bool CancelEmptyEvent { get; }

        public RegistrationDeleted(bool cancelEmptyEvent)
        {
            CancelEmptyEvent = cancelEmptyEvent;
        }
    }
}
