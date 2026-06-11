using System;

using Shift.Common;

namespace Shift.Common.Timeline.Commands
{
    /// <summary>
    /// Provides functions to convert between instances of ICommand and SerializedCommand.
    /// </summary>
    public static class CommandExtensions
    {
        /// <summary>
        /// Returns a deserialized command.
        /// </summary>
        public static ICommand Deserialize(this SerializedCommand x, bool ignoreAttributes)
        {
            try
            {
                var serializer = Services.ServiceLocator.Instance.GetService<IJsonSerializer>();
                var data = serializer.Deserialize<ICommand>(x.CommandData, Type.GetType(x.CommandClass), ignoreAttributes);

                data.AggregateIdentifier = x.AggregateIdentifier;
                data.ExpectedVersion = x.ExpectedVersion;

                data.OriginOrganization = x.OriginOrganization;
                data.OriginUser = x.OriginUser;

                data.CommandIdentifier = x.CommandIdentifier;

                return data;
            }
            catch (InvalidCastException ex)
            {
                throw new InvalidCastException($"{ex.Message} Command: Type = {x.CommandType}, Identifier = {x.CommandIdentifier}, Data = [{x.CommandData}]", ex);
            }
        }

        /// <summary>
        /// Returns a serialized command.
        /// </summary>
        public static SerializedCommand Serialize(this ICommand command, bool ignoreAttributes)
        {
            var serializer = Services.ServiceLocator.Instance.GetService<IJsonSerializer>();
            var data = ignoreAttributes
                ? serializer.Serialize(command, new[]
                {
                    nameof(ICommand.AggregateIdentifier),
                    nameof(ICommand.ExpectedVersion),
                    nameof(ICommand.OriginOrganization),
                    nameof(ICommand.OriginUser),
                    nameof(ICommand.CommandIdentifier)
                }, ignoreAttributes)
                : serializer.SerializeCommand(command);

            var serialized = new SerializedCommand
            {
                AggregateIdentifier = command.AggregateIdentifier,
                ExpectedVersion = command.ExpectedVersion,

                CommandClass = serializer.GetClassName(command.GetType()),
                CommandType = command.GetType().Name,
                CommandData = data,

                CommandIdentifier = command.CommandIdentifier,

                OriginOrganization = command.OriginOrganization,
                OriginUser = command.OriginUser
            };

            if (serialized.CommandClass.Length > 200)
                throw new OverflowException($"The assembly-qualified name for this command ({serialized.CommandClass}) exceeds the maximum character limit (200).");

            if (serialized.CommandType.Length > 100)
                throw new OverflowException($"The type name for this command ({serialized.CommandType}) exceeds the maximum character limit (100).");

            if ((serialized.SendStatus?.Length ?? 0) > 20)
                throw new OverflowException($"The send status ({serialized.SendStatus}) exceeds the maximum character limit (20).");

            return serialized;
        }
    }
}