using System;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Application.Records.Read;
using InSite.Domain.Records;

using Shift.Constant;

namespace InSite.Admin.Records.Items.Controls
{
    public partial class CategoryDetail : UserControl
    {
        public class CategoryItem
        {
            public Guid? ParentItem { get; set; }
            public string Code { get; set; }
            public string Hook { get; set; }
            public string Name { get; set; }
            public string ShortName { get; set; }
            public bool IncludeToReport { get; set; }
            public GradeItemWeighting Weighting { get; set; }
            public string Reference { get; set; }
            public decimal? PassPercent { get; set; }

            public AchievementPanel.AchievementItem Achievement { get; set; }
            public Guid[] Standards { get; set; }
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

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            CodeValidator.ServerValidate += CodeValidator_ServerValidate;
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

        public void InitGradebook(QGradebook gradebook, GradebookState data, string code)
        {
            GradebookIdentifier = gradebook.GradebookIdentifier;

            Code.Text = code;

            ParentItem.GradebookIdentifier = gradebook.GradebookIdentifier;
            ParentItem.RefreshData();

            AchievementPanel.SetAllowCondition(true);

            var hasStandards = data.Type == GradebookType.Standards || data.Type == GradebookType.ScoresAndStandards;

            StandardsCard.Visible = hasStandards;

            if (hasStandards)
                StandardPanel.SetInputValue(data, null);
        }

        public bool SetInputValue(Guid gradebookIdentifier, GradebookState data, Guid itemKey)
        {
            var item = data.FindItem(itemKey);

            if (item == null || item.Type != GradeItemType.Category)
                return false;

            OldCode = item.Code;

            ParentItem.DisableItemAndSubTree = itemKey;
            ParentItem.RefreshData();

            ParentItem.ValueAsGuid = item.Parent?.Identifier;
            Code.Text = item.Code;
            Name.Text = item.Name;
            ShortName.Text = item.ShortName;
            IncludeToReport.SelectedValue = item.IsReported.ToString().ToLower();

            ScoreCalculation.SelectedValue =
                item.Weighting == GradeItemWeighting.Equally
                ? "Equally"
                : (item.Weighting == GradeItemWeighting.EquallyWithNulls
                   ? "EquallyWithNulls"
                   : (item.Weighting == GradeItemWeighting.ByPoints
                      ? "Points"
                      : "Sum"));

            PassPercent.ValueAsDecimal = item.PassPercent * 100m;

            Reference.Text = item.Reference;
            Hook.Text = item.Hook;

            AchievementPanel.SetAchievement(gradebookIdentifier, item);

            if (data.Type == GradebookType.Standards || data.Type == GradebookType.ScoresAndStandards)
                StandardPanel.SetInputValue(data, itemKey);

            return true;
        }

        public CategoryItem GetInputValues()
        {
            return new CategoryItem
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
                       : (ScoreCalculation.SelectedValue == "Points"
                          ? GradeItemWeighting.ByPoints
                          : GradeItemWeighting.Sum)),
                Reference = Reference.Text,
                Achievement = AchievementPanel.GetAchievement(),
                Standards = StandardPanel.GetStandards(),
                PassPercent = PassPercent.ValueAsDecimal / 100.0m
            };
        }
    }
}