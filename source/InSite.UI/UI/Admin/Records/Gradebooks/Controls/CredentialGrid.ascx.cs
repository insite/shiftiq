using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Application.Records.Read;
using InSite.Common.Web.UI;

using Shift.Common.Linq;

namespace InSite.Admin.Records.Gradebooks.Controls
{
    public partial class CredentialGrid : SearchResultsGridViewController<VCredentialFilter>
    {
        [Serializable]
        private class AchievementItem
        {
            public Guid AchievementIdentifier { get; set; }
            public string AchievementTitle { get; set; }
            public int Count { get; set; }
        }

        public class Counts
        {
            public int CountInItems { get; set; }
            public int CountInUsers { get; set; }
        }

        protected override bool IsFinder => false;

        protected Guid? AchievementIdentifier => Filter?.AchievementIdentifier == null && Filter?.ExcludeAchievements != null ? Guid.Empty : Filter?.AchievementIdentifier;

        private List<AchievementItem> TopAchievements
        {
            get => (List<AchievementItem>)ViewState[nameof(TopAchievements)];
            set => ViewState[nameof(TopAchievements)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            AchievementType.AutoPostBack = true;
            AchievementType.ValueChanged += AchievementType_ValueChanged;

            FilterButton.Click += FilterButton_Click;
            AchievementRepeater.ItemCommand += AchievementRepeater_ItemCommand;
        }

        private void AchievementType_ValueChanged(object sender, EventArgs e)
        {
            LoadAchievements(Filter.UserGradebookIdentifier.Value);
            LoadData(Filter.UserGradebookIdentifier.Value, null);
        }

        private void FilterButton_Click(object sender, EventArgs e)
        {
            LoadData(Filter.UserGradebookIdentifier.Value, AchievementIdentifier);
        }

        private void AchievementRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Filter")
            {
                var achievementIdentifier = Guid.Parse((string)e.CommandArgument);

                if (achievementIdentifier == AchievementIdentifier)
                    LoadData(Filter.UserGradebookIdentifier.Value, null);
                else
                    LoadData(Filter.UserGradebookIdentifier.Value, achievementIdentifier);
            }
        }

        public Counts LoadData(Guid gradebookIdentifier)
        {
            var counts = LoadAchievements(gradebookIdentifier);

            LoadData(gradebookIdentifier, AchievementIdentifier);

            return counts;
        }

        private void LoadData(Guid gradebookIdentifier, Guid? achievementIdentifier)
        {
            var filter = new VCredentialFilter
            {
                OrganizationIdentifier = Organization.Identifier,
                ItemGradebookIdentifier = AchievementType.Value == "Gradebook" ? gradebookIdentifier : (Guid?)null,
                UserGradebookIdentifier = gradebookIdentifier,
                AchievementIdentifier = achievementIdentifier == Guid.Empty ? null : achievementIdentifier,
                UserFullName = FilterTextBox.Text,
                ExcludeAchievements = achievementIdentifier == Guid.Empty ? TopAchievements.Where(x => x.AchievementIdentifier != Guid.Empty).Select(x => x.AchievementIdentifier).ToArray() : null
            };

            Search(filter);

            AchievementRepeater.Visible = TopAchievements.Count > 0;
            AchievementRepeater.DataSource = TopAchievements;
            AchievementRepeater.DataBind();
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

        private Counts LoadAchievements(Guid gradebookIdentifier)
        {
            const int maxAchievements = 10;

            var itemAchievements = ServiceLocator.AchievementSearch.GetItemAndStudentAchievements(gradebookIdentifier, Organization.Identifier);
            var userAchievements = ServiceLocator.AchievementSearch.GetStudentAchievements(gradebookIdentifier, Organization.Identifier);

            var achievements = (AchievementType.Value == "Gradebook" ? itemAchievements : userAchievements)
                .Select(x => new AchievementItem
                {
                    AchievementIdentifier = x.Item1.AchievementIdentifier,
                    AchievementTitle = x.Item1.AchievementTitle,
                    Count = x.Item2
                })
                .OrderByDescending(x => x.Count)
                .ToList();

            TopAchievements = achievements
                .Take(maxAchievements)
                .OrderBy(x => x.AchievementTitle)
                .ToList();

            if (TopAchievements.Count < achievements.Count)
            {
                var count = achievements.Skip(maxAchievements).Sum(x => x.Count);
                TopAchievements.Add(new AchievementItem { AchievementIdentifier = Guid.Empty, AchievementTitle = "Other Achievements", Count = count });
            }

            return new Counts
            {
                CountInItems = itemAchievements.Sum(x => x.Item2),
                CountInUsers = userAchievements.Sum(x => x.Item2)
            };
        }

        protected string GetExpired()
        {
            var row = Page.GetDataItem();
            var actual = (DateTimeOffset?)DataBinder.Eval(row, "CredentialExpired");

            if (actual.HasValue)
                return LocalizeDate(actual);

            var expected = (DateTimeOffset?)DataBinder.Eval(row, "CredentialExpirationExpected");

            return expected.HasValue && expected.Value < DateTimeOffset.UtcNow
                ? LocalizeDate(expected.Value)
                : null;
        }
    }
}