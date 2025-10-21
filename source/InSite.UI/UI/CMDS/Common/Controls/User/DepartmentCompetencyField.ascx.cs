using System;
using System.Web.UI;

using InSite.Common.Web.Cmds;

using Shift.Common;
using Shift.Sdk.UI;

namespace InSite.Cmds.Controls.Contacts.Companies.Competencies
{
    public partial class DepartmentCompetencyField : UserControl
    {
        public DepartmentCompetency DepartmentCompetency
        {
            set
            {
                if (value.DepartmentIdentifier == Guid.Empty)
                {
                    Visible = false;
                    return;
                }

                Selected.Checked = value.IsSelected;
                Selected.Attributes["isTimeSensitive"] = value.IsTimeSensitive ? "true" : "false";
                Selected.Attributes["isCritical"] = value.IsCritical ? "true" : "false";

                SettingsButton.Style["display"] = value.IsSelected ? "" : "none";
                SettingsButton.Attributes["departmentId"] = value.DepartmentIdentifier.ToString();
                SettingsButton.Attributes["profileId"] = value.ProfileStandardIdentifier.ToString();
                SettingsButton.Attributes["competencyId"] = value.CompetencyStandardIdentifier.ToString();

                AlarmIcon.Style["display"] = value.IsTimeSensitive ? "" : "none";
                AlarmIcon.ToolTip = TimeSensitivityHelper.GetTimeSensitiveText(value.ValidForUnit, value.ValidForCount);

                CriticalIcon.Style["display"] = value.IsCritical ? "" : "none";

                SelectedOld.Value = value.IsSelected ? "true" : "false";
            }
        }

        public bool IsSelected
            => Selected.Checked;

        public bool IsSelectedOld
            => StringHelper.Equals(SelectedOld.Value, "true");

        public Guid DepartmentIdentifier
            => SettingsButton.Attributes["departmentId"] == null ? Guid.Empty : Guid.Parse(SettingsButton.Attributes["departmentId"]);

        public Guid CompetencyStandardIdentifier
            => SettingsButton.Attributes["competencyId"] == null ? Guid.Empty : Guid.Parse(SettingsButton.Attributes["competencyId"]);

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            Selected.Attributes["onclick"] = string.Format("selectedChanged('{0}')", Selected.ClientID);
            Selected.Attributes["settingsButtonId"] = SettingsButton.ClientID;
            Selected.Attributes["alarmImageId"] = AlarmIcon.ClientID;
            Selected.Attributes["criticalImageId"] = CriticalIcon.ClientID;
            Selected.Attributes["selectedOldId"] = SelectedOld.ClientID;

            SettingsButton.Attributes["onclick"] = string.Format("return showSettingsEditor('{0}', '{1}')",
                SettingsButton.ClientID, Selected.ClientID);
        }
    }
}