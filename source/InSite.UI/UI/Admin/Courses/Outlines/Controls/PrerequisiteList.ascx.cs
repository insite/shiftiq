using System;
using System.Linq;
using System.Web.UI.WebControls;

using Shift.Common.Timeline.Commands;

using InSite.Application.Courses.Read;
using InSite.Application.Courses.Write;
using InSite.Common.Web.UI;
using InSite.Domain.Courses;
using InSite.Persistence;

using Shift.Common;

using TriggerChangeEnum = InSite.Domain.Courses.TriggerChange;

namespace InSite.Admin.Courses.Outlines.Controls
{
    public partial class PrerequisiteList : BaseUserControl
    {
        public void BindModelToControls(
            Guid course,
            PrerequisiteObjectType objectType,
            Guid containerId,
            string determiner,
            string validationGroup
            )
        {
            CourseIdentifier = course;
            ObjectType = objectType;
            ContainerIdentifier = containerId;

            TriggerActivityIdentifierValidator.ValidationGroup = validationGroup;
            TriggerAssessmentFormIdentifierValidator.ValidationGroup = validationGroup;
            TriggerAssessmentQuestionIdentifierValidator.ValidationGroup = validationGroup;
            TriggerGradeItemIdentifierValidator.ValidationGroup = validationGroup;

            PrerequisiteRepeater.DataBind();
            SetPrerequisiteDeterminer(determiner);
        }

        public Guid CourseIdentifier
        {
            get => (Guid)ViewState[nameof(CourseIdentifier)];
            set
            {
                ViewState[nameof(CourseIdentifier)] = value;
                TriggerActivityIdentifier.CourseIdentifier = value;
            }
        }

        public Guid ContainerIdentifier
        {
            get => (Guid)ViewState[nameof(ContainerIdentifier)];
            set => ViewState[nameof(ContainerIdentifier)] = value;
        }

        private PrerequisiteObjectType ObjectType
        {
            get => (PrerequisiteObjectType)ViewState[nameof(ObjectType)];
            set => ViewState[nameof(ObjectType)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            TriggerChange.AutoPostBack = true;
            TriggerChange.ValueChanged += TriggerChange_ValueChanged;

            TriggerAssessmentBankIdentifier.AutoPostBack = true;
            TriggerAssessmentBankIdentifier.ValueChanged += TriggerAssessmentBankIdentifier_ValueChanged;

            PrerequisiteRepeater.ItemCommand += PrerequisiteRepeater_ItemCommand;
            PrerequisiteRepeater.DataBinding += PrerequisiteRepeater_DataBinding;
        }

        private void PrerequisiteRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName != "PrerequisiteDelete")
                return;

            var id = Guid.Parse((string)e.CommandArgument);

            ICommand command;

            switch (ObjectType)
            {
                case PrerequisiteObjectType.Unit:
                    command = new RemoveCourseUnitPrerequisite(CourseIdentifier, ContainerIdentifier, id);
                    break;
                case PrerequisiteObjectType.Module:
                    command = new RemoveCourseModulePrerequisite(CourseIdentifier, ContainerIdentifier, id);
                    break;
                case PrerequisiteObjectType.Activity:
                    command = new RemoveCourseActivityPrerequisite(CourseIdentifier, ContainerIdentifier, id);
                    break;
                default:
                    throw new ArgumentException($"Unsupported objectType: {ObjectType}");
            }

            ServiceLocator.SendCommand(command);

            PrerequisiteRepeater.DataBind();
        }

        private void TriggerChange_ValueChanged(object sender, EventArgs e)
        {
            var type = TriggerChange.Value;

            TriggerActivityField.Visible = type == "ActivityCompleted";

            TriggerAssessmentFormIdentifier.Filter.OrganizationIdentifier = Organization.Identifier;
            TriggerFormField.Visible = type.StartsWith("Assessment");
            TriggerConditionField.Visible = type == "AssessmentScored";

            TriggerAssessmentBankIdentifier.Filter.OrganizationIdentifier = Organization.Identifier;
            TriggerBankField.Visible = type.StartsWith("Question");
            TriggerQuestionField.Visible = type.StartsWith("Question");

            var course = CourseSearch.SelectCourse(CourseIdentifier);
            if (course.GradebookIdentifier.HasValue)
            {
                TriggerGradeItemIdentifier.GradebookIdentifier = course.GradebookIdentifier.Value;
                TriggerGradeItemIdentifier.RefreshData();
                TriggerGradeItemField.Visible = type.StartsWith("Grade");
            }
        }

        private void TriggerAssessmentBankIdentifier_ValueChanged(object sender, EventArgs e)
        {
            TriggerAssessmentQuestionIdentifier.Filter.BankIdentifier = TriggerAssessmentBankIdentifier.Value;
        }

        public void SaveChanges()
        {
            if (TriggerChange.Value == "None")
                return;

            TriggerType type;
            Guid? triggerId;
            int? scoreFrom = null, scoreThru = null;

            var triggerChange = (TriggerChangeEnum)Enum.Parse(typeof(TriggerChangeEnum), TriggerChange.Value, true);
            switch (triggerChange)
            {
                case TriggerChangeEnum.ActivityCompleted:
                    type = TriggerType.Activity;
                    triggerId = TriggerActivityIdentifier.ValueAsGuid.Value;
                    break;
                case TriggerChangeEnum.AssessmentFailed:
                case TriggerChangeEnum.AssessmentPassed:
                case TriggerChangeEnum.AssessmentScored:
                    type = TriggerType.AssessmentForm;
                    triggerId = TriggerAssessmentFormIdentifier.Value.Value;
                    scoreFrom = TriggerConditionScoreFrom.ValueAsInt;
                    scoreThru = TriggerConditionScoreThru.ValueAsInt;
                    break;
                case TriggerChangeEnum.QuestionAnsweredCorrectly:
                case TriggerChangeEnum.QuestionAnsweredIncorrectly:
                    type = TriggerType.AssessmentQuestion;
                    triggerId = TriggerAssessmentQuestionIdentifier.Value.Value;
                    break;
                case TriggerChangeEnum.GradeItemFailed:
                case TriggerChangeEnum.GradeItemPassed:
                    type = TriggerType.GradeItem;
                    triggerId = TriggerGradeItemIdentifier.ValueAsGuid.Value;
                    break;
                default:
                    return;
            }

            if (triggerId == null)
                return;

            AddPrerequisite(new Prerequisite
            {
                Identifier = UniqueIdentifier.Create(),
                TriggerIdentifier = triggerId.Value,
                TriggerType = type,
                TriggerChange = (TriggerChangeEnum)Enum.Parse(typeof(TriggerChangeEnum), TriggerChange.Value, true),
                TriggerConditionScoreFrom = scoreFrom,
                TriggerConditionScoreThru = scoreThru
            });
        }

        private void AddPrerequisite(Prerequisite p)
        {
            ICommand command;

            switch (ObjectType)
            {
                case PrerequisiteObjectType.Unit:
                    command = new AddCourseUnitPrerequisite(CourseIdentifier, ContainerIdentifier, p);
                    break;
                case PrerequisiteObjectType.Module:
                    command = new AddCourseModulePrerequisite(CourseIdentifier, ContainerIdentifier, p);
                    break;
                case PrerequisiteObjectType.Activity:
                    command = new AddCourseActivityPrerequisite(CourseIdentifier, ContainerIdentifier, p);
                    break;
                default:
                    throw new ArgumentException($"Unsupported objectType: {ObjectType}");
            }

            ServiceLocator.SendCommand(command);
        }

        private void PrerequisiteRepeater_DataBinding(object sender, EventArgs e)
        {
            var prerequisites = CourseSearch.GetPrerequisites(ContainerIdentifier);

            var dataSource = prerequisites
                .Select(x => new
                {
                    PrerequisiteIdentifier = x.PrerequisiteIdentifier,
                    TriggerChange = x.TriggerChange,
                    TriggerDescription = x.TriggerDescription,
                    TriggerUrl = GetTriggerUrl(x),
                    TriggerUrlTarget = string.Equals(x.TriggerType, "Activity", StringComparison.OrdinalIgnoreCase) ? null : "_blank"
                })
                .ToList();

            PrerequisiteRepeater.DataSource = dataSource;
            PrerequisiteRepeaterField.Visible = dataSource.Count > 0;
            PrerequisiteDeterminer.Visible = dataSource.Count > 1;
        }

        private string GetTriggerUrl(CourseSearch.TPrerequisiteSearchResult item)
        {
            if (!Enum.TryParse<TriggerType>(item.TriggerType, true, out var triggerType))
                throw new ApplicationError($"Unknown trigger type: ${item.TriggerType}");

            switch (triggerType)
            {
                case TriggerType.Activity:
                    return $"/ui/admin/courses/manage?course={CourseIdentifier}&activity={item.TriggerIdentifier}";

                case TriggerType.AssessmentForm:
                    var bankId1 = ServiceLocator.BankSearch.GetForm(item.TriggerIdentifier)?.BankIdentifier;
                    return $"/ui/admin/assessments/banks/outline?bank={bankId1}&form={item.TriggerIdentifier}";

                case TriggerType.AssessmentQuestion:
                    var bankId2 = ServiceLocator.BankSearch.GetQuestion(item.TriggerIdentifier)?.BankIdentifier;
                    return $"/ui/admin/assessments/banks/outline?bank={bankId2}&question={item.TriggerIdentifier}";

                case TriggerType.GradeItem:
                    var gradebookId = ServiceLocator.RecordSearch.GetGradeItem(item.TriggerIdentifier)?.GradebookIdentifier;
                    return $"/ui/admin/records/items/change-score?gradebook={gradebookId}&item={item.TriggerIdentifier}";

                default:
                    return null;
            }
        }

        public string GetPrerequisiteDeterminer()
        {
            return PrerequisiteDeterminerAll.Checked ? "All" : "Any";
        }

        public void SetPrerequisiteDeterminer(string determiner)
        {
            if (determiner == "All")
                PrerequisiteDeterminerAll.Checked = true;
            else
                PrerequisiteDeterminerAny.Checked = true;
        }
    }
}