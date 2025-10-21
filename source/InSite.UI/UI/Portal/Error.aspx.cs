using System;

using InSite.UI.Layout.Portal;

namespace InSite.UI.Portal
{
    public partial class Error : PortalBasePage
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            /*
            var problem = CurrentSessionState.LastException as PortalProblemException;
            if (problem != null )
            {
                ProblemTitle.Text = problem.Title;
                ProblemSource.Text = problem.Source;
                ProblemDescription.Text = Markdown.ToHtml(problem.Description);
            }
            else
            {
                ProblemTitle.Text = "Problem Not Described";
                if (Page.Request.UrlReferrer != null)
                    ProblemSource.Text = Page.Request.UrlReferrer.OriginalString;
                ProblemDescription.Text = "The cause of this problem has not yet been described. Please contact support with the steps to reproduce this error message";
            }
            */
        }
    }
}