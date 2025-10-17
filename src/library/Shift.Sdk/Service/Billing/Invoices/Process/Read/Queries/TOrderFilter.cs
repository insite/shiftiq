using System;

using Shift.Common;

namespace InSite.Application.Invoices.Read
{
    [Serializable]
    public class TOrderFilter : Filter
    {
        public Guid? OrganizationIdentifier { get; set; }
        public Guid? CustomerIdentifier { get; set; }
        public string FullName { get; set; }
    }
}
