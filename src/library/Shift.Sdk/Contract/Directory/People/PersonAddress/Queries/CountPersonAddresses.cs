using System;

using Shift.Common;

namespace Shift.Contract
{
    public class CountPersonAddresses : Query<int>, IPersonAddressCriteria
    {
        public string City { get; set; }
        public string Country { get; set; }
        public string Description { get; set; }
        public string PostalCode { get; set; }
        public string Province { get; set; }
        public string Street1 { get; set; }
        public string Street2 { get; set; }
    }
}