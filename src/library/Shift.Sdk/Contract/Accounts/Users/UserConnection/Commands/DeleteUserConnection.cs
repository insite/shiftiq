using System;

namespace Shift.Contract
{
    public class DeleteUserConnection
    {
        public Guid FromUserId { get; set; }
        public Guid ToUserId { get; set; }
    }
}