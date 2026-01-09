using System;

using Shift.Common;

namespace Shift.Contract
{
    public class AssertRegistrationAccommodation : Query<bool>
    {
        public Guid AccommodationIdentifier { get; set; }
    }
}