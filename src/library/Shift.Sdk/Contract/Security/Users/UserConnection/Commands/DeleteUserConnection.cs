using System;

namespace Shift.Contract
{
    public class DeleteUserConnection
    {
        public Guid FromUserIdentifier { get; set; }
        public Guid ToUserIdentifier { get; set; }
    }
}