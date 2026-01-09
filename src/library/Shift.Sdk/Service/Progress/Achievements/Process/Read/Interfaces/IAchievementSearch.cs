using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using InSite.Domain.Records;

using Shift.Constant;

namespace InSite.Application.Records.Read
{
    public interface IAchievementSearch
    {
        List<ContactSummaryWithExpiryReportItem> GetContactSummaryWithExpiryReport(Guid userIdentifier, Guid organizationIdentifier);
        List<ContactSummaryNoExpiryReportItem> GetContactSummaryNoExpiryReport(Guid userIdentifier, Guid organizationIdentifier);

        List<Tuple<QAchievement, int>> GetStudentAchievements(Guid gradebook, Guid organization);
        List<Tuple<QAchievement, int>> GetItemAndStudentAchievements(Guid gradebook, Guid organization);

        List<Tuple<QAchievement, int>> GetProgramStudentAchievements(Guid gradebook, Guid organization);
        List<Tuple<QAchievement, int>> GetProgramAndStudentAchievements(Guid gradebook, Guid organization);

        QAchievement GetAchievement(Guid organization, string title);
        QAchievement GetAchievement(Guid achievement, params Expression<Func<QAchievement, object>>[] includes);
        QAchievement[] GetAchievements(IEnumerable<Guid> achievementIdentifiers);
        string[] GetAchievementLabels(Guid organization);
        int CountAchievements(QAchievementFilter filter);
        List<VAchievement> GetAchievements(QAchievementFilter filter);

        int CountCredentials(VCredentialFilter filter);
        List<VCredential> GetCredentials(VCredentialFilter filter);
        List<VCredential> GetRecentCredentials(VCredentialFilter filter, int count);
        Guid[] GetLearnerProgramCredentials(Guid program);
        Guid[] GetLearnerTaskAndProgramCredentials(Guid program);
        VCredential GetCredential(Guid achievement, Guid user);
        List<VCredential> GetCredentials(IEnumerable<Guid> achievement, Guid user);
        VCredential GetCredential(Guid credential);

        CertificateWithMissingExpiry[] GetCertificatesWithMissingExpiry(Guid achievement);
        
        Guid GetCredentialIdentifier(Guid? credential, Guid achievement, Guid user);
        Guid GetCredentialIdentifier(Guid? credential, QCredentialHistory tombstone);
        QCredentialHistory[] GetCredentialHistory(IEnumerable<Guid> users, Func<Guid, IEnumerable<Guid>> getAchievements);
        QCredential[] GetCredentialsWithUnexpectedExpirationDelivery(ReminderType reminderType);
        
        ExpiredCredentialsSearchResult[] GetExpiredCredentials(ExpiredCredentialsSearchCriteria parameters, Guid identifier);
        
        int CountGradebookEnrollmentsForCredentials(Guid? learner, Guid achievement, bool pending, bool valid, bool expired);
        GradebookEnrollmentsForCredentialItem[] SelectGradebookEnrollmentsForCredentials(Guid? learner, Guid achievement, bool pending, bool valid, bool expired);
    }

    [Serializable]
    public class ExpiredCredentialsSearchCriteria
    {
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string AssetType { get; set; }
        public string AssetSubtype { get; set; }
        public string AssetTitle { get; set; }
        public DateTimeOffset? ExpiredSince { get; set; }
        public DateTimeOffset? ExpiredBefore { get; set; }
    }

    [Serializable]
    public class ExpiredCredentialsSearchResult
    {
        public string ContactFirstName { get; set; }
        public string ContactLastName { get; set; }
        public string ContactName => $"{ContactFirstName} {ContactLastName}";
        public string ContactEmail { get; set; }
        public string AssetType { get; set; }
        public string AssetSubtype { get; set; }
        public string AssetCode { get; set; }
        public string AssetTitle { get; set; }
        public DateTimeOffset Expired { get; set; }
        public int DaysSinceExpiration { get; set; }
    }

    public class GradebookEnrollmentsForCredentialItem
    {
        public Guid CourseIdentifier { get; set; }
        public Guid LearnerIdentifier { get; set; }
    }
}
