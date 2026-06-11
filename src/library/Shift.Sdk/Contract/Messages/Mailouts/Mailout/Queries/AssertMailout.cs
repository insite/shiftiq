using System;

using Shift.Common;

namespace Shift.Contract
{
    public class AssertMailout : Query<bool>
    {
        public Guid MailoutId { get; set; }
    }
}