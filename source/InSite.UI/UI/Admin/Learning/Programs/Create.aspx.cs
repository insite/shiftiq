using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;

using InSite.Application.Records.Read;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Records.Programs
{
    public partial class Create : AdminBasePage
    {
        private class AchievementItem
        {
            public Guid AchievementIdentifier { get; set; }
            public int? LifetimeMonths { get; set; }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ProgramType.AutoPostBack = true;
            ProgramType.CheckedChanged += (x, y) => ProgramTypeChanged();

            AchievementListEditor.InitDelegates(
                Organization.Identifier,
                (list) => FilterToSelectedAchievements(list),
                (achievements) => UnbookmarkAchievements(achievements),
                (achievements) => BookmarkAchievements(achievements),
                "template");

            if (!IsPostBack)
            {
                DepartmentIdentifier.Filter.OrganizationIdentifier = Organization.Identifier;
                DepartmentIdentifier.Value = null;

                // Offer only programs that can legally be a parent. The program being created has
                // no identifier yet, so only the nesting rule applies.
                ParentPrograms.Filter.EligibleParentForProgramIdentifier = Guid.Empty;
            }

            Step1SaveButton.Click += Step1SaveButton_Click;
            Step1NextButton.Click += Step1NextButton_Click;
            Step2NextButton.Click += Step2NextButton_Click;
            Step3SaveButton.Click += Step3SaveButton_Click;
        }

        public List<Guid> BookmarkedAchievements
        {
            get => (List<Guid>)ViewState[nameof(BookmarkedAchievements)] ?? new List<Guid>();
            set => ViewState[nameof(BookmarkedAchievements)] = value;
        }

        private int BookmarkAchievements(IEnumerable<Guid> achievements)
        {
            var list = new List<Guid>();

            foreach (var achievement in BookmarkedAchievements)
                list.Add(achievement);

            foreach (var achievement in achievements)
                if (!list.Contains(achievement))
                    list.Add(achievement);

            BookmarkedAchievements = list;

            Step2NextButton_Click(this, new EventArgs());

            return BookmarkedAchievements.Count;
        }

        /// <summary>
        /// Achievements supplied by the selected parent programs. These show as
        /// preselected in the wizard and are created as inherited tasks by the
        /// containment cascade, not as local tasks.
        /// </summary>
        private HashSet<Guid> GetParentAchievementIdentifiers()
        {
            var parentIds = ParentPrograms.Values;
            if (parentIds.Length == 0)
                return new HashSet<Guid>();

            return TaskSearch
                .Select(x => parentIds.Contains(x.ProgramIdentifier))
                .Select(x => x.ObjectIdentifier)
                .ToHashSet();
        }

        private List<AchievementListGridItem> FilterToSelectedAchievements(List<AchievementListGridItem> list)
        {
            var selected = GetParentAchievementIdentifiers();

            foreach (var achievement in BookmarkedAchievements)
                selected.Add(achievement);

            return list.Where(x => selected.Contains(x.AchievementIdentifier)).ToList();
        }

        private void UnbookmarkAchievements(IEnumerable<Guid> achievements)
        {
            var requested = achievements.ToList();
            var parentAchievements = GetParentAchievementIdentifiers();

            BookmarkedAchievements = BookmarkedAchievements
                .Where(x => !requested.Contains(x) || parentAchievements.Contains(x))
                .ToList();

            if (requested.Any(x => parentAchievements.Contains(x)))
                AlertStatus.AddMessage(Shift.Constant.AlertType.Warning,
                    "Achievements supplied by a parent program cannot be removed here. Remove the parent program from the selection instead.");
        }

        private void ProgramTypeChanged()
        {
            var achievementsOnly = ProgramType.Checked;

            Step1SaveButton.Visible = !achievementsOnly;
            Step1NextButton.Visible = achievementsOnly;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            if (ServiceLocator.Partition.IsE03())
                ProgramType.Checked = true;

            ProgramTypeChanged();

            PageHelper.AutoBindHeader(this);

            CreationType.EnsureDataBound();
            CreationType.SetVisibleOptions(CreationTypeEnum.One);

            CancelButton.NavigateUrl = "/ui/admin/records/home";
        }

        /// <summary>
        /// Program nesting is limited to one level: a program that already has parents
        /// of its own cannot be selected as a parent.
        /// </summary>
        private bool ValidateParentPrograms()
        {
            if (ParentPrograms.SelectedCount == 0)
                return true;

            foreach (var item in ParentPrograms.Items)
            {
                if (ProgramContainmentSearch.GetParentIdentifiers(item.Value).Length > 0)
                {
                    AlertStatus.AddMessage(Shift.Constant.AlertType.Error,
                        $"\"{item.Text}\" has a parent program of its own, so it cannot be a parent. Program nesting is limited to one level.");
                    return false;
                }
            }

            return true;
        }

        private void Step1SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            if (!ValidateParentPrograms())
                return;

            var programId = UniqueIdentifier.Create();
            var program = new TProgram
            {
                OrganizationIdentifier = Organization.Identifier,
                ProgramIdentifier = programId,
                ProgramCode = ProgramCode.Text,
                ProgramName = ProgramName.Text,
                ProgramDescription = ProgramDescription.Text,
                ProgramSlug = StringHelper.Sanitize(ProgramName.Text, '-'),
                ProgramType = ProgramType.Checked ? "Achievements Only" : null
            };

            if (ProgramType.Checked)
                program.ProgramType = "Achievements Only";

            ProgramStore.Insert(program, User.Identifier);
            InsertContent(program);

            if (ParentPrograms.SelectedCount > 0)
                ProgramContainmentStore.Insert(ParentPrograms.Values, programId, Organization.Identifier, User.Identifier);

            Outline.Redirect(programId);
        }

        private void InsertContent(TProgram program)
        {
            var content = new ContentContainer();
            content.Title.Text.Default = program.ProgramName;
            ServiceLocator.ContentStore.SaveContainer(program.OrganizationIdentifier, ContentContainerType.Program, program.ProgramIdentifier, content);
        }

        private void Step1NextButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            if (!ValidateParentPrograms())
                return;

            Step2Section.Visible = true;
            Step2Section.IsSelected = true;

            AchievementListEditor.SetEditable(true, false);
            AchievementListEditor.LoadAchievements(GroupByEnum.Type, DepartmentIdentifier.Value);
        }

        private void Step2NextButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            Step3Section.Visible = true;
            Step3Section.IsSelected = true;

            var achievementIds = GetParentAchievementIdentifiers();

            foreach (var achievement in BookmarkedAchievements)
                achievementIds.Add(achievement);

            TaskGrid.BindModelToControls(achievementIds, ParentPrograms.Values);
        }

        private void Step3SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            if (!ValidateParentPrograms())
                return;

            var achievements = TaskGrid.GetAchievements();
            var parentAchievements = GetParentAchievementIdentifiers();

            var programId = UniqueIdentifier.Create();
            var list = new TProgram
            {
                GroupIdentifier = DepartmentIdentifier.Value,
                OrganizationIdentifier = Organization.Identifier,
                ProgramDescription = ProgramDescription.Text,
                ProgramIdentifier = programId,
                ProgramName = ProgramName.Text,
                ProgramType = ProgramType.Checked ? "Achievements Only" : null,
                Tasks = new List<TTask>()
            };

            foreach (var achievement in achievements)
            {
                // Parent-supplied achievements become inherited tasks through the
                // containment cascade when the parent links are saved below.
                if (parentAchievements.Contains(achievement.AchievementIdentifier))
                    continue;

                var item = new TTask
                {
                    ObjectType = "Achievement",
                    ObjectIdentifier = achievement.AchievementIdentifier,
                    OrganizationIdentifier = Organization.Identifier,
                    ProgramIdentifier = programId,
                    TaskCompletionRequirement = "Credential Granted",
                    TaskIdentifier = UniqueIdentifier.Create(),
                    TaskIsPlanned = achievement.IsPlanned,
                    TaskIsRequired = achievement.IsRequired,
                    TaskLifetimeMonths = achievement.LifetimeMonths
                };

                list.Tasks.Add(item);
            }

            ProgramStore.Insert(list, User.Identifier);

            if (ParentPrograms.SelectedCount > 0)
                ProgramContainmentStore.Insert(ParentPrograms.Values, programId, Organization.Identifier, User.Identifier);

            NavPanel.Visible = false;

            Outline.Redirect(programId);
        }
    }
}
