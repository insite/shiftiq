using System;

using Shift.Common;

namespace Shift.Contract
{
    public class AssertUser : Query<bool>
    {
        public Guid UserId { get; set; }
    }
}