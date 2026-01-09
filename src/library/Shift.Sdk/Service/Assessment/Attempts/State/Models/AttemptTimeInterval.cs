using System;

using Newtonsoft.Json;

namespace InSite.Domain.Attempts
{
    public class AttemptTimeInterval
    {
        public DateTimeOffset Started { get; set; }
        public DateTimeOffset? Pinged { get; set; }
        public DateTimeOffset? Ended { get; set; }
        public int? SectionIndex { get; set; }

        [JsonIgnore]
        public double Duration => ((Ended ?? Pinged ?? Started) - Started).TotalSeconds;

        public AttemptTimeInterval()
        {

        }

        public AttemptTimeInterval(DateTimeOffset started, int? section)
        {
            Started = started;
            SectionIndex = section;
        }

        public double GetDuration(DateTimeOffset defaultEndDate)
        {
            return ((Ended ?? defaultEndDate) - Started).TotalSeconds;
        }
    }
}
