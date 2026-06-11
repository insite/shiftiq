using System;

using Shift.Common;

namespace Shift.Contract
{
    public class AssertUserSession : Query<bool>
    {
        public Guid SessionId { get; set; }
    }
}