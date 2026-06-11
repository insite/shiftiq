using Shift.Common.Timeline.Commands;

namespace Shift.Service.Timeline
{
    public class CommandQueue : ICommandQueue
    {
        private readonly Toolbox.TimelineClient _timelineClient;

        public CommandQueue(Toolbox.TimelineClient timelineClient)
        {
            _timelineClient = timelineClient;
        }

        public void Bookmark(ICommand command, DateTimeOffset expired)
        {
            throw new NotImplementedException();
        }

        public void Cancel(Guid command)
        {
            throw new NotImplementedException();
        }

        public void Complete(Guid command)
        {
            throw new NotImplementedException();
        }

        public void Override<T>(Action<T> action, Guid organization) where T : ICommand
        {
            throw new NotImplementedException();
        }

        public void Ping(Action<int> scheduledCommandRead)
        {
            throw new NotImplementedException();
        }

        public void Schedule(ICommand command, DateTimeOffset? at = null)
        {
            throw new NotImplementedException();
        }

        public void Send(ICommand command)
        {
            var result = _timelineClient.QueueCommand(command);

            if (result.Status != System.Net.HttpStatusCode.OK)
                throw new HttpRequestException("An unexpected error occurred.");
        }

        public bool Start(Guid command)
        {
            throw new NotImplementedException();
        }

        public void Subscribe<T>(Action<T> action) where T : ICommand
        {
            throw new NotImplementedException();
        }
    }
}
