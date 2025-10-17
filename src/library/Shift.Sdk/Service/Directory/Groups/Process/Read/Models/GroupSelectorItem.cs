using System;

namespace InSite.Application.Contacts.Read
{
    public class GroupSelectorItem
    {
        public Guid GroupIdentifier { get; set; }
        public string GroupName { get; set; }
        public string GroupCode { get; set; }
        public QGroupAddress ShippingAddress { get; set; }
    }
}
