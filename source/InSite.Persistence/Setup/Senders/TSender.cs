using System;
using System.Collections.Generic;

using InSite.Application.Messages.Read;

namespace InSite.Persistence
{
    public class TSender
    {
        public Guid SenderIdentifier { get; set; }
        public string SenderType { get; set; }
        public string SenderNickname { get; set; }
        public string SenderName { get; set; }
        public string SenderEmail { get; set; }
        public bool SenderEnabled { get; set; }
        
        public string SystemMailbox { get; set; }
        
        public string CompanyAddress { get; set; }
        public string CompanyCity { get; set; }
        public string CompanyPostalCode { get; set; }
        public string CompanyCountry { get; set; }

        public virtual ICollection<TSenderOrganization> Organizations { get; set; } = new HashSet<TSenderOrganization>();
        public virtual ICollection<QMessage> Messages { get; set; }
    }
}
