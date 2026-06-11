using System;

using Shift.Common.Timeline.Commands;

using Shift.Constant;

namespace InSite.Application.Groups.Write.Old
{
    public class Address
    {
        public Guid Identifier { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Description { get; set; }
        public string PostalCode { get; set; }
        public string Province { get; set; }
        public string Street1 { get; set; }
        public string Street2 { get; set; }
    }

    public class ChangeGroupAddress : Command
    {
        public AddressType Type { get; set; }
        public Address Address { get; set; }

        public ChangeGroupAddress(Guid group, AddressType type, Address address)
        {
            AggregateIdentifier = group;
            Type = type;
            Address = address;
        }
    }

    public class ChangeGroupAuthorization : Command
    {
        public bool AccessGrantedToAdmin { get; set; }
        public bool AccessGrantedToCmds { get; set; }
        public bool AccessGrantedToDesign { get; set; }
        public bool AccessGrantedToPortal { get; set; }
        public bool AddNewUsersAutomatically { get; set; }

        public ChangeGroupAuthorization(Guid group, 
            bool accessGrantedToAdmin, bool accessGrantedToCmds, bool accessGrantedToDesign, bool accessGrantedToPortal, 
            bool addNewUsersAutomatically)
        {
            AggregateIdentifier = group;
            AccessGrantedToAdmin = accessGrantedToAdmin;
            AccessGrantedToCmds = accessGrantedToCmds;
            AccessGrantedToDesign = accessGrantedToDesign;
            AccessGrantedToPortal = accessGrantedToPortal;
            AddNewUsersAutomatically = addNewUsersAutomatically;
        }
    }

    public class ChangeGroupLocation : Command
    {
        public string Office { get; set; }
        public string Region { get; set; }
        public string ShippingPreference { get; set; }
        public string WebSiteUrl { get; set; }

        public ChangeGroupLocation(Guid group, string office, string region, string shippingPreference, string webSiteUrl)
        {
            AggregateIdentifier = group;
            Office = office;
            Region = region;
            ShippingPreference = shippingPreference;
            WebSiteUrl = webSiteUrl;
        }
    }

    public class ChangeGroupPhone : Command
    {
        public string Phone { get; set; }
        public string Fax { get; set; }

        public ChangeGroupPhone(Guid group, string phone, string fax)
        {
            AggregateIdentifier = group;
            Phone = phone;
            Fax = fax;
        }
    }

    public class ChangeGroupCapacity : Command
    {
        public int? Capacity { get; set; }

        public ChangeGroupCapacity(Guid group, int? capacity)
        {
            AggregateIdentifier = group;
            Capacity = capacity;
        }
    }

    public class ChangeGroupStatus : Command
    {
        public string Status { get; set; }

        public ChangeGroupStatus(Guid group, string status)
        {
            AggregateIdentifier = group;
            Status = status;
        }
    }

    public class AddGroupToContainer : Command
    {
        public Guid Container { get; set; }

        public AddGroupToContainer(Guid group, Guid container)
        {
            AggregateIdentifier = group;
            Container = container;
        }
    }

    public class RemoveGroupFromContainer : Command
    {
        public Guid Container { get; set; }

        public RemoveGroupFromContainer(Guid group, Guid container)
        {
            AggregateIdentifier = group;
            Container = container;
        }
    }

    public class CreateGroup : Command
    {
        public string Type { get; set; }
        public string Name { get; set; }

        public CreateGroup(Guid group, string type, string name)
        {
            AggregateIdentifier = group;
            Type = type;
            Name = name;
        }
    }

    public class DestroyGroup : Command
    {
        public string Reason { get; set; }

        public DestroyGroup(Guid group, string reason)
        {
            AggregateIdentifier = group;
            Reason = reason;
        }
    }

    public class RenameGroup : Command
    {
        public string Type { get; set; }
        public string Name { get; set; }

        public RenameGroup(Guid group, string type, string name)
        {
            AggregateIdentifier = group;
            Type = type;
            Name = name;
        }
    }

    public class DescribeGroup : Command
    {
        public string Category { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string Label { get; set; }

        public DescribeGroup(Guid group, string category, string code, string description, string label)
        {
            AggregateIdentifier = group;
            Category = category;
            Code = code;
            Description = description;
            Label = label;
        }
    }
}
