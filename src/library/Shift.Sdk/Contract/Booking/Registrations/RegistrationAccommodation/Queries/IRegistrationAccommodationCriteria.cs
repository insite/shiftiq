using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface IRegistrationAccommodationCriteria
    {
        QueryFilter Filter { get; set; }
        
        Guid? OrganizationIdentifier { get; set; }
        Guid? RegistrationIdentifier { get; set; }

        string AccommodationName { get; set; }
        string AccommodationType { get; set; }

        int? TimeExtension { get; set; }
    }
}