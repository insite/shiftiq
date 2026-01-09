using System;

using Shift.Common.Timeline.Commands;

namespace Shift.Common.Kernel
{
    public class CommandSerializer
    {
        private readonly IJsonSerializerBase _serializer;

        public CommandSerializer(IJsonSerializerBase serializer)
        {
            _serializer = serializer;
        }

        public Command Deserialize(Type commandType, string commandArguments)
        {
            try
            {
                var commandObject = _serializer.Deserialize<Command>(commandType, commandArguments, JsonPurpose.Storage);

                return commandObject;
            }
            catch (InvalidCastException ex)
            {
                throw new InvalidCastException($"{ex.Message} Command: Type = {commandType.Name}, Criteria = [{commandArguments}]", ex);
            }
        }
    }
}