using System;

namespace Shift.Sdk.UI
{
    [Serializable]
    public class PersonItem
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? Birthdate { get; set; }
        public string PersonCode { get; set; }
        public string Phone { get; set; }
    }
}