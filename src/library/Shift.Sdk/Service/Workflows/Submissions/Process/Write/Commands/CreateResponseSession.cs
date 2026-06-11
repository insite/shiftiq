using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Responses.Write
{
    public class CreateResponseSession : Command
    {
        public CreateResponseSession(Guid session, string source, Guid tenant, Guid form, Guid assessor, Guid respondent)
        {
            AggregateIdentifier = session;

            Source = source;
            Tenant = tenant;

            Form = form;
            Assessor = assessor;
            Respondent = respondent;
        }

        public string Source { get; set; }
        public Guid Tenant { get; set; }

        public Guid Form { get; set; }
        public Guid Assessor { get; set; }
        public Guid Respondent { get; set; }
    }
}