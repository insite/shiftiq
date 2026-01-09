using System;

namespace Shift.Sdk.UI
{
    public interface IActivityEdit
    {
        Guid ActivityIdentifier { get; set; }
        Guid CourseIdentifier { get; set; }
    }
}