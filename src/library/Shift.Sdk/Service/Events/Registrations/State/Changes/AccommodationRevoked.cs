
using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Registrations
{
    public class AccommodationRevoked : Change
    {
        public string Type { get; set; }

        public AccommodationRevoked(string type)
        {
            Type = type;
        }
    }
}
