using System;
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

            LearnerGenders.LoadItems(VLearnerActivitySearch.GetUserGenders(Organization.Identifier));
            LearnerCitizenships.LoadItems(VLearnerActivitySearch.GetUserCitizenships(Organization.Identifier));
            GradebookIdentifiers.LoadItems(VLearnerActivitySearch.GetGradebooks(Organization.Identifier));
            ProgramIdentifiers.LoadItems(VLearnerActivitySearch.GetPrograms(Organization.Identifier));

            MembershipStatusItemIdentifiers.Settings.CollectionName = CollectionName.Contacts_People_Membership_Status;
            MembershipStatusItemIdentifiers.Settings.OrganizationIdentifier = Organization.Key;
            MembershipStatusItemIdentifiers.RefreshData();

            PersonCode.EmptyMessage = GetEmptyMessage("Person Code");
        }

        public override VLearnerActivityFilter Filter
        {
            get
            {
                var filter = new VLearnerActivityFilter
                {
                    OrganizationIdentifier = Organization.OrganizationIdentifier,

                    CredentialGrantedSince = AchievementGrantedSince.Value,
                    CredentialGrantedBefore = AchievementGrantedBefore.Value,

                    EnrollmentCreatedSince = EnrollmentStartedSince.Value,
                    EnrollmentCreatedBefore = EnrollmentStartedBefore.Value,
                    UserEmail = LearnerEmail.Text,
                    UserFirstName = LearnerNameFirst.Text,
                    UserLastName = LearnerNameLast.Text,
                    IsAdministrator = LearnerRole.Value == "Administrator" ? true : (bool?)null,
                    IsLearner = LearnerRole.Value == "Learner" ? true : (bool?)null,
                    PersonCode = PersonCode.Text,

                    UserGenders = LearnerGenders.Values.NullIfEmpty()?.ToArray(),
                    UserCitizenships = LearnerCitizenships.Values.NullIfEmpty()?.ToArray(),
                    GradebookIdentifiers = GradebookIdentifiers.ValuesAsGuidArray,

                    MembershipStatusItemIdentifiers = MembershipStatusItemIdentifiers.ValuesAsGuidArray,
                    EmployerGroupIdentifiers = EmployerGroupIdentifiers.Values,
                    ProgramIdentifiers = ProgramIdentifiers.ValuesAsGuidArray,
                };

                GetCheckedShowColumns(filter);

                return filter;
            }
            set
            {
                AchievementGrantedSince.Value = value.CredentialGrantedSince;
                AchievementGrantedBefore.Value = value.CredentialGrantedBefore;

                EnrollmentStartedSince.Value = value.EnrollmentCreatedSince;
                EnrollmentStartedBefore.Value = value.EnrollmentCreatedBefore;
                LearnerEmail.Text = value.UserEmail;
                LearnerNameFirst.Text = value.UserFirstName;
                LearnerNameLast.Text = value.UserLastName;
                LearnerRole.Value = value.IsAdministrator == true ? "Administrator" : value.IsLearner == true ? "Learner" : null;
                PersonCode.Text = value.PersonCode;

                LearnerGenders.Values = value.UserGenders;
                LearnerCitizenships.Values = value.UserCitizenships;
                MembershipStatusItemIdentifiers.ValuesAsGuid = value.MembershipStatusItemIdentifiers;
                EmployerGroupIdentifiers.Values = value.EmployerGroupIdentifiers;
                ProgramIdentifiers.ValuesAsGuid = value.ProgramIdentifiers;
                GradebookIdentifiers.ValuesAsGuid = value.GradebookIdentifiers;
            }
        }

        public override void Clear()
        {
            AchievementGrantedSince.Value = null;
            AchievementGrantedBefore.Value = null;

            EnrollmentStartedSince.Value = null;
            EnrollmentStartedBefore.Value = null;
            LearnerEmail.Text = null;
            LearnerNameFirst.Text = null;
            LearnerNameLast.Text = null;
            LearnerRole.Value = null;
            PersonCode.Text = null;

            LearnerGenders.ClearSelection();
            LearnerCitizenships.ClearSelection();
            MembershipStatusItemIdentifiers.ClearSelection();
            MembershipStatusItemIdentifiers.ClearSelection();
            EmployerGroupIdentifiers.Values = null;
            ProgramIdentifiers.Values = null;
            GradebookIdentifiers.Values = null;
        }

        protected static string GetEmptyMessage(string text) => LabelHelper.GetLabelContentText(text);
    }
}