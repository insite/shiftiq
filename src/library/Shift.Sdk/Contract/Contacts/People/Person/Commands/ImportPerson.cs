using System;

namespace Shift.Contract
{
    public class ImportPerson
    {
        public Guid Identifier { get; set; }

        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string BusinessTitle { get; set; }
        public string EmployeeType { get; set; }
        public string Location { get; set; }
        public string Manager { get; set; }
    }
}
