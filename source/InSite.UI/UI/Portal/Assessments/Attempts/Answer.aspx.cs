using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI.WebControls;

using Humanizer;

using InSite.Admin.Assets.Glossaries.Utilities;
using InSite.Common.Web;
using InSite.Domain.Attempts;
using InSite.Domain.Banks;
using InSite.Persistence;
using InSite.UI.Layout.Portal;
using InSite.UI.Portal.Assessments.Attempts.Utilities;

using Shift.Common;
using Shift.Sdk.UI;

namespace InSite.Portal.Assessments.Attempts
{
    public partial class Answer : PortalBasePage, IOverrideWebRouteParent, IHasParentLinkParameters
    {
        #region Classes

        private class UploadInfo
        {
            public Guid UploadIdentifier { get; set; }
            public string Name { get; set; }
            public string NavigateUrl { get; set; }
            public int? ContentSize { get; set; }
            public string ContentType { get; set; }
        }

        #endregion

        #region Fields

        private AnswerLoader _loader;
        private AnswerLoadData _loadData;

        #endregion

        #region Initialization and loading

        protected override void OnInit(EventArgs e)
        {
            _loader = new AnswerLoader(this);

            HandleAjaxRequest();

            base.OnInit(e);

            ViewAttachmentsLink.Attributes["href"] = "#addendum";

            ViewAcronymsLink.Attributes["href"] = "#acronyms";
            ViewAcronymsLink.Attributes["target"] = "_blank";

            ViewFormulasLink.Attributes["href"] = "#formulas";
            ViewFormulasLink.Attributes["target"] = "_blank";
        }

        private void HandleAjaxRequest()
        {
            if (!HttpRequestHelper.IsAjaxRequest)
                return;

            if (Identity.IsImpersonating)
                HttpResponseHelper.SendHttp403(false);

            var action = _loader.Request.Form["action"];
            var (isHandled, content) = AnswerActions.HandleAction(action, _loader);

            if (!isHandled)
                content = "ERROR";

            Response.Clear();

            if (content.IsNotEmpty())
            {
                Response.ContentType = action == "upload"
                    ? "application/json; charset=utf-8"
                    : "text/plain";
                Response.Write(content);
            }
            else
            {
                Response.StatusCode = (int)HttpStatusCode.NoContent;
            }

            Response.End();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                PortalMaster.SidebarVisible(false);
                PortalMaster.HideBreadcrumbsAndTitle();
            }

            AppRelativeTemplateSourceDirectory = string.Empty;

            var action = _loader.LoadStartData(out _loadData);

            if (action == null)
            {
                SetInputValues();

                if (_loadData.ContentStyle.IsNotEmpty())
                    SetStyle(_loadData.ContentStyle);
            }
            else
            {
                MainPanel.Visible = false;
                CommandsPanel.Visible = false;

                action.Execute(AlertMessage, Notification);
            }
        }

        protected override void OnPreRenderComplete(EventArgs e)
        {
            SetInitializationScript();

            base.OnPreRenderComplete(e);
        }

        #endregion

        #region Load data

        private void SetStyle(string style)
        {
            var hasStyle = style.IsNotEmpty();
            ContentStyle.Visible = hasStyle;
            ContentStyle.Text = hasStyle ? $"<style type=\"text/css\">{style}</style>" : null;
        }

        private void SetInputValues()
        {
            var bankForm = _loadData.BankForm;
            var attempt = _loadData.Attempt;

            QuestionNav.Visible = false;
            QuestionList.Visible = false;
            KioskMode.Visible = bankForm.Invigilation.IsKioskModeRequired;

            var bankTitle = bankForm.Content.Title?[CurrentSessionState.Identity.Language] ?? bankForm.Content.Title?.Default;

            PageTitle.InnerText = bankTitle;
            if (bankForm.ThirdPartyAssessmentIsEnabled && attempt.AssessorUserIdentifier != attempt.LearnerUserIdentifier)
                PageLearner.Text = GetPageLearnerTitle(attempt.LearnerUserIdentifier);

            var (acronyms, formulas, documents) = GetImages(bankForm);

            ViewAttachmentsLink.Visible = documents.IsNotEmpty();

            ViewAcronymsLink.Visible = acronyms != null;
            ViewAcronymsLink.HRef = acronyms?.Url;

            ViewFormulasLink.Visible = formulas != null;
            ViewFormulasLink.HRef = formulas?.Url;

            FeedbackLink.Visible = bankForm.Publication.AllowFeedback;

            AddendumDialogPanel.Visible = documents.IsNotEmpty();

            var glossary = new GlossaryHelper(CurrentSessionState.Identity.Language);
            var attemptInfo = new AttemptInfo(bankForm, attempt, glossary);

            if (attempt.SectionsAsTabsEnabled)
            {
                QuestionNav.Visible = true;
                QuestionNav.LoadData(attemptInfo, !attempt.TabNavigationEnabled && attempt.SingleQuestionPerTabEnabled);
            }
            else
            {
                QuestionList.Visible = true;
                QuestionList.LoadData(attemptInfo);
            }

            AddendumDialogPanel.Visible = documents.IsNotEmpty();

            if (documents.IsNotEmpty())
            {
                DocumentRepeater.DataSource = documents;
                DocumentRepeater.DataBind();
            }

            FooterScript.Visible = true;

            _loadData.GlossaryTermsScript = glossary.GetJavaScriptDictionary().IfNullOrEmpty("null");
            _loadData.LastQuestionIndex = attemptInfo.GetAllQuestions().Count - 1;
        }

        private void SetInitializationScript()
        {
            if (_loadData == null)
                return;

            var url = _loadData.Url;
            var bankForm = _loadData.BankForm;
            var attempt = _loadData.Attempt;
            var sessionId = RandomStringGenerator.Create(4);

            AnswerSectionsContainer sections = null;
            int questionNumber = -1, questionCount = -1;

            if (attempt.SectionsAsTabsEnabled && !attempt.TabNavigationEnabled)
            {
                sections = new AnswerSectionsContainer();

                for (var i = 0; i < QuestionNav.NavItemsCount; i++)
                {
                    var section = QuestionNav.GetSectionData(i);
                    sections.Add(section.Index, section.Html);
                }

                if (attempt.ActiveSectionIndex.HasValue)
                    QuestionNav.SetActiveSection(attempt.ActiveSectionIndex.Value);

                QuestionNav.HideInactive();

                if (attempt.SingleQuestionPerTabEnabled)
                {
                    FeedbackLink.Visible = false;

                    questionNumber = 1;
                    questionCount = QuestionNav.GetQuestionCount();

                    if (attempt.ActiveQuestionIndex.HasValue)
                    {
                        var index = attempt.ActiveQuestionIndex.Value;
                        QuestionNav.SetActiveQuestion(index);
                        questionNumber = QuestionNav.GetQuestionNumber(index);
                    }
                }

                sections.Save(attempt.AttemptIdentifier, sessionId);
            }

            var examShowTimer = bankForm.Invigilation.IsTimerVisible ? "true" : "false";
            var images = AttemptImageInfo.CreateDictionary(bankForm.Specification.Bank.EnumerateAllAttachments());
            var pingInterval = attempt.AttemptPingInterval ?? AttemptConfiguration.DefaultPingInterval;

            InitFieldScript.Text = $@"
<script type=""text/javascript"">
    const _accessTimerCount = {pingInterval * 2};
    const _examShowTimer = {examShowTimer};
    const _pingInterval = {pingInterval};
    const _attemptId = '{attempt.AttemptIdentifier}';
    const _sessionId = '{sessionId}';
    const _imageUrlPattern = '{AttemptImageInfo.ImageUrlPattern}';
    const _images = {JsonHelper.SerializeJsObject(images)};
    const _termsData = {_loadData.GlossaryTermsScript};
    const _navId = '{QuestionNav.NavClientID}';
    const _tabsEnabled = {attempt.SectionsAsTabsEnabled.ToString().ToLower()};
    const _tabsNavigationEnabled = {attempt.TabNavigationEnabled.ToString().ToLower()};
    const _tabsSingleQuestionEnabled = {attempt.SingleQuestionPerTabEnabled.ToString().ToLower()};
    const _sectionIndex = {attempt.ActiveSectionIndex?.ToString() ?? "null"};
    const _lastSectionIndex = {(!attempt.FormSectionsCount.HasValue ? "null" : (attempt.FormSectionsCount.Value - 1).ToString())};
    const _questionIndex = {attempt.ActiveQuestionIndex?.ToString() ?? "null"};
    const _lastQuestionIndex = {_loadData.LastQuestionIndex};
    const _questionNumber = {questionNumber};
    const _questionCount = {questionCount};
    const _answerStrings = {{
        bookmark: {{
            removeConfirm: {TranslateAndJsEncode("Are you sure you want to remove this bookmark?")},
            goToQuestion: {TranslateAndJsEncode("Go to bookmarked question")},
            remove: {TranslateAndJsEncode("Remove Bookmark")}
        }},
        attempt: {{
            confirmSome: {TranslateAndJsEncode("You have answered $1 out of $2 questions.")},
            confirmAll: {TranslateAndJsEncode("You have answered all questions.")},
            confirmQuestion: {TranslateAndJsEncode("Are you ready to submit?")},
            confirmReminder: {TranslateAndJsEncode("Please remember, once submitted, you cannot change any of your answers.")}
        }},
        nextSection: {{
            confirmSome: {TranslateAndJsEncode("You have answered $1 out of $2 questions.")},
            confirmAll: {TranslateAndJsEncode("You have answered all questions.")},
            confirmQuestion: {TranslateAndJsEncode("Are you ready to go to the next page?")},
            confirmReminder: {TranslateAndJsEncode("Please remember, after you move on to the next page, you will not be able to go back and change any of your answers.")}
        }},
        nextQuestion: {{
            confirmNotAnswered: {TranslateAndJsEncode("You have not answered the question.")},
            confirmQuestion: {TranslateAndJsEncode("Are you ready to go to the next page?")},
            confirmReminder: {TranslateAndJsEncode("Please remember, after you move on to the next page, you will not be able to go back and change any of your answers.")}
        }},
        endBreak: {{
            confirmSome: {TranslateAndJsEncode("You have answered $1 out of $2 questions.")},
            confirmAll: {TranslateAndJsEncode("You have answered all questions.")},
            confirmQuestion: {TranslateAndJsEncode("Are you ready to go to the next page?")},
            confirmReminder: {TranslateAndJsEncode("Please remember, after you move on to the next page, you will not be able to go back and change any of your answers.")}
        }},
        singleQuestionCount: {TranslateAndJsEncode("Question $1 of $2")},
        composedVoice: {{
            removeConfirm: {TranslateAndJsEncode("Are you sure you want to re-record your answer?")}
        }},
    }};
</script>
";
            string TranslateAndJsEncode(string text)
            {
                return HttpUtility.JavaScriptStringEncode(Translate(text), true);
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

        private (AttemptAttachmentInfo Acronyms, AttemptAttachmentInfo formulas, List<AttemptAttachmentInfo> documents) GetImages(Form bankForm)
        {
            if (bankForm.Addendum.IsEmpty)
                return (null, null, null);

            var bankAttachments = bankForm.Specification.Bank.EnumerateAllAttachments()
                .ToDictionary(x => (x.Asset, x.AssetVersion), x => x);
            var formAcronyms = GetAttachments(bankAttachments, bankForm.Addendum.Acronyms);
            var formFormulas = GetAttachments(bankAttachments, bankForm.Addendum.Formulas);
            var formFigures = GetAttachments(bankAttachments, bankForm.Addendum.Figures);
            var uploads = GetUploads(formAcronyms, formFormulas, formFigures);

            AttemptAttachmentInfo acronyms = null;
            AttemptAttachmentInfo formulas = null;
            var documents = new List<AttemptAttachmentInfo>(); ;

            AttachmentLoop(uploads, formAcronyms, (attachment, upload) =>
            {
                if (FileExtension.IsImage(upload.ContentType))
                {
                    acronyms = CreateInfo(attachment, upload, "file");
                    return false;
                }

                return true;
            });

            AttachmentLoop(uploads, formFormulas, (attachment, upload) =>
            {
                if (FileExtension.IsImage(upload.ContentType))
                {
                    formulas = CreateInfo(attachment, upload, "file");
                    return false;
                }

                return true;
            });

            AttachmentLoop(uploads, formFigures, (attachment, upload) =>
            {
                var isHtml = upload.ContentType.Equals(".html", StringComparison.OrdinalIgnoreCase)
                    || upload.ContentType.Equals(".htm", StringComparison.OrdinalIgnoreCase);
                var isPdf = upload.ContentType.Equals(".pdf", StringComparison.OrdinalIgnoreCase);
                var isTxt = upload.ContentType.Equals(".txt", StringComparison.OrdinalIgnoreCase);

                if (isHtml || isPdf || isTxt)
                {
                    var icon = isHtml ? "file-code" : isPdf ? "file-pdf" : "file-alt";
                    documents.Add(CreateInfo(attachment, upload, icon));
                }

                return true;
            });

            return (acronyms, formulas, documents);
        }

        private static Dictionary<Guid, UploadInfo> GetUploads(Attachment[] formAcronyms, Attachment[] formFormulas, Attachment[] formFigures)
        {
            var uploadsFilter = formAcronyms.Concat(formFormulas).Concat(formFigures)
                            .Select(x => x.Upload).Distinct().ToArray();
            var uploads = UploadSearch
                .Bind(
                    x => new UploadInfo
                    {
                        UploadIdentifier = x.UploadIdentifier,
                        Name = x.Name,
                        NavigateUrl = x.NavigateUrl,
                        ContentSize = x.ContentSize,
                        ContentType = x.ContentType
                    },
                    x => uploadsFilter.Contains(x.UploadIdentifier))
                .ToDictionary(x => x.UploadIdentifier);
            return uploads;
        }

        private static Attachment[] GetAttachments(Dictionary<(int Asset, int AssetVersion), Attachment> bankAttachments, IEnumerable<FormAddendumItem> items)
        {
            return items.Select(x => (x.Asset, x.Version))
                .Where(x => bankAttachments.ContainsKey(x))
                .Select(x => bankAttachments[x])
                .ToArray();
        }

        private static void AttachmentLoop(Dictionary<Guid, UploadInfo> uploads, IEnumerable<Attachment> attachments, Func<Attachment, UploadInfo, bool> action)
        {
            foreach (var attachment in attachments)
            {
                if (!uploads.ContainsKey(attachment.Upload))
                    continue;

                var upload = uploads[attachment.Upload];
                if (upload.ContentType.IsEmpty())
                    continue;

                if (!action(attachment, upload))
                    break;
            }
        }

        private static AttemptAttachmentInfo CreateInfo(Attachment attachment, UploadInfo upload, string icon)
        {
            return new AttemptAttachmentInfo
            {
                Title = (attachment.Content?.Title.Default).IfNullOrEmpty(upload.Name),
                Url = "/files" + upload.NavigateUrl,
                Size = upload.ContentSize.HasValue ? upload.ContentSize.Value.Bytes().Humanize("0.##") : string.Empty,
                Icon = icon
            };
        }

        #endregion

        #region Route Hierarchy

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
    }
}
