using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Application.Records.Read;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Sdk.UI;

using AlertType = Shift.Constant.AlertType;

namespace InSite.Admin.Records.Programs
{
    public partial class Duplicate : AdminBasePage, ICmdsUserControl, IHasParentLinkParameters
    {
        [Serializable]
        private class TemplateInfo
        {
            public Guid ProgramIdentifier { get; }
            public string Description { get; }

            public AchievementInfo[] Achievements { get; }

            public TemplateInfo(TProgram list)
            {
                ProgramIdentifier = list.ProgramIdentifier;

                Description = list.ProgramDescription;

                var enterpriseId = ServiceLocator.AppSettings.Application.Organizations.Global;

                var organizationId = Organization.Identifier;

                var scope = "Organization";

                Achievements = VCmdsAchievementSearch
                    .SelectDepartmentTemplateAchievements(enterpriseId, organizationId, scope, null, ProgramIdentifier)
                    .Select(x => new AchievementInfo(x)).Distinct()
                    .OrderBy(x => x.Type).ThenBy(x => x.Category).ThenBy(x => x.Title)
                    .ToArray();
            }
        }

        [Serializable]
        private class AchievementInfo : IEquatable<AchievementInfo>
        {
            public Guid AchievementIdentifier { get; }
            public string Type { get; }
            public string Category { get; }
            public string Title { get; }
            public bool IsValid { get; set; }

            public AchievementInfo(AchievementListGridItem row)
            {
                AchievementIdentifier = row.AchievementIdentifier;
                Type = row.AchievementLabel;
                Category = row.CategoryName;
                Title = row.AchievementTitle;
                IsValid = false;
            }

            public override bool Equals(object obj) =>
                Equals(obj as AchievementInfo);

            public bool Equals(AchievementInfo other)
            {
                return other != null && (
                    this == other
                    || this.AchievementIdentifier == other.AchievementIdentifier
                );
            }

            public override int GetHashCode()
            {
                return AchievementIdentifier.GetHashCode();
            }
        }

        public override void ApplyAccessControlForCmds() { }

        private TemplateInfo SourceTemplate
        {
            get => (TemplateInfo)ViewState[nameof(SourceTemplate)];
            set => ViewState[nameof(SourceTemplate)] = value;
        }

        private Guid AchievementId => Guid.TryParse(Request.QueryString["id"], out var id) ? id : Guid.Empty;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DepartmentIdentifier.AutoPostBack = true;
            DepartmentIdentifier.ValueChanged += DepartmentIdentifier_ValueChanged;

            AchievementTypeRepeater.ItemCreated += AchievementTypeRepeater_ItemCreated;
            AchievementTypeRepeater.ItemDataBound += AchievementTypeRepeater_ItemDataBound;

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (SourceTemplate == null)
            {
                var asset = Guid.TryParse(Request.QueryString["id"], out var achievementListIdentifier)
                    ? ProgramSearch1.SelectFirst(x => x.ProgramIdentifier == achievementListIdentifier)
                    : null;

                if (asset == null)
                    Search.Redirect();

                SourceTemplate = new TemplateInfo(asset);

                PageHelper.AutoBindHeader(Page);

                CancelButton.NavigateUrl = Outline.GetNavigateUrl(SourceTemplate.ProgramIdentifier);

                DepartmentIdentifier.Filter.OrganizationIdentifier = Organization.Identifier;
                DepartmentIdentifier.Value = null;

                OnDepartmentSelected();
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var asset = Guid.TryParse(Request.QueryString["id"], out var achievementListIdentifier)
                    ? ProgramSearch1.SelectFirst(x => x.ProgramIdentifier == achievementListIdentifier)
                    : null;

            var departmentTemplates = TaskSearch
                .Select(x => x.ProgramIdentifier == SourceTemplate.ProgramIdentifier)
                .ToDictionary(x => x.ObjectIdentifier);

            var listIdentifier = UniqueIdentifier.Create();
            var list = new TProgram
            {
                GroupIdentifier = DepartmentIdentifier.Value.Value,
                ProgramIdentifier = listIdentifier,
                ProgramName = ProgramName.Text,
                ProgramDescription = SourceTemplate.Description,
                OrganizationIdentifier = Organization.Identifier,
                ProgramType = asset.ProgramType,
            };

            foreach (var achievement in SourceTemplate.Achievements)
            {
                if (!achievement.IsValid)
                    continue;

                var copy = new TTask
                {
                    ObjectType = "Achievement",
                    ObjectIdentifier = achievement.AchievementIdentifier,
                    OrganizationIdentifier = Organization.Identifier,
                    ProgramIdentifier = listIdentifier,
                    TaskCompletionRequirement = "Credential Granted",
                    TaskIdentifier = UniqueIdentifier.Create(),
                    TaskIsPlanned = false,
                    TaskIsRequired = false,
                    TaskLifetimeMonths = 0
                };

                if (departmentTemplates.TryGetValue(achievement.AchievementIdentifier, out var source))
                {
                    copy.TaskLifetimeMonths = source.TaskLifetimeMonths;
                    copy.TaskIsPlanned = source.TaskIsPlanned;
                    copy.TaskIsRequired = source.TaskIsRequired;
                }

                list.Tasks.Add(copy);
            }

            ProgramStore.Insert(list, User.Identifier);

            Outline.Redirect(listIdentifier, status: "saved");
        }

        private void DepartmentIdentifier_ValueChanged(object sender, EventArgs e) => OnDepartmentSelected();

        private void OnDepartmentSelected()
        {
            AchievementsSection.Visible = false;
            ScreenStatus.Visible = false;
            SaveButton.Enabled = false;

            if (!DepartmentIdentifier.HasValue || SourceTemplate.Achievements.Length == 0)
                return;

            AchievementsSection.Visible = true;
            SaveButton.Enabled = true;

            var validCount = 0;
            var departmentAchievements = VCmdsAchievementSearch
                .SelectAllNewDepartmentTemplateAchievements(DepartmentIdentifier.Value.Value, Organization.Identifier)
                .Select(x => x.AchievementIdentifier)
                .ToHashSet();

            foreach (var r in SourceTemplate.Achievements)
            {
                r.IsValid = departmentAchievements.Contains(r.AchievementIdentifier);

                if (r.IsValid)
                    validCount++;
            }

            AchievementTypeRepeater.DataSource = SourceTemplate.Achievements.GroupBy(x => x.Type);
            AchievementTypeRepeater.DataBind();

            var message = $"The department you selected has access to {validCount} of the {SourceTemplate.Achievements.Length} " +
                $"achievements ({(SourceTemplate.Achievements.Length == 0 ? 0d : validCount / (double)SourceTemplate.Achievements.Length):p0}) " +
                $"from the original program. Are you sure you want to duplicate this program?";

            ScreenStatus.AddMessage(AlertType.Information, message);
        }

        private void AchievementTypeRepeater_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var categoryRepeater = (Repeater)e.Item.FindControl("CategoryRepeater");
            categoryRepeater.ItemDataBound += CategoryRepeater_ItemDataBound;
        }

        private void AchievementTypeRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var dataItem = (IEnumerable<AchievementInfo>)e.Item.DataItem;

            var categoryRepeater = (Repeater)e.Item.FindControl("CategoryRepeater");
            categoryRepeater.DataSource = dataItem.GroupBy(x => x.Category);
            categoryRepeater.DataBind();
        }

        private void CategoryRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var dataItem = ((IEnumerable<AchievementInfo>)e.Item.DataItem).ToArray();

            const int colCount = 3;
            var itemsPerColumn = 4;
            if (dataItem.Length > (itemsPerColumn * colCount))
                itemsPerColumn = (int)Math.Ceiling(dataItem.Length / (double)colCount);

            var itemRepeater = (Repeater)e.Item.FindControl("ItemRepeater");
            itemRepeater.DataSource = dataItem.Select((x, i) => new
            {
                x.Title,
                IconHtml = x.IsValid
                    ? "<i class='far fa-check-square text-success' title='The achievement is assigned to the selected department'></i>"
                    : "<i class='far fa-exclamation-square text-danger' title='The achievement is NOT assigned to the selected department'></i>",
                IsColumnSeparator = i > 0 && (i % itemsPerColumn) == 0
            });
            itemRepeater.DataBind();
        }

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
            => (parent != null && parent.Name.EndsWith("/outline")) ? $"id={AchievementId}" : null;
    }
}