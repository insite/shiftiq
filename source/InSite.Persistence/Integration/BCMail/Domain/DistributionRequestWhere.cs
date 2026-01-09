namespace InSite.Persistence.Integration.BCMail
{
    public class DistributionRequestWhere
    {
        public string VenueLocationName { get; set; }
        public string VenueLocationRoom { get; set; }
        public string VenueContactName { get; set; }
        public string VenueContactPhone { get; set; }
        public string InvigilatingOffice { get; set; }
        public DistributionRequestLocation VenueLocationShipping { get; set; }
        public DistributionRequestLocation VenueLocationPhysical { get; set; }

        public DistributionRequestWhere()
        {
            VenueLocationPhysical = new DistributionRequestLocation();
            VenueLocationShipping = new DistributionRequestLocation();
        }
    }
}