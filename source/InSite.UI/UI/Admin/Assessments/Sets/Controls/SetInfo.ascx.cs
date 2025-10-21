using System.Linq;

using InSite.Domain.Banks;

namespace InSite.Admin.Assessments.Sets.Controls
{
    public partial class SetInfo : System.Web.UI.UserControl
    {
        public void BindSet(Set set, bool showName = true, bool showStandard = true, bool showRandom = true)
        {
            if (set == null)
                return;

            SetNumber.Text = $"{set.Sequence} of {set.Bank.Sets.Count}";
            SetQuestionCount.Text = $"{set.Questions.Count}";
            BankQuestionCount.Text = $"{set.Bank.Sets.SelectMany(x => x.Questions).Count()}";
            Name.Text = set.Name;
            NameDiv.Visible = showName;
            StandardDiv.Visible = showStandard;
            Standard.AssetID = set.Standard;

            if (set.CutScore.HasValue)
                CutScore.Text = set.CutScore.Value.ToString("p2");
            else
                CutScoreDiv.Visible = false;

            if (set.Points.HasValue)
                Points.Text = set.Points.Value.ToString("n2");
            else
                PointsDiv.Visible = false;

            if (set.Randomization.Enabled)
            {
                Randomization.Text = "Enabled";
                RandomizationDescription.Text = $"<div class='form-text'>{set.Randomization.Count:n0} question(s)</div>";
            }
            else
            {
                Randomization.Text = "Disabled";
                RandomizationDescription.Text = string.Empty;
            }
            RandomizationDiv.Visible = showRandom;
        }
    }
}