using System;

using Shift.Common;

namespace Shift.Contract
{
    public class RetrieveUserSession : Query<UserSessionModel>
    {
        public Guid SessionId { get; set; }
    }
}