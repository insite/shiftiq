using System;

using InSite.Application.Contacts.Read;
using InSite.Application.Organizations.Read;

namespace InSite.Domain.Integration
{
    public class ApiRequest
    {
        public virtual QUser User { get; set; }
        public virtual QOrganization Organization { get; set; }

        public Guid RequestIdentifier { get; set; }
        public Guid? OrganizationIdentifier { get; set; }
        public Guid? UserIdentifier { get; set; }
        public Guid? ResponseLogIdentifier { get; set; }
        public String DeveloperEmail { get; set; }
        public String DeveloperHostAddress { get; set; }
        public String DeveloperName { get; set; }
        public String ExecutionErrors { get; set; }
        public String RequestContentData { get; set; }
        public String RequestContentType { get; set; }
        public String RequestHeaders { get; set; }
        public String RequestMethod { get; set; }
        public String RequestPath { get; set; }
        public String RequestStatus { get; set; }
        public String RequestUri { get; set; }
        public String RequestDirection { get; set; }
        public String ResponseContentData { get; set; }
        public String ResponseContentType { get; set; }
        public String ResponseStatusName { get; set; }
        public String ValidationErrors { get; set; }
        public String ValidationStatus { get; set; }
        public Int32? ResponseStatusNumber { get; set; }
        public Int32? ResponseTime { get; set; }
        public DateTimeOffset RequestStarted { get; set; }
        public DateTimeOffset? ResponseCompleted { get; set; }
    }
}