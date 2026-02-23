using System;

using Shift.Common;

namespace Shift.Contract
{
    public class AssertUserConnection : Query<bool>
    {
        public Guid FromUserId { get; set; }
        public Guid ToUserId { get; set; }
    }
}