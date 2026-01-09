using System;
using System.ComponentModel.DataAnnotations;

using Shift.Common.Timeline.Changes;

namespace InSite.Persistence.Plugin.CMDS
{
    [Serializable]
    public class CmdsTrainingRegistrationSubmitted : Change
    {
        [Required]
        public Guid SessionIdentifier { get; set; }

        public string SessionTitle { get; set; }

        [Required]
        public string RegistrantName { get; set; }

        [Required(ErrorMessage = "Email is required."), EmailAddress(ErrorMessage = "Invalid email address.")]
        public string RegistrantEmail { get; set; }

        [Required]
        public string RegistrantCompany { get; set; }

        public string Comment { get; set; }
    }
}