using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using Shift.Common.Timeline.Commands;

using InSite.Application.Courses.Read;
using InSite.Application.Courses.Write;
using InSite.Common.Web.UI;
using InSite.Domain.Banks;
using InSite.Domain.Courses;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Assessments.Questions.Controls
{
    public partial class QuestionPrerequisiteRepeater : BaseUserControl
    {
        #region Classes

        [Serializable]
        private class ItemInfo
        {
            public const TriggerType DefaultTriggerType = TriggerType.AssessmentQuestion;

            public Guid CoursePrerequisiteIdentifier { get; set; }
            public Guid CourseIdentifier { get; set; }
            public Guid ObjectIdentifier { get; set; }
            public Guid TriggerIdentifier { get; set; }
            public TriggerChange TriggerChange { get; set; }

            public static ItemInfo FromEntity(QCoursePrerequisite entity) => new ItemInfo
            {
                CoursePrerequisiteIdentifier = entity.CoursePrerequisiteIdentifier,
                CourseIdentifier = entity.CourseIdentifier,
                ObjectIdentifier = entity.ObjectIdentifier,
                TriggerIdentifier = entity.TriggerIdentifier,
                TriggerChange = entity.TriggerChange.ToEnum<TriggerChange>(),
            };

            public bool Equal(QCoursePrerequisite entity)
            {
                return CoursePrerequisiteIdentifier == entity.CoursePrerequisiteIdentifier
                    && CourseIdentifier == entity.CourseIdentifier
                    && ObjectIdentifier == entity.ObjectIdentifier
                    && TriggerIdentifier == entity.TriggerIdentifier
                    && TriggerChange == entity.TriggerChange.ToEnum<TriggerChange>()
                    && DefaultTriggerType == entity.TriggerType.ToEnum<TriggerType>();
            }
        }

        #endregion

        #region Properties

        public Guid QuestionIdentifier
        {
            get => (Guid)ViewState[nameof(QuestionIdentifier)];
            set => ViewState[nameof(QuestionIdentifier)] = value;
        }

        private List<ItemInfo> Prerequisites
        {
            get => (List<ItemInfo>)ViewState[nameof(Prerequisites)];
            set => ViewState[nameof(Prerequisites)] = value;
        }

        #endregion

        #region Initialization and Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PrerequisiteRepeater.ItemCreated += PrerequisiteRepeater_ItemCreated;
            PrerequisiteRepeater.ItemDataBound += PrerequisiteRepeater_ItemDataBound;
            PrerequisiteRepeater.ItemCommand += PrerequisiteRepeater_ItemCommand;

            AddPrerequisiteButton.Click += AddPrerequisiteButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (Prerequisites == null)
                return;

            for (var i = 0; i < Prerequisites.Count; i++)
            {
                var item = PrerequisiteRepeater.Items[i];

                var activity = (FindActivity)item.FindControl("ActivitySelector");
                if (!activity.Value.HasValue)
                    continue;

                var id = GetKey(item);
                var info = Prerequisites.FirstOrDefault(x => x.CoursePrerequisiteIdentifier == id);
                if (info == null)
                    continue;

                var score = (ComboBox)item.FindControl("QuestionScore");
                info.TriggerChange = score.Value.ToEnum<TriggerChange>();

                if (info.ObjectIdentifier != activity.Value.Value)
                {
                    var courseId = ServiceLocator.CourseSearch.GetCourseIdByActivityId(activity.Value.Value);

                    if (courseId.HasValue)
                    {
                        info.CourseIdentifier = courseId.Value;
                        info.ObjectIdentifier = activity.Value.Value;
                    }
                }
            }
        }

        protected override void SetupValidationGroup(string groupName)
        {
            if (!IsBaseControlLoaded)
                return;

            for (var i = 0; i < PrerequisiteRepeater.Items.Count; i++)
                SetupRepeaterItemValidator(PrerequisiteRepeater.Items[i], groupName);
        }

        private void SetupRepeaterItemValidator(RepeaterItem item, string groupName)
        {
            var activitySelectorRequiredValidator = (BaseValidator)item.FindControl("ActivitySelectorRequiredValidator");
            activitySelectorRequiredValidator.ValidationGroup = groupName;
        }

        #endregion

        #region Event handlers

        private void AddPrerequisiteButton_Click(object sender, EventArgs e)
        {
            Prerequisites.Add(new ItemInfo
            {
                CoursePrerequisiteIdentifier = UniqueIdentifier.Create(),
                TriggerChange = TriggerChange.QuestionAnsweredIncorrectly,
                TriggerIdentifier = QuestionIdentifier
            });
            BindPrerequisiteRepeater();
        }

        private void PrerequisiteRepeater_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            SetupRepeaterItemValidator(e.Item, ValidationGroup);
        }

        private void PrerequisiteRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var prerequisite = (ItemInfo)e.Item.DataItem;

            var activityInput = (FindActivity)e.Item.FindControl("ActivitySelector");
            activityInput.Value = prerequisite.ObjectIdentifier;

            var scoreInput = (ComboBox)e.Item.FindControl("QuestionScore");
            scoreInput.Value = prerequisite.TriggerChange.GetName();
        }

        private void PrerequisiteRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                var id = GetKey(e.Item);
                var item = Prerequisites.FirstOrDefault(x => x.CoursePrerequisiteIdentifier == id);

                if (item != null)
                {
                    Prerequisites.Remove(item);
                    BindPrerequisiteRepeater();
                }
            }
        }

        #endregion

        #region Public methods

        public void LoadData(Question question)
        {
            QuestionIdentifier = question.Identifier;
            Prerequisites = ServiceLocator.CourseSearch.GetPrerequisitesByTrigger(question.Identifier)
                .Select(x => ItemInfo.FromEntity(x)).ToList();

            BindPrerequisiteRepeater();
        }

        public Command[] GetCommands(Question question)
        {
            if (question == null)
                throw new ArgumentNullException(nameof(question));

            var commands = new List<Command>();

            var existing = ServiceLocator.CourseSearch.GetPrerequisitesByTrigger(question.Identifier);

            var deletes = existing
                .Where(x => !Prerequisites.Any(y => y.CoursePrerequisiteIdentifier == x.CoursePrerequisiteIdentifier && y.Equal(x)))
                .ToList();

            foreach (var p in deletes)
                commands.Add(new RemoveCourseActivityPrerequisite(p.CourseIdentifier, p.ObjectIdentifier, p.CoursePrerequisiteIdentifier));

            var inserts = Prerequisites
                .Where(x => !existing.Any(y => y.CoursePrerequisiteIdentifier == x.CoursePrerequisiteIdentifier && x.Equal(y)))
                .ToList();

            foreach (var p in inserts)
            {
                commands.Add(new AddCourseActivityPrerequisite(p.CourseIdentifier, p.ObjectIdentifier, new Prerequisite
                {
                    Identifier = p.CoursePrerequisiteIdentifier,
                    TriggerIdentifier = p.TriggerIdentifier,
                    TriggerType = ItemInfo.DefaultTriggerType,
                    TriggerChange = p.TriggerChange
                }));
            }

            return commands.ToArray();
        }

        #endregion

        #region Helper methods

        private Guid GetKey(RepeaterItem item)
        {
            var identifier = (ITextControl)item.FindControl("PrerequisiteIdentifier");
            return Guid.Parse(identifier.Text);
        }

        private void BindPrerequisiteRepeater()
        {
            PrerequisiteRepeater.Visible = Prerequisites.Count > 0;
            PrerequisiteRepeater.DataSource = Prerequisites;
            PrerequisiteRepeater.DataBind();
        }

        #endregion
    }
}