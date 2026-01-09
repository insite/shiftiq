using System;

namespace Shift.Common.Kernel
{
    public class CommandBuilder
    {
        private readonly CommandTypeCollection _commandTypes;
        private readonly IJsonSerializerBase _serializer;

        public CommandBuilder(CommandTypeCollection commandTypes, IJsonSerializerBase serializer)
        {
            _commandTypes = commandTypes;
            _serializer = serializer;
        }

        public Type GetCommandType(string commandName)
        {
            var commandType = _commandTypes.GetCommandType(commandName);

            if (commandType == null)
                throw new BadQueryException($"{commandName} is not a registered command type.");

            return commandType;
        }

        public object BuildCommand(Type commandType, string requestBody)
        {
            var commandObject = CreateCommand(commandType, requestBody);

            if (commandObject == null)
                throw new BadQueryException($"{commandType.Name} query object creation failed unexpectedly.");

            return commandObject;
        }

        private object CreateCommand(Type commandType, string requestBody)
        {
            var deserializer = new CommandSerializer(_serializer);

            var commandObject = deserializer.Deserialize(commandType, requestBody);

            return commandObject;
        }
    }
}
