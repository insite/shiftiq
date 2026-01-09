using System;

using InSite.Application.Records.Read;
using InSite.Common;
using InSite.Common.Web.UI;

using Shift.Constant;

namespace InSite.Admin.Achievements.Credentials.Controls
{
    public partial class SearchCriteria : SearchCriteriaController<VCredentialFilter>
    {
        public override VCredentialFilter Filter
        {
            get
            {
                var filter = new VCredentialFilter
                {
                    OrganizationIdentifier = Organization.OrganizationIdentifier,

                    AchievementTitle = AchievementTitle.Text,
                    UserFullName = LearnerName.Text,
                    UserEmail = LearnerEmail.Text,
                    PersonCode = PersonCode.Text,
                    EmployerGroupIdentifier = EmployerGroupIdentifier.Value,
                    UserRegion = EmployerRegion.Value,
                    CredentialStatus = CredentialStatus.Value,
                    AchievementLabel = AchievementLabel.Value,
                    EmployerGroupStatus = EmployerGroupStatus.Value,

                    CredentialGrantedSince = CredentialGrantedSince.Value,
                    CredentialGrantedBefore = CredentialGrantedBefore.Value,
                    CredentialRevokedSince = CredentialRevokedSince.Value,
                    CredentialRevokedBefore = CredentialRevokedBefore.Value,
                    CredentialExpiredSince = CredentialExpiredSince.Value,
                    CredentialExpiredBefore = CredentialExpiredBefore.Value
                };

                GetCheckedShowColumns(filter);

                filter.OrderBy = SortColumns.Value;

                return filter;
            }
            set
            {
                InitAchievementLabel();

                AchievementTitle.Text = value.AchievementTitle;
                LearnerName.Text = value.UserFullName;
                LearnerEmail.Text = value.UserEmail;
                PersonCode.Text = value.PersonCode;
                EmployerGroupIdentifier.Value = value.EmployerGroupIdentifier;
                EmployerRegion.Value = value.UserRegion;
                CredentialStatus.Value = value.CredentialStatus;
                AchievementLabel.Value = value.AchievementLabel;
                EmployerGroupStatus.Value = value.EmployerGroupStatus;

                CredentialGrantedSince.Value = value.CredentialGrantedSince;
                CredentialGrantedBefore.Value = value.CredentialGrantedBefore;
                CredentialRevokedSince.Value = value.CredentialRevokedSince;
                CredentialRevokedBefore.Value = value.CredentialRevokedBefore;
                CredentialExpiredSince.Value = value.CredentialExpiredSince;
                CredentialExpiredBefore.Value = value.CredentialExpiredBefore;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            EmployerGroupIdentifier.Filter.OrganizationIdentifier = Organization.Key;

            EmployerRegion.Settings.CollectionName = CollectionName.Contacts_People_Location_Region;
            EmployerRegion.Settings.OrganizationIdentifier = Organization.Key;

            EmployerGroupStatus.Settings.CollectionName = CollectionName.Contacts_Groups_Status_Name;
            EmployerGroupStatus.Settings.OrganizationIdentifier = Organization.Key;

            PersonCode.EmptyMessage = GetEmptyMessage("Person Code");
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            InitAchievementLabel();
        }

        public override void Clear()
        {
            InitAchievementLabel();

            AchievementTitle.Text = null;
            LearnerName.Text = null;
            LearnerEmail.Text = null;
            PersonCode.Text = null;
            EmployerGroupIdentifier.Value = null;
            EmployerRegion.Value = null;
            CredentialStatus.ClearSelection();
            AchievementLabel.ClearSelection();
            EmployerGroupStatus.ClearSelection();

            CredentialGrantedSince.Value = null;
            CredentialGrantedBefore.Value = null;
            CredentialRevokedSince.Value = null;
            CredentialRevokedBefore.Value = null;
            CredentialExpiredSince.Value = null;
            CredentialExpiredBefore.Value = null;
        }

        private void InitAchievementLabel()
        {
            if (AchievementLabel.Items.Count > 0)
                return;

            AchievementLabel.Items.Add(new ComboBoxOption());

            var labels = ServiceLocator.AchievementSearch.GetAchievementLabels(Organization.Identifier);
            foreach (var label in labels)
                AchievementLabel.Items.Add(new ComboBoxOption(label, label));
        }

        protected static string GetEmptyMessage(string text) => LabelHelper.GetLabelContentText(text);
    }
}