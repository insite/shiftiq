using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface IPersonAddressCriteria
    {
        QueryFilter Filter { get; set; }
        
        string City { get; set; }
        string Country { get; set; }
        string Description { get; set; }
        string PostalCode { get; set; }
        string Province { get; set; }
        string Street1 { get; set; }
        string Street2 { get; set; }
    }
}