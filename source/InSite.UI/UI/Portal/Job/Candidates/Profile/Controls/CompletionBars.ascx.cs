using System;
using System.Web.UI;

using InSite.Persistence;

namespace InSite.UI.Portal.Jobs.Candidates.MyPortfolio.Controls
{
    public partial class CompletionBars : UserControl
    {
        public Guid? ContactKey
        {
            get => ViewState[nameof(ContactKey)] as Guid?;
            set => ViewState[nameof(ContactKey)] = value;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (ContactKey.HasValue)
            {
                var organization = CurrentSessionState.Identity.Organization.Identifier;

                ProfileCompletionBar.Text = GetCompletionBarHtml(UserSearch.GetCompletionProfilePercent(organization, ContactKey.Value));
                ResumeCompletionBar.Text = GetCompletionBarHtml(UserSearch.GetCompletionResumePercent(organization, ContactKey.Value));
            }
            else
                Visible = false;
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