using System;

using Shift.Common.Timeline.Commands;

using InSite.Application.Credentials.Write;

namespace InSite.Persistence.Plugin.CMDS
{
    public class VCmdsCredentialStore
    {
        private readonly Action<ICommand> _send;

        public VCmdsCredentialStore(Action<ICommand> send)
        {
            _send = send;
        }

        public int ExpireForDepartment(Guid department, Guid achievement, bool pending, bool valid, bool expired)
        {
            var learners = MembershipSearch.Select(x => x.GroupIdentifier == department && x.MembershipType == "Department");

            foreach (var learner in learners)
                ExpireForLearner(learner.UserIdentifier, achievement, pending, valid, expired);

            return learners.Length;
        }

        public int ExpireForLearner(Guid user, Guid achievement, bool pending, bool valid, bool expired)
        {
            var credentials = VCmdsCredentialSearch.Select(x => x.UserIdentifier == user && x.AchievementIdentifier == achievement);

            foreach (var credential in credentials)
                _send(new ExpireCredential(credential.CredentialIdentifier, DateTimeOffset.Now));

            return credentials.Count;
        }

        public int ExpireForOrganization(Guid organization, Guid achievement, bool pending, bool valid, bool expired)
        {
            var credentials = VCmdsCredentialSearch.Select(x => x.OrganizationIdentifier == organization && x.AchievementIdentifier == achievement);

            foreach (var credential in credentials)
                _send(new ExpireCredential(credential.CredentialIdentifier, DateTimeOffset.Now));

            return credentials.Count;
        }
    }
}