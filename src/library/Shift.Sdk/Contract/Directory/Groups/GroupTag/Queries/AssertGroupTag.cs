using System;

using Shift.Common;

namespace Shift.Contract
{
    public class AssertGroupTag : Query<bool>
    {
        public Guid TagIdentifier { get; set; }
    }
}