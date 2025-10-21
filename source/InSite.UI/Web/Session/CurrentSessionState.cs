using System;
using System.Collections.Generic;

using InSite.Common.Web;
using InSite.Domain.Foundations;
using InSite.UI.Admin.Messages.Messages.Utilities;

using Shift.Common;
using Shift.Sdk.UI;

namespace InSite
{
    public static class CurrentSessionState
    {
        private static readonly InSiteSessionState _state = new InSiteSessionState();

        public static string AchievementCertificateRequest
        {
            get => _state.GetValue() as string;
            set => _state.SetValue(value);
        }

        public static int ActiveCompetencyDisplayMethod
        {
            get => _state.GetValue() as int? ?? 0;
            set => _state.SetValue(value);
        }

        public static bool AllowDisplayWhatsNew => !DateWhatsNewLoaded.HasValue || DateSignedIn.HasValue && DateSignedIn.Value > DateWhatsNewLoaded.Value;

        public static DateTimeOffset? AssetsUpdatedOn
        {
            get => _state.GetValue() as DateTimeOffset?;
            set => _state.SetValue(value);
        }

        public static ICollection<int> AuthorizedMenuAssets
        {
            get => _state.GetValue() as ICollection<int>;
            set => _state.SetValue(value);
        }

        public static string CompetencyValidationSort
        {
            get => _state.GetValue() as string;
            set => _state.SetValue(value);
        }

        #region CompetencyWorkingSet

        public static Guid[] GetCompetencyWorkingSet(string action) =>
            _state.GetValue($"{nameof(GetCompetencyWorkingSet)}.{action}") as Guid[];

        public static void SetCompetencyWorkingSet(string action, object value) =>
            _state.SetValue(value, $"{nameof(GetCompetencyWorkingSet)}.{action}");

        #endregion

        public static string ContactGroupEnvelopes
        {
            get => _state.GetValue() as string;
            set => _state.SetValue(value);
        }

        public static DateTime? DateSignedIn
        {
            get => _state.GetValue() as DateTime?;
            set => _state.SetValue(value);
        }

        public static DateTime? DateWhatsNewLoaded
        {
            get => _state.GetValue() as DateTime?;
            set => _state.SetValue(value);
        }

        public static string EditorStatusValue
        {
            get => _state.GetValue() as string;
            set => _state.SetValue(value);
        }

        public static Guid? ElectionID
        {
            get => _state.GetValue() as Guid?;
            set => _state.SetValue(value);
        }

        public static string EmployeeProfileGridDisplayFormat
        {
            get => _state.GetValue() as string;
            set => _state.SetValue(value);
        }

        public static bool EnableCmdsNavigation
        {
            get => (bool?)_state.GetValue() ?? false;
            set => _state.SetValue(value);
        }

        public static bool EnableDebugMode
        {
            get => (bool?)_state.GetValue() ?? false;
            set => _state.SetValue(value);
        }

        public static bool EnableUserRegistration
        {
            get => (bool?)_state.GetValue() ?? false;
            set => _state.SetValue(value);
        }

        public static int? FailedAttemptsCount
        {
            get => _state.GetValue() as int?;
            set => _state.SetValue(value);
        }

        public static ISecurityFramework Identity
        {
            get => _state.GetValue() as ISecurityFramework;
            set => _state.SetValue(value);
        }

        public static string InitialClientFingerprint
        {
            get => _state.GetValue() as string;
            set => _state.SetValue(value);
        }

        public static KeywordDictionary KeywordDictionary
        {
            get => _state.GetValue() as KeywordDictionary;
            set => _state.SetValue(value);
        }

        public static string LastPortalPageUrl
        {
            get => _state.GetValue() as string;
            set => _state.SetValue(value);
        }

        public static UI.HomeCmds.CachedAchievements MainMenuCachedAchievements
        {
            get => _state.GetValue() as UI.HomeCmds.CachedAchievements;
            set => _state.SetValue(value);
        }

        public static UI.HomeCmds.CachedAchievements MainMenuCachedAchievements2
        {
            get => _state.GetValue() as UI.HomeCmds.CachedAchievements;
            set => _state.SetValue(value);
        }

        public static Dictionary<Guid, ContentSession> MarkdownSessionStateStorage
        {
            get => _state.GetValue() as Dictionary<Guid, ContentSession>;
            set => _state.SetValue(value);
        }

        public static Dictionary<MultiKey, object> PersonalizationCache
        {
            get => _state.GetValue() as Dictionary<MultiKey, object>;
            set => _state.SetValue(value);
        }

        #region QuestionVisibilities

        public static bool[] GetQuestionVisibilities(int evaluationID, int questionID) =>
            _state.GetValue($"SV{evaluationID}.S{questionID}.Visibilities") as bool[];

        public static void SetQuestionVisibilities(int evaluationID, int questionID, bool[] value) =>
            _state.SetValue(value, $"SV{evaluationID}.S{questionID}.Visibilities");

        #endregion

        public static int? ResetAttemptsCount
        {
            get => _state.GetValue() as int?;
            set => _state.SetValue(value);
        }

        public static bool ShowMetadata
        {
            get => (bool?)_state.GetValue() ?? false;
            set => _state.SetValue(value);
        }

        #region ShuffledIndexes

        public static int[] GetShuffledIndexes(int questionID) =>
            _state.GetValue($"Q{questionID}.ShuffledIndexes") as int[];

        public static void SetShuffledIndexes(int questionID, int[] value) =>
            _state.SetValue(value, $"Q{questionID}.ShuffledIndexes");

        #endregion

        public static int? SignInAttemptsCount
        {
            get => _state.GetValue() as int?;
            set => _state.SetValue(value);
        }

        public static int? SignInFailedAttemptsCount
        {
            get => _state.GetValue() as int?;
            set => _state.SetValue(value);
        }

        public static DateTime? SignInLockedOn
        {
            get => _state.GetValue() as DateTime?;
            set => _state.SetValue(value);
        }

        public static bool? Step9IsPositiveAnswer
        {
            get => (bool?)_state.GetValue();
            set => _state.SetValue(value);
        }

        public static object StepSelections
        {
            get => _state.GetValue();
            set => _state.SetValue(value);
        }

        public static Dictionary<Guid, string> OrganizationWallpapers
        {
            get => _state.GetValue() as Dictionary<Guid, string>;
            set => _state.SetValue(value);
        }

        public static Guid? MultiFactorAuthenticationUserIdentifier
        {
            get => _state.GetValue() as Guid?;
            set => _state.SetValue(value);
        }
    }
}
