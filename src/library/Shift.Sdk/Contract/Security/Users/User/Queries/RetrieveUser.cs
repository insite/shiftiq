using System;

using Shift.Common;

namespace Shift.Contract
{
    public class RetrieveUser : Query<UserModel>
    {
        public Guid UserId { get; set; }
    }
}