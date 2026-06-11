using System;

namespace InSite.Application.Events.Read
{
    [Serializable]
    public class QExamSessionsScheduled
    {
        public int Classes { get; set; }
        public int Sittings { get; set; }
        public int Accommodated { get; set; }
        public int Individuals { get; set; }
        public int Online { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
    }
}
