using System;

namespace Shift.Contract
{
    public partial class UserConnectionMatch
    {
        public Guid FromUserIdentifier { get; set; }
        public Guid ToUserIdentifier { get; set; }
    }
}