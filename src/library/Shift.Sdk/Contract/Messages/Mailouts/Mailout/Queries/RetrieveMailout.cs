using System;

using Shift.Common;

namespace Shift.Contract
{
    public class RetrieveMailout : Query<MailoutModel>
    {
        public Guid MailoutId { get; set; }
    }
}