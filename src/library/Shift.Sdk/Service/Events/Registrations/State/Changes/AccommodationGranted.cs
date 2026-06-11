using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Registrations
{
    public class AccommodationGranted : Change
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public int? TimeExtension { get; set; }

        public AccommodationGranted(string type, string name, int? timeExtension)
        {
            Type = type;
            Name = name;
            TimeExtension = timeExtension;
        }
    }
}
