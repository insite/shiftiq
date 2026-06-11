
using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Registrations
{
    public class RegistrationFeeAssigned : Change
    {
        public decimal? Fee { get; set; }

        public RegistrationFeeAssigned(decimal? fee)
        {
            Fee = fee;
        }
    }
}
