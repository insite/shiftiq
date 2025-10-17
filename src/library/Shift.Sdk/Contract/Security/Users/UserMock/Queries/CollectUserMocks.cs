using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class CollectUserMocks : Query<IEnumerable<UserMockModel>>, IUserMockCriteria
    {
        public string AccountCode { get; set; }
        public string AddressPostalCode { get; set; }
        public string AddressStreet { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }

        public int? MockNumber { get; set; }

        public DateTime? Birthdate { get; set; }
    }
}