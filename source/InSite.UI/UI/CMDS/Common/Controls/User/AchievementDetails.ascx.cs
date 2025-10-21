using System;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Persistence.Plugin.CMDS;

using Shift.Common;
using Shift.Constant.CMDS;
using Shift.Sdk.UI;

namespace InSite.Cmds.Controls.Training.Achievements
{
    public partial class AchievementDetails : UserControl
    {
        public class AchievementInfo
        {
            public string AchievementTitle { get; set; }
            public string AchievementLabel { get; set; }
            public string AchievementDescription { get; set; }
            public int? LifetimeMonths { get; set; }
            public Guid OrganizationIdentifier { get; set; }
            public bool AllowSelfDeclared { get; set; }
        }

        private Guid? AchievementIdentifier
        {
            get => ViewState[nameof(AchievementIdentifier)] as Guid?;
            set => ViewState[nameof(AchievementIdentifier)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            UniqueTitle.ServerValidate += UniqueTitle_ServerValidate;

            SubType.AutoPostBack = true;
            SubType.ValueChanged += SubType_ValueChanged;
        }

        private void SubType_ValueChanged(object sender, ComboBoxValueChangedEventArgs e)
        {
            AchievementHierarchy.SetAchievementType(SubType.Value);
        }

        private void UniqueTitle_ServerValidate(object source, ServerValidateEventArgs args)
        {
            var subType = SubType.Value.NullIfEmpty();
            var achievement = VCmdsAchievementSearch.SelectFirst(x => x.AchievementTitle == Title.Text && x.AchievementLabel == subType);

            args.IsValid = achievement == null || achievement.AchievementIdentifier == AchievementIdentifier;
        }

        public void SetDefaultValues()
        {
            AchievementHierarchy.SetDefaultValues();
        }

        public void SetInputValues(VCmdsAchievement info)
        {
            AchievementIdentifier = info.AchievementIdentifier;

            Title.Text = info.AchievementTitle;
            AllowSelfDeclared.Checked = info.AchievementAllowSelfDeclared;
            IsTimeSensitive.Checked = info.ValidForCount.HasValue;
            ValidForCount.ValueAsInt = info.ValidForCount;
            ValidForUnit.Value = ValidForUnits.Months;
            SubType.Value = info.AchievementLabel;
            Description.Text = info.AchievementDescription;

            RowValidForCount.Style["display"] = info.ValidForCount.HasValue ? string.Empty : "none";
            TimeSensitiveImage.Style["display"] = info.ValidForCount.HasValue ? string.Empty : "none";

            AchievementEditUploadsLink.NavigateUrl = string.Format("/ui/cmds/admin/achievements/edit-uploads?id={0}", info.AchievementIdentifier);

            AchievementHierarchy.Enabled = false;
            AchievementHierarchy.SetInputValues(info);

            DownloadRow.Visible = true;
            DownloadPanel.Visible = DownloadList.LoadUploads(info.AchievementIdentifier);
        }

        public AchievementInfo GetInputValues()
        {
            var organizationIdentifier = AchievementHierarchy.OrganizationIdentifier;

            return new AchievementInfo
            {
                AchievementTitle = Title.Text,
                AchievementLabel = SubType.Value,
                AchievementDescription = Description.Text,
                LifetimeMonths = !IsTimeSensitive.Checked
                    ? null
                    : ValidForCount.ValueAsInt.HasValue && ValidForUnit.Value == ValidForUnits.Years
                        ? 12 * ValidForCount.ValueAsInt.Value
                        : ValidForCount.ValueAsInt,
                OrganizationIdentifier = organizationIdentifier,
                AllowSelfDeclared = AllowSelfDeclared.Checked
            };
        }

        public void SaveAchievementCategoriesAndAccessControl(Guid achievementIdentifier)
        {
            AchievementHierarchy.SaveAchievementCategories(achievementIdentifier);
        }

        public void DisableEdit()
        {
            SubType.Enabled = false;
        }
    }
}