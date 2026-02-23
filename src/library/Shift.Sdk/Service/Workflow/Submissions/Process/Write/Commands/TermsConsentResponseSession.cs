using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Responses.Write
{
    public class TermsConsentResponseSession : Command
    {
        public TermsConsentResponseSession(Guid session, Guid question, Guid organization, Guid user)
        {
            AggregateIdentifier = session;

            QuestionIdentifier = question;
            OrganizationIdentifier = organization;
            ResponseSessionIdentifier = session;
            UserIdentifier = user;

        }

        public Guid QuestionIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid ResponseSessionIdentifier { get; set; }
        public Guid UserIdentifier { get; set; }
    }
}
