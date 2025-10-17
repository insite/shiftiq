using System;

using Shift.Common;

namespace Shift.Contract
{
    public class RetrieveEvent : Query<EventModel>
    {
        public Guid EventIdentifier { get; set; }
    }
}