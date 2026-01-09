using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class RetrieveInput : Query<InputModel>
    {
        public Guid ContentIdentifier { get; set; }
    }
}