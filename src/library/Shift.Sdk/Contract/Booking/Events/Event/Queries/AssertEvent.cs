using System;

using Shift.Common;

namespace Shift.Contract
{
    public class AssertEvent : Query<bool>
    {
        public Guid EventIdentifier { get; set; }
    }
}