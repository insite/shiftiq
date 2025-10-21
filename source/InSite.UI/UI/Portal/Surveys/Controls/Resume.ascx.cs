using System;

namespace InSite.Portal.Surveys.Responses
{
    public partial class Resume : ResponseSessionControl
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DebugButton.Click += (sender, args) => Continue();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Current.Debug)
                Continue();
        }

        private void Continue()
        {
            if (!Current.IsValid)
                return;

            var goTo = ResponseSessionHelper.GoToLastPage(Current);
            Navigator.RedirectToAnswerPage(Current.SessionIdentifier, goTo.Page, goTo.Question);
        }
    }
}