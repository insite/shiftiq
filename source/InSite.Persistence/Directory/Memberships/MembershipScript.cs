using System.Collections.Generic;

using Shift.Common.Timeline.Commands;

using InSite.Application;

namespace InSite.Persistence
{
    internal class MembershipScript
    {
        List<ICommand> commands = new List<ICommand>();

        public void Add(ICommand command)
        {
            commands.Add(command);
        }

        public void Execute(ICommander _commander)
        {
            foreach (var command in commands)
                _commander.Send(command);
        }
    }
}
