using System;
using System.Web.UI.WebControls;

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

            CompletedNotificationValidator.ServerValidate += CompletedNotificationValidator_ServerValidate;

            SaveButton.Click += SaveButton_Click;
        }

        private void CompletedNotificationValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = CourseMessageCompletedToLearner.Value == null
                    && CourseMessageCompletedToAdministrator.Value == null
                || CompletionActivityIdentifier.ValueAsGuid.HasValue;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var course = ServiceLocator.CourseSearch.GetCourse(CourseIdentifier);

            course.StalledToLearnerMessageIdentifier = CourseMessageStalledToLearner.Value;
            course.StalledToAdministratorMessageIdentifier = CourseMessageStalledToAdministrator.Value;
            course.CompletedToLearnerMessageIdentifier = CourseMessageCompletedToLearner.Value;
            course.CompletedToAdministratorMessageIdentifier = CourseMessageCompletedToAdministrator.Value;
            course.SendMessageStalledAfterDays = SendMessageStalledAfterDays.ValueAsInt;
            course.SendMessageStalledMaxCount = SendMessageStalledMaxCount.ValueAsInt;
            course.CompletionActivityIdentifier = CompletionActivityIdentifier.ValueAsGuid;

            Course2Store.UpdateCourse(course, null);
        }

        public void BindModelToControls(Course course)
        {
            CourseIdentifier = course.Identifier;

            CompletionActivityIdentifier.CourseIdentifier = course.Identifier;
            CompletionActivityIdentifier.RefreshData();
            CompletionActivityIdentifier.ValueAsGuid = course.CompletionActivityIdentifier;

            CourseMessageStalledToLearner.Filter.Type = MessageTypeName.Notification;
            CourseMessageStalledToLearner.Value = course.CourseMessageStalledToLearner;

            CourseMessageStalledToAdministrator.Filter.Type = MessageTypeName.Notification;
            CourseMessageStalledToAdministrator.Value = course.CourseMessageStalledToAdministrator;

            CourseMessageCompletedToLearner.Filter.Type = MessageTypeName.Notification;
            CourseMessageCompletedToLearner.Value = course.CourseMessageCompletedToLearner;

            CourseMessageCompletedToAdministrator.Filter.Type = MessageTypeName.Notification;
            CourseMessageCompletedToAdministrator.Value = course.CourseMessageCompletedToAdministrator;

            SendMessageStalledAfterDays.ValueAsInt = course.SendMessageStalledAfterDays;
            SendMessageStalledMaxCount.ValueAsInt = course.SendMessageStalledMaxCount;
        }
    }
}