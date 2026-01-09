using System;
using System.ComponentModel;

using InSite.Application.Records.Read;
using InSite.Common.Web.UI;

using Shift.Common.Linq;

namespace InSite.Admin.Records.Programs.Controls
{
    public partial class CredentialGrid : SearchResultsGridViewController<VCredentialFilter>
    {
        protected override bool IsFinder => false;

        private Guid ProgramIdentifier
        {
            get => (Guid)ViewState[nameof(ProgramIdentifier)];
            set => ViewState[nameof(ProgramIdentifier)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            AchievementType.AutoPostBack = true;
            AchievementType.ValueChanged += AchievementType_ValueChanged;

            FilterButton.Click += FilterButton_Click;
        }

        private void AchievementType_ValueChanged(object sender, EventArgs e)
        {
            LoadData(ProgramIdentifier, AchievementType.Value != "Program");
        }

        private void FilterButton_Click(object sender, EventArgs e)
        {
            LoadData(ProgramIdentifier, AchievementType.Value != "Program");
        }

        public void LoadData(Guid programIdentifier)
        {
            ProgramIdentifier = programIdentifier;

            LoadData(programIdentifier, AchievementType.Value != "Program");
        }

        private void LoadData(Guid programIdentifier, bool allTaskCredentials)
        {
            Guid[] learnersCredentials;

            if (allTaskCredentials)
                learnersCredentials = ServiceLocator.AchievementSearch.GetLearnerTaskAndProgramCredentials(programIdentifier);
            else
                learnersCredentials = ServiceLocator.AchievementSearch.GetLearnerProgramCredentials(programIdentifier);

            if (learnersCredentials == null || learnersCredentials.Length == 0)
                return;

            var filter = new VCredentialFilter
            {
                OrganizationIdentifier = Organization.Identifier,
                CredentialIdentifiers = learnersCredentials,
                UserFullName = FilterTextBox.Text,
            };

            Search(filter);
        }

        protected override int SelectCount(VCredentialFilter filter)
        {
            return ServiceLocator.AchievementSearch.CountCredentials(filter);
        }

        protected override IListSource SelectData(VCredentialFilter filter)
        {
            return ServiceLocator.AchievementSearch
                .GetCredentials(filter)
                .ToSearchResult();
        }
    }
}