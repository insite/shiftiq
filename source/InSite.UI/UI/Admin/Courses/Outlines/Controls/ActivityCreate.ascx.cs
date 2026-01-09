using System;
using System.Collections.Generic;

using Shift.Common.Timeline.Commands;

using InSite.Admin.Courses.Courses;
using InSite.Application.Banks.Write;
using InSite.Application.Courses.Write;
using InSite.Common.Web.UI;
using InSite.Domain.Banks;
using InSite.Domain.Foundations;
using InSite.Persistence;

using Shift.Common;
using Shift.Common.Events;
using Shift.Constant;
using Shift.Sdk.UI;

using ActivityTypeEnum = InSite.Domain.Courses.ActivityType;

namespace InSite.Admin.Courses.Outlines.Controls
{
    public partial class ActivityCreate : BaseUserControl
    {
        #region Events

        public event GuidValueHandler SaveClicked;

        private void OnSaveClicked()
        {
            try
            {
                ActivityCreateError.Visible = false;

                if (!Page.IsValid)
                    return;

                var courseCommands = new List<ICommand>();
                var bankCommands = new List<ICommand>();

                var unitId = GetOrCreateUnit(courseCommands);
                var moduleId = GetOrCreateModule(unitId, courseCommands);
                var activityId = CreateActivity(moduleId, courseCommands, bankCommands);

                ServiceLocator.SendCommands(bankCommands);
                ServiceLocator.SendCommand(new RunCommands(CourseIdentifier.Value, courseCommands.ToArray()));

                DomainCache.Instance.RemoveCourse(CourseIdentifier.Value);

                SaveClicked?.Invoke(this, new GuidValueArgs(activityId));
            }
            catch (ApplicationError apperr)
            {
                ActivityCreateError.Visible = true;
                ActivityCreateError.InnerHtml = apperr.Message;
            }
        }

        public event EventHandler CancelClicked;

        private void OnCancelClicked() => CancelClicked?.Invoke(this, EventArgs.Empty);

        #endregion

        private ActivityTypeEnum ActivityType
        {
            get => (ActivityTypeEnum)ViewState[nameof(ActivityType)];
            set => ViewState[nameof(ActivityType)] = value;
        }

        public Guid? CourseIdentifier
        {
            get => (Guid?)ViewState[nameof(CourseIdentifier)];
            set => ViewState[nameof(CourseIdentifier)] = value;
        }

        public Guid? ModuleIdentifier
        {
            get => (Guid?)ViewState[nameof(ModuleIdentifier)];
            set => ViewState[nameof(ModuleIdentifier)] = value;
        }

        public Guid? UnitIdentifier
        {
            get => (Guid?)ViewState[nameof(UnitIdentifier)];
            set => ViewState[nameof(UnitIdentifier)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            LinkType.AutoPostBack = true;
            LinkType.ValueChanged += (sender, args) =>
            {
                LinkField.Visible = LinkType.Value != "SCORM";
                ActivityPlatform.Visible = LinkType.Value == "SCORM";
            };

            AssessmentFormIdentifier.AutoPostBack = true;
            AssessmentFormIdentifier.ValueChanged += AssessmentFormIdentifier_ValueChanged;

            SurveyFormIdentifier.AutoPostBack = true;
            SurveyFormIdentifier.ValueChanged += SurveyFormIdentifier_ValueChanged;

            SaveButton.Click += SaveButton_Click;
            CancelButton.Click += CancelButton_Click;
        }

        private void AssessmentFormIdentifier_ValueChanged(object sender, EventArgs e)
        {
            AssessmentFormError.Visible = false;

            if (!AssessmentFormIdentifier.HasValue)
                return;

            var isAlreadyAssigned = IsAssessmentFormAssignedToActivity(AssessmentFormIdentifier.Value.Value);

            AssessmentFormError.Visible = isAlreadyAssigned;

            if (isAlreadyAssigned)
                return;

            var form = ServiceLocator.BankSearch.GetForm(AssessmentFormIdentifier.Value.Value);
            if (form.FormPassingScore.HasValue)
                PassingScore.ValueAsDecimal = 100m * form.FormPassingScore.Value;
        }

        private bool IsAssessmentFormAssignedToActivity(Guid form)
        {
            if (!CourseSearch.ActivityExists(x => x.AssessmentFormIdentifier == form))
                return false;

            var entity = ServiceLocator.BankSearch.GetForm(form);

            AssessmentFormError.InnerHtml = $"<strong>{entity?.FormName}</strong> is already assigned to another course activity.";
            AssessmentFormIdentifier.Value = null;

            return true;
        }

        private void SurveyFormIdentifier_ValueChanged(object sender, FindEntityValueChangedEventArgs e)
        {
            SurveyFormError.Visible = false;

            if (!e.NewValue.HasValue)
                return;

            if (!CourseSearch.ActivityExists(x => x.SurveyFormIdentifier == e.NewValue.Value))
                return;

            var entity = ServiceLocator.SurveySearch.GetSurveyForm(e.NewValue.Value);

            SurveyFormError.Visible = true;
            SurveyFormError.InnerHtml = $"<strong>{entity?.SurveyFormName}</strong> is already assigned to another course activity.";

            SurveyFormIdentifier.Value = null;
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            AssessmentFormField.Style["display"] = AssessmentType.SelectedValue == "New" ? "none" : null;
            QuestionCountField.Style["display"] = AssessmentType.SelectedValue == "Existing" ? "none" : null;

            AssessmentType.Attributes["onclick"] =
                $"var $selectorField = $('#{AssessmentFormField.ClientID}');" +
                $"var $countField = $('#{QuestionCountField.ClientID}');" +
                "var isNew = $(this).find(':checked').val() === 'New';" +
                $"ValidatorEnable(document.getElementById('{AssessmentFormValidator.ClientID}'), !isNew);" +
                "if(isNew) {{ $selectorField.hide(); $countField.show(); }} else {{ $selectorField.show(); $countField.hide(); }}";

            AssessmentFormValidator.Enabled = AssessmentType.SelectedValue == "Existing";
        }

        private void SaveButton_Click(object sender, EventArgs e) => OnSaveClicked();

        private void CancelButton_Click(object sender, EventArgs e) => OnCancelClicked();

        public void BindModelToControls(Guid courseIdentifier, Guid? unitIdentifier, Guid? moduleIdentifier, string activityType)
        {
            CourseIdentifier = courseIdentifier;
            UnitIdentifier = unitIdentifier;
            ModuleIdentifier = moduleIdentifier;
            ActivityType = Enum.TryParse<ActivityTypeEnum>(activityType, true, out var activityTypeEnum) ? activityTypeEnum : ActivityTypeEnum.Lesson;

            var createUnit = unitIdentifier == null;
            var createModule = moduleIdentifier == null;

            ActivityCreateHeading.Text =
                createUnit
                ? $"New {ActivityType} in New Unit"
                    : createModule
                    ? $"New {ActivityType} in New Module"
                    : $"New {ActivityType}";

            var isAssessment = ActivityType == ActivityTypeEnum.Assessment;
            var isSurvey = ActivityType == ActivityTypeEnum.Survey;
            var isLink = ActivityType == ActivityTypeEnum.Link;
            var isVideo = ActivityType == ActivityTypeEnum.Video;
            var isQuiz = ActivityType == ActivityTypeEnum.Quiz;

            UnitNameField.Visible = createUnit;
            ModuleNameField.Visible = createModule;
            ActivityNameLabel.Text = $"{ActivityType} Name";
            ActivityName.Text = $"New {ActivityType}";
            ActivityNameField.Visible = !isSurvey;
            AssessmentInfo.Visible = isAssessment;
            AssessmentField.Visible = isAssessment;
            LinkTypeField.Visible = isLink;
            LinkField.Visible = isLink;
            SurveyField.Visible = isSurvey;
            VideoUrlField.Visible = isVideo;
            VideoTargetField.Visible = isVideo;
            QuizField.Visible = isQuiz;

            ActivityNameField.Visible = !isQuiz;

            ModuleName.Text = null;
            ActivityName.Text = null;
            AssessmentFormIdentifier.Value = null;
            NavigateUrl.Text = null;
            NavigateUrlTarget.Checked = false;
            LinkType.ClearSelection();
            SurveyFormIdentifier.Value = null;
            QuizIdentifier.Value = null;
        }

        public Guid GetOrCreateUnit(List<ICommand> courseCommands)
        {
            if (UnitIdentifier.HasValue)
                return UnitIdentifier.Value;

            var unit = EntityHelper.CreateUnit(CourseIdentifier.Value, UnitName.Text);

            UnitCommandCreator.Create(null, null, unit, GetContent(unit.UnitName), courseCommands);

            return unit.UnitIdentifier;
        }

        public Guid GetOrCreateModule(Guid unitId, List<ICommand> courseCommands)
        {
            if (ModuleIdentifier.HasValue)
                return ModuleIdentifier.Value;

            var module = EntityHelper.CreateModule(unitId, ModuleName.Text);

            ModuleCommandCreator.Create(CourseIdentifier.Value, null, null, module, GetContent(module.ModuleName), courseCommands);

            return module.ModuleIdentifier;
        }

        public Guid CreateActivity(Guid moduleId, List<ICommand> courseCommands, List<ICommand> bankCommands)
        {
            var passingScore = (PassingScore.ValueAsDecimal ?? 50) / 100.0M;

            var activity = EntityHelper.CreateActivity(moduleId, ActivityType, ActivityName.Text);

            if (ActivityType == ActivityTypeEnum.Assessment)
            {
                bool isNew = AssessmentType.SelectedValue == "New";
                activity.AssessmentFormIdentifier = isNew
                    ? CreateAssessment(bankCommands, passingScore)
                    : AssessmentFormIdentifier.Value;
            }
            else if (ActivityType == ActivityTypeEnum.Link)
            {
                activity.ActivityUrl = NavigateUrl.Text;
                activity.ActivityUrlTarget = NavigateUrlTarget.Checked ? "_blank" : "_self";
                activity.ActivityUrlType = LinkType.Value;

                if (activity.ActivityUrl.StartsWith("http://") || activity.ActivityUrl.StartsWith("https://") || activity.ActivityUrl.StartsWith("mailto:"))
                    activity.ActivityUrlType = "External";
                else if (LinkType.Value == "SCORM")
                {
                    activity.ActivityUrlType = "SCORM";
                    activity.ActivityMode = "Preview";
                    activity.ActivityUrl = $"/ui/portal/integrations/scorm/launch/{activity.ActivityIdentifier}";
                    activity.ActivityPlatform = ActivityPlatform.Value;
                }
                else
                    activity.ActivityUrlType = "Internal";
            }
            else if (ActivityType == ActivityTypeEnum.Survey && SurveyFormIdentifier.HasValue)
            {
                var formId = SurveyFormIdentifier.Value.Value;

                var form = ServiceLocator.SurveySearch.GetSurveyForm(formId);
                if (form != null)
                {
                    activity.SurveyFormIdentifier = formId;
                    activity.ActivityName = form.SurveyFormName;
                }
            }
            else if (ActivityType == ActivityTypeEnum.Video)
            {
                activity.ActivityUrl = VideoUrl.Text;
                activity.ActivityUrlTarget = VideoTarget.Value;
            }
            else if (ActivityType == ActivityTypeEnum.Quiz)
            {
                var option = QuizIdentifier.GetSelectedOption();
                if (option != null)
                {
                    activity.QuizIdentifier = Guid.Parse(option.Value);
                    activity.ActivityName = option.Text;
                }
            }

            ActivityCommandCreator.Create(CourseIdentifier.Value, null, null, activity, GetContent(activity.ActivityName), courseCommands);

            return activity.ActivityIdentifier;
        }

        private static ContentContainer GetContent(string title)
        {
            var content = new ContentContainer();
            content.Title.Text.Default = title;
            return content;
        }

        private Guid CreateAssessment(List<ICommand> commands, decimal passingScore)
        {
            var formId = UniqueIdentifier.Create();
            var name = ActivityName.Text;

            var set = CreateSet();

            var criterion = new Criterion();
            {
                criterion.Identifier = UniqueIdentifier.Create();
                criterion.FilterType = CriterionFilterType.All;
                criterion.QuestionLimit = 0;
                criterion.SetIdentifiers.Add(set.Identifier);
                criterion.Sets.Add(set);
            }

            var section = new Section();
            {
                section.Identifier = UniqueIdentifier.Create();
                section.CriterionIdentifier = criterion.Identifier;
                section.Criterion = criterion;

                foreach (var question in set.Questions)
                {
                    section.Fields.Add(new Field
                    {
                        Identifier = UniqueIdentifier.Create(),
                        QuestionIdentifier = question.Identifier,
                        Question = question
                    });
                }
            }

            var form = new Form();
            {
                form.Identifier = formId;
                form.Name = name;
                form.Content.Title.Default = name;
                form.Invigilation.AttemptLimit = 0;
                form.Invigilation.TimeLimit = 60;
                form.Invigilation.AttemptLimitPerSession = 3;
                form.Invigilation.TimeLimitPerSession = 60;
                form.Invigilation.TimeLimitPerLockout = 1440;

                form.Sections.Add(section);
                section.Form = form;
            }

            var spec = new Specification();
            {
                spec.Identifier = UniqueIdentifier.Create();
                spec.Type = SpecificationType.Dynamic;
                spec.Name = name;
                spec.Consequence = ConsequenceType.Medium;
                spec.FormLimit = 1;
                spec.QuestionLimit = set.Questions.Count < 10 ? set.Questions.Count : 10;

                spec.Calculation.Disclosure = DisclosureType.Full;
                spec.Calculation.PassingScore = passingScore;
                spec.Calculation.SuccessWeight = 1M;
                spec.Calculation.FailureWeight = 1M;

                spec.Criteria.Add(criterion);
                criterion.Specification = spec;

                spec.Forms.Add(form);
                form.Specification = spec;
            }

            var today = DateTime.Today;

            var bank = new BankState();
            {
                bank.Identifier = UniqueIdentifier.Create();
                bank.Name = name;
                bank.Content.Title.Default = name;
                bank.Tenant = Organization.OrganizationIdentifier;
                bank.Edition = new Edition(today.Year, today.Month);

                bank.Sets.Add(set);
                set.Bank = bank;

                bank.Specifications.Add(spec);
                spec.Bank = bank;
            }

            BankHelper.AssignAssetNumbers(bank, count => Sequence.IncrementMany(Organization.OrganizationIdentifier, SequenceType.Asset, count));

            commands.Add(new OpenBank(bank));

            var publication = new FormPublication
            {
                AllowFeedback = false,
                AllowRationaleForCorrectAnswers = false,
                AllowRationaleForIncorrectAnswers = false,
                AllowDownloadAssessmentsQA = false,
                FirstPublished = DateTimeOffset.Now
            };
            commands.Add(new PublishForm(bank.Identifier, form.Identifier, publication));

            return formId;
        }

        private Set CreateSet()
        {
            var set = new Set
            {
                Identifier = UniqueIdentifier.Create(),
                Name = "Draft Set"
            };
            set.Randomization.Enabled = true;

            var optionNumber = 1;
            var questionCount = QuestionCount.ValueAsInt.Value;

            for (int i = 0; i < questionCount; i++)
            {
                var question = new Question
                {
                    Type = QuestionItemType.SingleCorrect,
                    Identifier = UniqueIdentifier.Create(),
                    CalculationMethod = QuestionCalculationMethod.Default,
                    Points = 1,
                    Options = new List<Option>(),
                };

                question.Content.Title.Default = $"Question {i + 1}";

                for (int k = 0; k < 4; k++)
                {
                    var option = new Option
                    {
                        Points = 0m,
                        Number = optionNumber++
                    };
                    option.Content.Title.Default = $"Option {k + 1}";

                    question.Options.Add(option);
                }

                set.Questions.Add(question);
                question.Set = set;
            }

            return set;
        }
    }
}