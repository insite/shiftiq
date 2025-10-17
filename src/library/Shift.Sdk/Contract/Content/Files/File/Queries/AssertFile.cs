using System;

using Shift.Common;

namespace Shift.Contract
{
    public class AssertFile : Query<bool>
    {
        public Guid FileIdentifier { get; set; }
    }
}