using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using InSite.Application.Achievements.Write;
using InSite.Application.Records.Read;
using InSite.Common.Web;
using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;
using InSite.UI.Admin.Records.Programs.Utilities;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

using AggregateOutline = InSite.Admin.Logs.Aggregates.Outline;

namespace InSite.Admin.Records.Programs
{
    public partial class Outline : AdminBasePage
    {
        private Guid? ProgramID => Guid.TryParse(Request["id"], out var result) ? result : (Guid?)null;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            AchievementIdentifier.AutoPostBack = true;
            AchievementIdentifier.ValueChanged += (x, y) => BindModelToControlsForAchievement(AchievementIdentifier.Value, false);
            AchievementCreateButton.Click += (x, y) => BindModelToControlsForAchievement(null, true);

            AchievementSaveButton.Click += (x, y) => SaveChangesToAchievement();
            AchievementCancelButton.Click += (x, y) => BindModelToControlsForAchievement(AchievementIdentifier.Value, false);

            AchievementEditor.InitDelegates(
                Organization.Identifier,
                GetAssignedAchievements,
                DeleteAchievements,
                InsertAchievements,
                "program");

            AchievementEditor.Refreshed += AchievementEditor_Refreshed;

            SaveAchievementsButton.Click += (x, y) => SaveAchievements();

            InitTab();
        }

        private List<AchievementListGridItem> GetAssignedAchievements(List<AchievementListGridItem> list)
        {
            var programId = ProgramID.Value;

            var programTaskIds = TaskSearch.SelectByProgram(programId);

            var assignedAchievementIds = programTaskIds.Select(x => x.AchievementIdentifier).ToList();

            return list
                .Where(x => assignedAchievementIds.Contains(x.AchievementIdentifier))
                .ToList();
        }

        private void InitTab()
        {
            var parser = new UrlParser();
            var _url = parser.Parse(Request.RawUrl);
            if (_url.Parameters != null && _url.Parameters.Count > 0)
                SelectTab(_url.Get("tab"));
        }

        private void SelectTab(string tab)
        {
            ContentTab.IsSelected = tab == "content";
            LearnerTab.IsSelected = tab == "enrollments";
            PublicationTab.IsSelected = tab == "publication";
            NotificationTab.IsSelected = tab == "notification";
            PrivacyTab.IsSelected = tab == "privacy";
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            ValidateQueryString();
            BindModelToControls(InitModel());
        }

        private void Task_Updated(object sender, EventArgs e)
        {
            PublicationSetup.Refresh();
        }

        private void BindModelToControls(TProgram model)
        {
            PageHelper.AutoBindHeader(this, qualifier: model.ProgramName);

            ProgramCode.Text = model.ProgramCode;
            ProgramCodeField.Visible = !string.IsNullOrEmpty(model.ProgramCode);

            if (model.GroupIdentifier != null)
            {
                var group = ServiceLocator.GroupSearch.GetGroup(model.GroupIdentifier.Value);
                if (group != null)
                {
                    GroupLabel.Text = group.GroupType;
                    GroupName.Text = group.GroupName;
                    GroupField.Visible = true;
                }
            }

            CatalogName.Text = "None";

            if (model.CatalogIdentifier != null)
            {
                var catalog = CourseSearch.GetCatalog(model.OrganizationIdentifier, model.CatalogIdentifier.Value);

                if (catalog != null)
                {
                    CatalogName.Text = catalog.CatalogName;

                    if (catalog.IsHidden)
                        CatalogName.Text += " <span class='badge bg-danger fs-sm ms-2'>Hidden</span>";
                }
            }

            ProgramName.Text = model.ProgramName;
            ProgramDescription.Text = model.ProgramDescription;
            ProgramIdentifier.Text = model.ProgramIdentifier.ToString();

            if (model.ProgramType != null)
            {
                ProgramType.Text = $"<span class='badge bg-primary fs-sm'>{model.ProgramType}</span>";
            }

            ProgramTag.Text = model.ProgramTag.HasValue() ? model.ProgramTag : "None";
            ProgramTagField.Visible = model.ProgramTag.HasValue();

            PrivacySettingRepeater.LoadData(model.ProgramIdentifier, "Program");
            ProgramContent.LoadData(model);
            NotificationSetup.LoadData(model);
            PublicationSetup.LoadData(model);

            RecodeLink.NavigateUrl =
                RetagLink.NavigateUrl =
                RenameLink.NavigateUrl =
                DescribeLink.NavigateUrl =
                ModifyGroupLink.NavigateUrl =
                ModifyCatalogLink.NavigateUrl =
                $"/ui/admin/learning/programs/describe?id={model.ProgramIdentifier}";

            AchievementIdentifier.Value = model.AchievementIdentifier;
            BindModelToControlsForAchievement(model.AchievementIdentifier, false);

            ProgramCategoryList.SetProgram(model.ProgramIdentifier);

            ViewHistoryLink.NavigateUrl = AggregateOutline.GetUrl(model.ProgramIdentifier, $"/ui/admin/learning/programs/outline?id={model.ProgramIdentifier}");
            DeleteLink.NavigateUrl = $"/ui/admin/learning/programs/delete?id={model.ProgramIdentifier}";
            EditButton.NavigateUrl = $"/ui/admin/learning/programs/tasks/assign?id={model.ProgramIdentifier}";
            DuplicateLink.NavigateUrl = $"/ui/admin/learning/programs/duplicate?id={model.ProgramIdentifier}";

            ProgramUserGrid.LoadDataByProgram(model.ProgramIdentifier, model.AchievementIdentifier, true, Request.RawUrl);

            if (model.ProgramType == "Achievements Only")
            {
                AchievementTaskRepeater.BindModelToControls(model.ProgramIdentifier, true);

                AchievementTaskRepeater.Visible = true;
                SaveAchievementsButton.Visible = true;
            }
            else
            {
                AssessmentTaskRepeater.BindModelToControls(model.ProgramIdentifier);
                LogbookTaskRepeater.BindModelToControls(model.ProgramIdentifier);
                SurveyTaskRepeater.BindModelToControls(model.ProgramIdentifier);
                CourseTaskRepeater.BindModelToControls(model.ProgramIdentifier);

                AssessmentTaskRepeater.Visible = LogbookTaskRepeater.Visible = SurveyTaskRepeater.Visible = CourseTaskRepeater.Visible = true;
            }

            CurrentDepartmentIdentifier = model.GroupIdentifier ?? Guid.Empty;

            if (model.ProgramType == "Achievements Only")
            {
                AchievementSection.Visible = true;
                AchievementTaskSection.Visible = true;
                AchievementEditor.SetEditable(true, true);
                AchievementEditor.LoadAchievements(GroupByEnum.TypeAndCategory);
                EditButton.Visible = false;
                TaskGrid.BindModelToControls(model.ProgramIdentifier);
            }
            else
            {
                CredentialGrid.LoadData(model.ProgramIdentifier);
                CredentialPanel.Visible = true;
            }
        }

        private void ValidateQueryString()
        {
            if (ProgramID == null)
                HttpResponseHelper.Redirect("/ui/admin/learning/programs/search");
        }

        private TProgram InitModel()
        {
            var model = ProgramSearch.GetProgram(ProgramID.Value);
            if (model == null)
                HttpResponseHelper.Redirect("/ui/admin/learning/programs/search");
            return model;
        }

        #region Program Achievement

        public void BindModelToControlsForAchievement(Guid? achievementId, bool forceNew)
        {
            AchievementFields.Visible = forceNew;
            AchievementIdentifierField.Visible = !forceNew;
            AchievementCancelButton.Visible = forceNew;
            AchievementSaveButton.Text = "Update Achievement";

            if (forceNew)
            {
                AchievementName.Text = "New Achievement";
                AchievementExpiration.SetExpiration();
                AchievementLabel.Text = string.Empty;
                AchievementLayout.Value = null;
                AchievementSaveButton.Text = "Add Achievement";
                return;
            }

            var achievement = achievementId.HasValue
                ? ServiceLocator.AchievementSearch.GetAchievement(achievementId.Value)
                : null;

            if (achievement == null)
                return;

            AchievementOutlineLink.NavigateUrl = $"/ui/admin/records/achievements/outline?id={achievementId}";
            AchievementName.Text = achievement.AchievementTitle;
            AchievementLabel.Text = achievement.AchievementLabel;
            AchievementExpiration.SetExpiration(achievement);
            AchievementLayout.Value = achievement.CertificateLayoutCode;

            AchievementFields.Visible = true;
        }

        private void SaveChangesToAchievement()
        {
            if (!Page.IsValid)
                return;

            var program = ProgramID.HasValue ? ProgramSearch.GetProgram(ProgramID.Value) : null;
            if (program == null)
                return;

            if (!AchievementIdentifierField.Visible)
                CreateAchievement(program);
            else
                ModifyAchievement(program);
        }

        private void CreateAchievement(TProgram program)
        {
            var id = UniqueIdentifier.Create();
            var expiration = AchievementExpiration.GetExpiration();

            ServiceLocator.SendCommand(new Application.Achievements.Write.CreateAchievement(
                id, Organization.OrganizationIdentifier, AchievementLabel.Text, AchievementName.Text, null, expiration, null));

            var layout = AchievementLayout.Value;
            if (layout.IsNotEmpty())
                ServiceLocator.SendCommand(new ChangeCertificateLayout(id, layout));

            AssignAchievementToProgram(id, program);
            CheckProgramCompletion(program);
            BindModelToControlsForAchievement(AchievementIdentifier.Value = id, false);

            StatusAlert.AddMessage(AlertType.Success, "The achievement have been created");
        }

        private static void CheckProgramCompletion(TProgram program)
        {
            var enrollments = ProgramSearch1.GetProgramUsers(new VProgramEnrollmentFilter()
            { ProgramIdentifier = program.ProgramIdentifier, OrganizationIdentifier = program.OrganizationIdentifier })
                .Select(x => x.UserIdentifier).ToList();

            foreach (var userIdentifier in enrollments)
            {
                if ((program.CompletionTaskIdentifier.HasValue && ServiceLocator.ProgramSearch.IsTaskCompletedByLearner(program.CompletionTaskIdentifier.Value, userIdentifier)) ||
                    ServiceLocator.ProgramSearch.IsProgramFullyCompletedByLearner(program.ProgramIdentifier, userIdentifier))
                {
                    if (program.AchievementIdentifier.HasValue)
                        ProgramHelper.SendGrantCommands(TriggerEffectCommand.Grant, CurrentSessionState.Identity.Organization.Identifier, program.AchievementIdentifier.Value, userIdentifier);

                    ProgramStore.InsertEnrollment(Organization.Identifier, program.ProgramIdentifier, userIdentifier, User.Identifier, DateTimeOffset.UtcNow);
                }
            }
        }

        private void ModifyAchievement(TProgram program)
        {
            if (AchievementIdentifier.HasValue)
            {
                var achievement = ServiceLocator.AchievementSearch.GetAchievement(AchievementIdentifier.Value.Value);

                if (achievement == null)
                    return;

                if (!achievement.AchievementIsEnabled)
                {
                    StatusAlert.AddMessage(AlertType.Warning, "Modifications are not permitted while the achievement is locked. Please unlock it before making any changes.");
                    return;
                }

                if (achievement.AchievementLabel != AchievementLabel.Text || achievement.AchievementTitle != AchievementName.Text)
                    ServiceLocator.SendCommand(new DescribeAchievement(
                        achievement.AchievementIdentifier, AchievementLabel.Text, AchievementName.Text, achievement.AchievementDescription, false));

                if (achievement.CertificateLayoutCode != AchievementLayout.Value)
                    ServiceLocator.SendCommand(new ChangeCertificateLayout(
                        achievement.AchievementIdentifier, AchievementLayout.Value));

                ServiceLocator.SendCommand(new ChangeAchievementExpiry(AchievementIdentifier.Value.Value, AchievementExpiration.GetExpiration()));
            }

            if (program.AchievementIdentifier == null && AchievementIdentifier.Value == null)
                return;

            if (program.AchievementIdentifier == null && AchievementIdentifier.Value != null)
                AssignAchievementToProgram(AchievementIdentifier.Value.Value, program);

            else if (program.AchievementIdentifier != null && AchievementIdentifier.Value == null)
            {
                program.AchievementIdentifier = null;
                program.AchievementWhenChange = null;
                program.AchievementWhenGrade = null;
                program.AchievementThenCommand = null;
                program.AchievementElseCommand = null;
                program.AchievementFixedDate = null;
                ProgramStore.Update(program, User.Identifier);
            }

            else
            {
                program.AchievementIdentifier = AchievementIdentifier.Value.Value;
                ProgramStore.Update(program, User.Identifier);
            }

            CheckProgramCompletion(program);
            BindModelToControlsForAchievement(AchievementIdentifier.Value, false);

            StatusAlert.AddMessage(AlertType.Success, "The achievement have been updated");
        }

        private void AssignAchievementToProgram(Guid achievement, TProgram program)
        {
            program.AchievementIdentifier = achievement;
            program.AchievementWhenChange = TriggerCauseChange.Changed.ToString();
            program.AchievementWhenGrade = TriggerCauseGrade.Pass.ToString();
            program.AchievementThenCommand = TriggerEffectCommand.Grant.ToString();
            program.AchievementElseCommand = TriggerEffectCommand.Void.ToString();

            ProgramStore.Update(program, User.Identifier);
        }

        #endregion

        #region Achievements Only

        private Guid CurrentDepartmentIdentifier
        {
            get => (Guid)ViewState[nameof(CurrentDepartmentIdentifier)];
            set => ViewState[nameof(CurrentDepartmentIdentifier)] = value;
        }

        private void AchievementEditor_Refreshed(object sender, EventArgs e)
        {
            TaskGrid.BindModelToControls(ProgramID.Value);
        }

        private List<AchievementListGridItem> SelectAchievements(Guid enterpriseId, Guid organizationId, string scope, string keyword)
        {
            return VCmdsAchievementSearch.SelectDepartmentTemplateAchievements(
                enterpriseId, organizationId, scope, keyword,
                ProgramID.Value);
        }

        private void DeleteAchievements(IEnumerable<Guid> achievements)
        {
            TaskStore.Delete(ProgramID.Value, achievements);

            TaskGrid.BindModelToControls(ProgramID.Value);
        }

        private int InsertAchievements(IEnumerable<Guid> achievements)
        {
            var items = new List<TTask>();

            foreach (var id in achievements)
            {
                var entity = new TTask
                {
                    ProgramIdentifier = ProgramID.Value,
                    ObjectIdentifier = id,
                    ObjectType = "Achievement",
                    OrganizationIdentifier = Organization.Identifier,
                    TaskCompletionRequirement = "Credential Granted",
                    TaskIdentifier = UniqueIdentifier.Create()
                };

                items.Add(entity);
            }

            TaskStore.Insert(items);

            TaskGrid.BindModelToControls(ProgramID.Value);

            return items.Count;
        }

        private void SaveAchievements()
        {
            var achievements = TaskGrid.GetAchievements();
            var items = new List<TTask>();

            foreach (var achievement in achievements)
            {
                var item = new TTask
                {
                    ProgramIdentifier = ProgramID.Value,
                    ObjectIdentifier = achievement.AchievementIdentifier,
                    TaskLifetimeMonths = achievement.LifetimeMonths,
                    TaskIsRequired = achievement.IsRequired,
                    TaskIsPlanned = achievement.IsPlanned
                };

                items.Add(item);
            }

            TaskStore.Update(items);

            TaskGrid.BindModelToControls(ProgramID.Value);

            ProgramCategoryList.SaveData();
        }

        #endregion
    }
}