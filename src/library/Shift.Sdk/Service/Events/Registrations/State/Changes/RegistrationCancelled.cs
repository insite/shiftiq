using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Registrations
{
    public class RegistrationCancelled : Change
    {
        public string Reason { get; }
        public bool CancelEmptyEvent { get; }

        public RegistrationCancelled(string reason, bool cancelEmptyEvent)
        {
            Reason = reason;
            CancelEmptyEvent = cancelEmptyEvent;
        }
    }
}
