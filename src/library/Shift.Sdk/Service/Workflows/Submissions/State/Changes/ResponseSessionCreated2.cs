using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Surveys.Sessions
{
    public class ResponseSessionCreated2 : Change
    {
        public ResponseSessionCreated2(string source, Guid tenant, Guid form, Guid assessor, Guid respondent)
        {
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

        #region Obsolete

        private class ResponseSessionCreated1 : Change
        {
            public ResponseSessionCreated1(string source, Guid tenant, Guid form, Guid user)
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

        public static ResponseSessionCreated2 Upgrade(SerializedChange serializedChange)
        {
            var v1 = serializedChange.Deserialize<ResponseSessionCreated1>();

            var v2 = new ResponseSessionCreated2(v1.Source, v1.Tenant, v1.Form, v1.User, v1.User)
            {
                AggregateState = serializedChange.AggregateState,
                AggregateIdentifier = v1.AggregateIdentifier,
                AggregateVersion = v1.AggregateVersion,
                OriginOrganization = v1.OriginOrganization,
                OriginUser = v1.OriginUser,
                ChangeTime = v1.ChangeTime
            };

            return v2;
        }

        #endregion
    }
}
