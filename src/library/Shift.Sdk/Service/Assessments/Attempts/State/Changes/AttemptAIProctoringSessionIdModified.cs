using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Attempts
{
    public class AttemptAIProctoringSessionIdModified : Change
    {
        public string SessionId { get; set; }

        public AttemptAIProctoringSessionIdModified(string sessionId)
        {
            SessionId = sessionId;
        }
    }
}
