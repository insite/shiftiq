using System;

using InSite.Application.Records.Read;
using InSite.Common.Web.UI;

namespace InSite.UI.Portal.Records.Credentials.Instructors.Controls
{
    public partial class SearchCriteria : SearchCriteriaController<VCredentialFilter>
    {
        public override VCredentialFilter Filter
        {
            get
            {
                var query = new VCredentialFilter
                {
                    OrganizationIdentifier = Organization.OrganizationIdentifier,
                    UserEmail = UserEmail.Text,
                    UserFullName = UserName.Text,
                    EmployerGroupName = EmployerGroupName.Text,
                    AchievementIdentifiers = AchievementIdentifier.Values,
                    AchievementLabel = AchievementLabel.Text,
                    CredentialStatus = CredentialStatus.Value,
                    CredentialGrantedSince = GrantedSince.Value,
                    CredentialGrantedBefore = GrantedBefore.Value,
                    CredentialExpirationExpectedSince = ExpirySince.Value,
                    CredentialExpirationExpectedBefore = ExpiryBefore.Value
                };

                if (ServiceLocator.Partition.IsE03())
                    query.AchievementLabels = new[] { "Orientation", "Time-Sensitive Safety Certificate" };

                GetCheckedShowColumns(query);

                return query;
            }
            set
            {
                UserEmail.Text = value.UserEmail;
                UserName.Text = value.UserFullName;
                EmployerGroupName.Text = value.EmployerGroupName;
                AchievementLabel.Text = value.AchievementLabel;
                AchievementIdentifier.Values = value.AchievementIdentifiers;
                CredentialStatus.Value = value.CredentialStatus;
                GrantedSince.Value = value.CredentialGrantedSince?.DateTime;
                GrantedBefore.Value = value.CredentialGrantedBefore?.DateTime;
                ExpirySince.Value = value.CredentialExpirationExpectedSince?.DateTime;
                ExpiryBefore.Value = value.CredentialExpirationExpectedBefore?.DateTime;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            var query = AchievementIdentifier.Filter;

            query.OrganizationIdentifiers.Add(Organization.Identifier);

            if (ServiceLocator.Partition.IsE03())
            {
                query.AchievementLabels.Add("Orientation");
                query.AchievementLabels.Add("Time-Sensitive Safety Certificate");
                AchievementIdentifier.Filter.AchievementLabels = query.AchievementLabels;
            }

            RenderSearchCriteria();
        }

        public override void Clear()
        {
            UserEmail.Text = null;
            UserName.Text = null;
            EmployerGroupName.Text = null;

            AchievementLabel.Text = null;
            AchievementIdentifier.Values = null;
            CredentialStatus.ClearSelection();

            GrantedSince.Value = null;
            GrantedBefore.Value = null;

            ExpirySince.Value = null;
            ExpiryBefore.Value = null;
        }

        private void RenderSearchCriteria()
        {
            AchievementLabel.Text = Request.QueryString["achievement-label"];
        }
    }
}