using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Humanizer;
using Humanizer.Localisation;

using InSite.Application.Attempts.Read;
using InSite.Application.Contents.Read;
using InSite.Common.Web;
using InSite.Domain.Banks;
using InSite.Persistence;
using InSite.Portal.Assessments.Attempts.Models;
using InSite.UI.Layout.Portal;
using InSite.UI.Layout.Portal.Controls;
using InSite.UI.Portal.Assessments.Attempts.Controls;
using InSite.UI.Portal.Assessments.Attempts.Utilities;
using InSite.Web.Helpers;
using InSite.Web.Routing;

using Shift.Common;
using Shift.Constant;

namespace InSite.Portal.Assessments.Attempts
{
    public partial class Result : PortalBasePage, IOverrideWebRouteParent, IHasParentLinkParameters
    {
        #region Classes

        private class ButtonInfo
        {
            public ButtonStyle Style { get; }
            public string Url { get; }
            public string Text { get; }
            public string Icon { get; }

            public ButtonInfo(string icon, string text, string url, ButtonStyle style)
            {
                Url = url;
                Text = text;
                Icon = icon;
                Style = style;
            }
        }

        #endregion

        #region Properties

        private Guid ResourceId => Guid.TryParse(Request.QueryString["resource"], out var value) ? value : Guid.Empty;

        private Guid FormId => Guid.TryParse(Request.QueryString["form"], out var value) ? value : Guid.Empty;

        private string AttemptId => Request.QueryString["attempt"];

        private bool IsForm => FormId != Guid.Empty;

        protected bool IsFormThirdPartyEnabled
        {
            get => (bool)(ViewState[nameof(IsFormThirdPartyEnabled)] ?? false);
            set => ViewState[nameof(IsFormThirdPartyEnabled)] = value;
        }

        private AttemptResultModel Model { get; set; }

        #endregion

        #region Initialization and Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            QuestionRepeater.DataBinding += QuestionRepeater_DataBinding;
            QuestionRepeater.ItemDataBound += QuestionRepeater_ItemDataBound;

            DownloadPDFButton.Click += DownloadPDFButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            var result = LoadResourceOrExam();

            if (result != null)
            {
                MainContainer.Visible = false;

                result.Execute(AlertMessage, Notification);
            }

            base.OnLoad(e);

            if (IsPostBack)
                return;

            PortalMaster.SidebarVisible(false);
            PortalMaster.HideBreadcrumbsAndTitle();
        }

        public AttemptHelper.IAction LoadResourceOrExam()
        {
            var url = IsForm
                ? (AttemptUrlBase)AttemptUrlForm.Load(AttemptActionType.Result, FormId, AttemptId)
                : AttemptUrlResource.Load(AttemptActionType.Result, ResourceId, AttemptId);

            if (url == null || !url.AttemptID.HasValue)
                return GetErrorResult();

            var actionResult = AttemptHelper.LoadForm(url, out var bankForm);
            if (actionResult != null)
                return actionResult;

            AttemptResultModel model;

            if (bankForm.AssetVersion > 0)
            {
                actionResult = AttemptHelper.LoadAttemptResult(bankForm, url, out var attemptModel);
                if (actionResult != null)
                    return actionResult;

                if (attemptModel == null)
                    return GetErrorResult();

                model = new AttemptResultModel
                {
                    IsSuccess = attemptModel.AttemptIsPassing,
                    ScoreScaled = attemptModel.AttemptScore ?? 0.0m,

                    BankForm = bankForm,
                    Attempt = attemptModel,
                };

                if (attemptModel.AttemptSubmitted.HasValue)
                {
                    var summary = ServiceLocator.LearnerAttemptSummarySearch.GetSummary(bankForm.Identifier, attemptModel.LearnerUserIdentifier);

                    model.AttemptOrdinal = summary.AttemptTotalCount.ToOrdinalWords();

                    if (summary.AttemptTotalCount == 0 || summary.AttemptTotalCount < bankForm.Invigilation.AttemptLimit || bankForm.Invigilation.AttemptLimit == 0)
                    {
                        var retryCount = bankForm.Invigilation.AttemptLimit - summary.AttemptTotalCount;
                        var again = bankForm.Invigilation.AttemptLimit > 0
                            ? $"{retryCount.ToWords()} more time{(retryCount != 1 ? "s" : "")}"
                            : "again";

                        model.RetryInstruction = $"You can <a href='{url.GetStartUrl(null, true)}'>retry this assessment</a> {again}.";
                    }
                }
            }
            else
            {
                model = new AttemptResultModel
                {
                    IsSuccess = false,
                    ScoreScaled = 0.0m,

                    BankForm = bankForm,
                    Attempt = null,
                };
            }

            SetInputValues(model);

            return null;
        }

        AttemptHelper.IAction GetErrorResult()
            => AttemptHelper.GetErrorResult("Invalid Request", "The attempt key is invalid.");

        private void SetInputValues(AttemptResultModel model)
        {
            var bankForm = model.BankForm;
            var disclosure = model.BankForm.Specification.Calculation.Disclosure;
            var attemptLimit = bankForm.Invigilation.AttemptLimit;
            var allowDownload = bankForm.Publication.AllowDownloadAssessmentsQA;

            var attempt = model.Attempt;
            var hasAttempt = attempt != null;
            var isAttemptSubmitted = hasAttempt && attempt.AttemptSubmitted.HasValue;
            var isAttemptGraded = hasAttempt && attempt.AttemptGraded.HasValue;
            var isScoreVisible = disclosure == DisclosureType.Score || disclosure == DisclosureType.Full;
            var isAnswersVisible = disclosure == DisclosureType.Answers || disclosure == DisclosureType.Full;
            var scorePanelType = !isScoreVisible ? "primary" : (!isAttemptGraded ? "warning" : !model.IsSuccess ? "danger" : "success");
            var conclusion = bankForm.Specification.Content.Conclusion?.Get(CurrentSessionState.Identity.Language);
            var hasConclusion = conclusion.IsNotEmpty();

            KioskMode.Visible = bankForm.Invigilation.IsKioskModeRequired;
            IsFormThirdPartyEnabled = bankForm.ThirdPartyAssessmentIsEnabled;

            PageTitle.InnerText = bankForm.Content.Title?.Get(CurrentSessionState.Identity.Language);
            if (hasAttempt && bankForm.ThirdPartyAssessmentIsEnabled && attempt.AssessorUserIdentifier != attempt.LearnerUserIdentifier)
                PageLearner.Text = GetPageLearnerTitle(attempt.LearnerUserIdentifier);

            Model = model;

            ScorePanel.CssClass = $"alert alert-{scorePanelType}";
            ScoreHeader.Visible = isScoreVisible && isAttemptGraded;
            ScoreScaled.Text = model.ScoreScaled.ToString("P0");

            SubmissionCompletionHeader.Visible = (!isAttemptGraded || !isScoreVisible) && !hasConclusion;

            DownloadPDFButton.Visible = allowDownload;
            DownloadPdfScript.Visible = allowDownload;

            AttemptPoints.Visible = hasAttempt;
            AttemptPoints.InnerHtml = hasAttempt ? $" &#9656; {attempt.AttemptPoints ?? 0:n2} / {attempt.FormPoints ?? 0:n2} " + Translate("points") : null;

            CompletionInstruction.Text = hasConclusion ? Markdown.ToHtml(conclusion) : null;

            if (isAttemptSubmitted && isAttemptGraded)
            {
                string attemptStatusText1;

                if (!model.IsSuccess)
                {
                    attemptStatusText1 = attemptLimit == 0 || attemptLimit > 1
                        ? Translate($"You have not successfully completed this assessment on your {model.AttemptOrdinal} attempt.")
                        : Translate("You have not successfully completed this assessment.");
                }
                else
                {
                    attemptStatusText1 = attemptLimit == 0 || attemptLimit > 1
                        ? Translate($"You have successfully completed this assessment on your {model.AttemptOrdinal} attempt.")
                        : Translate("You have successfully completed this assessment.");
                }

                var attemptStatusText = attemptStatusText1;

                if (attempt.AttemptDuration.HasValue)
                {
                    var attemptStatusText2 = Translate("The time taken is {0}.");
                    var culture = CultureInfo.GetCultureInfo(CurrentSessionState.Identity.Language);
                    var timeText = ((double)attempt.AttemptDuration).Seconds().Humanize(precision: 3, culture: culture, maxUnit: TimeUnit.Hour, minUnit: TimeUnit.Second);

                    attemptStatusText += " " + string.Format(attemptStatusText2, timeText);
                }

                if (isAttemptGraded && !model.IsSuccess)
                    attemptStatusText += $" <span>{model.RetryInstruction}</span>";

                AttemptStatus.Visible = disclosure == DisclosureType.Score || disclosure == DisclosureType.Full;
                AttemptStatus.InnerHtml = attemptStatusText;
            }

            AnswersContainer.Visible = isAttemptSubmitted && isAnswersVisible;

            if (isAttemptSubmitted && isAnswersVisible)
                QuestionRepeater.DataBind();

            BindModelToButtons(model);
        }

        #endregion

        #region UI Navigation

        private BreadcrumbDataItem BreadcrumbParent
        {
            get => (BreadcrumbDataItem)ViewState[nameof(BreadcrumbParent)];
            set => ViewState[nameof(BreadcrumbParent)] = value;
        }

        IWebRoute IOverrideWebRouteParent.GetParent() =>
            BreadcrumbParent;

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent) =>
            parent is BreadcrumbDataItem bRoute ? bRoute.NavigationParameters : null;

        #endregion

        #region UI Event Handling

        private void DownloadPDFButton_Click(object sender, EventArgs e)
        {
            var pdfBody = PdfBody.Value;
            if (string.IsNullOrEmpty(pdfBody))
                return;

            var title = Model.BankForm.Content.Title?.Get(CurrentSessionState.Identity.Language);
            var person = ServiceLocator.ContactSearch.GetPerson(Model.Attempt.LearnerUserIdentifier, Organization.Identifier);

            var logoImageUrl = PortalHeader.GetLogoImageUrl(CurrentSessionState.Identity, Page.Server);
            var logoImageElement = (logoImageUrl == null)
                ? ServiceLocator.Partition.GetPlatformName()
                : $"<img alt='' src='{logoImageUrl}' />";

            var bodyHtml = GetFileContent("~/UI/Portal/Assessments/Attempts/Content/ResultPdfBody.html");
            bodyHtml = bodyHtml
                .Replace("<!-- LOGO_IMG -->", logoImageElement)
                .Replace("<!-- TITLE -->", title)
                .Replace("<!-- LEARNER_NAME -->", person?.UserFullName)
                .Replace("<!-- LEARNER_EMAIL -->", person?.UserEmail)
                .Replace("<!-- EXAM_DATE -->", Model.Attempt.AttemptSubmitted.Format(User.TimeZone))
                .Replace("<!-- BODY -->", pdfBody);
            bodyHtml = HtmlHelper.ResolveRelativePaths(Page.Request.Url.Scheme + "://" + Page.Request.Url.Host + Page.Request.RawUrl, bodyHtml);

            var settings = new HtmlConverterSettings(ServiceLocator.AppSettings.Application.WebKitHtmlToPdfExePath)
            {
                EnableSmartShrinking = false,
                Dpi = 350,
                PageSize = PageSizeType.Letter,
                Viewport = new HtmlConverterSettings.ViewportSize(1280, 1652),
                Zoom = 0.8f,

                MarginRight = 13f,
                MarginLeft = 13f,

                MarginBottom = 22,
                FooterTextLeft = title,
                FooterTextCenter = DateTimeOffset.Now.FormatDateOnly(User.TimeZone),
                FooterTextRight = "Page [page] of [topage]",
                FooterFontName = "arial",
                FooterFontSize = 10,
                FooterSpacing = 8.1f,
            };

            var data = HtmlConverter.HtmlToPdf(bodyHtml, settings);
            if (data == null)
                return;

            Response.SendFile("assessment-results.pdf", data);

            string GetFileContent(string virtualPath)
            {
                var physPath = MapPath(virtualPath);

                return System.IO.File.ReadAllText(physPath);
            }
        }

        private void QuestionRepeater_DataBinding(object sender, EventArgs e)
        {
            var attempt = new AttemptInfo(Model.BankForm, Model.Attempt, null);
            var comments = IsFormThirdPartyEnabled || Model.Attempt.AssessorUserIdentifier == User.Identifier
                ? ServiceLocator.AttemptSearch.GetQAttemptComments(attempt.Attempt.AttemptIdentifier, Model.Attempt.AssessorUserIdentifier)
                    .Where(x => x.AssessmentQuestionIdentifier.HasValue)
                    .GroupBy(x => x.AssessmentQuestionIdentifier.Value)
                    .ToDictionary(x => x.Key, x => x.FirstOrDefault())
                : new Dictionary<Guid, QComment>();

            QuestionRepeater.DataSource = attempt.GetQuestions().Select(q => new
            {
                Attempt = attempt,
                AttemptQuestion = q,
                BankQuestion = attempt.Bank.FindQuestion(q.QuestionIdentifier),
                Comment = comments.GetOrDefault(q.QuestionIdentifier)
            });
        }

        private void QuestionRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var dataItem = e.Item.DataItem;
            var output = (ResultQuestionOutput)e.Item.FindControl("QuestionOutput");
            var attempt = (AttemptInfo)DataBinder.Eval(dataItem, "Attempt");
            var question = (QAttemptQuestion)DataBinder.Eval(dataItem, "AttemptQuestion");

            var commentPanel = e.Item.FindControl("AssessorCommentPanel");
            var comment = (QComment)DataBinder.Eval(dataItem, "Comment");

            if (comment != null && comment.CommentText.IsNotEmpty() && IsFormThirdPartyEnabled)
            {
                var commentOutput = (ITextControl)e.Item.FindControl("AssessorCommentOutput");
                commentOutput.Text = comment.CommentText;
                commentPanel.Visible = true;
            }

            output.LoadData(attempt, question);

            BindQuestionFooter(e.Item, attempt, question);
        }

        private void BindModelToButtons(AttemptResultModel result)
        {
            var buttons = new List<ButtonInfo>();

            CoreLoadButtons(buttons, result);

            CommandRepeater.DataSource = CommandRepeaterBottom.DataSource = buttons;
            CommandRepeater.DataBind();

            CommandRepeaterBottom.DataBind();
        }

        private void BindQuestionFooter(RepeaterItem item, AttemptInfo attempt, QAttemptQuestion question)
        {
            var panel = (HtmlGenericControl)item.FindControl("FooterPanel");

            var lang = attempt.Attempt.AttemptLanguage;
            var publication = attempt.BankForm.Publication;
            var showCorrectRationale = publication.AllowRationaleForCorrectAnswers;
            var showIncorrectRationale = publication.AllowRationaleForIncorrectAnswers;

            var type = question.QuestionType.ToEnum<QuestionItemType>();
            var hasFeedbackPoints = type.IsComposed() && question.AnswerPoints.HasValue;
            var bankQuestion = attempt.Bank.FindQuestion(question.QuestionIdentifier);

            var rationaleText = bankQuestion?.Content.Rationale?.Get(lang);
            var hasRationale = rationaleText.IsNotEmpty();
            if (hasRationale)
                rationaleText = Markdown.ToHtml(rationaleText);

            var rationaleForCorrect = bankQuestion?.Content.RationaleOnCorrectAnswer?.Get(lang);
            var hasRationaleForCorrect = rationaleForCorrect.IsNotEmpty() && showCorrectRationale && question.AnswerPoints.HasValue && question.AnswerPoints.Value > 0;
            if (hasRationaleForCorrect)
                rationaleForCorrect = Markdown.ToHtml(rationaleForCorrect);

            var rationaleForIncorrect = bankQuestion?.Content.RationaleOnIncorrectAnswer?.Get(lang);
            var hasRationaleForIncorrect = rationaleForIncorrect.IsNotEmpty() && showIncorrectRationale && question.AnswerPoints.HasValue && question.AnswerPoints.Value == 0;
            if (hasRationaleForIncorrect)
                rationaleForIncorrect = Markdown.ToHtml(rationaleForIncorrect);

            var isVisible = hasRationale || hasRationaleForCorrect || hasRationaleForIncorrect || hasFeedbackPoints;
            panel.Visible = isVisible;

            if (!isVisible)
                return;

            var html = new StringBuilder();

            if (hasRationale)
                html.Append("<div class=\"h6 mb-1\">").Append(Translate("Rationale:")).Append("</div>").Append(rationaleText);

            if (hasRationaleForCorrect)
                html.Append("<div class=\"h6 mb-1\">").Append(Translate("Rationale:")).Append("</div>").Append(rationaleForCorrect);

            if (hasRationaleForIncorrect)
                html.Append("<div class=\"h6 mb-1\">").Append(Translate("Rationale:")).Append("</div>").Append(rationaleForIncorrect);

            if (hasFeedbackPoints)
                html.Append("<div class=\"h6 mb-1\">").Append(Translate("Rationale:")).Append("</div>")
                    .AppendFormat("<p>{0:n2} / {1:n2}</p>", question.AnswerPoints, question.QuestionPoints);

            panel.InnerHtml = html.ToString();
        }

        private void CoreLoadButtons(List<ButtonInfo> buttons, AttemptResultModel result)
        {
            Core2LoadButtons(buttons, result);
        }

        private bool Core2LoadButtons(List<ButtonInfo> buttons, AttemptResultModel result)
        {
            var course = CourseSearch.BindActivityFirst(
                x => new { Activity = x, x.Module.Unit.Course },
                x => x.AssessmentFormIdentifier == FormId);

            if (course == null || TGroupPermissionSearch.IsAccessDenied(course.Course.CourseIdentifier, CurrentSessionState.Identity))
                return false;

            var courseUrl = CmdsCourseUrl();

            if (courseUrl == null)
            {
                if (TGroupPermissionSearch.IsAccessDenied(course.Activity.ActivityIdentifier, CurrentSessionState.Identity))
                    return false;

                courseUrl = RoutingConfiguration.PortalCourseUrl(course.Course.CourseIdentifier, course.Activity.ActivityIdentifier);
            }

            buttons.Add(new ButtonInfo("fas fa-sitemap", "Return to Course Outline", courseUrl, ButtonStyle.Default));

            return true;

            string CmdsCourseUrl()
            {
                if (result.Attempt == null)
                    return null;

                var form = ServiceLocator.BankSearch.GetForm(result.Attempt.FormIdentifier);
                if (!ServiceLocator.Partition.IsE03())
                    return null;

                if (course.Course.CourseLabel != "Orientation")
                    return null;

                var page = ServiceLocator.PageSearch
                    .Bind(x => x.PageIdentifier, x => x.ObjectType == "Course" && x.ObjectIdentifier == course.Course.CourseIdentifier)
                    .FirstOrDefault();

                return page == Guid.Empty
                    ? null
                    : ServiceLocator.PageSearch.GetPagePath(page, false) + "?tab=certificates";
            }
        }

        #endregion

        #region Methods (helpers)

        private string GetPageLearnerTitle(Guid learnerID)
        {
            var learner = ServiceLocator.ContactSearch.GetPerson(learnerID, Organization.OrganizationIdentifier);

            if (learner == null)
                return null;

            return $"Learner: {learner.UserFullName} ({learner.PersonCode})";
        }

        protected string GetTagsAndLabels()
        {
            var question = (Question)DataBinder.Eval(Page.GetDataItem(), "BankQuestion");
            if (question == null)
                return null;

            var environment = ServiceLocator.AppSettings.Environment.Name;
            if (environment == EnvironmentName.Production || environment == EnvironmentName.Sandbox)
                return null;

            return question.Classification?.Tag.NullIfEmpty();
        }

        #endregion
    }
}
