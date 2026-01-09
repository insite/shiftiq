using System;
using System.ComponentModel;

using InSite.Application.Records.Read;
using InSite.Common.Web.UI;

using Shift.Common.Linq;

namespace InSite.Admin.Records.Logbooks.Controls
{
    public partial class AchievementGrid : SearchResultsGridViewController<VCredentialFilter>
    {
        [Serializable]
        private class AchievementItem
        {
            public Guid AchievementIdentifier { get; set; }
            public string AchievementTitle { get; set; }
            public int Count { get; set; }
        }

        protected override bool IsFinder => false;

        protected Guid? AchievementIdentifier => Filter?.AchievementIdentifier == null && Filter?.ExcludeAchievements != null ? Guid.Empty : Filter?.AchievementIdentifier;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            FilterButton.Click += FilterButton_Click;
        }

        private void FilterButton_Click(object sender, EventArgs e)
        {
            LoadData(Filter.JournalSetupIdentifier.Value);
        }

        public int LoadData(Guid journalSetupIdentifier)
        {
            var filter = new VCredentialFilter
            {
                OrganizationIdentifier = Organization.Identifier,
                JournalSetupIdentifier = journalSetupIdentifier,
                UserFullName = FilterTextBox.Text
            };

            Search(filter);

            return RowCount;
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