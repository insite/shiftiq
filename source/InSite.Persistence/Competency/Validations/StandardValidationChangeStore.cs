using System;
using System.Collections.Generic;

using Shift.Common.Timeline.Commands;

using InSite.Application.Standards.Read;
using InSite.Application.StandardValidations.Write;

using Shift.Constant;

namespace InSite.Persistence
{
    public static class StandardValidationChangeStore
    {
        private static Action<ICommand> _sendCommand;
        private static Action<IEnumerable<ICommand>> _sendCommands;

        public static void Initialize(Action<ICommand> sendCommand, Action<IEnumerable<ICommand>> sendCommands)
        {
            _sendCommand = sendCommand;
            _sendCommands = sendCommands;
        }

        public static void Delete(Guid standardValidationId, Guid logId)
        {
            _sendCommand(new RemoveStandardValidationLog(standardValidationId, logId));
        }

        public static void Insert(QStandardValidationLog entity)
        {
            _sendCommand(new AddStandardValidationLog(
                entity.StandardValidationIdentifier,
                entity.LogIdentifier,
                entity.AuthorUserIdentifier ?? UserIdentifiers.Someone,
                entity.LogPosted,
                entity.LogStatus,
                entity.LogComment));
        }

        public static void Update(QStandardValidationLog entity)
        {
            _sendCommand(new ModifyStandardValidationLog(
                entity.StandardValidationIdentifier,
                entity.LogIdentifier,
                entity.AuthorUserIdentifier ?? UserIdentifiers.Someone,
                entity.LogPosted,
                entity.LogStatus,
                entity.LogComment));
        }
    }
}
