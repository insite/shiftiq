using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application
{
    public interface ICommander
    {
        Action<ICommand> Send { get; }
        Action<ICommand, DateTimeOffset> Schedule { get; }
        Action<ICommand, DateTimeOffset> Bookmark { get; }

        void Start(Guid timer);
    }
}