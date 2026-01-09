using System;

using InSite.Application.QuizAttempts.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

namespace InSite.UI.Admin.Assessments.QuizAttempts
{
    public partial class Search : SearchPage<TQuizAttemptFilter>
    {
        public const string NavigateUrl = "/ui/admin/assessment/quizzes-attempts/search";

        public static void Redirect() => HttpResponseHelper.Redirect(NavigateUrl);

        public override string EntityName => "Quiz";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PageHelper.AutoBindHeader(this);
        }
    }
}