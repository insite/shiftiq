using System;
using System.ComponentModel;
using System.Linq;

using Humanizer;

using InSite.Application.Records.Read;
using InSite.Common.Web.UI;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Admin.Achievements.Achievements.Controls
{
    public partial class SearchResults : SearchResultsGridViewController<QAchievementFilter>
    {
        protected override int SelectCount(QAchievementFilter filter)
        {
            return ServiceLocator.AchievementSearch.CountAchievements(filter);
        }

        protected override IListSource SelectData(QAchievementFilter filter)
        {
            return ServiceLocator.AchievementSearch
                .GetAchievements(filter)
                .ToSearchResult();
        }

        protected string GetLocalTime(DateTimeOffset? item)
        {
            return item.FormatDateOnly(User.TimeZone, nullValue: string.Empty);
        }

        protected string GetExpirationHtml(string expirationType, DateTimeOffset? expirationFixedDate, int? expirationLifetimeQuantity, string expirationLifetimeUnit)
        {
            if (!expirationType.HasValue() || expirationType == "None")
                return "None";

            if (expirationType == "Fixed" && expirationFixedDate.HasValue)
                return "Fixed Date = " + GetLocalTime(expirationFixedDate);

            if (expirationType == "Relative" && expirationLifetimeQuantity.HasValue && expirationLifetimeUnit.HasValue())
            {
                return $"{expirationLifetimeUnit.ToQuantity(expirationLifetimeQuantity.Value)}";
            }

            return null;
        }

        protected string GetExpirationLifetime(string expirationType, int? expirationLifetimeQuantity, string expirationLifetimeUnit)
        {
            if (expirationType.HasValue() && expirationType != "None" && expirationType != "Fixed"
                && expirationLifetimeQuantity.HasValue && expirationLifetimeUnit.HasValue())
            {
                return $"{expirationLifetimeUnit.ToQuantity(expirationLifetimeQuantity.Value)}";
            }

            return null;
        }

        public class ExportDataItem
        {
            public Guid AchievementIdentifier { get; set; }
            public Guid OrganizationIdentifier { get; set; }

            public string AchievementDescription { get; set; }
            public string AchievementTag { get; set; }
            public string AchievementTitle { get; set; }
            public string CertificateLayoutCode { get; set; }
            public string ExpirationLifetimeUnit { get; set; }
            public string ExpirationType { get; set; }

            public bool AchievementIsEnabled { get; set; }

            public int? ExpirationLifetimeQuantity { get; set; }
            public int CredentialCount { get; set; }

            public DateTimeOffset? ExpirationFixedDate { get; set; }
        }

        public override IListSource GetExportData(QAchievementFilter filter, bool empty)
        {
            return ServiceLocator.AchievementSearch.GetAchievements(filter).Cast<VAchievement>().Select(x => new ExportDataItem
            {
                AchievementIdentifier = x.AchievementIdentifier,
                OrganizationIdentifier = x.OrganizationIdentifier,
                AchievementDescription = x.AchievementDescription,
                AchievementTag = x.AchievementLabel,
                AchievementTitle = x.AchievementTitle,
                CertificateLayoutCode = x.CertificateLayoutCode,
                ExpirationLifetimeUnit = x.ExpirationLifetimeUnit,
                ExpirationType = x.ExpirationType,
                AchievementIsEnabled = x.AchievementIsEnabled,
                ExpirationLifetimeQuantity = x.ExpirationLifetimeQuantity,
                CredentialCount = x.CredentialCount,
                ExpirationFixedDate = x.ExpirationFixedDate,
            }).ToList().ToSearchResult();
        }
    }
}