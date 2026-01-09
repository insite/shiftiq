using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;
using InSite.Domain.Records;
using InSite.Persistence;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Records.Gradebooks.Controls
{
    public partial class GradeItemsGrid : UserControl
    {
        protected bool HideControls
        {
            get => (bool)ViewState[nameof(HideControls)];
            set => ViewState[nameof(HideControls)] = value;
        }
        private class IncludedItem
        {
            public string Name { get; set; }
            public decimal? Score { get; set; }
        }

        private class StandardItem
        {
            public string Title { get; set; }
        }

        private class GradeGridItem
        {
            public Guid Key { get; set; }
            public Guid? ParentKey { get; set; }
            public string Code { get; set; }
            public string Hook { get; set; }
            public string Name { get; set; }
            public string ShortName { get; set; }
            public string Type { get; set; }
            public Guid? AchievementIdentifier { get; set; }
            public string AchievementTitle { get; set; }
            public int Level { get; set; }
            public bool IncludeToReport { get; set; }
            public bool HasReorderOption { get; set; }

            public List<IncludedItem> IncludedItems { get; set; }
            public List<string> Options { get; set; }
            public List<StandardItem> Standards { get; set; }
        }

        private Guid _gradebookIdentifier;
        private bool _isLocked;
        private Dictionary<Guid, Standard> _standards;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ItemRepeater.ItemDataBound += ItemRepeater_ItemDataBound;
        }

        private void ItemRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var controlColumn = e.Item.FindControl("GradeItemControls");
            if (controlColumn != null && HideControls)
                controlColumn.Visible = false;

            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var item = (GradeGridItem)e.Item.DataItem;

            var reorderLink = (IconLink)e.Item.FindControl("ReorderLink");
            reorderLink.Visible = !_isLocked && item.HasReorderOption;
            reorderLink.NavigateUrl = $"/ui/admin/records/items/reorder?gradebook={_gradebookIdentifier}&parent={item.ParentKey}";

            var editLink = (IconLink)e.Item.FindControl("EditLink");
            editLink.Visible = !_isLocked;

            switch (item.Type)
            {
                case "Category":
                    editLink.NavigateUrl = $"/ui/admin/records/items/change-category?gradebook={_gradebookIdentifier}&item={item.Key}";
                    break;
                case "Score":
                    editLink.NavigateUrl = $"/ui/admin/records/items/change-score?gradebook={_gradebookIdentifier}&item={item.Key}";
                    break;
                case "Calculation":
                    editLink.NavigateUrl = $"/ui/admin/records/items/change-calculation?gradebook={_gradebookIdentifier}&item={item.Key}";

                    e.Item.FindControl("NoIncludedItems").Visible = item.IncludedItems.IsEmpty();

                    var includedItemsRepeater = (Repeater)e.Item.FindControl("IncludedItems");
                    includedItemsRepeater.Visible = item.IncludedItems.IsNotEmpty();
                    includedItemsRepeater.DataSource = item.IncludedItems;
                    includedItemsRepeater.DataBind();
                    break;
            }

            var voidLink = (IconLink)e.Item.FindControl("DeleteLink");
            voidLink.Visible = !_isLocked;
            voidLink.NavigateUrl = $"/admin/records/items/delete?gradebook={_gradebookIdentifier}&item={item.Key}";

            var optionRepeater = (Repeater)e.Item.FindControl("OptionRepeater");
            optionRepeater.DataSource = item.Options;
            optionRepeater.DataBind();

            var standardsRepeater = (Repeater)e.Item.FindControl("Standards");
            standardsRepeater.Visible = item.Standards.IsNotEmpty();
            standardsRepeater.DataSource = item.Standards;
            standardsRepeater.DataBind();
        }

        public void LoadData(Guid gradebookIdentifier, GradebookState dataGradebook, bool hideControls = false)
        {
            var data = new List<GradeGridItem>();
            var root = dataGradebook.RootItems;

            AddItems(dataGradebook, null, data, root, 0);

            HideControls = hideControls;
            NoItems.Visible = data.Count == 0;
            ItemRepeater.Visible = data.Count > 0;

            _gradebookIdentifier = gradebookIdentifier;
            _isLocked = dataGradebook.IsLocked;

            ItemRepeater.DataSource = data;
            ItemRepeater.DataBind();
        }

        private void AddItems(GradebookState dataGradebook, Guid? parentKey, List<GradeGridItem> output, List<GradeItem> input, int level)
        {
            if (input.IsEmpty())
                return;

            foreach (var inputItem in input)
            {
                var achievementTitle = inputItem.Achievement != null
                    ? ServiceLocator.AchievementSearch.GetAchievement(inputItem.Achievement.Achievement)?.AchievementTitle
                    : null;

                var outputItem = new GradeGridItem
                {
                    Key = inputItem.Identifier,
                    ParentKey = parentKey,
                    Code = inputItem.Code,
                    Hook = inputItem.Hook,
                    Name = inputItem.Name,
                    ShortName = inputItem.ShortName,
                    Type = inputItem.Type.ToString(),
                    AchievementIdentifier = inputItem.Achievement?.Achievement,
                    AchievementTitle = achievementTitle,
                    Level = level,
                    IncludeToReport = inputItem.IsReported,
                    IncludedItems = inputItem.Type == GradeItemType.Calculation && inputItem.Parts != null
                        ? inputItem.Parts.Select(x => new IncludedItem
                        {
                            Name = dataGradebook.FindItem(x.Item).Name,
                            Score = inputItem.IsEqualWeighting ? (decimal?)null : x.Score
                        }).ToList()
                        : null,
                    HasReorderOption = input.Count > 1,
                    Options = new List<string>()
                };

                if (inputItem.Format != GradeItemFormat.None)
                {
                    var description = inputItem.Format.GetDescription();
                    var color = inputItem.Format.GetContextualClass();

                    if (string.Equals(color, "default", StringComparison.OrdinalIgnoreCase))
                        color = "custom-default";

                    outputItem.Options.Add($"<span class='badge bg-{color}'>{description}</span>");
                }

                if (inputItem.Type == GradeItemType.Score && inputItem.Format == GradeItemFormat.Point)
                {
                    var maxPoint = inputItem.MaxPoints.HasValue ? $"{inputItem.MaxPoints:n1}" : "N/A";
                    outputItem.Options.Add($"<div class='form-text'>Max Points: {maxPoint}</div>");
                }

                if (inputItem.Weighting != GradeItemWeighting.None)
                    outputItem.Options.Add($"<div class='form-text'>Calculation: {inputItem.Weighting.GetDescription()}</div>");

                if (inputItem.PassPercent.HasValue)
                    outputItem.Options.Add($"<div class='form-text'>Passing score: {inputItem.PassPercent:p1}</div>");

                output.Add(outputItem);

                AddStandards(inputItem, outputItem);

                AddItems(dataGradebook, inputItem.Identifier, output, inputItem.Children, level + 1);
            }
        }

        private void AddStandards(GradeItem inputItem, GradeGridItem outputItem)
        {
            if (inputItem.Competencies.IsEmpty())
                return;

            outputItem.Standards = new List<StandardItem>();

            if (_standards == null)
                _standards = new Dictionary<Guid, Standard>();

            foreach (var identifier in inputItem.Competencies)
            {
                if (!_standards.TryGetValue(identifier, out var standard))
                    _standards.Add(identifier, standard = StandardSearch.Select(identifier));

                if (standard != null)
                {
                    outputItem.Standards.Add(new StandardItem
                    {
                        Title = standard.ContentTitle
                    });
                }
            }
        }

        protected string GetCodeAndShortName(object code, object shortName)
        {
            string html = string.Empty;

            if (code != null)
                html += $"#{code}";

            if (shortName != null)
                html += $" ({shortName})";

            return html;
        }
    }
}