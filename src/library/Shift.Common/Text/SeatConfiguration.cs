using System;
using System.Collections.Generic;

namespace Shift.Common
{
    [Serializable]
    public class SeatConfiguration
    {
        [Serializable]
        public class Price
        {
            public decimal Amount { get; set; }
            public string Name { get; set; }
            public string GroupStatus { get; set; }
        }

        public List<Price> Prices { get; set; }
        public List<string> BillingCustomers { get; set; }
    }
}
