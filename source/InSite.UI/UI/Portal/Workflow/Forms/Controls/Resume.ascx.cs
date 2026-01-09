using System;

namespace InSite.UI.Portal.Workflow.Forms.Controls
{
    public partial class Resume : SubmissionSessionControl
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

            var goTo = SubmissionSessionHelper.GoToLastPage(Current);
            Navigator.RedirectToAnswerPage(Current.SessionIdentifier, goTo.Page, goTo.Question);
        }
    }
}