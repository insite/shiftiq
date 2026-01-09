using System;
using System.Collections.Generic;
using System.Linq;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class Achievement : AggregateState
    {
        public Guid Tenant { get; set; }

        public string CertificateLayout { get; set; }
        public string Description { get; set; }
        public string Label { get; set; }
        public string Title { get; set; }
        public bool Enabled { get; set; }
        public bool ReportingDisabled { get; set; }
        public Expiration Expiration { get; set; }
        public string Type { get; set; }
        public bool? HasBadgeImage { get; set; }
        public string BadgeImageUrl { get; set; }

        /// <summary>
        /// This property represents a table of Achievement options. Prerequisites are understood to be satisfied only if ALL prerequisite conditions 
        /// are satisfied. 
        /// </summary>
        /// <example>
        /// Suppose we have an Achievement X with two prerequisites: 
        ///   Y = Mathematics 271 OR Mathematics 273
        ///   Z = Philosophy 279 OR Philosophy 377
        /// The prerequisites for X are satisfied only if the conditions for both Y AND Z are satisfied.
        /// </example>
        public List<AchievementPrerequisite> Prerequisites { get; set; }

        public Achievement()
        {
            Expiration = new Expiration();
            Prerequisites = new List<AchievementPrerequisite>();
        }

        public void When(AchievementDeleted _)
        {
            Enabled = false;
        }

        public void When(AchievementCreated c)
        {
            Tenant = c.OriginOrganization;

            Label = c.Label;
            Title = c.Title;
            Description = c.Description;
            Expiration = c.Expiration;

            Enabled = true;
        }

        public void When(AchievementDescribed c)
        {
            Label = c.Label;
            Title = c.Title;
            Description = c.Description;
        }

        public void When(AchievementLocked _)
        {
            Enabled = false;
        }

        public void When(AchievementNotChanged _)
        {

        }

        public void When(AchievementExpiryChanged c)
        {
            Expiration = c.Expiration;
        }

        public void When(AchievementTenantChanged c)
        {
            Tenant = c.Tenant;
        }

        public void When(AchievementTypeChanged c)
        {
            Type = c.Type;
        }

        public void When(AchievementBadgeImageChanged c)
        {
            BadgeImageUrl = c.BadgeImageUrl;
        }

        public void When(AchievementBadgeImageDisabled _)
        {
            ReportingDisabled = false;
        }

        public void When(AchievementBadgeImageEnabled _)
        {
            ReportingDisabled = true;
        }

        public void When(AchievementUnlocked _)
        {
            Enabled = true;
        }

        public void When(CertificateLayoutChanged c)
        {
            CertificateLayout = c.Code;
        }

        public void When(AchievementPrerequisiteAdded c)
        {
            var prerequisite = new AchievementPrerequisite
            {
                Identifier = c.Prerequisite,
                Conditions = c.Conditions.ToList()
            };
            Prerequisites.Add(prerequisite);
        }

        public void When(AchievementPrerequisiteDeleted c)
        {
            var prerequisite = Prerequisites.Single(x => x.Identifier == c.Prerequisite);
            if (prerequisite != null)
                Prerequisites.Remove(prerequisite);
        }

        public void When(AchievementReportingDisabled _)
        {
            ReportingDisabled = true;
        }

        public void When(AchievementReportingEnabled _)
        {
            ReportingDisabled = false;
        }

        public void When(SerializedChange _)
        {
            // Obsolete changes go here
        }
    }
}
