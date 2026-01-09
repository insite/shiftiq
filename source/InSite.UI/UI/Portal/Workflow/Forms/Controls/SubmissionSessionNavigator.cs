using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using InSite.Common.Web;
using InSite.Domain.Surveys.Forms;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.UI.Portal.Workflow.Forms.Controls
{
    public class SubmissionSessionNavigator
    {
        private readonly List<ResponseSessionNavigatorStep> _steps = new List<ResponseSessionNavigatorStep>();

        public void Initialize(SurveyForm form)
        {
            var pages = form.GetPages();
            var i = 0;
            foreach (var question in form.Questions)
            {
                if (question.HasInput)
                {
                    var step = new ResponseSessionNavigatorStep
                    {
                        PageNumber = pages.Single(x => x.Questions.Any(y => y.Identifier == question.Identifier)).Sequence,
                        QuestionIdentifier = question.Identifier
                    };

                    if (question.DisplayAnswerInput()) // if (!question.IsHidden)
                        step.QuestionNumber = ++i;

                    _steps.Add(step);
                }
            }
        }

        public int GetQuestionNumber(Guid question)
        {
            return _steps.FirstOrDefault(x => x.QuestionIdentifier == question)?.QuestionNumber ?? 0;
        }

        private static void Redirect(WebUrl url)
        {
            CopyQueryString(url);

            HttpResponseHelper.Redirect(url, true);
        }

        public static void RedirectTo(ResponseVerb action, Guid session)
        {
            Redirect(GetPageUrl(action, session));
        }

        public static void RedirectToStart(Guid session)
        {
            Redirect(GetAnswerPageUrl(session, "1", null));
        }

        public void RedirectToAnswerPage(Guid session, int page, Guid? question)
        {
            Redirect(GetAnswerPageUrl(session, page, question));
        }

        public void RedirectToCompletePage(Guid session)
        {
            var url = GetReviewPageUrl(session);
            Redirect(url);
        }

        public void RedirectToConfirmPage(Guid session)
        {
            Redirect(GetPageUrl(ResponseVerb.Confirm, session));
        }

        public void RedirectToNextPage(Guid session, int page)
        {
            var url = GetPageUrl(ResponseVerb.GoTo, session);
            url.QueryString.Add("page", page.ToString());
            url.QueryString.Add("to", "next");
            Redirect(url);
        }

        public void RedirectToPreviousPage(Guid session, int page)
        {
            var url = GetPageUrl(ResponseVerb.GoTo, session);
            url.QueryString.Add("page", page.ToString());
            url.QueryString.Add("to", "previous");
            Redirect(url);
        }

        public static void RedirectToLaunchPage(int form, Guid user)
        {
            Redirect(GetLaunchPageUrl(form, user));
        }

        public WebUrl GetAnswerPageUrl(Guid session, int page, Guid? question) =>
            GetAnswerPageUrl(session, page.ToString(), question);

        private static WebUrl GetAnswerPageUrl(Guid session, string page, Guid? question)
        {
            var url = GetPageUrl(ResponseVerb.Answer, session);

            url.QueryString["page"] = page;

            if (question.HasValue)
                url.QueryString["question"] = question.Value.ToString();

            return url;
        }

        public WebUrl GetLaunchPageUrl(Guid session)
        {
            return GetPageUrl(ResponseVerb.Launch, session);
        }

        public static WebUrl GetLaunchPageUrl(int form, Guid user)
        {
            var url = GetPageUrl(ResponseVerb.Launch, null);
            url.QueryString.Add("form", form.ToString());
            url.QueryString.Add("user", user.ToString());
            return url;
        }

        public static WebUrl GetResumePageUrl(Guid session)
        {
            return GetPageUrl(ResponseVerb.Resume, session);
        }

        public static WebUrl GetReviewPageUrl(Guid session)
        {
            var url = GetPageUrl(ResponseVerb.Review, session);

            CopyQueryString(url);

            return url;
        }

        private static void CopyQueryString(WebUrl url)
        {
            var queryString = HttpContext.Current.Request.QueryString;

            var callerValue = queryString["caller"];
            if (callerValue.IsNotEmpty())
                url.QueryString["caller"] = callerValue;

            var returnValue = queryString["return"];
            if (returnValue.IsNotEmpty())
                url.QueryString["return"] = returnValue;
        }

        public static WebUrl GetStartPageUrl(Guid session)
        {
            return GetAnswerPageUrl(session, "1", null);
        }

        public static WebUrl GetDeletePageUrl(Guid session)
        {
            return GetPageUrl(ResponseVerb.Delete, session);
        }

        private static WebUrl GetPageUrl(ResponseVerb verb, Guid? session)
        {
            var url = new WebUrl("/ui/portal/workflow/forms/submit/" + verb.GetNameLowerCase());

            if (session.HasValue)
                url.QueryString["session"] = session.Value.ToString();

            return url;
        }
    }
}
