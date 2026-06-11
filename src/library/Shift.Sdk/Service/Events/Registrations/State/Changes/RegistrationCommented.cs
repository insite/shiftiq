
using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Registrations
{
    public class RegistrationCommented : Change
    {
        public string Comment { get; set; }

        public RegistrationCommented(string comment)
        {
            Comment = comment;
        }
    }
}
