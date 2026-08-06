using System.ComponentModel.DataAnnotations;

namespace Shift.Contract
{
    public class ImportPerson
    {
        public enum EmployeeStatusEnum { Active, Deceased, LaidOff, Leave, Retired, Terminated }

        [Required]
        [MaxLength(20)]
        public string PersonCode { get; set; }

        [Required]
        [MaxLength(254)]
        public string UserEmail { get; set; }

        [Required]
        [MaxLength(40)]
        public string UserFirstName { get; set; }

        [Required]
        [MaxLength(40)]
        public string UserLastName { get; set; }

        [MaxLength(38)]
        public string UserMiddleName { get; set; }
        
        [MaxLength(100)]
        public string JobDivision { get; set; }

        [MaxLength(256)]
        public string JobTitle { get; set; }

        [MaxLength(200)]
        public string WorkAddressStreet1 { get; set; }

        [MaxLength(200)]
        public string WorkAddressStreet2 { get; set; }

        [MaxLength(128)]
        public string WorkAddressCity { get; set; }

        [MaxLength(64)]
        public string WorkAddressProvince { get; set; }

        [MaxLength(20)]
        public string WorkAddressPostalCode { get; set; }

        public string GroupCode { get; set; }

        [Required]
        public EmployeeStatusEnum EmployeeStatus { get; set; }
    }
}
