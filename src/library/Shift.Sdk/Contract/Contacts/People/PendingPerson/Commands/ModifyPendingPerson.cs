using System;

namespace Shift.Contract
{
    public class ModifyPendingPerson
    {
        public Guid PendingId { get; set; }
        public Guid? PersonId { get; set; }
        public Guid? UserId { get; set; }

        public string PersonCode { get; set; }
        public string UserEmail { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
    }
}
