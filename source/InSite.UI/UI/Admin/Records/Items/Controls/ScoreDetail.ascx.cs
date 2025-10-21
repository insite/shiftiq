using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Application.Records.Read;
using InSite.Common.Web.UI;
using InSite.Domain.Records;

using Shift.Constant;

namespace InSite.Admin.Records.Items.Controls
{
    public partial class ScoreDetail : BaseUserControl
    {
        public class ScoreItem
        {
            public Guid? ParentItem { get; set; }
            public string Code { get; set; }
            public string Hook { get; set; }
            public string Name { get; set; }
            public string ShortName { get; set; }
            public bool IncludeToReport { get; set; }
            public GradeItemFormat Format { get; set; }
            public decimal? PassPercent { get; set; }
            public string Reference { get; set; }
            public decimal? MaxPoint { get; set; }

            public AchievementPanel.AchievementItem Achievement { get; set; }
            public Guid[] Standards { get; set; }
            public Notification[] Notifications { get; set; }
        }

        private Guid GradebookIdentifier
        {
            get => (Guid)ViewState[nameof(GradebookIdentifier)];
            set => ViewState[nameof(GradebookIdentifier)] = value;
        }

        private string OldCode
        {
            get => (string)ViewState[nameof(OldCode)];
            set => ViewState[nameof(OldCode)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ItemFormat.AutoPostBack = true;
            ItemFormat.SelectedIndexChanged += ItemFormat_SelectedIndexChanged;

            CodeValidator.ServerValidate += CodeValidator_ServerValidate;

            ProgressCompletedMessageIdentifier.Filter.OrganizationIdentifier = Organization.Identifier;
            ProgressCompletedMessageIdentifier.Filter.Type = "Notification";
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (ItemFormat.SelectedValue != "Point")
                MaxPointField.Style["display"] = "none";
        }

        private void ItemFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            var allowCondition = ItemFormat.SelectedValue == "Percent";
            AchievementPanel.SetAllowCondition(allowCondition);
        }

        private void CodeValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = string.Equals(args.Value, OldCode);

            if (!args.IsValid)
            {
                var data = ServiceLocator.RecordSearch.GetGradebookState(GradebookIdentifier);
                args.IsValid = !data.ContainsCode(args.Value);
            }
        }

        public void InitGradebook(QGradebook gradebook, GradebookState data, string code)
        {
            GradebookIdentifier = gradebook.GradebookIdentifier;

            Code.Text = code;

            ParentItem.GradebookIdentifier = gradebook.GradebookIdentifier;
            ParentItem.RefreshData();

            AchievementPanel.SetAllowCondition(true);

            var hasStandards = data.Type == GradebookType.Standards || data.Type == GradebookType.ScoresAndStandards;

            StandardsCard.Visible = hasStandards;

            if (hasStandards)
                StandardPanel.SetInputValue(data, null);
        }

        public bool SetInputValue(Guid gradebookIdentifier, GradebookState data, Guid itemKey)
        {
            var item = data.FindItem(itemKey);

            if (item == null || item.Type != GradeItemType.Score)
                return false;

            OldCode = item.Code;

            ParentItem.DisableItemAndSubTree = itemKey;
            ParentItem.RefreshData();

            ParentItem.ValueAsGuid = item.Parent?.Identifier;
            Code.Text = item.Code;
            Name.Text = item.Name;
            ShortName.Text = item.ShortName;
            IncludeToReport.SelectedValue = item.IsReported.ToString().ToLower();
            Reference.Text = item.Reference;
            Hook.Text = item.Hook;
            MaxPoint.ValueAsDecimal = item.MaxPoints;
            PassPercentField.Visible = item.Format == GradeItemFormat.Percent;

            switch (item.Format)
            {
                case GradeItemFormat.Boolean:
                    ItemFormat.SelectedValue = "Boolean";
                    break;
                case GradeItemFormat.Percent:
                    ItemFormat.SelectedValue = "Percent";
                    break;
                case GradeItemFormat.Point:
                    ItemFormat.SelectedValue = "Point";
                    break;
                case GradeItemFormat.Text:
                    ItemFormat.SelectedValue = "Text";
                    break;
                default:
                    ItemFormat.SelectedValue = "Number";
                    break;
            }

            PassPercent.ValueAsDecimal = item.PassPercent * 100;

            AchievementPanel.SetAchievement(gradebookIdentifier, item);

            if (data.Type == GradebookType.Standards || data.Type == GradebookType.ScoresAndStandards)
                StandardPanel.SetInputValue(data, itemKey);

            SetProgressNotifications(item.Notifications);

            return true;
        }

        private void SetProgressNotifications(Notification[] notifications)
        {
            var notification = notifications.FirstOrDefault(x => x.Change == "ProgressCompleted");
            if (notification == null)
                return;

            ProgressCompletedMessageIdentifier.Value = notification.Message;
        }

        private Notification[] GetProgressNotifications()
        {
            var notifications = new List<Notification>();
            var message = ProgressCompletedMessageIdentifier.Value;
            notifications.Add(new Notification { Change = "ProgressCompleted", Message = message });
            return notifications.ToArray();
        }

        public ScoreItem GetInputValues()
        {
            GradeItemFormat format;

            switch (ItemFormat.SelectedValue)
            {
                case "Boolean":
                    format = GradeItemFormat.Boolean;
                    break;
                case "Percent":
                    format = GradeItemFormat.Percent;
                    break;
                case "Point":
                    format = GradeItemFormat.Point;
                    break;
                case "Text":
                    format = GradeItemFormat.Text;
                    break;
                default:
                    format = GradeItemFormat.Number;
                    break;
            }

            return new ScoreItem
            {
                ParentItem = ParentItem.ValueAsGuid,
                Code = Code.Text,
                Name = Name.Text,
                ShortName = ShortName.Text,
                IncludeToReport = bool.Parse(IncludeToReport.SelectedValue),
                Format = format,
                PassPercent = PassPercent.ValueAsDecimal / 100.0m,
                Reference = Reference.Text,
                Hook = Hook.Text,
                MaxPoint = MaxPoint.ValueAsDecimal,
                Achievement = AchievementPanel.GetAchievement(),
                Standards = StandardPanel.GetStandards(),
                Notifications = GetProgressNotifications()
            };
        }
    }
}