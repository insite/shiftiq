using System.Web.UI;

using InSite.Persistence;

namespace InSite.UI.Portal.Jobs.Candidates.MyPortfolio.Controls
{
    public partial class ProgressSection : UserControl
    {
        public void BindModelToControls(Person person)
        {
            var organization = CurrentSessionState.Identity.Organization.Identifier;

            ProfileCompletionBar.Text = GetCompletionBarHtml(UserSearch.GetCompletionProfilePercent(organization, person.UserIdentifier));
            ResumeCompletionBar.Text = GetCompletionBarHtml(UserSearch.GetCompletionResumePercent(organization, person.UserIdentifier));
        }

        private string GetCompletionBarHtml(int? percent)
        {
            if (percent == null)
                return null;

            var completionStatus = UserSearch.GetCompletionStatus(percent.Value);

            return
                $@"<div class='progress profile-fields-complete' data-toggle='tooltip' data-placement='bottom'>
                    <div class='progress-bar progress-bar-{completionStatus}' role='progressbar' aria-valuenow='{percent.Value}' aria-valuemin='0' aria-valuemax='100' style='min-width: 4em; width: {percent.Value}%'>
                        <span>{percent.Value}% complete</span>
                    </div>
                </div>";
        }
    }
}