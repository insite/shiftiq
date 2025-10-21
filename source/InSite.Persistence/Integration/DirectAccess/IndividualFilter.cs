using System;

using Shift.Common;

namespace InSite.Persistence.Integration.DirectAccess
{
    [Serializable]
    public class IndividualFilter : Filter
    {
        public string City { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        
        public bool? IsActive { get; set; }

        public int[] IndividualKeys { get; set; }
    }
}
