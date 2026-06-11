using System;

using Shift.Common;

namespace Shift.Contract
{
    public class AssertRecipient : Query<bool>
    {
        public Guid RecipientId { get; set; }
    }
}