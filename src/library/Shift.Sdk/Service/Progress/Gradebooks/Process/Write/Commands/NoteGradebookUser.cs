using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Gradebooks.Write
{
    public class NoteGradebookUser : Command
    {
        public NoteGradebookUser(Guid record, Guid user, string note, DateTimeOffset? added)
        {
            AggregateIdentifier = record;
            User = user;
            Note = note;
            Added = added;
        }

        public Guid User { get; set; }
        public string Note { get; set; }
        public DateTimeOffset? Added { get; set; }
    }
}