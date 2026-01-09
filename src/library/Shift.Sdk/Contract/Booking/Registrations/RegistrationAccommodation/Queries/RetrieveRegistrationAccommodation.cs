using System;

using Shift.Common;

namespace Shift.Contract
{
    public class RetrieveRegistrationAccommodation : Query<RegistrationAccommodationModel>
    {
        public Guid AccommodationIdentifier { get; set; }
    }
}