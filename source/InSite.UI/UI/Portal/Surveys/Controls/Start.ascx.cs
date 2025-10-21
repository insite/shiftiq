using System;

using InSite.Application.Responses.Write;
using InSite.Common.Web;
using InSite.Portal.Surveys.Pages;

namespace InSite.Portal.Surveys.Responses
{
    public partial class Start : ResponseSessionControl
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DebugButton.Click += (sender, args) => Continue();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (Current?.Survey != null && LockedGradebookHelper.HasLockedGradebook(Current.Survey.Identifier, Current.Survey.Hook))
                HttpResponseHelper.Redirect(GetLaunchUrl());

            if (Current.IsValid && !Current.Debug && Current.SessionIdentifier != Guid.Empty)
                Continue();
        }

        private void Continue()
        {
            ServiceLocator.SendCommand(new StartResponseSession(Current.SessionIdentifier, DateTimeOffset.Now));
            Navigator.RedirectToAnswerPage(Current.SessionIdentifier, 1, null);
        }

        private string GetLaunchUrl()
        {
            return $"/ui/survey/respond/launch/{Current.Survey.Asset}/{Current.UserIdentifier}";
        }
    }
}