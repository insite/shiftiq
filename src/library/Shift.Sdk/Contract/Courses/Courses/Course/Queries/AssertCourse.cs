using System;

using Shift.Common;

namespace Shift.Contract
{
    public class AssertCourse : Query<bool>
    {
        public Guid CourseIdentifier { get; set; }
    }
}