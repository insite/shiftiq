using System;

using InSite.Admin.Assessments.Attempts.Forms;
using InSite.Application.Banks.Read;
using InSite.Application.Contacts.Read;
using InSite.Application.Registrations.Read;

using Shift.Common;

namespace InSite.Admin.Assessments.Attempts.Models
{
    [Serializable]
    public class AdHocAttemptDataItem
    {
        public Guid SubmissionIdentifier { get; }
        public string ProfileTitle { get; }
        public string FrameworkTitle { get; }
        public string FormName { get; }
        public int? FormAsset { get; }
        public int? FormAssetVersion { get; }
        public DateTimeOffset? FormFirstPublished { get; }
        public string CandidateName { get; }
        public string CandidateCode { get; }
        public string EventFormat { get; }
        public string AttemptTag { get; }
        public string AttemptStartedText => AttemptStartedValue.Format(CurrentSessionState.Identity.User.TimeZone, nullValue: string.Empty);
        public DateTimeOffset? AttemptStartedValue { get; set; }
        public string AttemptCompletedText => AttemptCompletedValue.Format(CurrentSessionState.Identity.User.TimeZone, nullValue: string.Empty);
        public DateTimeOffset? AttemptCompletedValue { get; set; }
        public string AttemptDuration { get; set; }
        public decimal? AttemptScore { get; }
        public string AttemptGrade { get; }

        public AdHocAttemptDataItem(AdHoc.AttemptData entity, QBankForm form, VPerson candidate, QRegistration registration)
        {
            SubmissionIdentifier = entity.AttemptIdentifier;
            ProfileTitle = form?.VBank?.OccupationTitle;
            FrameworkTitle = form?.VBank?.FrameworkTitle;
            FormName = form?.FormName;
            FormAsset = form?.FormAsset;
            FormAssetVersion = form?.FormAssetVersion;
            FormFirstPublished = form?.FormFirstPublished;
            CandidateName = candidate?.UserFullName;
            CandidateCode = candidate?.PersonCode;
            EventFormat = registration?.Event?.EventFormat;
            AttemptTag = entity.AttemptTag;
            AttemptStartedValue = entity.AttemptStarted;
            AttemptCompletedValue = entity.AttemptGraded;
            AttemptDuration = entity.AttemptDuration.HasValue ? $"{entity.AttemptDuration} minutes" : string.Empty;
            AttemptScore = entity.AttemptScore;
            AttemptGrade = entity.AttemptGrade;
        }
    }
}
