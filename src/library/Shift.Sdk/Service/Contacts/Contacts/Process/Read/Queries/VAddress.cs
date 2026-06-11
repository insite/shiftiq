using System;
using System.Collections.Generic;

using Shift.Common;

namespace InSite.Application.Contacts.Read
{
    public class VAddress
    {
        public VAddress()
        {
            BillingPersons = new HashSet<VPerson>();
            HomePersons = new HashSet<VPerson>();
            ShippingPersons = new HashSet<VPerson>();
            WorkPersons = new HashSet<VPerson>();
        }

        public Guid AddressIdentifier { get; set; }
        public String City { get; set; }
        public String Country { get; set; }
        public String Description { get; set; }
        public String PostalCode { get; set; }
        public String Province { get; set; }
        public String Street1 { get; set; }
        public String Street2 { get; set; }

        public virtual ICollection<VPerson> BillingPersons { get; set; }
        public virtual ICollection<VPerson> HomePersons { get; set; }
        public virtual ICollection<VPerson> ShippingPersons { get; set; }
        public virtual ICollection<VPerson> WorkPersons { get; set; }

        public override string ToString() =>
            LocationHelper.ToString(Street1, Street2, City, Province, PostalCode, Country, null, null, "\r\n");
    }
}
