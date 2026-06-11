using System;
using System.Collections.Generic;

using Shift.Common.Timeline.Commands;

using InSite.Application.Standards.Read;
using InSite.Application.Standards.Write;
using InSite.Application.StandardValidations.Write;

namespace InSite.Persistence
{
    public static class StandardValidationStore
    {
        private static Action<ICommand> _sendCommand;
        private static Action<IEnumerable<ICommand>> _sendCommands;

        public static void Initialize(Action<ICommand> sendCommand, Action<IEnumerable<ICommand>> sendCommands)
        {
            _sendCommand = sendCommand;
            _sendCommands = sendCommands;
        }

        public static void Insert(QStandardValidation entity)
        {
            var commands = StandardValidationCommandCreator.Insert(entity);
            _sendCommands(commands);
        }

        public static void Delete(Guid validationId)
        {
            _sendCommand(new RemoveStandardValidation(validationId));
        }

        public static void Update(QStandardValidation entity)
        {
            var commands = StandardValidationCommandCreator.Update(entity);
            _sendCommands(commands);
        }
    }
}
