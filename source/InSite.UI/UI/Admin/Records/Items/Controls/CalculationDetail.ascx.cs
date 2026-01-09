using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Application.Records.Read;
using InSite.Domain.Records;

using Shift.Constant;

namespace InSite.Admin.Records.Items.Controls
{
    public partial class CalculationDetail : UserControl
    {
        public class CalculationItem
        {
            public Guid? ParentItem { get; set; }
            public string Code { get; set; }
            public string Hook { get; set; }
            public string Name { get; set; }
            public string ShortName { get; set; }
            public bool IncludeToReport { get; set; }
            public GradeItemWeighting Weighting { get; set; }
            public CalculationPart[] Parts { get; set; }
            public string Reference { get; set; }
            public decimal? PassPercent { get; set; }

            public AchievementPanel.AchievementItem Achievement { get; set; }
            public Guid[] Standards { get; set; }
        }

        [Serializable]
        private class PartItemType
        {
            public Guid ItemKey { get; set; }
            public string Name { get; set; }
            public decimal Score { get; set; }
        }

        private Guid GradebookIdentifier
        {
            get => (Guid)ViewState[nameof(GradebookIdentifier)];
            set => ViewState[nameof(GradebookIdentifier)] = value;
        }

        private string OldCode
        {
            get => (string)ViewState[nameof(OldCode)];
            set => ViewState[nameof(OldCode)] = value;
        }

        private List<PartItemType> Parts =>
            (List<PartItemType>)(ViewState[nameof(Parts)] ?? (ViewState[nameof(Parts)] = new List<PartItemType>()));

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            CodeValidator.ServerValidate += CodeValidator_ServerValidate;

            AddPartButton.Click += AddPartButton_Click;
            PartRepeater.ItemCommand += PartRepeater_ItemCommand;

            PartRepeater.DataBinding += PartRepeater_DataBinding;
            PartRepeater.ItemDataBound += PartRepeater_ItemDataBound;

            ScoreCalculation.AutoPostBack = true;
            ScoreCalculation.SelectedIndexChanged += ScoreCalculation_SelectedIndexChanged;
        }

        private void PartRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var scoreField = e.Item.FindControl("ScoreField");
            scoreField.Visible = ScoreCalculation.SelectedValue == "Percents";
        }

        private void CodeValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = string.Equals(args.Value, OldCode);

            if (!args.IsValid)
            {
                var data = ServiceLocator.RecordSearch.GetGradebookState(GradebookIdentifier);
                args.IsValid = !data.ContainsCode(args.Value);
            }
        }

        private void AddPartButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var itemKey = PartItem.ValueAsGuid.Value;

            if (Parts.Find(x => x.ItemKey == itemKey) == null)
            {
                var data = ServiceLocator.RecordSearch.GetGradebookState(GradebookIdentifier);
                var item = data.FindItem(itemKey);
                var score = ScoreCalculation.SelectedValue == "Percents" ? PartScore.ValueAsDecimal.Value / 100m : 1m;

                Parts.Add(new PartItemType { ItemKey = itemKey, Name = item.Name, Score = score });

                PartRepeater.DataSource = Parts;
                PartRepeater.DataBind();
            }

            PartItem.ClearSelection();
            PartScore.ValueAsDecimal = null;
        }

        private void PartRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                var itemKey = Guid.Parse((string)e.CommandArgument);
                var item = Parts.Find(x => x.ItemKey == itemKey);

                if (item != null)
                {
                    Parts.Remove(item);
                    PartRepeater.DataSource = Parts;
                    PartRepeater.DataBind();
                }
            }
        }

        private void PartRepeater_DataBinding(object sender, EventArgs e)
        {
            PartRepeater.DataSource = Parts;
        }

        private void ScoreCalculation_SelectedIndexChanged(object sender, EventArgs e)
        {
            InitScoreVisibility();

            PartRepeater.DataBind();
        }

        public void InitGradebook(QGradebook gradebook, GradebookState data, string code)
        {
            GradebookIdentifier = gradebook.GradebookIdentifier;

            Code.Text = code;

            ParentItem.GradebookIdentifier = gradebook.GradebookIdentifier;
            ParentItem.RefreshData();

            PartItem.GradebookIdentifier = GradebookIdentifier;
            PartItem.RefreshData();

            InitScoreVisibility();

            AchievementPanel.SetAllowCondition(true);

            var hasStandards = data.Type == GradebookType.Standards || data.Type == GradebookType.ScoresAndStandards;

            StandardHeader.Visible = hasStandards;
            StandardPanel.Visible = hasStandards;

            if (hasStandards)
                StandardPanel.SetInputValue(data, null);
        }

        public bool SetInputValue(Guid gradebookIdentifier, GradebookState data, Guid itemKey)
        {
            var item = data.FindItem(itemKey);

            if (item == null || item.Type != GradeItemType.Calculation)
                return false;

            OldCode = item.Code;

            ParentItem.DisableItemAndSubTree = itemKey;
            ParentItem.RefreshData();

            PartItem.DisableItem = itemKey;
            PartItem.DisableScoreItems = true;
            PartItem.RefreshData();

            ParentItem.ValueAsGuid = item.Parent?.Identifier;
            Code.Text = item.Code;
            Name.Text = item.Name;
            ShortName.Text = item.ShortName;
            IncludeToReport.SelectedValue = item.IsReported.ToString().ToLower();

            SetWeighting(item);

            PassPercent.ValueAsDecimal = item.PassPercent * 100m;

            Reference.Text = item.Reference;
            Hook.Text = item.Hook;

            Parts.Clear();

            if (item.Parts != null)
            {
                foreach (var part in item.Parts)
                {
                    var partItem = data.FindItem(part.Item);
                    Parts.Add(new PartItemType { ItemKey = partItem.Identifier, Name = partItem.Name, Score = part.Score });
                }
            }

            InitScoreVisibility();

            PartRepeater.DataBind();

            AchievementPanel.SetAchievement(gradebookIdentifier, item);

            if (data.Type == GradebookType.Standards || data.Type == GradebookType.ScoresAndStandards)
                StandardPanel.SetInputValue(data, itemKey);

            return true;
        }

        private void SetWeighting(GradeItem item)
        {
            switch (item.Weighting)
            {
                case GradeItemWeighting.Equally:
                    ScoreCalculation.SelectedValue = "Equally";
                    break;
                case GradeItemWeighting.ByPercent:
                    ScoreCalculation.SelectedValue = "Percents";
                    break;
                case GradeItemWeighting.EquallyWithNulls:
                    ScoreCalculation.SelectedValue = "EquallyWithNulls";
                    break;
            }
        }

        public CalculationItem GetInputValues()
        {
            return new CalculationItem
            {
                ParentItem = ParentItem.ValueAsGuid,
                Code = Code.Text,
                Hook = Hook.Text,
                Name = Name.Text,
                ShortName = ShortName.Text,
                IncludeToReport = bool.Parse(IncludeToReport.SelectedValue),
                Weighting = ScoreCalculation.SelectedValue == "Equally"
                    ? GradeItemWeighting.Equally
                    : (ScoreCalculation.SelectedValue == "EquallyWithNulls"
                        ? GradeItemWeighting.EquallyWithNulls
                        : GradeItemWeighting.ByPercent),
                Parts = Parts.Select(x => new CalculationPart { Item = x.ItemKey, Score = x.Score }).ToArray(),
                Reference = Reference.Text,
                Achievement = AchievementPanel.GetAchievement(),
                Standards = StandardPanel.GetStandards(),
                PassPercent = PassPercent.ValueAsDecimal / 100.0m
            };
        }

        public void InitScoreVisibility()
        {
            PartScore.Visible = ScoreCalculation.SelectedValue == "Percents";
            PercentLiteral.Visible = ScoreCalculation.SelectedValue == "Percents";
            PartScoreRequired.Visible = ScoreCalculation.SelectedValue == "Percents";
        }
    }
}