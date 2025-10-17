using System;

using Shift.Common;

namespace Shift.Contract
{
    public class AssertUserConnection : Query<bool>
    {
        public Guid FromUserIdentifier { get; set; }
        public Guid ToUserIdentifier { get; set; }
    }
}