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
        public int? QuestionIndex { get; set; }

        [JsonIgnore]
        public double Duration => ((Ended ?? Pinged ?? Started) - Started).TotalSeconds;

        public AttemptTimeInterval()
        {

        }

        public AttemptTimeInterval(DateTimeOffset started, int? section, int? question)
        {
            Started = started;
            SectionIndex = section;
            QuestionIndex = question;
        }

        public double GetDuration(DateTimeOffset defaultEndDate)
        {
            return ((Ended ?? defaultEndDate) - Started).TotalSeconds;
        }
    }
}
