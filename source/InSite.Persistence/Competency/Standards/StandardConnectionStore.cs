using System;
using System.Collections.Generic;
using System.Linq;

using Shift.Common.Timeline.Commands;

using InSite.Application.Standards.Write;

using Shift.Common;

namespace InSite.Persistence
{
    public static class StandardConnectionStore
    {
        #region Initialization

        private static Action<ICommand> _sendCommand;
        private static Action<IEnumerable<ICommand>> _sendCommands;

        public static void Initialize(Action<ICommand> sendCommand, Action<IEnumerable<ICommand>> sendCommands)
        {
            _sendCommand = sendCommand;
            _sendCommands = sendCommands;
        }

        #endregion

        public static void Insert(IEnumerable<StandardConnection> list)
        {
            var commands = new List<ICommand>();
            foreach (var group in list.GroupBy(x => x.FromStandardIdentifier))
                commands.Add(new AddStandardConnection(
                    group.Key,
                    group.Select(x => new InSite.Domain.Standards.StandardConnection
                    {
                        ToStandardId = x.ToStandardIdentifier,
                        ConnectionType = x.ConnectionType
                    }).ToArray()
                ));

            if (commands.IsNotEmpty())
                _sendCommands(commands);
        }

        public static void Delete(Guid fromStandardIdentifier, Guid toStandardIdentifier)
        {
            _sendCommand(new RemoveStandardConnection(fromStandardIdentifier, toStandardIdentifier));
        }
    }
}
