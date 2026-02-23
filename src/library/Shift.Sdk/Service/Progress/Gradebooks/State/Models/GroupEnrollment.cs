using System;

namespace InSite.Domain.Records
{
    public class GroupEnrollment
    {
        public Guid Enrollment { get; set; }
        public Guid Group { get; set; }
    }
}
