using System;

namespace Shift.Common
{
    public interface IHasVersionControl<T> where T: IHasVersionControl<T>
    {
        DateTimeOffset? FirstPublished { get; }
        T PreviousVersion { get; set; }
        T NextVersion { get; set; }
    }
}
