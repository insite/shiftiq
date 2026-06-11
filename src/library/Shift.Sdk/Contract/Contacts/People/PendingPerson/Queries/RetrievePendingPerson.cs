using System;

using Shift.Common;

namespace Shift.Contract
{
    public class RetrievePendingPerson : Query<PendingPersonModel>
    {
        public Guid PendingId { get; set; }
    }
}
