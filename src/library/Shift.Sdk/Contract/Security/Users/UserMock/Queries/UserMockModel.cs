using System;

namespace Shift.Contract
{
    public partial class UserMockModel
    {
        public Guid MockIdentifier { get; set; }

        public string AccountCode { get; set; }
        public string AddressPostalCode { get; set; }
        public string AddressStreet { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }

        public int MockNumber { get; set; }

        public DateTime Birthdate { get; set; }
    }
}