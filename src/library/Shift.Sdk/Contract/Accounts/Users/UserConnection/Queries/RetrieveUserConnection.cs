using System;

using Shift.Common;

namespace Shift.Contract
{
    public class RetrieveUserConnection : Query<UserConnectionModel>
    {
        public Guid FromUserId { get; set; }
        public Guid ToUserId { get; set; }
    }
}