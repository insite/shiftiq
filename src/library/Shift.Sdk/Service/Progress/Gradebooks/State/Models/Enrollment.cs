using System;

namespace InSite.Domain.Records
{
    public class Enrollment
    {
        public Guid Id { get; set; }
        public Guid Learner { get; set; }
        public Guid? Period { get; set; }
        public string Comment { get; set; }
    }
}
