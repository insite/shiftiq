using System;
using System.Collections.Generic;

using Shift.Common.Timeline.Commands;

using InSite.Application.Credentials.Write;
using InSite.Domain.Records;

using Shift.Common;

namespace InSite.Application.Records.Write
{
    public static class AchievementCommandGenerator
    {
        public static List<ICommand> GetCommands(CredentialState credential, Guid achievement, Guid organization)
        {
            var id = UuidFactory.Create();
            var script = new List<ICommand>
            {
                new CreateCredential(id, organization, achievement, credential.User, credential.Assigned),
                new TagCredential(id, credential.Necessity, credential.Priority)
            };

            if (credential.Granted.HasValue)
            {
                script.Add(new GrantCredential(
                    id,
                    credential.Granted.Value,
                    credential.GrantedReason,
                    credential.Score,
                    credential.EmployerGroup,
                    credential.EmployerGroupStatus
                    ));
            }

            if (credential.Revoked.HasValue)
                script.Add(new RevokeCredential(id, credential.Revoked.Value, credential.RevokedReason, credential.Score));

            if (credential.Expired.HasValue)
                script.Add(new ExpireCredential(id, credential.Expired.Value));

            return script;
        }
    }
}
