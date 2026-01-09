using System;

using Shift.Common.Timeline.Commands;

using InSite.Application;

namespace InSite
{
    public class Commander : ICommander
    {
        public Action<ICommand> Send => ServiceLocator.SendCommand;

        public Action<ICommand, DateTimeOffset> Schedule => ServiceLocator.ScheduleCommand;

        public Action<ICommand, DateTimeOffset> Bookmark => ServiceLocator.BookmarkCommand;

        public void Start(Guid timer) => ServiceLocator.CommandQueue.Start(timer);
    }
}