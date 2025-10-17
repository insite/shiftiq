using System;

using Shift.Common;

namespace Shift.Contract
{
    public class RetrieveEventUser : Query<EventUserModel>
    {
        public Guid EventIdentifier { get; set; }
        public Guid UserIdentifier { get; set; }
    }
}