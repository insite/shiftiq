using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Surveys.Sessions
{
    public class ResponseUserChanged : Change
    {
        public Guid User { get; }

        public ResponseUserChanged(Guid user)
        {
            User = user;
        }
    }
}
