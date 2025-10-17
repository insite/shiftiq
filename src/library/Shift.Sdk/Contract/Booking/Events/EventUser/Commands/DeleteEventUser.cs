using System;

namespace Shift.Contract
{
    public class DeleteEventUser
    {
        public Guid EventIdentifier { get; set; }
        public Guid UserIdentifier { get; set; }
    }
}