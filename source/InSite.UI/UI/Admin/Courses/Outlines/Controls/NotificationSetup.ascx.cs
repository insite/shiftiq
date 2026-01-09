using System;

using InSite.Common.Web.UI;
using InSite.Domain.CourseObjects;
using InSite.Persistence;

using Shift.Common;

namespace InSite.UI.Admin.Courses.Outlines.Controls
{
    public partial class NotificationSetup : BaseUserControl
    {
        private Guid CourseIdentifier
        {
            get => (Guid)ViewState[nameof(CourseIdentifier)];
            set => ViewState[nameof(CourseIdentifier)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var course = ServiceLocator.CourseSearch.GetCourse(CourseIdentifier);

            course.StalledToLearnerMessageIdentifier = StalledToLearnerMessageIdentifier.Value;
            course.StalledToAdministratorMessageIdentifier = StalledToAdministratorMessageIdentifier.Value;
            course.CompletedToLearnerMessageIdentifier = CompletedToLearnerMessageIdentifier.Value;
            course.CompletedToAdministratorMessageIdentifier = CompletedToAdministratorMessageIdentifier.Value;
            course.SendMessageStalledAfterDays = SendMessageStalledAfterDays.ValueAsInt;
            course.SendMessageStalledMaxCount = SendMessageStalledMaxCount.ValueAsInt;

            Course2Store.UpdateCourse(course, null);
        }

        public void BindModelToControls(Course course)
        {
            CourseIdentifier = course.Identifier;

            StalledToLearnerMessageIdentifier.Filter.Type = MessageTypeName.Notification;
            StalledToLearnerMessageIdentifier.Value = course.CourseMessageStalledToLearner;

            StalledToAdministratorMessageIdentifier.Filter.Type = MessageTypeName.Notification;
            StalledToAdministratorMessageIdentifier.Value = course.CourseMessageStalledToAdministrator;

            CompletedToLearnerMessageIdentifier.Filter.Type = MessageTypeName.Notification;
            CompletedToLearnerMessageIdentifier.Value = course.CourseMessageCompletedToLearner;

            CompletedToAdministratorMessageIdentifier.Filter.Type = MessageTypeName.Notification;
            CompletedToAdministratorMessageIdentifier.Value = course.CourseMessageCompletedToAdministrator;

            SendMessageStalledAfterDays.ValueAsInt = course.SendMessageStalledAfterDays;
            SendMessageStalledMaxCount.ValueAsInt = course.SendMessageStalledMaxCount;

            ReminderCompletionActivity.Visible = !course.CompletionActivityIdentifier.HasValue;
        }
    }
}