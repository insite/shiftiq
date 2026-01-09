using System;

namespace Shift.Common.Timeline.Commands
{
    /// <summary>
    /// Defines the methods needed from the command store.
    /// </summary>
    public interface ICommandStore
    {
        /// <summary>
        /// Returns true if a command exists.
        /// </summary>
        bool Exists(Guid command);

        /// <summary>
        /// Gets the serialized version of specific command.
        /// </summary>
        SerializedCommand Get(Guid command);

        /// <summary>
        /// Gets all unstarted commands that are scheduled to send now.
        /// </summary>
        SerializedCommand[] GetExpired(DateTimeOffset at);

        /// <summary>
        /// Inserts a serialized command.
        /// </summary>
        void Insert(SerializedCommand command);

        /// <summary>
        /// Updates a serialized command.
        /// </summary>
        void Update(SerializedCommand command);

        /// <summary>
        /// Returns the serialized version of a command.
        /// </summary>
        SerializedCommand Serialize(ICommand command);
        
        /// <summary>
        /// Deletes a command from the store.
        /// </summary>
        void Delete(Guid id);
    }
}
