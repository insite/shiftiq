using System;

using Shift.Common;

namespace Shift.Contract
{
    public class RetrieveRecipient : Query<RecipientModel>
    {
        public Guid RecipientId { get; set; }
    }
}