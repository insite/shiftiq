using System;
using System.Web;
using System.Web.UI;

using Shift.Constant;

namespace InSite.UI.Portal.Assessments.Attempts.Utilities
{
    public class AnswerLoader
    {
        public string AttemptId { get; private set; }

        public string SessionId { get; private set; }

        public Guid ResourceId { get; private set; }

        public Guid FormId { get; private set; }

        public bool IsResource { get; private set; }

        public bool IsForm { get; private set; }

        public HttpRequest Request => _page.Request;

        public AttemptUrlBase Url { get; private set; }

        private Page _page;

        public AnswerLoader(Page page)
        {
            _page = page;

            AttemptId = page.Request.QueryString["attempt"];
            SessionId = page.Request.Form["session"];

            if (Guid.TryParse(page.Request.QueryString["resource"], out var resId))
            {
                ResourceId = resId;
                IsResource = true;
            }
            else if (Guid.TryParse(page.Request.QueryString["form"], out var frmId))
            {
                FormId = frmId;
                IsForm = true;
            }
        }

        public AttemptHelper.IAction LoadStartData(out AnswerLoadData data)
        {
            data = null;

            if (CurrentSessionState.Identity.IsImpersonating)
            {
                return AttemptHelper.GetErrorResult(
                    "Permission Denied",
                    "You cannot start an assessment while impersonating another user.");
            }
            else if (IsResource)
            {
                return LoadResource(out data);
            }
            else if (IsForm)
            {
                return LoadForm(out data);
            }
            else
                return AttemptHelper.GetErrorResult(
                    "Invalid URL",
                    "A valid resource identifier is required.");
        }

        private AttemptHelper.IAction LoadResource(out AnswerLoadData data)
        {
            data = null;

            var url = AttemptUrlResource.Load(AttemptActionType.Answer, ResourceId, AttemptId);
            if (url == null)
                return GetErrorResult();

            Url = url;

            var actionResult = AttemptHelper.LoadResource(url.PageIdentifier, out var form, out var contentStyle);
            if (actionResult != null)
                return actionResult;

            actionResult = AttemptHelper.LoadAttemptAnswer(form, url, out var attemptModel);
            if (actionResult != null)
                return actionResult;

            if (attemptModel == null)
                return GetErrorResult();

            data = new AnswerLoadData
            {
                Url = url,
                BankForm = form,
                Attempt = attemptModel,
                ContentStyle = contentStyle
            };

            return null;

            AttemptHelper.IAction GetErrorResult()
                => AttemptHelper.GetErrorResult("Invalid Request", "The attempt key is invalid.");
        }

        private AttemptHelper.IAction LoadForm(out AnswerLoadData data)
        {
            data = null;

            var url = AttemptUrlForm.Load(AttemptActionType.Answer, FormId, AttemptId);
            if (url == null)
                return GetErrorResult();

            Url = url;

            var actionResult = AttemptHelper.LoadForm(url, out var form);
            if (actionResult != null)
                return actionResult;

            actionResult = AttemptHelper.LoadAttemptAnswer(form, url, out var attemptModel);
            if (actionResult != null)
                return actionResult;

            if (attemptModel == null)
                return GetErrorResult();

            if (LockedGradebookHelper.HasLockedGradebook(attemptModel.AttemptIdentifier, form.Identifier, form.Hook))
                return AttemptHelper.GetErrorResult(
                    "Gradebook Locked",
                    "The gradebook is locked, please contact the administrator for details.");

            data = new AnswerLoadData
            {
                Url = url,
                BankForm = form,
                Attempt = attemptModel
            };

            return null;

            AttemptHelper.IAction GetErrorResult()
                => AttemptHelper.GetErrorResult("Invalid Request", "The attempt key is invalid.");
        }

        public Control LoadControl(string path)
        {
            return _page.LoadControl(path);
        }
    }
}