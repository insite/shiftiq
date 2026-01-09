using System;

using Shift.Common;

namespace Shift.Contract
{
    public class RetrieveAction : Query<ActionModel>
    {
        public Guid ActionIdentifier { get; set; }
    }
}