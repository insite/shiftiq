using System;
using System.Collections.Generic;

using InSite.Application.Events.Read;

using Shift.Common;

using ExpirationTypeEnum = Shift.Constant.ExpirationType;
using ShiftHumanizer = Shift.Common.Humanizer;

namespace InSite.Application.Records.Read
{
    public class QAchievement
    {
        public Guid AchievementIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }

        public string AchievementDescription { get; set; }
        public string AchievementLabel { get; set; }
        public string AchievementTitle { get; set; }
        public string CertificateLayoutCode { get; set; }
        public string ExpirationLifetimeUnit { get; set; }
        public string ExpirationType { get; set; }
        public bool AchievementIsEnabled { get; set; }
        public bool AchievementAllowSelfDeclared { get; set; }
        public bool AchievementReportingDisabled { get; set; }
        public int? ExpirationLifetimeQuantity { get; set; }
        public DateTimeOffset? ExpirationFixedDate { get; set; }
        public string AchievementType { get; set; }
        public bool? HasBadgeImage { get; set; }
        public string BadgeImageUrl { get; set; }

        public virtual ICollection<QEvent> Events { get; set; } = new HashSet<QEvent>();
        public virtual ICollection<QCredential> Credentials { get; set; } = new HashSet<QCredential>();
        public virtual ICollection<QGradebook> Gradebooks { get; set; } = new HashSet<QGradebook>();
        public virtual ICollection<QGradeItem> GradeItems { get; set; } = new HashSet<QGradeItem>();
        public virtual ICollection<QJournalSetup> JournalSetups { get; set; } = new HashSet<QJournalSetup>();
        public virtual ICollection<QAchievementPrerequisite> Prerequisites { get; set; } = new HashSet<QAchievementPrerequisite>();
        public virtual ICollection<TProgram> Programs { get; set; } = new HashSet<TProgram>();

        public string ExpirationToString()
            => ExpirationToString(TimeZones.Pacific);

        public string ExpirationToString(TimeZoneInfo tz)
        {
            var s = ExpirationType;

            if (string.Equals(ExpirationType, ExpirationTypeEnum.Fixed.ToString(), StringComparison.OrdinalIgnoreCase)
                && ExpirationFixedDate.HasValue
                )
            {
                s += " " + TimeZones.FormatDateOnly(ExpirationFixedDate.Value, tz);
            }
            else if (string.Equals(ExpirationType, ExpirationTypeEnum.Relative.ToString(), StringComparison.OrdinalIgnoreCase)
                && ExpirationLifetimeQuantity.HasValue
                && ExpirationLifetimeQuantity > 0
                && !string.IsNullOrEmpty(ExpirationLifetimeUnit)
                )
            {
                s += " " + ShiftHumanizer.ToQuantity(ExpirationLifetimeQuantity.Value, ExpirationLifetimeUnit);
            }

            return s;
        }
    }

    public class VAchievement
    {
        public Guid AchievementIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }

        public string AchievementDescription { get; set; }
        public string AchievementLabel { get; set; }
        public string AchievementTitle { get; set; }
        public string CertificateLayoutCode { get; set; }
        public string ExpirationLifetimeUnit { get; set; }
        public string ExpirationType { get; set; }

        public bool AchievementIsEnabled { get; set; }

        public int? ExpirationLifetimeQuantity { get; set; }
        public int CredentialCount { get; set; }

        public DateTimeOffset? ExpirationFixedDate { get; set; }
    }

    public class QAchievementPrerequisite
    {
        public Guid AchievementIdentifier { get; set; }
        public Guid PrerequisiteAchievementIdentifier { get; set; }
        public Guid PrerequisiteIdentifier { get; set; }

        public virtual QAchievement Achievement { get; set; }
    }
}
