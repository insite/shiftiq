using System;

using Shift.Common;

namespace InSite.Application.Contacts.Read
{
    [Serializable]
    public class QUserConnectionFilter : Filter
    {
        public Guid? FromUserOrganizationId { get; set; }
        public Guid? FromUserId { get; set; }
        public Guid? ToUserOrganizationId { get; set; }
        public Guid? ToUserId { get; set; }

        public string FromUserName { get; set; }
        public string ToUserName { get; set; }

        public bool IsManager { get; set; }
        public bool IsSupervisor { get; set; }
        public bool IsValidator { get; set; }
    }
}
