using System;

namespace InSite.Application.Contacts.Read
{
    public class SupergroupDataItem
    {
        public Guid GroupIdentifier { get; set; }
        public string GroupType { get; set; }
        public string Name { get; set; }
        public bool Selected { get; set; }
        public bool Enabled { get; set; }
    }

    public class SubgroupDataItem
    {
        public Guid Identifier { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
    }
}
