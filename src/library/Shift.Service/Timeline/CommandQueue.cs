using Shift.Common.Timeline.Commands;

using Shift.Common;

namespace Shift.Service.Timeline
{
    public class CommandQueue : ICommandQueue
    {
        private readonly IShiftIdentityService _identityService;

        public CommandQueue(IShiftIdentityService identityService, Toolbox.TimelineClient timelineClient)
        {
            _identityService = identityService;
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
            var principal = _identityService.GetPrincipal();

            command.OriginOrganization = principal.Organization.Identifier;

            command.OriginUser = principal.User.Identifier;

            var commandName = command.GetType().Name;

            var endpoint = $"react/commands?c={commandName}";

            // var result = TaskRunner.RunSync(_timelineClient.HttpPost, endpoint, command);
            // if (result.Status != System.Net.HttpStatusCode.OK)
            //    throw new Exception("An unexpected error occurred.");

            throw new NotImplementedException();
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
