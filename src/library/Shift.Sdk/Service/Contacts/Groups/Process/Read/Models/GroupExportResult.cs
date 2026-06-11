using System.Collections.Generic;

namespace InSite.Application.Contacts.Read
{
    public class GroupExportResult
    {
        public QGroup Group { get; set; }
        public int GroupSize { get; set; }
        public int Subgroups { get; set; }
        public QGroupAddress ShippingAddress { get; set; }
        public QGroupAddress BillingAddress { get; set; }
        public QGroupAddress PhysicalAddress { get; set; }
        public QGroup Parent { get; set; }
        public ICollection<QGroupConnection> Ancestors { get; set; }
        public string SurveyFormName { get; set; }
        public string MembershipProductName { get; set; }
        public int MembershipStatusSize { get; set; }
    }
}
