using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using Common.Timeline.Commands;

using InSite.Application.Credentials.Write;
using InSite.Common.Web.Cmds;
using InSite.Common.Web.UI;
using InSite.Domain.Records;
using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;
using InSite.UI.Layout.Admin;

using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Cmds.Admin.Records.Programs
{
    public partial class Assign : AdminBasePage, ICmdsUserControl
    {
        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DepartmentIdentifier.AutoPostBack = true;
            DepartmentIdentifier.ValueChanged += DepartmentIdentifier_ValueChanged;

            Step1NextButton.Click += Step1NextButton_Click;
            ConfirmButton.Click += ConfirmButton_Click;

            EmployeeRequired.ServerValidate += EmployeeRequired_ServerValidate;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                PageHelper.AutoBindHeader(this);

                DepartmentIdentifier.Filter.OrganizationIdentifier = Organization.Identifier;
                DepartmentIdentifier.Value = null;

                ProgramIdentifier.DepartmentIdentifier = Guid.Empty;
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            SelectAllLearnersButton.OnClientClick = $"return setCheckboxes('{LearnersPanel.ClientID}', true);";
            UnselectAllLearnersButton.OnClientClick = $"return setCheckboxes('{LearnersPanel.ClientID}', false);";

            base.OnPreRender(e);
        }

        #endregion

        #region Event handlers

        private void DepartmentIdentifier_ValueChanged(object sender, EventArgs e)
        {
            var departmentId = DepartmentIdentifier.Value;

            TemplateField.Visible = departmentId.HasValue;

            ProgramIdentifier.DepartmentIdentifier = departmentId ?? Guid.Empty;
            ProgramIdentifier.Value = null;

            LearnersPanel.Visible = departmentId.HasValue;

            if (departmentId.HasValue)
            {
                var persons = UserSearch.Bind(
                    x => new
                    {
                        x.UserIdentifier,
                        x.FullName,
                    },
                    new UserFilter
                    {
                        MembershipGroupIdentifier = departmentId,
                        MembershipType = "Department",
                        MembershipTypeAnd = true
                    },
                    "FullName"
                );

                LearnersRepeater.Visible = persons.Length > 0;
                LearnersRepeater.DataSource = persons;
                LearnersRepeater.DataBind();

                NoLearnersPanel.Visible = persons.Length == 0;
            }
        }

        private void Step1NextButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            Step2.Visible = true;
            Step2.IsSelected = true;

            var count = LoadAssignLearnerItems();

            Step2.SetTitle("Pending Changes", count);
        }

        private void ConfirmButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var settings = GetAssignLearnerItems();

            if (settings.Count == 0)
            {
                EditorStatus.AddMessage(AlertType.Error, "No achievements selected");
                return;
            }

            var programIdentifier = ProgramIdentifier.Value.Value;
            var contactIds = settings.Select(x => x.UserIdentifier).Distinct().ToList();
            var achievementIds = settings.Select(x => x.AchievementIdentifier).Distinct().ToList();
            var credentials = VCmdsCredentialSearch.Select(x => contactIds.Contains(x.UserIdentifier) && achievementIds.Contains(x.AchievementIdentifier));
            var commands = new List<Command>();

            foreach (var setting in settings)
            {
                var achievement = setting.AchievementIdentifier;
                var learner = setting.UserIdentifier;

                try
                {
                    ProgramStore.InsertEnrollment(Organization.Identifier, programIdentifier, setting.UserIdentifier, User.Identifier);

                    var label = ServiceLocator.AchievementSearch.GetAchievement(achievement).AchievementLabel;
                    var credential = credentials.FirstOrDefault(x => x.UserIdentifier == learner && x.AchievementIdentifier == achievement);

                    var expiration = setting.LifetimeMonths.HasValue
                        ? new Expiration { Type = ExpirationType.Relative, Lifetime = new Lifetime { Quantity = setting.LifetimeMonths.Value, Unit = "Month" } }
                        : null;

                    var necessity = setting.IsRequired ? "Mandatory" : "Optional";
                    var priority = setting.IsPlanned ? "Planned" : "Unplanned";
                    var authority = EmployeeAchievementHelper.TypeAllowsSignOff(label) ? "Self" : null;

                    if (credential == null)
                    {
                        var id = ServiceLocator.AchievementSearch.GetCredentialIdentifier(null, achievement, learner);
                        commands.Add(new CreateCredential(id, Organization.OrganizationIdentifier, achievement, learner, DateTimeOffset.Now));
                        commands.Add(new ChangeCredentialExpiration(id, expiration));
                        commands.Add(new TagCredential(id, necessity, priority));
                        commands.Add(new ChangeCredentialAuthority(id, null, null, authority, null, null, null));
                    }
                    else if (credential.CredentialExpirationLifetimeQuantity != setting.LifetimeMonths
                        || credential.CredentialIsMandatory != setting.IsRequired
                        || credential.IsInTrainingPlan != setting.IsPlanned
                        )
                    {
                        commands.Add(new ChangeCredentialExpiration(credential.CredentialIdentifier, expiration));
                        commands.Add(new TagCredential(credential.CredentialIdentifier, necessity, priority));
                    }
                }
                catch (Exception ex)
                {
                    var message = $"Unable to assign achievement {achievement} to learner {learner}.";
                    throw new PresentationException(message, ex);
                }
            }

            foreach (var command in commands)
            {
                try
                {
                    ServiceLocator.SendCommand(command);
                }
                catch (DuplicateCredentialException)
                {
                    // Ignore if the credential already exists.
                }
            }

            LoadAssignLearnerItems();

            EditorStatus.AddMessage(AlertType.Success, "This training plan has been successfully assigned to the users you selected.");

            Step2.Visible = false;
            Step1.IsSelected = true;

            DepartmentIdentifier.Value = null;

            ProgramIdentifier.Value = null;
            TemplateField.Visible = false;

            LearnersPanel.Visible = false;
        }

        private void EmployeeRequired_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = GetSelectedUsers().Any();
        }

        #endregion

        #region Helper methods

        private int LoadAssignLearnerItems()
        {
            var achievementListIdentifier = ProgramIdentifier.Value.Value;
            var userIds = GetSelectedUsers().ToArray();
            var users = UserSearch.Bind(
                x => new { x.UserIdentifier, x.FullName },
                new UserFilter { IncludeUserIdentifiers = userIds }, "FullName");
            var tasks = TaskSearch.SelectByProgram(achievementListIdentifier);
            var achievementIds = tasks.Select(x => x.AchievementIdentifier);
            var achievements = VCmdsAchievementSearch.Select(x => achievementIds.Contains(x.AchievementIdentifier)).OrderBy(x => x.AchievementTitle);
            var credentials = VCmdsCredentialSearch.Select(x => userIds.Contains(x.UserIdentifier) && achievementIds.Contains(x.AchievementIdentifier));

            var changes = new List<AssignLearnerItem>();

            foreach (var user in users)
            {
                foreach (var achievement in achievements)
                {
                    var task = tasks.FirstOrDefault(
                        x => x.AchievementIdentifier == achievement.AchievementIdentifier);

                    var credential = credentials.FirstOrDefault(
                        x => x.UserIdentifier == user.UserIdentifier &&
                        x.AchievementIdentifier == achievement.AchievementIdentifier);

                    var isNew = credential == null;

                    var change = new AssignLearnerItem
                    {
                        UserIdentifier = user.UserIdentifier,
                        FullName = user.FullName,
                        AchievementIdentifier = achievement.AchievementIdentifier,
                        AchievementTitle = achievement.AchievementTitle,
                        AchievementLabel = achievement.AchievementLabel,
                        LifetimeMonths = task.LifetimeMonths,
                        Action = isNew ? "New" : "Update",
                        IsRequired = task.IsRequired,
                        IsPlanned = task.IsPlanned
                    };

                    changes.Add(change);
                }

                if (AssignStrategy_NoChange.Checked)
                    continue;

                // Find all other achievements previously assigned to the learner

                var currentCredentials = VCmdsCredentialSearch
                    .Select(x => x.UserIdentifier == user.UserIdentifier &&
                                 !achievementIds.Contains(x.AchievementIdentifier))
                    .OrderBy(x => x.AchievementTitle);

                foreach (var current in currentCredentials)
                {
                    var isPlanned = AssignStrategy_PlanAndRequire.Checked || AssignStrategy_PlanAndRecommend.Checked;

                    var isRequired = AssignStrategy_PlanAndRequire.Checked;

                    var action = (isPlanned && isRequired)
                        ? "Add to training plan (required)"
                        : (isPlanned && !isRequired)
                        ? "Add to training plan (not required)"
                        : (!isPlanned && !isRequired)
                        ? "Remove from training plan (not required)"
                        : "Remove from training plan (but still required!)";

                    if (isPlanned == current.IsInTrainingPlan)
                    {
                        if (isRequired && current.CredentialPriority == "Required")
                            continue;

                        if (!isRequired && current.CredentialPriority != "Required")
                            continue;
                    }

                    var change = new AssignLearnerItem
                    {
                        UserIdentifier = user.UserIdentifier,
                        FullName = user.FullName,
                        AchievementIdentifier = current.AchievementIdentifier,
                        AchievementTitle = current.AchievementTitle,
                        AchievementLabel = current.AchievementLabel,
                        LifetimeMonths = current.CredentialExpirationLifetimeQuantity,
                        Action = action,
                        IsRequired = isRequired,
                        IsPlanned = isPlanned
                    };

                    if (current.CredentialExpirationLifetimeUnit == "Year")
                        change.LifetimeMonths = current.CredentialExpirationLifetimeQuantity * 12;

                    else if (current.CredentialExpirationLifetimeUnit == "Month")
                        change.LifetimeMonths = current.CredentialExpirationLifetimeQuantity;

                    changes.Add(change);
                }
            }

            LearnerSettings.DataSource = changes;
            LearnerSettings.DataBind();

            return changes.Count;
        }

        private IEnumerable<Guid> GetSelectedUsers()
        {
            foreach (RepeaterItem repeaterItem in LearnersRepeater.Items)
            {
                var isSelected = (ICheckBoxControl)repeaterItem.FindControl("IsSelected");
                if (!isSelected.Checked)
                    continue;

                var userIdentifier = (ITextControl)repeaterItem.FindControl("UserIdentifier");

                yield return Guid.Parse(userIdentifier.Text);
            }
        }

        private List<AssignLearnerItem> GetAssignLearnerItems()
        {
            var list = new List<AssignLearnerItem>();

            foreach (RepeaterItem repeaterItem in LearnerSettings.Items)
            {
                var isRequired = ((ICheckBoxControl)repeaterItem.FindControl("IsRequired")).Checked;
                var isPlanned = ((ICheckBoxControl)repeaterItem.FindControl("IsPlanned")).Checked;

                var data = ((ITextControl)repeaterItem.FindControl("Data")).Text.Split(new[] { ':' });
                var userIdentifier = Guid.Parse(data[0]);
                var achievementIdentifier = Guid.Parse(data[1]);
                var lifetimeMonths = data.Length > 2 && !string.IsNullOrEmpty(data[2]) ? int.Parse(data[2]) : (int?)null;

                var item = new AssignLearnerItem
                {
                    UserIdentifier = userIdentifier,
                    AchievementIdentifier = achievementIdentifier,
                    LifetimeMonths = lifetimeMonths,
                    IsRequired = isRequired,
                    IsPlanned = isPlanned
                };

                list.Add(item);
            }

            return list;
        }

        #endregion
    }
}