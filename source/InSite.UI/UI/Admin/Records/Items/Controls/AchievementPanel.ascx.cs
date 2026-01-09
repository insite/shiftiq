using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Application.Credentials.Write;
using InSite.Application.Records.Read;
using InSite.Domain.Records;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Records.Items.Controls
{
    public partial class AchievementPanel : UserControl
    {
        public class AchievementItem
        {
            public GradeItemAchievement Achievement { get; set; }
        }

        private Guid GradebookIdentifier
        {
            get => (Guid)ViewState[nameof(GradebookIdentifier)];
            set => ViewState[nameof(GradebookIdentifier)] = value;
        }

        private bool AllowCondition
        {
            get => (bool)ViewState[nameof(AllowCondition)];
            set => ViewState[nameof(AllowCondition)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            AchievementIdentifier.AutoPostBack = true;
            AchievementIdentifier.ValueChanged += AchievementIdentifier_ValueChanged;

            RemoveCredentialsButton.Click += RemoveCredentialsButton_Click;

            EffectiveAsAt.AutoPostBack = true;
            EffectiveAsAt.SelectedIndexChanged += EffectiveAsAt_SelectedIndexChanged;
        }

        private void AchievementIdentifier_ValueChanged(object sender, EventArgs e)
            => OnAchievementChanged();

        private void OnAchievementChanged()
            => AchievementConditionPanel.Visible = AchievementIdentifier.HasValue && AllowCondition;

        private void RemoveCredentialsButton_Click(object sender, EventArgs e)
        {
            var studentCredentials = GetStudentCredentials();

            if (studentCredentials != null)
            {
                foreach (var credential in studentCredentials)
                {
                    var command = new DeleteCredential(credential.CredentialIdentifier);
                    ServiceLocator.SendCommand(command);
                }
            }

            AchievementIdentifier.Enabled = true;
            WarningPanel.Visible = false;
        }

        private void EffectiveAsAt_SelectedIndexChanged(object sender, EventArgs e)
        {
            AchievementFixedDateField.Visible = EffectiveAsAt.SelectedValue == "Fixed";
        }

        public void SetAllowCondition(bool allowCondition)
        {
            AllowCondition = allowCondition;

            OnAchievementChanged();
        }

        public void SetAchievement(Guid gradebookIdentifier, GradeItem item)
        {
            GradebookIdentifier = gradebookIdentifier;
            AllowCondition = item.Format == GradeItemFormat.Boolean || item.Format == GradeItemFormat.Percent || item.Type == GradeItemType.Category || item.Type == GradeItemType.Calculation;

            AchievementIdentifier.Value = item.Achievement?.Achievement;

            OnAchievementChanged();

            if (item.Achievement != null)
            {
                WhenChange.SelectedValue = item.Achievement.WhenChange == TriggerCauseChange.Changed ? "Changed" : "Released";
                WhenGrade.SelectedValue = item.Achievement.WhenGrade == TriggerCauseGrade.Pass ? "Pass" : "Fail";
                ThenCommand.SelectedValue = GetTriggerEffectName(item.Achievement.ThenCommand);
                ElseCommand.SelectedValue = GetTriggerEffectName(item.Achievement.ElseCommand);
                EffectiveAsAt.SelectedValue = item.Achievement.AchievementFixedDate.HasValue ? "Fixed" : "Current";

                AchievementFixedDateField.Visible = item.Achievement.AchievementFixedDate.HasValue;
                AchievementFixedDate.Value = item.Achievement.AchievementFixedDate;
            }
            else
            {
                WhenChange.SelectedValue = "Changed";
                WhenGrade.SelectedValue = "Pass";
                ThenCommand.SelectedValue = GetTriggerEffectName(TriggerEffectCommand.Grant);
                ElseCommand.SelectedValue = GetTriggerEffectName(TriggerEffectCommand.Void);
                EffectiveAsAt.SelectedValue = "Current";

                AchievementFixedDateField.Visible = false;
                AchievementFixedDate.Value = null;
            }

            var studentCredentials = GetStudentCredentials();

            AchievementIdentifier.Enabled = studentCredentials.IsEmpty();

            WarningPanel.Visible = studentCredentials.IsNotEmpty();
        }

        public AchievementItem GetAchievement()
        {
            return new AchievementItem
            {
                Achievement = AchievementIdentifier.HasValue
                    ? new GradeItemAchievement
                    {
                        WhenChange = WhenChange.SelectedValue == "Changed" ? TriggerCauseChange.Changed : TriggerCauseChange.Released,
                        WhenGrade = WhenGrade.SelectedValue == "Pass" ? TriggerCauseGrade.Pass : TriggerCauseGrade.Fail,
                        ThenCommand = GetTriggerEffectEnum(ThenCommand.SelectedValue),
                        ElseCommand = GetTriggerEffectEnum(ElseCommand.SelectedValue),
                        Achievement = AchievementIdentifier.Value.Value,
                        AchievementFixedDate = EffectiveAsAt.SelectedValue == "Fixed" ? AchievementFixedDate.Value : null
                    }
                    : null
            };
        }

        private List<VCredential> GetStudentCredentials()
        {
            if (AchievementIdentifier.Value == null)
                return null;

            var credentials = ServiceLocator.AchievementSearch.GetCredentials(new VCredentialFilter
            {
                AchievementIdentifier = AchievementIdentifier.Value.Value
            });

            if (credentials.Count == 0)
                return null;

            var students = ServiceLocator.RecordSearch.GetEnrollments(new QEnrollmentFilter { GradebookIdentifier = GradebookIdentifier });

            return credentials.Where(x => students.Any(y => y.LearnerIdentifier == x.UserIdentifier)).ToList();
        }

        private static string GetTriggerEffectName(TriggerEffectCommand effect)
        {
            switch (effect)
            {
                case TriggerEffectCommand.Ignore:
                    return "Ignore";
                case TriggerEffectCommand.Grant:
                    return "Grant";
                case TriggerEffectCommand.Revoke:
                    return "Revoke";
                case TriggerEffectCommand.Void:
                    return "Void";
                default:
                    throw new ArgumentException("Unknown trigger effect: " + effect);
            }
        }

        private static TriggerEffectCommand GetTriggerEffectEnum(string name)
        {
            switch (name)
            {
                case "Ignore":
                    return TriggerEffectCommand.Ignore;
                case "Grant":
                    return TriggerEffectCommand.Grant;
                case "Revoke":
                    return TriggerEffectCommand.Revoke;
                case "Void":
                    return TriggerEffectCommand.Void;
                default:
                    throw new ArgumentException("Unknown trigger effect: " + name);
            }
        }
    }
}