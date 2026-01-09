using System;

namespace InSite.Application.Contacts.Read
{
    public class QGroupConnection
    {
        public Guid ChildGroupIdentifier { get; set; }
        public Guid ParentGroupIdentifier { get; set; }

        public virtual QGroup ChildGroup { get; set; }
        public virtual QGroup ParentGroup { get; set; }
    }
}
