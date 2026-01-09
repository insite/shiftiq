using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class AssertInput : Query<bool>
    {
        public Guid ContentIdentifier { get; set; }
    }
}