using System;

namespace Shift.Common.Timeline.Commands
{
    /// <summary>
    /// Provides a serialization wrapper for commands so that common properties are not embedded inside the command data.
    /// </summary>
    public class SerializedCommand : ICommand
    {
        public Guid AggregateIdentifier { get; set; }
        public int? ExpectedVersion { get; set; }

        public Guid OriginOrganization { get; set; }
        public Guid OriginUser { get; set; }

        public string CommandClass { get; set; }
        public string CommandType { get; set; }
        public string CommandData { get; set; }
        public string CommandDescription { get; set; }

        public Guid CommandIdentifier { get; set; }

        public DateTimeOffset? SendScheduled { get; set; }
        public DateTimeOffset? SendStarted { get; set; }
        public DateTimeOffset? SendCompleted { get; set; }
        public DateTimeOffset? SendCancelled { get; set; }

        public DateTimeOffset? BookmarkAdded { get; set; }
        public DateTimeOffset? BookmarkExpired { get; set; }

        public string SendStatus { get; set; }
        public string SendError { get; set; }

        public int? RecurrenceInterval { get; set; }
        public string RecurrenceUnit { get; set; }
        public string RecurrenceWeekdays { get; set; }
    }
}