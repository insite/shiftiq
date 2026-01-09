using System;

namespace Shift.Sdk.UI
{
    public class LoginModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public Guid? OrganizationIdentifier { get; set; }
    }
}