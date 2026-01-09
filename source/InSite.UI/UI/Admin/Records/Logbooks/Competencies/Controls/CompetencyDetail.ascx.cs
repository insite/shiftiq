using System.Web.UI;

using InSite.Application.Records.Read;
using InSite.Persistence;

namespace InSite.UI.Admin.Records.Logbooks.Competencies.Controls
{
    public partial class CompetencyDetail : UserControl
    {
        public void LoadData(QExperienceCompetency experienceCompetency)
        {
            var competency = experienceCompetency.Competency;

            var framework = competency.FrameworkIdentifier.HasValue
                ? StandardSearch.Select(competency.FrameworkIdentifier.Value)
                : null;

            Framework.Text = framework?.StandardIdentifier != null
                ? $"<a href='/ui/admin/standards/edit?id={framework.StandardIdentifier}'>{framework.ContentTitle}</a>"
                : "<i>None</i>";

            GAC.Text = competency.AreaIdentifier.HasValue
                ? $"<a href='/ui/admin/standards/edit?id={competency.AreaIdentifier}'>{competency.AreaTitle}</a>"
                : "<i>None</i>";

            Competency.Text = competency.CompetencyTitle;
            Competency.Text = $"<a href='/ui/admin/standards/edit?id={competency.CompetencyIdentifier}'>{competency.CompetencyTitle}</a>";

            Hours.Text = experienceCompetency.CompetencyHours.HasValue
                ? $"{experienceCompetency.CompetencyHours:n2}"
                : "<i>None</i>";

            SatisfactionLevel.Text = experienceCompetency.SatisfactionLevel ?? "<i>None</i>";

            SkillRating.Text = experienceCompetency.SkillRating.HasValue
                ? experienceCompetency.SkillRating.ToString()
                : "<i>None</i>";
        }
    }
}