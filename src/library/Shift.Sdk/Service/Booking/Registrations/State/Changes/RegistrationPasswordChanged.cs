using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Registrations
{
    public class RegistrationPasswordChanged : Change
    {
        public string Password { get; set; }

        public RegistrationPasswordChanged(string password)
        {
            Password = password;
        }
    }
}
