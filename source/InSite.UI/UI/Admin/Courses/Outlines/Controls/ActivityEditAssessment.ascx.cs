using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Shift.Common.Timeline.Commands;

using InSite.Application.Banks.Write;
using InSite.Application.Courses.Read;
using InSite.Common.Web.Infrastructure;
using InSite.Common.Web.UI;
using InSite.Domain.Banks;
using InSite.Persistence;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Admin.Courses.Outlines.Controls
{
    public partial class ActivityEditAssessment : BaseActivityEdit
    {
        #region Classes

        [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
        private class QuestionJson
        {
            [JsonProperty(PropertyName = "index")]
            public int Index { get; private set; }

            [JsonProperty(PropertyName = "correct")]
            public int? CorrectOptionIndex { get; private set; }

            [JsonProperty(PropertyName = "options")]
            public int OptionsCount { get; private set; }

            [JsonConstructor]
            private QuestionJson()
            {

            }

            public QuestionJson(Question question, int index)
            {
                Index = index;
                CorrectOptionIndex = question.Options.FindIndex(o => o.HasPoints);
                OptionsCount = question.Options.Count;
            }
        }

        [Serializable]
        private class ControlDataItem
        {
            public Guid BankIdentifier { get; }

            public int BankAsset { get; }

            public Guid FormIdentifier { get; }

            public ControlDataItem(Form form)
            {
                FormIdentifier = form.Identifier;

                var bank = form.Specification.Bank;
                BankIdentifier = bank.Identifier;
                BankAsset = bank.Asset;
            }
        }

        #endregion

        #region Properties

        protected bool AllowReorder
        {
            get => (bool)(ViewState[nameof(AllowReorder)] ?? false);
            set => ViewState[nameof(AllowReorder)] = value;
        }

        private ControlDataItem ControlData
        {
            get => (ControlDataItem)ViewState[nameof(ControlData)];
            set => ViewState[nameof(ControlData)] = value;
        }

        private List<Guid> QuestionRepeaterKeys
        {
            get => (List<Guid>)ViewState[nameof(QuestionRepeaterKeys)];
            set => ViewState[nameof(QuestionRepeaterKeys)] = value;
        }

        #endregion

        #region Initialization and Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            BindControlsToHandlers(ActivitySetup, Language, ContentRepeater, ActivitySaveButton, ActivityCancelButton);

            AddQuestionTopButton.Click += AddQuestionButton_Click;
            AddQuestionBottomButton.Click += AddQuestionButton_Click;

            QuestionRepeater.ItemCreated += QuestionRepeater_ItemCreated;
            QuestionRepeater.ItemDataBound += QuestionRepeater_ItemDataBound;
            QuestionRepeater.ItemCommand += QuestionRepeater_ItemCommand;

            AssessmentFormIdentifier.AutoPostBack = true;
            AssessmentFormIdentifier.ValueChanged += AssessmentFormIdentifier_ValueChanged;
        }

        #endregion

        #region Binding

        protected override void BindModelToControls(QActivity activity)
        {
            var form = GetForm(activity.AssessmentFormIdentifier);
            var hasForm = form != null;

            QuestionTab.Visible = hasForm;

            if (!hasForm)
                return;

            var spec = form.Specification;
            var bank = spec.Bank;

            ControlData = new ControlDataItem(form);

            AssessmentFormIdentifier.Value = form.Identifier;
            AssessmentFormLink.NavigateUrl = $"/ui/admin/assessments/banks/outline?bank={bank.Identifier}&form={form.Identifier}";
            AssessmentSpecType.SelectedValue = spec.Type.ToString();
            AssessmentFormTitle.Text = form.Content.Title.Default;
            AssessmentSpecQuestionLimit.ValueAsInt = spec.QuestionLimit;
            AssessmentSpecPassingScore.ValueAsDecimal = spec.Calculation.PassingScore * 100m;
            AssessmentSpecDisclosureType.Value = spec.Calculation.Disclosure.ToString();
            AssessmentFormPublicationStatus.SelectedValue = form.Publication.Status.ToString();

            LoadQuestions(form);
        }

        private void LoadQuestions(Form form)
        {
            var questions = form.GetQuestions();

            AllowReorder = questions.Count == 0
                || questions.All(q => q.Type.IsRadioList());

            if (AllowReorder)
            {
                if (form.Specification.Type == SpecificationType.Static)
                    AllowReorder = form.Sections.Count <= 1;
                else
                    AllowReorder = form.Specification.Criteria.Count == 0 && form.Specification.Bank.Sets.Count <= 1
                        || form.Specification.Criteria.Count == 1 && form.Specification.Criteria[0].Sets.Count <= 1;
            }

            questions = questions
                // .Where(q => q.Type == QuestionItemType.SingleCorrect || q.Type == QuestionItemType.TrueOrFalse)
                .ToList();

            SetQuestionsJson(questions.Select((q, i) => new QuestionJson(q, i)));
            UpdateQuestions(questions);
        }

        private void UpdateQuestions()
        {
            var form = GetForm(ControlData.FormIdentifier);
            var questions = form.GetQuestions()
                .Where(q => q.Type.IsRadioList())
                .ToArray();

            var jsonData = GetQuestionsJson();
            if (questions.Length != jsonData.Count)
            {
                LoadQuestions(form);
                return;
            }

            for (var i = 0; i < QuestionRepeaterKeys.Count; i++)
            {
                var questionId = QuestionRepeaterKeys[i];

                var question = questions.FirstOrDefault(x => x.Identifier == questionId);
                if (question == null)
                    continue;

                var qRepeaterItem = QuestionRepeater.Items[i];
                var questionText = (MarkdownEditor)qRepeaterItem.FindControl("QuestionText");
                var optionRepeater = (Repeater)qRepeaterItem.FindControl("OptionRepeater");

                question.Content.Title.Default = questionText.Value.Trim();

                if (question.Options.Count == optionRepeater.Items.Count)
                {
                    for (var j = 0; j < optionRepeater.Items.Count; j++)
                    {
                        var oRepeaterItem = optionRepeater.Items[j];
                        var optionText = (ITextBox)oRepeaterItem.FindControl("OptionText");

                        question.Options[j].Content.Title.Default = optionText.Text.Trim();
                    }
                }
            }

            UpdateQuestions(questions);
        }

        private void UpdateQuestions(IEnumerable<Question> questions)
        {
            QuestionRepeaterKeys = new List<Guid>();

            QuestionRepeater.DataSource = questions;
            QuestionRepeater.DataBind();

            var isLimitExceeded = QuestionRepeater.Items.Count > 3;

            CollapseTopButton.Visible = isLimitExceeded;
            CollapseBottomButton.Visible = isLimitExceeded;
            AddQuestionBottomButton.Visible = isLimitExceeded;
        }

        protected override void BindControlsToModel(QActivity activity)
        {
            if (!activity.AssessmentFormIdentifier.HasValue)
                return;

            var commands = new List<ICommand>();
            var form = ServiceLocator.BankSearch.GetFormData(activity.AssessmentFormIdentifier.Value);

            SaveAssessment(form, commands);

            foreach (var cmd in commands)
                ServiceLocator.SendCommand(cmd);
        }

        #endregion

        #region Database Operations

        private bool AddQuestion(QuestionItemType type)
        {
            var form = GetForm(AssessmentFormIdentifier.Value);
            var spec = form.Specification;
            var bank = spec.Bank;
            var isStatic = spec.Type == SpecificationType.Static;
            var set = isStatic
                ? form.Sections.LastOrDefault()?.Criterion.Sets.LastOrDefault()
                : null;

            if (set == null)
                set = spec.Criteria.Count > 0
                    ? spec.Criteria.LastOrDefault()?.Sets.LastOrDefault()
                    : spec.Bank.Sets.LastOrDefault();

            if (set == null)
            {
                set = new Set { Identifier = UniqueIdentifier.Create() };

                ServiceLocator.SendCommand(new AddSet(bank.Identifier, set.Identifier, "Default", Guid.Empty));
            }

            var question = new Question();
            {
                question.Identifier = UniqueIdentifier.Create();
                question.Asset = Sequence.Increment(CurrentSessionState.Identity.Organization.OrganizationIdentifier, SequenceType.Asset);
                question.Type = type;
                question.CalculationMethod = QuestionCalculationMethod.Default;
                question.Points = 1;
                question.Content.Title.Default = "New Question";

                if (type == QuestionItemType.SingleCorrect)
                {
                    AddOption("Option A", 1);
                    AddOption("Option B", 0);
                    AddOption("Option C", 0);
                    AddOption("Option D", 0);
                }
                else if (type == QuestionItemType.TrueOrFalse)
                {
                    AddOption("True", 1);
                    AddOption("False", 0);
                }

                void AddOption(string text, decimal points)
                {
                    var option = new Option();
                    option.Content.Title.Default = text;
                    option.Points = points;

                    question.Options.Add(option);
                }
            }

            ServiceLocator.SendCommand(new AddQuestion(bank.Identifier, set.Identifier, question));

            if (isStatic)
            {
                var criterion = spec.Criteria.FirstOrDefault(x => x.SetIdentifiers.Contains(set.Identifier));
                if (criterion == null)
                {
                    criterion = new Criterion { Identifier = UniqueIdentifier.Create() };

                    ServiceLocator.SendCommand(new AddCriterion(bank.Identifier, spec.Identifier, new[] { set.Identifier }, criterion.Identifier, "Default", 1, 0, null, null));
                }

                var section = criterion.Sections.LastOrDefault(x => x.Form.Identifier == form.Identifier);
                if (section == null)
                {
                    section = new Domain.Banks.Section { Identifier = UniqueIdentifier.Create() };

                    ServiceLocator.SendCommand(new AddSection(bank.Identifier, form.Identifier, section.Identifier, criterion.Identifier));
                }

                ServiceLocator.SendCommand(new AddField(bank.Identifier, UniqueIdentifier.Create(), section.Identifier, question.Identifier, -1));
            }

            var jsonData = GetQuestionsJson();
            jsonData.Add(new QuestionJson(question, jsonData.Count));
            SetQuestionsJson(jsonData);

            return true;
        }

        private bool DeleteQuestion(int index)
        {
            var questionId = QuestionRepeaterKeys[index];
            var form = GetForm(ControlData.FormIdentifier);

            var isValid = false;

            if (form.Specification.Type == SpecificationType.Static)
            {
                var field = form.Sections.SelectMany(x => x.Fields).FirstOrDefault(x => x.QuestionIdentifier == questionId);
                if (isValid = field != null)
                    ServiceLocator.SendCommand(new DeleteField(ControlData.BankIdentifier, field.Identifier, form.Identifier, field.QuestionIdentifier));
            }
            else
            {
                var question = form.Specification.Bank.FindQuestion(questionId);
                if (isValid = question != null)
                    ServiceLocator.SendCommand(new DeleteQuestion(ControlData.BankIdentifier, question.Identifier, false));
            }

            if (isValid)
            {
                var jsonData = GetQuestionsJson();
                jsonData.RemoveAt(index);
                SetQuestionsJson(jsonData);
            }

            return isValid;
        }

        private void SaveAssessment(Form form, List<ICommand> commands)
        {
            var formContent = form.Content?.Clone() ?? new ContentExamForm();
            var spec = form.Specification;
            var calc = form.Specification.Calculation;
            var bankId = form.Specification.Bank.Identifier;

            var specType = AssessmentSpecType.SelectedValue.ToEnum<SpecificationType>();
            var specQuestionLimit = AssessmentSpecQuestionLimit.ValueAsInt.HasValue
                ? AssessmentSpecQuestionLimit.ValueAsInt.Value
                : spec.QuestionLimit;
            var calcPassingScore = (AssessmentSpecPassingScore.ValueAsDecimal ?? 0) / 100m;
            var calcDisclosure = AssessmentSpecDisclosureType.Value.ToEnum<DisclosureType>();
            var formPublicationStatus = AssessmentFormPublicationStatus.SelectedValue.ToEnum<PublicationStatus>();

            if (spec.Type != specType)
            {
                commands.Add(new RetypeSpecification(bankId, spec.Identifier, specType));
            }

            if (formContent.Title.Default != AssessmentFormTitle.Text)
            {
                formContent.Title.Default = AssessmentFormTitle.Text;
                commands.Add(new ChangeFormContent(bankId, form.Identifier, formContent, form.HasDiagrams, form.HasReferenceMaterials));
            }

            if (specQuestionLimit != spec.QuestionLimit)
            {
                commands.Add(new ReconfigureSpecification(bankId, spec.Identifier, spec.Consequence, spec.FormLimit, specQuestionLimit));
            }

            if (calc.PassingScore != calcPassingScore || calc.Disclosure != calcDisclosure)
            {
                calc.PassingScore = calcPassingScore;
                calc.Disclosure = calcDisclosure;

                commands.Add(new ChangeSpecificationCalculation(bankId, spec.Identifier, calc));
            }

            if (form.Publication.Status != formPublicationStatus)
            {
                form.Publication.Status = formPublicationStatus;
                if (AssessmentFormPublicationStatus.SelectedValue == "Unpublished")
                    commands.Add(new UnpublishForm(bankId, form.Identifier));
                else
                    commands.Add(new PublishForm(bankId, form.Identifier, form.Publication));
            }

            SaveQuestions(form, commands);
        }

        private void SaveQuestions(Form form, List<ICommand> commands)
        {
            var bank = form.Specification.Bank;

            var questions = form.GetQuestions();
            var jsonData = GetQuestionsJson();

            var reorderData = AllowReorder && questions.Count == QuestionRepeaterKeys.Count ? new Dictionary<int, int>() : null;
            var isReordered = false;

            for (var i = 0; i < QuestionRepeaterKeys.Count; i++)
            {
                var questionId = QuestionRepeaterKeys[i];

                var question = questions.FirstOrDefault(x => x.Identifier == questionId);
                if (question == null || !question.Type.IsRadioList())
                {
                    reorderData = null;
                    continue;
                }

                var jsonItem = jsonData[i];
                if (reorderData != null)
                {
                    if (jsonItem.Index != i)
                        isReordered = true;

                    reorderData.Add(i + 1, jsonItem.Index + 1);
                }

                var qRepeaterItem = QuestionRepeater.Items[i];
                var questionText = (MarkdownEditor)qRepeaterItem.FindControl("QuestionText");
                var optionRepeater = (Repeater)qRepeaterItem.FindControl("OptionRepeater");

                {
                    var content = question.Content.Clone();

                    content.Title.Default = questionText.Value.Trim();

                    if (!content.IsEqual(question.Content))
                        commands.Add(new ChangeQuestionContent(bank.Identifier, question.Identifier, content));
                }

                if (question.Options.Count == optionRepeater.Items.Count)
                {
                    var correctOption = question.Options.Count == jsonItem.OptionsCount
                        ? jsonItem.CorrectOptionIndex
                        : null;

                    for (var j = 0; j < optionRepeater.Items.Count; j++)
                    {
                        var oRepeaterItem = optionRepeater.Items[j];
                        var optionText = (ITextBox)oRepeaterItem.FindControl("OptionText");
                        var option = question.Options[j];

                        var content = option.Content.Clone();
                        content.Title.Default = optionText.Text.Trim();

                        var points = !correctOption.HasValue
                            ? option.Points
                            : correctOption.Value == j
                                ? 1
                                : 0;

                        if (points != option.Points || !content.IsEqual(option.Content))
                            commands.Add(new ChangeOption(bank.Identifier, question.Identifier, option.Number, content, points, option.IsTrue, option.CutScore, option.Standard));
                    }
                }
            }

            if (reorderData != null && isReordered)
            {
                if (form.Specification.Type == SpecificationType.Static)
                {
                    var section = form.Sections.SingleOrDefault();
                    if (section != null)
                        commands.Add(new ReorderFields(bank.Identifier, section.Identifier, reorderData));
                }
                else
                {
                    var set = form.Specification.Criteria.Count == 1
                        ? form.Specification.Criteria[0].Sets.SingleOrDefault()
                        : form.Specification.Criteria.Count == 0
                            ? form.Specification.Bank.Sets.SingleOrDefault()
                            : null;

                    if (set != null)
                        commands.Add(new ReorderQuestions(bank.Identifier, set.Identifier, reorderData));
                }
            }
        }

        #endregion

        #region UI Event Handling

        protected override void OnAlert(AlertType type, string message)
        {
            ScreenStatus.AddMessage(type, message);
        }

        private void QuestionRepeater_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var editorUpload = (EditorUpload)e.Item.FindControl("QuestionUpload");
            editorUpload.Custom += EditorUpload_Custom;
        }

        private void QuestionRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var viewer = (HtmlGenericControl)e.Item.FindControl("QuestionViewer");
            var editor = (HtmlGenericControl)e.Item.FindControl("QuestionEditor");

            var question = (Question)e.Item.DataItem;
            editor.Visible = question.Type.IsRadioList();
            viewer.Visible = !editor.Visible;

            QuestionRepeaterKeys.Add(question.Identifier);

            if (editor.Visible)
            {
                var optionRepeater = (Repeater)e.Item.FindControl("OptionRepeater");
                optionRepeater.DataSource = question.Options;
                optionRepeater.DataBind();
            }
            else
            {

            }
        }

        private void QuestionRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "RemoveQuestion")
            {
                if (DeleteQuestion(e.Item.ItemIndex))
                    UpdateQuestions();
                else
                    LoadQuestions(GetForm(ControlData.FormIdentifier));
            }
            else
                throw new Exception("Unexpected command: " + e.CommandName);
        }

        private void AddQuestionButton_Click(object sender, CommandEventArgs e)
        {
            QuestionItemType questionType;

            if (e.CommandName == "AddMultipleChoice")
                questionType = QuestionItemType.SingleCorrect;
            else if (e.CommandName == "AddTrueOrFalse")
                questionType = QuestionItemType.TrueOrFalse;
            else
                throw ApplicationError.Create("Unexpected command name: " + e.CommandName);

            if (AddQuestion(questionType))
                UpdateQuestions();
            else
                LoadQuestions(GetForm(ControlData.FormIdentifier));
        }

        private void EditorUpload_Custom(object sender, EditorUpload.CustomEventArgs args)
        {
            var questionText = (MarkdownEditor)((System.Web.UI.Control)sender).NamingContainer.FindControl("QuestionText");

            var name = System.IO.Path.GetFileNameWithoutExtension(args.File.FileName);
            var extension = System.IO.Path.GetExtension(args.File.FileName);
            var filename = StringHelper.ToIdentifier(name) + extension;

            var attachmentEntity = Assessments.Attachments.Forms.Add.AttachFile(ControlData.BankIdentifier, ControlData.BankAsset, filename, filename, args.File.InputStream);
            var uploadEntity = UploadSearch.Select(attachmentEntity.Upload);

            questionText.SetupCallback(args.Callback, uploadEntity.Name, FileHelper.GetUrl(uploadEntity.NavigateUrl));
        }

        private void AssessmentFormIdentifier_ValueChanged(object sender, FindEntityValueChangedEventArgs e)
        {
            if (!AssessmentFormIdentifier.HasValue)
                return;

            var formId = AssessmentFormIdentifier.Value.Value;

            if (IsAssessmentFormAssignedToActivity(formId, e.OldValue))
                return;

            Course2Store.ConnectCourseActivityAssessmentForm(CourseIdentifier, ActivityIdentifier, formId);

            var activity = CourseSearch.SelectActivity(ActivityIdentifier);

            BindModelToControls(activity);
        }

        private bool IsAssessmentFormAssignedToActivity(Guid form, Guid? oldForm)
        {
            if (!CourseSearch.ActivityExists(x => x.AssessmentFormIdentifier == form))
                return false;

            var entity = ServiceLocator.BankSearch.GetForm(form);

            AssessmentTabStatus.AddMessage(
                AlertType.Error,
                $"<strong>{entity?.FormName}</strong> is already assigned to another course activity.");
            AssessmentFormIdentifier.Value = oldForm;

            return true;
        }

        #endregion

        #region Helpers

        private Form GetForm(Guid? formId) =>
            formId.HasValue ? ServiceLocator.BankSearch.GetFormData(formId.Value) : null;

        private void SetQuestionsJson(IEnumerable<QuestionJson> data) =>
            QuestionsData.Value = JsonConvert.SerializeObject(data);

        private List<QuestionJson> GetQuestionsJson() =>
            JsonConvert.DeserializeObject<List<QuestionJson>>(QuestionsData.Value);

        #endregion
    }
}