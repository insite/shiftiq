using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Registrations.Write
{
    public class PublishGrade : Command
    {
        public string Reference { get; set; }
        public string Errors { get; set; }

        public PublishGrade(Guid aggregate, string reference, string errors)
        {
            AggregateIdentifier = aggregate;
            Reference = reference;
            Errors = errors;
        }
    }
}
