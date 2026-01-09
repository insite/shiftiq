using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Surveys.Sessions
{
    public class ResponseSessionCreated : Change
    {
        public ResponseSessionCreated(string source, Guid tenant, Guid form, Guid user)
        {
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
