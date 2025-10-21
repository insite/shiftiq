﻿using System;
using System.Linq;

using InSite.Common;
using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Records.Reports.LearnerActivity.Controls
{
    public partial class SearchCriteria : SearchCriteriaController<VLearnerActivityFilter>
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            LearnerGenders.LoadItems(VLearnerActivitySearch.GetComboBoxItems(Organization.Identifier, "LearnerGender"));

            LearnerCitizenships.LoadItems(VLearnerActivitySearch.GetComboBoxItems(Organization.Identifier, "LearnerCitizenship"));

            ImmigrationStatuses.LoadItems(VLearnerActivitySearch.GetComboBoxItems(Organization.Identifier, "ImmigrationStatus"));

            ReferrerNames.LoadItems(VLearnerActivitySearch.GetComboBoxItems(Organization.Identifier, "ReferrerName"));

            ProgramNames.LoadItems(VLearnerActivitySearch.GetComboBoxItems(Organization.Identifier, "ProgramName"));

            GradebookNames.LoadItems(VLearnerActivitySearch.GetComboBoxItems(Organization.Identifier, "GradebookName"));
        }

        public override VLearnerActivityFilter Filter
        {
            get
            {
                var filter = new VLearnerActivityFilter
                {
                    OrganizationIdentifier = Organization.OrganizationIdentifier,

                    CertificateStatus = AchievementStatus.Value,
                    AchievementGrantedSince = AchievementGrantedSince.Value,
                    AchievementGrantedBefore = AchievementGrantedBefore.Value,

                    EnrollmentStatus = EnrollmentStatus.Value,
                    EnrollmentStartedSince = EnrollmentStartedSince.Value,
                    EnrollmentStartedBefore = EnrollmentStartedBefore.Value,
                    LearnerEmail = LearnerEmail.Text,
                    LearnerNameFirst = LearnerNameFirst.Text,
                    LearnerNameLast = LearnerNameLast.Text,
                    LearnerRole = LearnerRole.Value,
                    PersonCode = PersonCode.Text,
                    CountStrategy = CountStrategy.SelectedValue,

                    LearnerGenders = LearnerGenders.Values != null ? LearnerGenders.Values.ToArray() : null,
                    LearnerCitizenships = LearnerCitizenships.Values != null ? LearnerCitizenships.Values.ToArray() : null,
                    ImmigrationStatuses = ImmigrationStatuses.Values != null ? ImmigrationStatuses.Values.ToArray() : null,
                    ReferrerNames = ReferrerNames.Values != null ? ReferrerNames.Values.ToArray() : null,
                    ProgramNames = ProgramNames.Values != null ? ProgramNames.Values.ToArray() : null,
                    GradebookNames = GradebookNames.Values != null ? GradebookNames.Values.ToArray() : null,

                    EngagementStatus = EngagementStatus.Value.ToEnum(EngagementStatusType.None),
                };

                GetCheckedShowColumns(filter);

                return filter;
            }
            set
            {
                AchievementStatus.Value = value.CertificateStatus;
                AchievementGrantedSince.Value = value.AchievementGrantedSince;
                AchievementGrantedBefore.Value = value.AchievementGrantedBefore;

                EnrollmentStatus.Value = value.EnrollmentStatus;
                EnrollmentStartedSince.Value = value.EnrollmentStartedSince;
                EnrollmentStartedBefore.Value = value.EnrollmentStartedBefore;
                LearnerEmail.Text = value.LearnerEmail;
                LearnerNameFirst.Text = value.LearnerNameFirst;
                LearnerNameLast.Text = value.LearnerNameLast;
                LearnerRole.Value = value.LearnerRole;
                PersonCode.Text = value.PersonCode;
                CountStrategy.SelectedValue = value.CountStrategy;

                LearnerGenders.Values = value.LearnerGenders;
                LearnerCitizenships.Values = value.LearnerCitizenships;
                ImmigrationStatuses.Values = value.ImmigrationStatuses;
                ReferrerNames.Values = value.ReferrerNames;
                ProgramNames.Values = value.ProgramNames;
                GradebookNames.Values = value.GradebookNames;

                EngagementStatus.Value = value.EngagementStatus.GetName(EngagementStatusType.None);
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PersonCode.EmptyMessage = GetEmptyMessage("Person Code");
        }

        public override void Clear()
        {
            AchievementStatus.Value = null;
            AchievementGrantedSince.Value = null;
            AchievementGrantedBefore.Value = null;

            EnrollmentStartedSince.Value = null;
            EnrollmentStartedBefore.Value = null;
            EnrollmentStatus.Value = null;
            LearnerEmail.Text = null;
            LearnerNameFirst.Text = null;
            LearnerNameLast.Text = null;
            LearnerRole.Value = null;
            PersonCode.Text = null;

            LearnerGenders.Values = null;
            LearnerCitizenships.Values = null;
            ImmigrationStatuses.Values = null;
            ReferrerNames.Values = null;
            ProgramNames.Values = null;
            GradebookNames.Values = null;

            CountStrategy.SelectedValue = "Summary";

            EngagementStatus.Value = null;
        }

        protected static string GetEmptyMessage(string text) => LabelHelper.GetLabelContentText(text);
    }
}