using System;

using Shift.Common;

namespace Shift.Contract
{
    public class AssertPage : Query<bool>
    {
        public Guid PageIdentifier { get; set; }
    }
}