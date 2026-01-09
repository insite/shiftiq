using System;

using Shift.Common;

namespace Shift.Contract
{
    public class AssertEventUser : Query<bool>
    {
        public Guid EventIdentifier { get; set; }
        public Guid UserIdentifier { get; set; }
    }
}