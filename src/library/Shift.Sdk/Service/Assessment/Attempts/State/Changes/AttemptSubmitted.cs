using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Attempts
{
    public class AttemptSubmitted : Change
    {
        public string UserAgent { get; }
        public bool Grade { get; }

        public AttemptSubmitted(string userAgent, bool grade)
        {
            UserAgent = userAgent;
            Grade = grade;
        }
    }
}
