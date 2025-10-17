using System;

using Shift.Common;

namespace Shift.Contract
{
    public class RetrieveUserConnection : Query<UserConnectionModel>
    {
        public Guid FromUserIdentifier { get; set; }
        public Guid ToUserIdentifier { get; set; }
    }
}