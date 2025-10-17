
using System;

namespace Shift.Sdk.UI
{
    public class CustomerPermission
    {
        public Guid GroupIdentifier { get; set; }
        public Guid UserIdentifier { get; set; }
        public Guid InvoiceIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
    }
}