using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Surveys.Sessions
{
    public class ResponseSessionFormConsent : Change
    {
        public ResponseSessionFormConsent(Guid question, Guid tenant, Guid session, Guid user)
        {
            QuestionIdentifier = question;
            TenantIdentifier = tenant;
            ResponseSessionIdentifier = session;
            UserIdentifier = user;
        }

        public Guid QuestionIdentifier { get; set; }
        public Guid TenantIdentifier { get; set; }
        public Guid ResponseSessionIdentifier { get; set; }
        public Guid UserIdentifier { get; set; }
    }
}
