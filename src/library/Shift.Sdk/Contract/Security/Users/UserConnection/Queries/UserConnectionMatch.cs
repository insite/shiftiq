using System;

namespace Shift.Contract
{
    public partial class UserConnectionMatch
    {
        public Guid FromUserId { get; set; }
        public Guid ToUserId { get; set; }
    }
}