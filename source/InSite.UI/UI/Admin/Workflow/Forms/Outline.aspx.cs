using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using Humanizer;

using InSite.Admin.Workflow.Forms.Controls;
using InSite.Application.Surveys.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Surveys.Forms;
using InSite.Persistence;
using InSite.UI.Layout.Admin;
using InSite.UI.Portal.Workflow.Forms.Models;

using Shift.Common;
using Shift.Constant;

using AggregateOutline = InSite.Admin.Logs.Aggregates.Outline;
using SurveyForm = InSite.Domain.Surveys.Forms.SurveyForm;

namespace InSite.Admin.Workflow.Forms
{
    public class OutlineBranch
    {
        public Guid QuestionIdentifier { get; set; }
        public int QuestionSequence { get; set; }
        public string QuestionCode { get; set; }
        public string QuestionTitle { get; set; }

        public Guid OptionListIdentifier { get; set; }
        public int OptionListSequence { get; set; }
        public string OptionListTitle { get; set; }

        public Guid OptionItemIdentifier { get; set; }
        public int OptionItemSequence { get; set; }
        public string OptionItemLetter { get; set; }
        public string OptionItemTitle { get; set; }

        public string BranchToQuestionCode { get; set; }
        public string BranchToQuestionTitle { get; set; }
        public int BranchToQuestionSequence { get; set; }
        public int BranchToPageNumber { get; set; }
        public bool IsNotFirstQuestion { get; set; }
        public bool IsNotFollowingPage { get; set; }
        public bool SingleList { get; set; }
    }

    public class OutlineCondition
    {
        public int MaskingQuestionSequence { get; internal set; }
        public string MaskingQuestionCode { get; internal set; }
        public string MaskingQuestionTitle { get; internal set; }

        public int MaskingListSequence { get; internal set; }
        public string MaskingListTitle { get; internal set; }
        public bool ShowList { get; internal set; }

        public Guid MaskingOptionIdentifier { get; internal set; }
        public string MaskingOptionTitle { get; internal set; }

        public Guid MaskedQuestionIdentifier { get; internal set; }
        public int MaskedQuestionSequence { get; internal set; }
        public string MaskedQuestionCode { get; internal set; }
        public string MaskedQuestionTitle { get; internal set; }
    }

    public class OutlineScaleDto
    {
        public string Category { get; set; }
        public string Question { get; set; }
        public string Rows { get; set; }
        public string Size { get; set; }
    }

    public partial class Outline : AdminBasePage
    {
        private int? PageNumber => int.TryParse(Request.QueryString["page"], out var value) ? value : (int?)null;

        protected Guid? SurveyID => Guid.TryParse(Request.QueryString["form"], out var value) ? value : (Guid?)null;

        new private bool CanCreate { get; set; }
        new protected bool CanEdit { get; set; }
        new private bool CanDelete { get; set; }
        private bool CanLock { get; set; }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ConditionsRepeater.ItemDataBound += ConditionsRepeater_ItemDataBound;
            BranchesRepeater.ItemDataBound += BranchesRepeater_ItemDataBound;

            QuestionsManager.Copied += QuestionsManager_Copied;

            PageHelper.AutoBindHeader(this);
        }

        protected override void CreateChildControls()
        {
            if (ContentTabs.ItemsCount == 0)
            {
                var (nonInstructionList, instructionList) = SplitLabels();

                for (var i = 0; i < nonInstructionList.Count; i++)
                    AddContentNavItem(out _, out _);

                AddContentNavItem(out _, out _);
            }

            base.CreateChildControls();
        }

        protected override void OnLoad(EventArgs e)
        {
            EnsureChildControls();

            base.OnLoad(e);

            if (IsPostBack)
                return;

            LoadData();
            SetupActivePanel();
        }

        private void SetupActivePanel()
        {
            var activeTab = Request.QueryString["tab"];
            var activePanel = Request.QueryString["panel"];

            if (activePanel == "messages")
            {
                NotificationsButton.IsSelected = true;

                if (activeTab == "Invitation")
                    InvitationTab.IsSelected = true;
                else if (activeTab == "ResponseStartedAdministrator")
                    ResponseStartedAdministratorTab.IsSelected = true;
                else if (activeTab == "ResponseCompletedAdministrator")
                    ResponseCompletedAdministratorTab.IsSelected = true;
                else if (activeTab == "ResponseCompletedRespondent")
                    ResponseCompletedRespondentTab.IsSelected = true;
            }
            else if (activePanel == "notes")
            {
                AdminNotesTab.IsSelected = true;
            }
        }

        private void LoadData()
        {
            var survey = SurveyID.HasValue
                ? ServiceLocator.SurveySearch.GetSurveyState(SurveyID.Value)
                : null;

            if (survey == null || survey.Form.Tenant != Organization.OrganizationIdentifier)
            {
                HttpResponseHelper.Redirect("/ui/admin/workflow/forms/search");
                return;
            }

            var hasWrite = CurrentSessionState.Identity.IsGranted(Route.ToolkitName, PermissionOperation.Write);

            CanCreate = hasWrite;
            CanEdit = survey.Form.Locked == null && hasWrite;
            CanDelete = survey.Form.Locked == null && CurrentSessionState.Identity.IsGranted(Route.ToolkitName, PermissionOperation.Delete);
            CanLock = hasWrite;

            SurveyTitle.Text = (survey.Form.Content?.Title?.GetText(survey.Form.Language)).IfNullOrEmpty("None");

            CheckDuplicates(survey);
            CheckValidity(survey);
            CheckPageBreaks(survey);

            PageHelper.BindTitle(Page, survey.Form.Name);

            InternalName.Text = survey.Form.Name;
            WarningText.Visible = survey.Form.RequireUserIdentification;
            SurveyHook.Text = string.IsNullOrEmpty(survey.Form.Hook) ? "None" : survey.Form.Hook;

            SurveyLockStatus.Text = survey.Form.Locked.HasValue
                ? $"<span class='text-danger'>Locked</span> on {survey.Form.Locked.Format(User.TimeZone, true)}"
                : $"<span class='text-success'>Unlocked</span>";

            CurrentStatus.Text = survey.Form.Status.ToString();
            Opened.Text = survey.Form.Opened.HasValue ? survey.Form.Opened.Value.Format(User.TimeZone, true) : "None";
            Closed.Text = survey.Form.Closed.HasValue ? survey.Form.Closed.Value.Format(User.TimeZone, true) : "None";
            DurationMinutes.Text = survey.Form.DurationMinutes.HasValue
                ? TimeSpan.FromMinutes(survey.Form.DurationMinutes.Value).Humanize(precision: 2)
                : "Not Specified";

            UserFeedback.Text = survey.Form.UserFeedback.GetDescription();

            IsLimitResponses.Text = survey.Form.ResponseLimitPerUser.HasValue
                ? "Limited"
                : "Not Limited";
            AllowAnonymousResponsesField.Visible = !survey.Form.ResponseLimitPerUser.HasValue;
            if (AllowAnonymousResponsesField.Visible)
            {
                AllowAnonymousResponses.Text = !survey.Form.RequireUserIdentification
                    ? "Permitted"
                    : "Not Permitted";
            }

            EnableUserConfidentiality.Text = survey.Form.EnableUserConfidentiality ? "Enabled" : "Disabled";
            DisplaySummaryChart.Text = survey.Form.DisplaySummaryChart ? "Display" : "Not Display";
            IssueWorkflowLink.NavigateUrl = $"/ui/admin/workflow/forms/configure-workflow?form={SurveyID}";
            IssueWorkflowText.Text = survey.WorkflowConfiguration == null ? "Disabled" : "Enabled";

            var responseCount = ServiceLocator.SurveySearch.CountResponseSessions(
                new QResponseSessionFilter
                {
                    OrganizationIdentifier = Organization.OrganizationIdentifier,
                    SurveyFormIdentifier = SurveyID
                }
            );

            ResponseCount.Text = "form submission".ToQuantity(responseCount) + " started";

            BindConditionsRepeater(survey.Form);
            BindBranchRepeater(survey.Form);

            {
                var panel = Request.QueryString["panel"];
                var isQuestionsPanel = panel == "questions";

                if (!isQuestionsPanel || !Guid.TryParse(Request.QueryString["question"], out var questionId))
                    questionId = Guid.Empty;

                QuestionsPanel.IsSelected = isQuestionsPanel;
                QuestionsManager.LoadData(survey.Form, PageNumber, questionId, CanEdit);
                if (survey.Form.Status == SurveyFormStatus.Opened && responseCount > 0)
                    QuestionsLock.AddMessage(AlertType.Warning, "Changing questions and answer options on an open form may cause issues for users currently completing it. Please proceed with caution.");

                ContentTabs.ClearItems();

                ContentButton.IsSelected = panel == "content";

                var (nonInstructionList, instructionList) = SplitLabels();

                AddNonInstructionTabs(survey.Form, panel, nonInstructionList);
                AddInstructionTabs(survey.Form, panel, instructionList);
            }

            {
                var messages = survey.Form.Messages;

                var messageID = messages.FirstOrDefault(x => x.Type == SurveyMessageType.Invitation)?.Identifier;
                var invitation = messageID.HasValue ? ServiceLocator.MessageSearch.GetMessage(messageID.Value) : null;
                InvitationMessageDetails.SetInputValues(invitation, true, SurveyID.Value, CanEdit);

                messageID = messages.FirstOrDefault(x => x.Type == SurveyMessageType.ResponseStarted)?.Identifier;
                var responseStarted = messageID.HasValue ? ServiceLocator.MessageSearch.GetMessage(messageID.Value) : null;
                ResponseStartedMessageDetails.SetInputValues(responseStarted, false, SurveyID.Value, CanEdit, true, SurveyMessageType.ResponseStarted);

                messageID = messages.FirstOrDefault(x => x.Type == SurveyMessageType.ResponseCompleted)?.Identifier;
                var responseCompleted = messageID.HasValue ? ServiceLocator.MessageSearch.GetMessage(messageID.Value) : null;
                ResponseCompletedMessageDetails.SetInputValues(responseCompleted, false, SurveyID.Value, CanEdit, true, SurveyMessageType.ResponseCompleted);

                messageID = messages.FirstOrDefault(x => x.Type == SurveyMessageType.ResponseConfirmed)?.Identifier;
                var responseConfirmed = messageID.HasValue ? ServiceLocator.MessageSearch.GetMessage(messageID.Value) : null;
                ResponseCompletedRespondentMessageDetails.SetInputValues(responseConfirmed, false, SurveyID.Value, CanEdit, true, SurveyMessageType.ResponseConfirmed);
            }

            InitTranslations(survey.Form);

            LikertScaleRepeater.DataSource = CreateDataSourceForLikertScaleRepeater(survey.Form);
            LikertScaleRepeater.DataBind();

            SetupLinks(survey);
            EnableControls(survey, responseCount);

            CommentRepeater.LoadData(survey.Form.Identifier);
        }

        private void AddNonInstructionTabs(SurveyForm form, string panel, List<SurveyContentLabel> labels)
        {
            var selectedTab = panel == "content"
                ? Request.QueryString["tab"].NullIfWhiteSpace()
                : null;

            foreach (var label in labels)
            {
                if (label.Name == "Title")
                    continue;

                AddContentNavItem(out var navItem, out var container);

                navItem.Title = label.Name;

                if (label.Name == selectedTab)
                    navItem.IsSelected = true;

                var field = (OutlineContentField)container.LoadControl("~/UI/Admin/Workflow/Forms/Controls/OutlineContentField.ascx");
                field.LoadData(form, label, CanEdit);
            }
        }

        private void AddInstructionTabs(SurveyForm form, string panel, List<SurveyContentLabel> labels)
        {
            var selectedTab = panel == "content"
                ? Request.QueryString["tab"].NullIfWhiteSpace()
                : null;

            AddContentNavItem(out var navItem, out var container);

            navItem.Title = "Instructions";

            if (selectedTab == "Instructions")
                navItem.IsSelected = true;

            var field = (OutlineContentFieldList)container.LoadControl("~/UI/Admin/Workflow/Forms/Controls/OutlineContentFieldList.ascx");
            field.LoadData(form, labels, CanEdit);
        }

        private (List<SurveyContentLabel> nonInstructionList, List<SurveyContentLabel> instructionList) SplitLabels()
        {
            var nonInstructionList = new List<SurveyContentLabel>();
            var instructionList = new List<SurveyContentLabel>();

            foreach (var label in SurveyForm.ContentLabels)
            {
                if (label.Name == "Title")
                    continue;

                if (label.Name.EndsWith("Instructions"))
                    instructionList.Add(label);
                else
                    nonInstructionList.Add(label);
            }

            return (nonInstructionList, instructionList);
        }

        private void SetupLinks(SurveyState survey)
        {
            var respondent = UserSearch.Select(User.UserIdentifier);

            var url = survey.Form.RequireUserIdentification
                ? FormUrl.GetStartUrl(survey.Form.Asset, respondent.UserIdentifier)
                : FormUrl.GetStartUrl(survey.Form.Asset);

            TestSurveyLink.NavigateUrl = url;
            TestSurveyLink.Text = url;

            LockLink.NavigateUrl = $"/ui/admin/workflow/forms/lock?form={SurveyID}";

            UnlockLink.NavigateUrl = $"/ui/admin/workflow/forms/unlock?form={SurveyID}";

            ResponseSearchLink.NavigateUrl = $"/ui/admin/workflow/forms/submissions/search?form={SurveyID}";
            SurveyReportLink.NavigateUrl = $"/ui/admin/workflow/forms/report?form={SurveyID}";

            DeleteResponseLink.NavigateUrl = $"/ui/admin/workflow/forms/submissions/delete-many?form={SurveyID}";

            AddConditions.NavigateUrl = $"/ui/admin/workflow/forms/conditions/add?form={SurveyID}&returnpanel=form&returntab=Conditions";

            NewSurveyLink.NavigateUrl = $"/ui/admin/workflow/forms/create";
            DuplicateLink.NavigateUrl = $"/ui/admin/workflow/forms/create?action=duplicate&form={SurveyID}";
            TranslateSurveyLink.NavigateUrl = $"/ui/admin/workflow/forms/translate?form={SurveyID}";
            VoidSurveyLink.NavigateUrl = $"/ui/admin/workflow/forms/delete?form={SurveyID}";

            ChangeInternalName.NavigateUrl = $"/ui/admin/workflow/forms/rename?form={SurveyID}";
            ChangeHook.NavigateUrl = $"/ui/admin/workflow/forms/rename?form={SurveyID}";
            ChangeSurveyTitle.NavigateUrl = $"/ui/admin/workflow/forms/change-content?form={SurveyID}&tab=Title";
            ChangeSurveyStatus.NavigateUrl = $"/ui/admin/workflow/forms/change-status?form={SurveyID}";
            ChangeConfiguration1.NavigateUrl =
            ChangeConfiguration2.NavigateUrl =
            ChangeConfiguration3.NavigateUrl =
            ChangeConfiguration4.NavigateUrl =
            ChangeConfiguration5.NavigateUrl =
            ChangeConfiguration6.NavigateUrl = $"/ui/admin/workflow/forms/change-settings?form={SurveyID}";
            ViewHistoryLink.NavigateUrl = AggregateOutline.GetUrl(SurveyID.Value, $"/ui/admin/workflow/forms/outline?form={SurveyID}");
            DownloadSurveyLink.NavigateUrl = $"/ui/admin/workflow/forms/download?form={SurveyID}";
        }

        private void EnableControls(SurveyState survey, int responseCount)
        {
            LockLink.Visible = CanLock && survey.Form.Locked == null;
            UnlockLink.Visible = CanLock && survey.Form.Locked.HasValue;
            DeleteResponseLink.Visible = CanEdit && responseCount > 0;

            DuplicateLink.Visible = CanCreate;
            TranslateSurveyLink.Visible = CanEdit;
            ChangeInternalName.Visible = CanEdit;
            ChangeSurveyTitle.Visible = CanEdit;
            VoidSurveyLink.Visible = CanDelete;
            ChangeSurveyStatus.Visible = CanEdit;
            ChangeConfiguration1.Visible = CanEdit;
            ChangeConfiguration2.Visible = CanEdit;
            ChangeConfiguration3.Visible = CanEdit;
            ChangeConfiguration4.Visible = CanEdit;
            ChangeConfiguration5.Visible = CanEdit;
            ChangeConfiguration6.Visible = CanEdit;
            AddConditions.Visible = CanEdit;
            ChangeHook.Visible = CanEdit;
        }

        private List<OutlineScaleDto> CreateDataSourceForLikertScaleRepeater(SurveyForm form)
        {
            var list = new List<OutlineScaleDto>();

            var questions = form.Questions.Where(x => x.Type == SurveyQuestionType.Likert).ToList();
            foreach (var q in questions)
            {
                foreach (var scale in q.Scales)
                {
                    var item = new OutlineScaleDto
                    {
                        Category = scale.Category,
                        Rows = "row".ToQuantity(q.Options.Lists.Count(x => x.Category == scale.Category)),
                        Size = "item".ToQuantity(scale.Items.Count)
                    };

                    var indicator = q.Code.HasValue() ? q.Code : q.Sequence.ToString();
                    var indicatorStyle = q.GetIndicatorStyleName();
                    item.Question = $"<span class='badge bg-{indicatorStyle}'>{indicator}</span>";

                    list.Add(item);
                }
            }

            return list.OrderBy(x => x.Category).ThenBy(x => x.Question).ToList();
        }

        private static readonly MemoryCache<int> _cache = new MemoryCache<int>();

        private void InitTranslations(SurveyForm form)
        {
            var formLang = form.Language.IfNullOrEmpty("en");

            var formTranslations = form.LanguageTranslations != null
                ? form.LanguageTranslations
                    .Where(code => !string.IsNullOrEmpty(code) && !string.Equals(code, formLang, StringComparison.OrdinalIgnoreCase))
                    .Select(code => Language.GetName(code))
                    .OrderBy(x => x)
                    .ToArray()
                : null;

            SurveyLanguage.Text = Language.GetName(formLang);
            SurveyTranslationLanguages.Text = formTranslations.IsNotEmpty()
                ? string.Join(", ", formTranslations)
                : "None";
        }

        #region Event handlers

        private void BindConditionsRepeater(SurveyForm form)
        {
            var items = form.FlattenOptionItems().Where(x => x.MaskedQuestionIdentifiers.Count > 0).ToList();

            var conditions = new List<OutlineCondition>();
            foreach (var item in items)
            {
                foreach (var masked in item.MaskedQuestionIdentifiers)
                {
                    var condition = new OutlineCondition();

                    condition.MaskingOptionIdentifier = item.Identifier;
                    condition.MaskingOptionTitle = GetTitle(item.Identifier);
                    condition.MaskingListSequence = item.List.Sequence;
                    condition.MaskingListTitle = GetTitle(item.List.Identifier);
                    condition.ShowList = item.List.Question.Options.Lists.Count > 1;
                    condition.MaskingQuestionSequence = item.Question.Sequence;
                    condition.MaskingQuestionCode = item.Question.Code.HasValue() ? item.Question.Code : item.Question.Sequence.ToString();
                    condition.MaskingQuestionTitle = GetTitle(item.Question.Identifier);

                    var question = form.FindQuestion(masked);
                    condition.MaskedQuestionIdentifier = masked;

                    if (question != null)
                    {
                        condition.MaskedQuestionSequence = question.Sequence;
                        condition.MaskedQuestionCode = question.Code.HasValue() ? question.Code : question.Sequence.ToString();
                        condition.MaskedQuestionTitle = GetTitle(question.Identifier);
                    }

                    conditions.Add(condition);
                }
            }

            ConditionsRepeater.DataSource = conditions
                .OrderBy(x => x.MaskingQuestionSequence)
                .ThenBy(x => x.MaskingQuestionTitle)
                .ThenBy(x => x.MaskingListSequence)
                .ThenBy(x => x.MaskingListTitle)
                .ThenBy(x => x.MaskedQuestionSequence)
                .GroupBy(r => r.MaskingOptionIdentifier)
                .Select(group =>
                    {
                        var first = group.First();

                        return new
                        {
                            first.MaskingQuestionCode,
                            first.MaskingQuestionTitle,
                            first.MaskingListSequence,
                            first.MaskingListTitle,
                            first.ShowList,
                            first.MaskingOptionIdentifier,
                            first.MaskingOptionTitle,

                            MaskedQuestions = group.Select(row => new
                            {
                                row.MaskedQuestionCode,
                                row.MaskedQuestionTitle,
                                row.MaskingOptionIdentifier,
                                row.MaskedQuestionIdentifier
                            })
                        };
                    });
            ConditionsRepeater.DataBind();

            var hasConditions = conditions.Count > 0;
            ConditionsRepeater.Visible = hasConditions;
            ConditionsNoItemsMessage.Visible = !hasConditions;
        }

        private void ConditionsRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var maskedQuestionsRepeater = (Repeater)e.Item.FindControl("MaskedQuestionsRepeater");
            maskedQuestionsRepeater.DataSource = DataBinder.Eval(e.Item.DataItem, "MaskedQuestions");
            maskedQuestionsRepeater.DataBind();
        }

        private void BindBranchRepeater(SurveyForm form)
        {
            var items = form.FlattenOptionItems().Where(x => x.BranchToQuestionIdentifier.HasValue).ToList();

            var branches = new List<OutlineBranch>();
            foreach (var item in items)
            {
                var branch = new OutlineBranch();

                branch.QuestionIdentifier = item.Question.Identifier;
                branch.QuestionSequence = item.Question.Sequence;
                branch.QuestionCode = item.Question.Code.HasValue() ? item.Question.Code : item.Question.Sequence.ToString();
                branch.QuestionTitle = GetTitle(item.Question.Identifier);

                branch.OptionListIdentifier = item.List.Identifier;
                branch.OptionListSequence = item.List.Sequence;
                branch.OptionListTitle = GetTitle(item.List.Identifier);

                branch.OptionItemIdentifier = item.Identifier;
                branch.OptionItemSequence = item.Sequence;
                branch.OptionItemLetter = item.Letter;
                branch.OptionItemTitle = GetTitle(item.Identifier);

                branch.SingleList = item.Question.Options.Lists.Count == 1;

                if (item.BranchToQuestionIdentifier.HasValue)
                {
                    var branchToQuestion = form.FindQuestion(item.BranchToQuestionIdentifier.Value);
                    if (branchToQuestion != null)
                    {
                        var branchToPage = form.GetPage(item.BranchToQuestionIdentifier.Value);
                        var currentPage = form.GetPage(branch.QuestionIdentifier);

                        branch.BranchToQuestionCode = branchToQuestion.Code.HasValue() ? branchToQuestion.Code : branchToQuestion.Sequence.ToString();
                        branch.BranchToQuestionTitle = GetTitle(branchToQuestion.Identifier);
                        branch.BranchToQuestionSequence = branchToQuestion.Sequence;
                        branch.BranchToPageNumber = branchToPage.Sequence;
                        branch.IsNotFirstQuestion = branchToPage.Questions.IndexOf(branchToQuestion) != 0;
                        branch.IsNotFollowingPage = currentPage.Sequence >= branchToPage.Sequence;
                    }
                }

                branches.Add(branch);
            }

            foreach (var branch in branches)
            {
                if (branch.QuestionSequence >= branch.BranchToQuestionSequence)
                {
                    BranchValidationStatus.AddMessage(AlertType.Warning, $"Question {branch.QuestionSequence} branches to Question {branch.BranchToQuestionSequence}. Instead it should branch to a question that follows Q{branch.QuestionSequence}.");
                }
            }

            var data = branches
                .OrderBy(x => x.QuestionSequence)
                .ThenBy(x => x.OptionListSequence)
                .ThenBy(x => x.OptionItemSequence)
                .ToList();

            BranchesRepeater.DataSource = data.GroupBy(r => r.OptionListIdentifier).Select(group =>
            {
                var first = group.First();

                return new
                {
                    first.QuestionIdentifier,
                    first.QuestionCode,
                    first.QuestionTitle,

                    first.OptionListIdentifier,
                    first.OptionListSequence,
                    first.OptionListTitle,

                    first.SingleList,

                    Options = group.Select(row => new
                    {
                        Letter = row.OptionItemLetter,
                        Sequence = row.OptionItemSequence,
                        Title = row.OptionItemTitle,
                        row.BranchToQuestionCode,
                        row.BranchToQuestionTitle,
                        row.BranchToPageNumber,
                        row.IsNotFirstQuestion,
                        row.IsNotFollowingPage
                    })
                };
            });
            BranchesRepeater.DataBind();

            var hasBranches = data.Count > 0;
            BranchesRepeater.Visible = hasBranches;
            BranchesNoItemsMessage.Visible = !hasBranches;
        }

        private void BranchesRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var optionsRepeater = (Repeater)e.Item.FindControl("OptionsRepeater");
            optionsRepeater.DataSource = DataBinder.Eval(e.Item.DataItem, "Options");
            optionsRepeater.DataBind();
        }

        private void QuestionsManager_Copied(object sender, EventArgs e)
        {
            StatusAlert.AddMessage(AlertType.Success, "The question was successfully copied");
        }

        #endregion

        #region Helper methods

        private void CheckDuplicates(SurveyState survey)
        {
            if (!ServiceLocator.SurveySearch.IsDuplicate(survey.Form))
                return;

            DuplicateStatus.Text =
                $"The system contains multiple forms having the same name: {survey.Form.Name}. Please give each form a unique name.";
            DuplicateStatus.Indicator = AlertType.Warning;
        }

        private void CheckValidity(SurveyState survey)
        {
            if (!ServiceLocator.SurveySearch.IsValid(survey.Form.Identifier))
            {
                ValidationStatus.Text =
                    $"This form contains invalid conditional logic.";
                ValidationStatus.Indicator = AlertType.Warning;
            }
        }

        private void CheckPageBreaks(SurveyState survey)
        {
            for (var i = 1; i < survey.Form.Questions.Count; i++)
            {
                if (survey.Form.Questions[i - 1].Type == SurveyQuestionType.BreakPage
                    && survey.Form.Questions[i].Type == SurveyQuestionType.BreakPage
                    )
                {
                    StatusAlert.AddMessage(AlertType.Warning, "Usage of multiple consecutive page breaks could cause problems with navigation through the form.");
                    break;
                }
            }

            if (survey.Form.Questions.Count > 0)
            {
                if (survey.Form.Questions[survey.Form.Questions.Count - 1].Type == SurveyQuestionType.BreakPage)
                {
                    StatusAlert.AddMessage(AlertType.Warning, "This form is currently configured with a page break as the last question. Please delete this question if there is no additional content.");
                }
            }
        }

        protected string GetTitle(Guid id) =>
            ServiceLocator.ContentSearch.GetSnip(id, ContentLabel.Title);

        protected string GetText(object text, string defaultText)
        {
            var strText = text as string;
            if (!string.IsNullOrEmpty(strText))
                return strText;

            return defaultText;
        }

        private void AddContentNavItem(out NavItem navItem, out DynamicControl ctrl)
        {
            ContentTabs.AddItem(navItem = new NavItem());
            navItem.Controls.Add(ctrl = new DynamicControl { ID = "Container" });
        }

        #endregion
    }
}