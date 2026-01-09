using System;

namespace Shift.Contract
{
    public partial class PersonMatch
    {
        public Guid PersonId { get; set; }
        public Guid UserId { get; set; }

        public string UserName { get; set; }
        public string UserEmail { get; set; }

        public bool IsAdministrator { get; set; }
        public bool IsDeveloper { get; set; }
        public bool IsOperator { get; set; }

        public string TimeZone { get; set; }
    }
}