using System;
using System.Collections.Generic;
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
                null,
                null,
                (achievements) => BookmarkAchievements(achievements),
                "template");

            if (!IsPostBack)
            {
                DepartmentIdentifier.Filter.OrganizationIdentifier = Organization.Identifier;
                DepartmentIdentifier.Value = null;
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
                list.Add(achievement);

            BookmarkedAchievements = list;

            Step2NextButton_Click(this, new EventArgs());

            return BookmarkedAchievements.Count;
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

        private void Step1SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
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

            TaskGrid.BindModelToControls(BookmarkedAchievements);
        }

        private void Step3SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var achievements = TaskGrid.GetAchievements();

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

            NavPanel.Visible = false;

            Outline.Redirect(programId);
        }
    }
}
