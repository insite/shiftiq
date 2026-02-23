using System;
using System.Web.UI.WebControls;

using InSite.Application.Achievements.Write;
using InSite.Application.Records.Read;
using InSite.Common.Web.UI;

using Shift.Common;
using Shift.Common.Events;
using Shift.Constant;

namespace InSite.UI.Admin.Records.Achievements.Controls
{
    public partial class OutlineNotifications : BaseUserControl
    {
        public event AlertHandler Alert;

        private void OnAlert(AlertType type, string message) =>
            Alert?.Invoke(this, new AlertArgs(type, message));

        private Guid AchievementIdentifier
        {
            get => (Guid)ViewState[nameof(AchievementIdentifier)];
            set => ViewState[nameof(AchievementIdentifier)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            BeforeExpiryNotificationTimingValidator.ServerValidate += BeforeExpiryNotificationTimingValidator_ServerValidate;
            AfterExpiryNotificationTimingValidator.ServerValidate += AfterExpiryNotificationTimingValidator_ServerValidate;

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            BeforeExpiryLearnerMessageIdentifier.Filter.Type = MessageTypeName.Alert;
            BeforeExpiryAdministratorMessageIdentifier.Filter.Type = MessageTypeName.Alert;
            AfterExpiryLearnerMessageIdentifier.Filter.Type = MessageTypeName.Alert;
            AfterExpiryAdministratorMessageIdentifier.Filter.Type = MessageTypeName.Alert;
        }

        protected override void SetupValidationGroup(string groupName)
        {
            BeforeExpiryNotificationTimingValidator.ValidationGroup = groupName;
            AfterExpiryNotificationTimingValidator.ValidationGroup = groupName;
            SaveButton.ValidationGroup = groupName;
        }

        private void BeforeExpiryNotificationTimingValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = BeforeExpiryNotificationTiming.HasValue
                || !BeforeExpiryLearnerMessageIdentifier.HasValue && !BeforeExpiryAdministratorMessageIdentifier.HasValue;
        }

        private void AfterExpiryNotificationTimingValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = AfterExpiryNotificationTiming.HasValue
                || !AfterExpiryLearnerMessageIdentifier.HasValue && !AfterExpiryAdministratorMessageIdentifier.HasValue;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            ServiceLocator.SendCommand(new ChangeAchievementNotification(
                AchievementIdentifier,
                new Domain.Records.NotificationSettings
                {
                    BeforeExpiryLearnerMessageIdentifier = BeforeExpiryLearnerMessageIdentifier.Value,
                    BeforeExpiryAdministratorMessageIdentifier = BeforeExpiryAdministratorMessageIdentifier.Value,
                    BeforeExpiryNotificationTiming = BeforeExpiryNotificationTiming.ValueAsInt,

                    AfterExpiryLearnerMessageIdentifier = AfterExpiryLearnerMessageIdentifier.Value,
                    AfterExpiryAdministratorMessageIdentifier = AfterExpiryAdministratorMessageIdentifier.Value,
                    AfterExpiryNotificationTiming = AfterExpiryNotificationTiming.ValueAsInt
                }));

            OnAlert(AlertType.Success, "Your changes have been saved.");
        }

        public void BindModelToControls(QAchievement achievement)
        {
            AchievementIdentifier = achievement.AchievementIdentifier;

            BeforeExpiryLearnerMessageIdentifier.Value = achievement.BeforeExpiryLearnerMessageIdentifier;
            BeforeExpiryAdministratorMessageIdentifier.Value = achievement.BeforeExpiryAdministratorMessageIdentifier;
            BeforeExpiryNotificationTiming.ValueAsInt = achievement.BeforeExpiryNotificationTiming;

            AfterExpiryLearnerMessageIdentifier.Value = achievement.AfterExpiryLearnerMessageIdentifier;
            AfterExpiryAdministratorMessageIdentifier.Value = achievement.AfterExpiryAdministratorMessageIdentifier;
            AfterExpiryNotificationTiming.ValueAsInt = achievement.AfterExpiryNotificationTiming;
        }
    }
}