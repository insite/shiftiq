using System;
using System.Linq;
using System.Web.UI;

using InSite.Admin.Assets.Contents.Controls.ContentEditor;
using InSite.Common.Web.Infrastructure;
using InSite.Common.Web.UI;
using InSite.Domain.Surveys.Forms;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Admin.Workflow.Forms.Questions.Controls
{
    public partial class Detail : UserControl
    {
        private Guid FormIdentifier => (Guid)ViewState[nameof(FormIdentifier)];

        private Guid QuestionIdentifier
        {
            get => (Guid)(ViewState[nameof(QuestionIdentifier)] ?? Guid.Empty);
            set => ViewState[nameof(QuestionIdentifier)] = value;
        }

        private string Language
        {
            get => (string)ViewState[nameof(Language)];
            set => ViewState[nameof(Language)] = value;
        }

        private bool OpenIssueEnabled
        {
            get => (bool)(ViewState[nameof(OpenIssueEnabled)] ?? false);
            set => ViewState[nameof(OpenIssueEnabled)] = value;
        }

        public int PageNumber => PageID.ValueAsInt.Value;

        private SurveyForm Form
        {
            get
            {
                if (_survey == null)
                    _survey = ServiceLocator.SurveySearch.GetSurveyState(FormIdentifier).Form;

                return _survey;
            }
            set
            {
                _survey = value;
                ViewState[nameof(FormIdentifier)] = value.Identifier;
            }
        }

        private SurveyForm _survey;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PageID.AutoPostBack = true;
            PageID.ValueChanged += PageID_ValueChanged;

            Sequence.AutoPostBack = true;
            Sequence.ValueChanged += Sequence_ValueChanged;

            IndicatorValue.AutoPostBack = true;
            IndicatorValue.ValueChanged += IndicatorValue_ValueChanged;

            QuestionCode.AutoPostBack = true;
            QuestionCode.TextChanged += QuestionCode_TextChanged;

            QuestionTypeSelector.AutoPostBack = true;
            QuestionTypeSelector.ValueChanged += QuestionTypeSelector_ValueChanged;

            QuestionTitle.TranslationRequested += QuestionTitle_TranslationRequested;

            NumberAutoCalcFields.NeedDataCount += NumberAutoCalcFields_NeedDataCount;
            NumberAutoCalcFields.NeedDataSource += NumberAutoCalcFields_NeedDataSource;
            NumberAutoCalcFields.NeedSelectedItems += NumberAutoCalcFields_NeedSelectedItems;
        }

        private void PageID_ValueChanged(object sender, EventArgs e)
        {
            SetSequenceFromPage();
        }

        private void Sequence_ValueChanged(object sender, EventArgs e)
        {
            RefreshIndicatorPreview();
        }

        private void QuestionCode_TextChanged(object sender, EventArgs e)
        {
            RefreshIndicatorPreview();
        }

        private void IndicatorValue_ValueChanged(object sender, EventArgs e)
        {
            RefreshIndicatorPreview();
        }

        private void QuestionTypeSelector_ValueChanged(object sender, EventArgs e)
        {
            InitQuestionTypeControls(QuestionTypeSelector.ValueAsEnum);
        }

        private void QuestionTitle_TranslationRequested(object sender, TranslationRequestEventArgs args)
        {
            DetailList.Translate(args.FromLanguage, args.ToLanguages);
            DetailLikertTable.Translate(args.FromLanguage, args.ToLanguages);
        }

        private void NumberAutoCalcFields_NeedDataCount(object sender, FindEntity.CountArgs args)
        {
            args.Count = Form.Questions.Where(x => x.Type == SurveyQuestionType.Number).Count();
        }

        private void NumberAutoCalcFields_NeedDataSource(object sender, FindEntity.DataArgs args)
        {
            args.Items = GetNumberAutoCalcFieldsDataSource().ApplyPaging(args.Paging).ToArray();
        }

        private void NumberAutoCalcFields_NeedSelectedItems(object sender, FindEntity.ItemsArgs args)
        {
            args.Items = GetNumberAutoCalcFieldsDataSource().Where(x => args.Identifiers.Contains(x.Value)).ToArray();
        }

        private IQueryable<FindEntity.DataItem> GetNumberAutoCalcFieldsDataSource()
        {
            return Form.Questions.AsQueryable()
                .Where(x => x.Type == SurveyQuestionType.Number && x.Identifier != QuestionIdentifier)
                .Select(x => new FindEntity.DataItem
                {
                    Value = x.Identifier,
                    Text = $"Q{x.Sequence:00}: {x.Content.Title.Text.Default}",
                });
        }

        public void SetDefaultInputValues(SurveyState survey, int? pageIndex)
        {
            Form = survey.Form;
            Language = survey.Form.Language;
            OpenIssueEnabled = Form.State.WorkflowConfiguration != null;

            SetupPageID(survey.Form);

            PageID.ValueAsInt = pageIndex ?? survey.Form.GetPages().Count;

            QuestionTypeSelector.Visible = true;
            QuestionTypeSelectorRequired.Visible = true;

            SetQuestionTitle(null);

            InitQuestionTypeControls(null);

            SetSequenceFromPage();

            GlossaryTermSection.Visible = false;
        }

        public void SetInputValues(SurveyQuestion question)
        {
            if (question == null)
                return;

            Form = question.Form;
            QuestionIdentifier = question.Identifier;
            Language = question.Form.Language;
            OpenIssueEnabled = Form.State.WorkflowConfiguration != null;

            SetupPageID(question.Form);

            PageID.ValueAsInt = question.Page.Sequence;

            QuestionTypeSelector.Visible = false;
            QuestionTypeSelectorRequired.Visible = false;

            QuestionType.Text = question.Type.GetDescription();

            Sequence.MinValue = 1;
            Sequence.MaxValue = question.Form.Questions.Count;
            Sequence.ValueAsInt = question.Sequence;

            IndicatorValue.EnsureDataBound();
            IndicatorValue.Value = question.Indicator;

            QuestionCode.Text = question.Code;

            RefreshIndicatorPreview();

            QuestionAttribute.Text = question.Attribute;

            SetQuestionTitle(question.Content?.Title);

            InitQuestionTypeControls(question.Type);
            SetInputValuesByType(question);

            if (question.Content != null)
            {
                GlossaryTermSection.Visible = true;
                GlossaryTermGrid.LoadData(
                    question.Identifier,
                    "Form Question",
                    question.Options.Lists
                        .SelectMany(x => x.Items)
                        .Where(x => x.Content != null && x.Content.Title != null)
                        .Select(x => x.Content.Title.Text)
                        .Append(question.Content.Title.Text),
                    ContentLabel.Title,
                    true);
            }

            DetailAnswerRepeater.BindModelToControls(question);
        }

        private void SetQuestionTitle(ContentContainerItem value)
        {
            if (value == null)
                value = new ContentContainerItem();

            QuestionTitle.SetOptions(new AssetContentSection.MarkdownAndHtml("Title")
            {
                HtmlValue = value.Html,
                MarkdownValue = value.Text,
                IsMultiValue = true,
                AllowUpload = true,
                UploadFolderPath = FileHelper.GetSurveyUploadPath(FormIdentifier, "Form")
            });
            QuestionTitle.SetLanguage(Language ?? CurrentSessionState.Identity.Language);
        }

        public void GetInputValues(SurveyQuestion question)
        {
            if (QuestionTypeSelector.Visible)
                question.Type = QuestionTypeSelector.ValueAsEnum.Value;

            var sequence = Math.Min(Sequence.ValueAsInt.Value, question.Form.Questions.Count);
            if (sequence != question.Sequence)
            {
                question.Form.Questions.Remove(question);
                question.Form.Questions.Insert(sequence - 1, question);
            }

            question.Indicator = IndicatorValue.Value;
            question.Code = QuestionCode.Text;
            question.Attribute = QuestionAttribute.Text;

            if (question.Content == null)
                question.Content = new ContentContainer();

            question.Content.Title.Text = QuestionTitle.GetValue(ContentSectionDefault.BodyText.GetName());
            question.Content.Title.Html = QuestionTitle.GetValue(ContentSectionDefault.BodyHtml.GetName());

            GetInputValuesByType(question);
        }

        private void RefreshIndicatorPreview()
        {
            var indicatorPreview = QuestionCode.Text.HasValue() ? QuestionCode.Text : Sequence.ValueAsInt.ToString();
            var indicatorStyle = IndicatorValue.EnumValue?.GetContextualClass() ?? "primary";

            IndicatorPreview.Text = $"<span class=\"badge bg-{indicatorStyle}\">{indicatorPreview}</span>";
        }

        private void SetupPageID(SurveyForm survey)
        {
            PageID.Items.Clear();

            var pages = survey.GetPages();
            foreach (var page in pages)
                PageID.Items.Add(new ComboBoxOption($"Page {page.Sequence}", page.Sequence.ToString()));
        }

        private void SetSequenceFromPage()
        {
            var pageNumber = PageID.ValueAsInt.Value;
            var page = Form.GetPage(pageNumber);

            if (page != null)
            {
                int maxSequence;

                if (page.Questions.Count > 0)
                {
                    maxSequence = page.Questions.Max(x => x.Sequence);
                }
                else if (pageNumber > 1)
                {
                    var pageBreak = Form.Questions
                        .Where(x => x.Type == SurveyQuestionType.BreakPage)
                        .Skip(pageNumber - 2)
                        .FirstOrDefault();

                    maxSequence = pageBreak.Sequence;
                }
                else
                {
                    maxSequence = 0;
                }

                Sequence.ValueAsInt = maxSequence + 1;

                RefreshIndicatorPreview();
            }
        }

        private void InitQuestionTypeControls(SurveyQuestionType? questionType)
        {
            SettingsContainer.Visible = questionType.HasValue
                && questionType != SurveyQuestionType.BreakPage
                && questionType != SurveyQuestionType.BreakQuestion
                && questionType != SurveyQuestionType.Terminate;

            IsRequiredField.Visible = questionType.HasValue
                && questionType != SurveyQuestionType.CheckList;

            IsNestedField.Visible =
                   questionType == SurveyQuestionType.CheckList
                || questionType == SurveyQuestionType.Comment
                || questionType == SurveyQuestionType.Date
                || questionType == SurveyQuestionType.Number
                || questionType == SurveyQuestionType.Selection
                || questionType == SurveyQuestionType.RadioList
                || questionType == SurveyQuestionType.Upload;

            NumberEnableStatisticsField.Visible = questionType == SurveyQuestionType.Number
                || questionType == SurveyQuestionType.Selection
                || questionType == SurveyQuestionType.RadioList
                || questionType == SurveyQuestionType.Likert;

            NumberAutoCalcField.Visible = questionType == SurveyQuestionType.Number;

            NumberNaPermitted.Visible = questionType == SurveyQuestionType.Number;

            DetailCommentBox.Visible = questionType == SurveyQuestionType.Comment;

            ListSection.Visible = questionType == SurveyQuestionType.RadioList
                || questionType == SurveyQuestionType.Selection
                || questionType == SurveyQuestionType.CheckList;

            EnableCreateCaseField.Visible = OpenIssueEnabled;

            if (ListSection.Visible && questionType.HasValue)
                DetailList.SetDefaultInputValues(questionType.Value, Language);

            LikertTableSection.Visible = questionType == SurveyQuestionType.Likert;

            if (LikertTableSection.Visible)
                DetailLikertTable.SetDefaultInputValues(Language);
        }

        private void SetInputValuesByType(SurveyQuestion question)
        {
            switch (question.Type)
            {
                case SurveyQuestionType.Comment:
                    IsRequired.Checked = question.IsRequired;
                    IsNested.Checked = question.IsNested;
                    DetailCommentBox.SetInputValues(question);
                    EnableCreateCase.Checked = question.EnableCreateCase;
                    break;
                case SurveyQuestionType.Date:
                    IsRequired.Checked = question.IsRequired;
                    IsNested.Checked = question.IsNested;
                    EnableCreateCase.Checked = question.EnableCreateCase;
                    break;
                case SurveyQuestionType.Text:
                    IsRequired.Checked = question.IsRequired;
                    EnableCreateCase.Checked = question.EnableCreateCase;
                    break;
                case SurveyQuestionType.Number:
                    IsRequired.Checked = question.IsRequired;
                    IsNested.Checked = question.IsNested;
                    NumberEnableStatistics.Checked = question.NumberEnableStatistics;
                    NumberEnableAutoCalc.Checked = question.NumberEnableAutoCalc;
                    NumberAutoCalcFields.Values = question.NumberAutoCalcQuestions;
                    NumberEnableNa.Checked = !question.NumberEnableAutoCalc && question.NumberEnableNotApplicable;
                    EnableCreateCase.Checked = question.EnableCreateCase;
                    break;
                case SurveyQuestionType.RadioList:
                    IsRequired.Checked = question.IsRequired;
                    IsNested.Checked = question.IsNested;
                    NumberEnableStatistics.Checked = question.NumberEnableStatistics;
                    ListSection.SetTitle("Options", DetailList.SetInputValues(question, Language));
                    EnableCreateCase.Checked = question.EnableCreateCase;
                    break;
                case SurveyQuestionType.Selection:
                    IsRequired.Checked = question.IsRequired;
                    IsNested.Checked = question.IsNested;
                    NumberEnableStatistics.Checked = question.NumberEnableStatistics;
                    ListSection.SetTitle("Options", DetailList.SetInputValues(question, Language));
                    EnableCreateCase.Checked = question.EnableCreateCase;
                    break;
                case SurveyQuestionType.CheckList:
                    IsNested.Checked = question.IsNested;
                    ListSection.SetTitle("Options", DetailList.SetInputValues(question, Language));
                    EnableCreateCase.Checked = question.EnableCreateCase;
                    break;
                case SurveyQuestionType.Likert:
                    IsRequired.Checked = question.IsRequired;
                    NumberEnableStatistics.Checked = question.NumberEnableStatistics;
                    LikertTableSection.SetTitle("Options", DetailLikertTable.SetInputValues(question, Language));
                    EnableCreateCase.Checked = question.EnableCreateCase;
                    break;
                case SurveyQuestionType.Upload:
                    IsRequired.Checked = question.IsRequired;
                    IsNested.Checked = question.IsNested;
                    EnableCreateCase.Checked = question.EnableCreateCase;
                    break;
            }
        }

        private void GetInputValuesByType(SurveyQuestion question)
        {
            switch (question.Type)
            {
                case SurveyQuestionType.Comment:
                    question.IsRequired = IsRequired.Checked;
                    question.IsNested = IsNested.Checked;
                    DetailCommentBox.GetInputValues(question);
                    question.EnableCreateCase = EnableCreateCase.Checked;
                    break;
                case SurveyQuestionType.Date:
                    question.IsRequired = IsRequired.Checked;
                    question.IsNested = IsNested.Checked;
                    question.EnableCreateCase = EnableCreateCase.Checked;
                    break;
                case SurveyQuestionType.Text:
                    question.IsRequired = IsRequired.Checked;
                    question.EnableCreateCase = EnableCreateCase.Checked;
                    break;
                case SurveyQuestionType.Number:
                    question.IsRequired = IsRequired.Checked;
                    question.IsNested = IsNested.Checked;
                    question.NumberEnableStatistics = NumberEnableStatistics.Checked;
                    question.NumberEnableAutoCalc = NumberEnableAutoCalc.Checked;
                    question.NumberAutoCalcQuestions = question.NumberEnableAutoCalc ? NumberAutoCalcFields.Values : null;
                    question.NumberEnableNotApplicable = !question.NumberEnableAutoCalc && NumberEnableNa.Checked;
                    question.EnableCreateCase = EnableCreateCase.Checked;
                    break;
                case SurveyQuestionType.RadioList:
                    question.IsRequired = IsRequired.Checked;
                    question.IsNested = IsNested.Checked;
                    question.NumberEnableStatistics = NumberEnableStatistics.Checked;
                    DetailList.GetInputValues(question);
                    question.EnableCreateCase = EnableCreateCase.Checked;
                    break;
                case SurveyQuestionType.Selection:
                    question.IsRequired = IsRequired.Checked;
                    question.IsNested = IsNested.Checked;
                    question.NumberEnableStatistics = NumberEnableStatistics.Checked;
                    DetailList.GetInputValues(question);
                    question.EnableCreateCase = EnableCreateCase.Checked;
                    break;
                case SurveyQuestionType.CheckList:
                    question.IsNested = IsNested.Checked;
                    DetailList.GetInputValues(question);
                    question.EnableCreateCase = EnableCreateCase.Checked;
                    break;
                case SurveyQuestionType.Likert:
                    question.IsRequired = IsRequired.Checked;
                    question.NumberEnableStatistics = NumberEnableStatistics.Checked;
                    DetailLikertTable.GetInputValues(question);
                    question.EnableCreateCase = EnableCreateCase.Checked;
                    break;
                case SurveyQuestionType.Upload:
                    question.IsRequired = IsRequired.Checked;
                    question.IsNested = IsNested.Checked;
                    question.EnableCreateCase = EnableCreateCase.Checked;
                    break;
            }
        }
    }
}