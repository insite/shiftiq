using System;

using Shift.Common;

namespace Shift.Contract
{
    public class RetrieveCourse : Query<CourseModel>
    {
        public Guid CourseIdentifier { get; set; }
    }
}