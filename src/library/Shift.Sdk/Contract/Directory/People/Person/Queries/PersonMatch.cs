using System;

namespace Shift.Contract
{
    public partial class PersonMatch
    {
        // Identity

        public Guid PersonId { get; set; }
        public string PersonFirstName { get; set; }
        public string PersonLastName { get; set; }

        // User Account

        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public bool IsAdministrator { get; set; }
        public bool IsDeveloper { get; set; }
        public bool IsLearner { get; set; }
        public bool IsOperator { get; set; }

        // Demographics

        public string TimeZone { get; set; }
    }
}
