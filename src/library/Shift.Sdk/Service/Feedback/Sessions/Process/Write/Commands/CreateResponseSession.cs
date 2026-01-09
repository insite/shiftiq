using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Responses.Write
{
    public class CreateResponseSession : Command
    {
        public CreateResponseSession(Guid session, string source, Guid tenant, Guid form, Guid user)
        {
            AggregateIdentifier = session;

            Source = source;
            Tenant = tenant;

            Form = form;
            User = user;
        }

        public string Source { get; set; }
        public Guid Tenant { get; set; }

        public Guid Form { get; set; }
        public Guid User { get; set; }
    }
}