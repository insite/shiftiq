using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Attempts.Write
{
    public class ModifyAIProctoringSessionId : Command
    {
        public string SessionId { get; set; }

        public ModifyAIProctoringSessionId(Guid aggregate, string sessionId)
        {
            AggregateIdentifier = aggregate;
            SessionId = sessionId;
        }
    }
}
