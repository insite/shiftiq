using System;

namespace Shift.Contract
{
    public class PendingPersonImportModel
    {
        public Guid PendingPersonId { get; set; }

        public string PersonCode { get; set; }
        public string UserEmail { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public string EmployeeStatus { get; set; }
        public int MatchCount { get; set; }
    }
}
