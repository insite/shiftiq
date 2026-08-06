using System;
using System.ComponentModel.DataAnnotations;

namespace Shift.Contract
{
    public class CreatePendingPerson
    {
        [Required]
        public string PersonCode { get; set; }

        [Required]
        public string UserEmail { get; set; }

        [Required]
        public string UserFirstName { get; set; }

        [Required]
        public string UserLastName { get; set; }

        [Required]
        public string EmployeeStatus { get; set; }
    }
}
