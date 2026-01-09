using Shift.Common.Timeline.Changes;

using InSite.Persistence.Plugin.CMDS;

using Newtonsoft.Json;

using Shift.Common;

namespace InSite.Cmds.Admin.Achievements.Models
{
    public class AchievementObserver
    {
        public AchievementObserver(IChangeQueue publisher, EmailAddress author, CmdsAchievementChanged before)
        {
            _author = author;
            _before = before;
            _publisher = publisher;
        }

        public void Created(CmdsAchievementChanged after)
        {
            if (!IsSafetyCertificate(after.Type))
                return;

            SendEmailNotification("Created", after);
        }

        public bool Equals(CmdsAchievementChanged after)
        {
            if (_before == null && after == null)
                return true;

            if (_before == null && after != null)
                return false;

            var a = JsonConvert.SerializeObject(_before);
            var b = JsonConvert.SerializeObject(after);
            return StringHelper.Equals(a, b);
        }

        public bool IsSafetyCertificate(string achievementType)
        {
            return StringHelper.Equals(achievementType, AchievementTypes.TimeSensitiveSafetyCertificate);
        }

        public void Modified(CmdsAchievementChanged after)
        {
            if (!IsSafetyCertificate(after.Type))
                return;

            if (Equals(after))
                return;

            SendEmailNotification("Modified", after);
        }

        private EmailAddress _author;
        private CmdsAchievementChanged _before;
        private IChangeQueue _publisher;

        private void SendEmailNotification(string change, CmdsAchievementChanged achievement)
        {
            var organizationId = ServiceLocator.IdentityService.GetCurrentOrganization();

            var organization = ServiceLocator.OrganizationSearch.GetModel(organizationId);

            var enabled = organization.Toolkits?.Achievements?.IsChangeNotificationEnabled ?? false;

            if (!enabled)
                return;

            achievement.Change = change;

            achievement.AuthorEmail = _author.Address;

            achievement.AuthorName = _author.DisplayName;

            _publisher.Publish(achievement);
        }
    }
}