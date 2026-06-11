using System;

using Shift.Common.Timeline.Changes;

using InSite.Application.Contacts.Read;
using InSite.Application.Messages.Read;

using Shift.Common;

namespace InSite.Persistence.Plugin.CMDS
{
    public partial class CmdsProcessor
    {
        private readonly IChangeQueue _publisher;
        private readonly IEmailOutbox _postOffice;
        private readonly IContactSearch _contacts;

        public CmdsProcessor(IChangeQueue publisher, IEmailOutbox postOffice, IContactSearch contacts)
        {
            _publisher = publisher;
            _postOffice = postOffice;
            _contacts = contacts;

            _publisher.Subscribe<CmdsCollegeCertificationRequested>(Handle);
            _publisher.Subscribe<CmdsCompetenciesExpired>(Handle);
            _publisher.Subscribe<CmdsCompetencyChanged>(Handle);
            _publisher.Subscribe<CmdsAchievementChanged>(Handle);
            _publisher.Subscribe<CmdsAchievementExpirationDelivered>(Handle);
            _publisher.Subscribe<CmdsTrainingRegistrationSubmitted>(Handle);
        }

        public void Handle(CmdsCollegeCertificationRequested e) => _postOffice.Send(BuildEmail(e));
        public void Handle(CmdsCompetenciesExpired e) => _postOffice.Send(BuildEmail(e));
        public void Handle(CmdsCompetencyChanged e) => _postOffice.Send(BuildEmail(e));
        public void Handle(CmdsAchievementChanged e) => _postOffice.Send(BuildEmail(e));
        public void Handle(CmdsAchievementExpirationDelivered e) => _postOffice.Send(BuildEmail(e));
        public void Handle(CmdsTrainingRegistrationSubmitted e) => _postOffice.Send(BuildEmail(e));

        public EmailDraft BuildEmail(IChange e)
        {
            if (e is CmdsCollegeCertificationRequested) return BuildEmail((CmdsCollegeCertificationRequested)e);
            if (e is CmdsCompetenciesExpired) return BuildEmail((CmdsCompetenciesExpired)e);
            if (e is CmdsCompetencyChanged) return BuildEmail((CmdsCompetencyChanged)e);
            if (e is CmdsAchievementChanged) return BuildEmail((CmdsAchievementChanged)e);
            if (e is CmdsAchievementExpirationDelivered) return BuildEmail((CmdsAchievementExpirationDelivered)e);
            if (e is CmdsTrainingRegistrationSubmitted) return BuildEmail((CmdsTrainingRegistrationSubmitted)e);
            throw new ArgumentException($"Unexpected Change Type: {e.GetType().FullName}");
        }
    }
}