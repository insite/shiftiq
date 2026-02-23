using System;

using Shift.Common;

namespace Shift.Contract
{
    public class RetrieveStandard : Query<StandardModel>
    {
        public Guid StandardId { get; set; }
    }
}