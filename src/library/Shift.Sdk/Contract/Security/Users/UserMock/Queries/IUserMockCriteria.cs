using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface IUserMockCriteria
    {
        QueryFilter Filter { get; set; }
        
        string AccountCode { get; set; }
        string AddressPostalCode { get; set; }
        string AddressStreet { get; set; }
        string Email { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }
        string Phone { get; set; }

        int? MockNumber { get; set; }

        DateTime? Birthdate { get; set; }
    }
}