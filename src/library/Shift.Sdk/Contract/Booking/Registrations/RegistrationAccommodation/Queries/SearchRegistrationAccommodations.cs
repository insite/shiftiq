using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class SearchRegistrationAccommodations : Query<IEnumerable<RegistrationAccommodationMatch>>, IRegistrationAccommodationCriteria
    {
        public Guid? OrganizationIdentifier { get; set; }
        public Guid? RegistrationIdentifier { get; set; }

        public string AccommodationName { get; set; }
        public string AccommodationType { get; set; }

        public int? TimeExtension { get; set; }
    }
}