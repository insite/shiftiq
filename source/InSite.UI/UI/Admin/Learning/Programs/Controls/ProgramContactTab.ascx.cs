using System;
using System.Web;
using System.Web.UI;

using InSite.Application.Records.Read;
using InSite.Common.Web.UI;

namespace InSite.UI.Admin.Learning.Programs.Controls
{
    public partial class ProgramContactTab : BaseUserControl
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SearchInput.Click += FilterButton_Click;
        }

        public void LoadData(TProgram program)
        {
            var programId = program.ProgramIdentifier;
            var programAchievementId = program.AchievementIdentifier;

            PersonGrid.Visible = PersonGrid.LoadData(programId, programAchievementId);
            GroupGrid.Visible = GroupGrid.LoadData(programId);

            NoLearners.Visible = !PersonGrid.Visible && !GroupGrid.Visible;

            // Where Add Learners goes is decided by the fee first and the program type second. Only
            // Add Learners to Program carries the funding path a fee needs, and group assignment
            // refuses a paid program outright - so an Achievements Only program that also charges a
            // fee has to go to the paid page, or the button lands on a page that tells the admin to
            // use a page the button will not take them to.
            var isPaid = program.ProgramFee > 0;
            var isAchievementsOnly = program.ProgramType == "Achievements Only";

            if (isAchievementsOnly && !isPaid)
            {
                var returnUrl = new ReturnUrl("id&tab=enrollments");

                AddButton.NavigateUrl = returnUrl.GetRedirectUrl($"/ui/admin/learning/programs/enrollments/assign?program={program.ProgramIdentifier}");
            }
            else
            {
                var returnUrl = $"/ui/admin/learning/programs/outline?id={programId}&tab=enrollments";
                var returnUrlParam = "&return=" + HttpUtility.UrlEncode(returnUrl);

                AddButton.NavigateUrl = $"/ui/admin/learning/programs/enrollments/add?program={programId}" + returnUrlParam;
            }
        }

        private void FilterButton_Click(object sender, EventArgs e)
        {
            PersonGrid.SearchByKeyword(SearchInput.Text);
            GroupGrid.SearchByKeyword(SearchInput.Text);

            ScriptManager.RegisterStartupScript(
                Page,
                typeof(ProgramContactTab),
                "filter_focus",
                $"inSite.common.baseInput.focus('{SearchInput.ClientID}_box', true);",
                true);
        }
    }
}