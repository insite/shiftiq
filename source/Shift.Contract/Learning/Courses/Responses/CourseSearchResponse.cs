using System;

namespace Shift.Contract.Responses
{
    public class CourseSearchResponse
    {
        public Guid Identifier { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public string Platform { get; set; }
    }

    public class CourseResponse
    {
        public Guid Identifier { get; set; }
    }
}