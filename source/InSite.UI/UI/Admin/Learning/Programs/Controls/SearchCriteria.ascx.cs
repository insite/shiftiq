using System;

using InSite.Application.Records.Read;
using InSite.Common.Web.UI;

namespace InSite.Admin.Records.Programs.Controls
{
    public partial class SearchCriteria : SearchCriteriaController<TProgramFilter>
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (IsPostBack)
                return;

            DepartmentIdentifier.Filter.OrganizationIdentifier = Organization.Identifier;
            DepartmentIdentifier.Value = null;
        }

        public override TProgramFilter Filter
        {
            get
            {
                var filter = new TProgramFilter
                {
                    OrganizationIdentifier = Organization.OrganizationIdentifier,
                    ProgramCode = ProgramCode.Text,
                    ProgramName = ProgramName.Text,
                    ProgramDescription = ProgramDescription.Text,
                    ProgramTag = ProgramTag.Text,
                    GroupIdentifier = DepartmentIdentifier.Value,
                    AchievementIdentifiers = ProgramAchievementIdentifier.Values,
                    TaskObjectIdentifiers = TaskAchievementIdentifier.Values
                };

                GetCheckedShowColumns(filter);

                return filter;
            }
            set
            {
                ProgramCode.Text = value.ProgramCode;
                ProgramName.Text = value.ProgramName;
                ProgramDescription.Text = value.ProgramDescription;
                ProgramTag.Text = value.ProgramTag;
                DepartmentIdentifier.Value = value.GroupIdentifier;
                ProgramAchievementIdentifier.Values = value.AchievementIdentifiers;
                TaskAchievementIdentifier.Values = value.TaskObjectIdentifiers;
            }
        }

        public override void Clear()
        {
            ProgramCode.Text = null;
            ProgramName.Text = null;
            ProgramDescription.Text = null;
            ProgramTag.Text = null;
            DepartmentIdentifier.Value = null;
            ProgramAchievementIdentifier.Values = null;
            TaskAchievementIdentifier.Values = null;
        }
    }
}