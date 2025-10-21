using System;
using System.Linq;

using InSite.Common.Web.UI;
using InSite.Domain.Banks;

namespace InSite.Admin.Assessments.Sets.Controls
{
    public partial class SetDetails : BaseUserControl
    {
        #region Properties

        public Guid SetID
        {
            get => (Guid)ViewState[nameof(SetID)];
            private set => ViewState[nameof(SetID)] = value;
        }

        #endregion

        #region Data binding

        public void SetInputValues(Set set, bool canWrite)
        {
            if (set == null)
                return;

            SetID = set.Identifier;

            SetNumber.Text = $"{set.Sequence} of {set.Bank.Sets.Count}";
            SetQuestionCount.Text = $"{set.Questions.Count}";
            BankQuestionCount.Text = $"{set.Bank.Sets.SelectMany(x => x.Questions).Count()}";
            Name.Text = set.Name;
            Standard.AssetID = set.Standard;

            CutScore.Text = set.CutScore.HasValue ? set.CutScore.Value.ToString("p2") : "N/A";
            Points.Text = set.Points.HasValue ? set.Points.Value.ToString("n2") : "N/A";

            if (set.Randomization.Enabled)
            {
                Randomization.Text = "Enabled";
                RandomizationDescription.Text = $"<div class='form-text my-0'>{set.Randomization.Count:n0} question(s)</div>";
            }
            else
            {
                Randomization.Text = "Disabled";
                RandomizationDescription.Text = string.Empty;
            }

            var returnUrl = new ReturnUrl($"bank&set={set.Identifier}&panel=questions");

            EditStandardLink.NavigateUrl = $"/ui/admin/assessments/sets/change-standard?bank={set.Bank.Identifier}&set={set.Identifier}";
            EditRandomization.NavigateUrl = $"/ui/admin/assessments/sets/change-randomization?bank={set.Bank.Identifier}&set={set.Identifier}";
            RenameSetLink.NavigateUrl = returnUrl.GetRedirectUrl($"/ui/admin/assessments/sets/rename?bank={set.Bank.Identifier}&set={set.Identifier}");
            DeleteSetLink.NavigateUrl = returnUrl.GetRedirectUrl($"/admin/assessments/sets/delete?bank={set.Bank.Identifier}&set={set.Identifier}");

            EditStandardLink.Visible = canWrite;
            EditRandomization.Visible = canWrite;
            RenameSetLink.Visible = canWrite;
            DeleteSetLink.Visible = canWrite;
        }

        #endregion
    }
}