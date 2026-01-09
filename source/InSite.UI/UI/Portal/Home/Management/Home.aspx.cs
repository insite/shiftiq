using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using Shift.Common.Timeline.Commands;

using InSite.Admin.Contacts.People.Utilities;
using InSite.Application.Contacts.Read;
using InSite.Application.Courses.Read;
using InSite.Application.Courses.Write;
using InSite.Application.Registrations.Read;
using InSite.Application.Registrations.Write;
using InSite.Application.Sites.Read;
using InSite.Application.Users.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Messages;
using InSite.Persistence;
using InSite.UI.Admin.Assessments.Attempts.Controls.SkillsCheckReport;
using InSite.UI.Layout.Admin;
using InSite.UI.Layout.Portal;
using InSite.Web.Data;
using InSite.Web.Security;

using Shift.Common;
using Shift.Common.Events;
using Shift.Constant;

namespace InSite.UI.Portal.Home.Management
{
    public partial class Home : PortalBasePage
    {
        #region Classes

        [Serializable]
        private class GridKey
        {
            public Guid DistributionId { get; }
            public Guid? LearnerUserId { get; }
            public Guid? AttemptId { get; }

            public GridKey(CourseDistributionGridItem item)
            {
                DistributionId = item.CourseDistributionIdentifier;
                LearnerUserId = item.LearnerUserIdentifier;
                AttemptId = item.AttemptIdentifier;
            }
        }

        #endregion

        #region Properties

        private List<GridKey> GridKeys
        {
            get => (List<GridKey>)ViewState[nameof(GridKeys)];
            set => ViewState[nameof(GridKeys)] = value;
        }

        private HashSet<CourseDistributionGridItem.StatusType> SelectedStatuses
        {
            get => (HashSet<CourseDistributionGridItem.StatusType>)ViewState[nameof(SelectedStatuses)];
            set => ViewState[nameof(SelectedStatuses)] = value;
        }

        private bool IsSkillsCheckAdded => Request.QueryString["added"] == "1";

        #endregion

        #region Fields

        private static readonly CourseDistributionGridItem.StatusType[] _statusItems = new[]
        {
            CourseDistributionGridItem.StatusType.Completed,
            CourseDistributionGridItem.StatusType.InProgress,
            CourseDistributionGridItem.StatusType.NotStarted,
            CourseDistributionGridItem.StatusType.Unassigned
        };

        #endregion

        #region Intialization and Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            AssignUserIdentifier.Filter.UpstreamUserIdentifiers = GetManagerContactBook();
            AssignUserIdentifier.AutoPostBack = true;
            AssignUserIdentifier.ValueChanged += (s, a) => OnAssignUser(AssignUserIdentifier.Value);

            DistributionRepeater.DataBinding += DistributionRepeater_DataBinding;
            DistributionRepeater.ItemCommand += DistributionRepeater_ItemCommand;
            DistributionRepeater.ItemDataBound += DistributionRepeater_ItemDataBound;

            ProductFilter.AutoPostBack = true;
            ProductFilter.ValueChanged += (s, a) => OnFilter();

            StatusRepeater.ItemCreated += StatusRepeater_ItemCreated;

            AddContactUpdatePanel.Request += AddContactUpdatePanel_Request;
            AddContactSaveButton.Click += AddContactSaveButton_Click;

            DownloadButton.Click += DownloadButton_Click;

            CloseBannerButton.ServerClick += CloseBannerButton_ServerClick;
        }

        protected override void OnLoad(EventArgs e)
        {
            if (!IsPostBack)
                CheckRequiredUserFields();

            base.OnLoad(e);

            if (IsPostBack)
                return;

            PortalMaster.ShowAvatar(dashboardUrl: "/ui/portal/profile");
            PortalMaster.RenderHelpContent(null);
            PortalMaster.HideBreadcrumbsOnly();

            PageHelper.AutoBindHeader(this);

            var site = GetCurrentSite();
            Page.Title = site?.SiteTitle ?? "Shift iQ";

            Open();

            var purchasedCount = TPersonFieldSearch.GetSkillsCheckPurchasedCount(Organization.Identifier, User.Identifier);
            if (purchasedCount > 0)
            {
                BannerPanel.Visible = true;
                BannerMessage.InnerText = $"{purchasedCount:n0} SkillsCheck have been added to your account.";
            }
        }

        private void CheckRequiredUserFields()
        {
            var user = User != null ? UserSearch.Select(User.UserIdentifier) : null;
            if (user == null)
                return;

            var type = user.GetType();

            foreach (var field in Organization.Fields.User)
            {
                if (!field.IsRequired)
                    continue;

                var prop = type.GetProperty(field.FieldName);
                if (prop != null && prop.GetValue(user) == null)
                    Response.Redirect("/ui/portal/profile", true);
            }
        }

        public static QSite GetCurrentSite()
        {
            var organization = CurrentSessionState.Identity.Organization;

            var portalName = $"{organization.Code}.{ServiceLocator.AppSettings.Security.Domain}";

            return ServiceLocator.SiteSearch.BindFirst(x => x, x => x.SiteDomain == portalName);
        }

        #endregion

        #region Event handlers

        private void CloseBannerButton_ServerClick(object sender, EventArgs e)
        {
            TPersonFieldStore.SetSkillsCheckPurchasedCount(Organization.Identifier, User.Identifier, 0);
            BannerPanel.Visible = false;
        }

        private void StatusRepeater_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var isSelected = (ICheckBox)e.Item.FindControl("IsSelected");
            isSelected.CheckedChanged += Status_CheckedChanged;
        }

        private void Status_CheckedChanged(object sender, EventArgs e)
        {
            var checkbox = (ICheckBox)sender;
            var value = checkbox.Value.ToEnum<CourseDistributionGridItem.StatusType>();

            if (checkbox.Checked)
                SelectedStatuses.Add(value);
            else
                SelectedStatuses.Remove(value);

            OnFilter();
        }

        private void OnFilter()
        {
            var data = GetDistributions();
            var filteredData = ApplyFilters(data);

            RefreshView(filteredData, data);
        }

        private void OnAssignUser(Guid? userId)
        {
            var distributionId = ValueConverter.ToGuidNullable(AssignFormIdentifier.Value);

            if (userId.HasValue && distributionId.HasValue)
            {
                var distribution = ServiceLocator.CourseDistributionSearch.GetCourseDistribution(distributionId.Value);
                if (distribution == null || distribution.CourseEnrollmentIdentifier.HasValue)
                    return;

                AddLearnerToClass(distribution, userId.Value);
                AddLearnerToDistribution(distribution, userId.Value);
                SendWelcomeEmail(userId.Value);

                OnFilter();
            }

            AssignUserIdentifier.Value = null;
            AssignFormIdentifier.Value = null;
        }

        private void RefreshView(CourseDistributionGridItem[] filteredData, CourseDistributionGridItem[] allData)
        {
            BindDistributionsGrid(filteredData);
            BindStatusProgress(allData);
            BindSkillsChecksProgress(allData);
        }

        private void DistributionRepeater_DataBinding(object sender, EventArgs e)
        {
            GridKeys = new List<GridKey>();
        }

        private void DistributionRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var dataItem = (CourseDistributionGridItem)e.Item.DataItem;
            GridKeys.Add(new GridKey(dataItem));
        }

        private void DistributionRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            var key = GridKeys[e.Item.ItemIndex];

            if (e.CommandName == "Resend")
            {
                if (key.LearnerUserId.HasValue)
                    SendWelcomeEmail(key.LearnerUserId.Value);
            }
            else if (e.CommandName == "Cancel" && key.LearnerUserId.HasValue)
            {
                var distribution = ServiceLocator.CourseDistributionSearch.GetCourseDistribution(key.DistributionId);

                if (distribution == null)
                    return;

                RemoveLearnerFromClass(distribution, key.LearnerUserId.Value);
                RemoveLearnerFromDistribution(distribution, key.LearnerUserId.Value);

                OnFilter();
            }
        }

        private void DownloadButton_Click(object sender, EventArgs e)
        {
            var attemptId = Guid.Parse(AttemptIdField.Value);

            if (GridKeys.Find(x => x.AttemptId == attemptId) == null)
                return;

            var pdf = SkillsCheckReportControl.GetPdf(this, attemptId, User.Identifier, Organization.Identifier, User.TimeZone);

            Response.SendFile("skillscheck_report.pdf", pdf);
        }

        private void AddContactUpdatePanel_Request(object sender, StringValueArgs e)
        {
            if (e.Value == "init")
            {
                AddContactFirstName.Text = null;
                AddContactLastName.Text = null;
                AddContactEmail.Text = null;
            }
        }

        private void AddContactSaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var (user, _) = GetOrCreateContact(AddContactEmail.Text, AddContactFirstName.Text, AddContactLastName.Text);
            var command = new ConnectUser(User.Identifier, user.UserIdentifier, false, true, false, false, DateTimeOffset.UtcNow);

            var connection = ServiceLocator.UserSearch.GetConnection(User.Identifier, user.UserIdentifier);
            if (connection != null)
            {
                command.IsLeader = connection.IsLeader;
                command.IsSupervisor = connection.IsSupervisor;
                command.IsValidator = connection.IsValidator;
                command.Connected = connection.Connected;
            }

            ServiceLocator.SendCommand(command);

            OnFilter();

            ScriptManager.RegisterStartupScript(Page, typeof(Home), "close_addcontact", "dashboardHome.closeAddContact();", true);
        }

        #endregion

        #region Binding

        private void Open()
        {
            var distributions = GetDistributions();

            SelectedStatuses = new HashSet<CourseDistributionGridItem.StatusType>
            {
                CourseDistributionGridItem.StatusType.Assigned,
                CourseDistributionGridItem.StatusType.Completed,
                CourseDistributionGridItem.StatusType.InProgress,
                CourseDistributionGridItem.StatusType.NotStarted,
                CourseDistributionGridItem.StatusType.Unassigned,
            };

            BindSkillsChecksComboBox(distributions);
            RefreshView(distributions, distributions);

            if (IsSkillsCheckAdded)
                HomeStatus.AddMessage(AlertType.Success, "Your payment was successful, and the SkillsCheck has been added to your account.");
        }

        private CourseDistributionGridItem[] GetDistributions()
        {
            return ServiceLocator.CourseDistributionSearch
                .GetCourseDistributionsByManager(Organization.Identifier, User.Identifier)
                .ToArray();
        }

        private void AddLearnerToDistribution(TCourseDistribution distribution, Guid userLearnerIdentifier)
        {
            var commands = new List<Command>();

            var courseEnrollmentId = UniqueIdentifier.Create();
            commands.Add(new AddCourseEnrollment(distribution.CourseIdentifier.Value, userLearnerIdentifier, courseEnrollmentId, DateTimeOffset.UtcNow));

            ServiceLocator.SendCommands(commands);

            distribution.CourseEnrollmentIdentifier = courseEnrollmentId;
            distribution.DistributionStatus = CourseDistributionStatus.Assigned.ToString();
            distribution.DistributionAssigned = DateTimeOffset.UtcNow;

            ServiceLocator.CourseDistributionStore.UpdateCourseDistribution(distribution);

            TPersonFieldStore.SetHideSkillsCheckLearnerBanner(Organization.Identifier, userLearnerIdentifier, false);
        }

        private void BindSkillsChecksComboBox(IEnumerable<CourseDistributionGridItem> distributions)
        {
            ProductFilter.Items.Clear();

            var purchased = distributions
                .Where(d => d.ProductIdentifier != Guid.Empty)
                .Select(d => new
                {
                    d.ProductIdentifier,
                    d.ProductName
                })
                .Distinct()
                .OrderBy(x => x.ProductName)
                .ToList();

            if (purchased.Count == 0)
                return;

            ProductFilter.Items.Add(new ComboBoxOption());

            foreach (var item in purchased)
            {
                ProductFilter.Items.Add(new ComboBoxOption
                {
                    Text = item.ProductName,
                    Value = item.ProductIdentifier.ToString()
                });
            }
        }

        private void AddLearnerToClass(TCourseDistribution distribution, Guid userLearnerIdentifier)
        {
            var commands = new List<Command>();

            if (!distribution.EventIdentifier.HasValue)
                return;

            var eventId = distribution.EventIdentifier.Value;
            var seat = ServiceLocator.EventSearch.GetSeats(eventId, false).FirstOrDefault();

            if (seat == null)
                return;

            var employer = PersonCriteria.BindFirst(
                x => x.EmployerGroup,
                new PersonFilter
                {
                    OrganizationIdentifier = Organization.Identifier,
                    UserIdentifier = User.UserIdentifier
                });

            var registrationId = UniqueIdentifier.Create();

            commands.Add(new RequestRegistration(registrationId, Organization.OrganizationIdentifier, eventId, userLearnerIdentifier, null, null, null, null, null));
            commands.Add(new ModifyRegistrationRequestedBy(registrationId, User.UserIdentifier));

            Guid? userEmployer = employer?.GroupIdentifier;
            if (userEmployer.HasValue)
                commands.Add(new AssignEmployer(registrationId, userEmployer));

            commands.Add(new ChangeApproval(registrationId, "Registered", null, null, null));
            commands.Add(new AssignSeat(registrationId, seat.SeatIdentifier, null, null));

            ServiceLocator.SendCommands(commands);
        }

        private void RemoveLearnerFromDistribution(TCourseDistribution distribution, Guid userLearnerIdentifier)
        {
            if (!distribution.CourseEnrollmentIdentifier.HasValue || !distribution.CourseIdentifier.HasValue)
                return;

            ServiceLocator.SendCommand(new RemoveCourseEnrollment(
                distribution.CourseIdentifier.Value,
                distribution.CourseEnrollmentIdentifier.Value));

            distribution.CourseEnrollmentIdentifier = null;
            distribution.DistributionStatus = CourseDistributionStatus.NotAssigned.ToString();

            ServiceLocator.CourseDistributionStore.UpdateCourseDistribution(distribution);
        }

        private void RemoveLearnerFromClass(TCourseDistribution distribution, Guid userLearnerIdentifier)
        {
            if (!distribution.EventIdentifier.HasValue)
                return;

            var registration = ServiceLocator.RegistrationSearch.GetRegistration(new QRegistrationFilter()
            {
                EventIdentifier = distribution.EventIdentifier.Value,
                CandidateIdentifier = userLearnerIdentifier
            });

            if (registration == null)
                return;

            ServiceLocator.SendCommand(new DeleteRegistration(registration.RegistrationIdentifier, false));
        }

        private void BindStatusProgress(CourseDistributionGridItem[] distributions)
        {
            var counts = CalculateStatusCounts(distributions);
            var total = counts[CourseDistributionGridItem.StatusType.None];

            StatusRepeater.DataSource = _statusItems.Select(x => new
            {
                Title = x.GetDescription(),
                Value = x.GetName(),
                Total = total,
                Count = counts[x],
                Checked = SelectedStatuses.Contains(x)
            });
            StatusRepeater.DataBind();

            StatusFiltersUpdatePanel.Update();
        }

        private void BindSkillsChecksProgress(CourseDistributionGridItem[] distributions)
        {
            ProductProgress.Text = GetProgressHtml(
                distributions.Length,
                distributions.Where(x => x.CourseEnrollmentIdentifier != null).Count(),
                true, true);
            ProductUpdatePanel.Update();
        }

        private void BindDistributionsGrid(CourseDistributionGridItem[] distributions)
        {
            DistributionRepeater.Visible = distributions.Length > 0;
            DistributionRepeater.DataSource = distributions;
            DistributionRepeater.DataBind();
            DistributionUpdatePanel.Update();
        }

        private Dictionary<CourseDistributionGridItem.StatusType, int> CalculateStatusCounts(CourseDistributionGridItem[] distributions)
        {
            int unassigned = 0, assigned = 0, notStarted = 0, inProgress = 0, completed = 0;

            foreach (var item in distributions)
            {
                var status = item.GetStatus();
                switch (status)
                {
                    case CourseDistributionGridItem.StatusType.Unassigned:
                        unassigned++;
                        break;
                    case CourseDistributionGridItem.StatusType.Assigned:
                        assigned++;
                        break;
                    case CourseDistributionGridItem.StatusType.NotStarted:
                        notStarted++;
                        break;
                    case CourseDistributionGridItem.StatusType.InProgress:
                        inProgress++;
                        break;
                    case CourseDistributionGridItem.StatusType.Completed:
                        completed++;
                        break;
                }
            }

            return new Dictionary<CourseDistributionGridItem.StatusType, int>
            {
                { CourseDistributionGridItem.StatusType.None, distributions.Length },
                { CourseDistributionGridItem.StatusType.Unassigned, unassigned },
                { CourseDistributionGridItem.StatusType.Assigned, assigned },
                { CourseDistributionGridItem.StatusType.NotStarted, notStarted },
                { CourseDistributionGridItem.StatusType.InProgress, inProgress },
                { CourseDistributionGridItem.StatusType.Completed, completed }
            };
        }

        private CourseDistributionGridItem[] ApplyFilters(CourseDistributionGridItem[] distributions)
        {
            var distributionQuery = distributions
                .Where(x => SelectedStatuses.Contains(x.GetStatus()));

            if (ProductFilter.HasValue)
            {
                var productId = ProductFilter.ValueAsGuid.Value;
                distributionQuery = distributionQuery.Where(x => x.ProductIdentifier == productId);
            }

            return distributionQuery.ToArray();
        }

        protected string GetGridStatusHtml()
        {
            var item = (CourseDistributionGridItem)Page.GetDataItem();
            var status = item.GetStatus();

            switch (status)
            {
                case CourseDistributionGridItem.StatusType.Unassigned:
                    return "<i class=\"far fa-circle\"></i> Unassigned";
                case CourseDistributionGridItem.StatusType.NotStarted:
                case CourseDistributionGridItem.StatusType.Assigned:
                    return "<i class=\"fas fa-circle text-info\"></i> Not Started";
                case CourseDistributionGridItem.StatusType.InProgress:
                    return "<i class=\"fas fa-circle text-warning\"></i> In Progress";
                case CourseDistributionGridItem.StatusType.Completed:
                    return "<i class=\"fas fa-circle text-success\"></i> Completed on " + LocalizeDate(Eval("AttemptGraded"));
                default:
                    throw new ArgumentException($"Unsupporetd status: {status}");
            }
        }

        protected string GetGridScoreHtml()
        {
            var item = (CourseDistributionGridItem)Page.GetDataItem();
            return !item.AttemptGraded.HasValue ? string.Empty : $"{item.AttemptScore:p0}";
        }

        protected string GetProgressHtml(int total, int value, bool showValue, bool showTotal)
        {
            var progressValue = total == 0
                ? 0
                : Number.CheckRange(Math.Round((decimal)value / total * 100m, 0), 0, 100);

            var text = showValue && showTotal
                ? $"<span>{value:n0}/{total:n0}</span>"
                : showValue
                    ? $"<span>{value:n0}</span>"
                    : showTotal
                        ? $"<span>{total:n0}</span>"
                        : string.Empty;

            return $"<div class=\"circular-progress skills-progress\" style=\"--ar-progress-value:{progressValue};\">"
                 + text
                 + $"</div>";
        }

        #endregion

        #region Other

        private Guid[] GetManagerContactBook()
            => new Guid[] { User.UserIdentifier };

        private void SendWelcomeEmail(Guid userId)
        {
            var user = ServiceLocator.UserSearch.GetUser(userId)
                ?? throw new ArgumentException($"{userId} does not exist");

            if (user.IsDefaultPassword()
                && ServiceLocator.CourseDistributionSearch.CountCourseDistributions(new TCourseDistributionFilter { LearnerUserIdentifier = userId }) == 1
                )
            {
                SendWelcomeEmailToNewUser(user);
            }
            else
            {
                SendWelcomeEmailToExistingUser(user);
            }
        }

        private void SendWelcomeEmailToNewUser(QUser user)
        {
            var newLearnerAlert = new AlertLearningWelcomeEmail
            {
                AppUrl = ServiceLocator.Urls.GetApplicationUrl(Organization.Code),
                Email = user.Email,
                FirstName = user.FirstName,
                Password = user.DefaultPassword,
            };

            ServiceLocator.AlertMailer.Send(Organization.Identifier, user.UserIdentifier, newLearnerAlert);
        }

        private void SendWelcomeEmailToExistingUser(QUser user)
        {
            var alert = new AlertWelcomeLearner
            {
                AppUrl = ServiceLocator.Urls.GetApplicationUrl(Organization.Code),
                FirstName = user.FirstName
            };

            ServiceLocator.AlertMailer.Send(Organization.Identifier, user.UserIdentifier, alert);
        }

        private static (QUser User, QPerson Person) GetOrCreateContact(string email, string firstName, string lastName)
        {
            var user = ServiceLocator.UserSearch.GetUserByEmail(email);
            var isNewUser = user == null;
            var person = isNewUser
                ? null
                : ServiceLocator.PersonSearch.GetPerson(user.UserIdentifier, Organization.OrganizationIdentifier);
            var isNewPerson = person == null;
            var autoGroupJoinId = Organization.Toolkits.Accounts?.AutomaticGroupJoin;

            if (isNewUser)
            {
                user = UserFactory.Create();
                user.FirstName = firstName;
                user.LastName = lastName;
                user.Email = email;
            }

            if (isNewPerson)
            {
                var currentPerson = ServiceLocator.PersonSearch.GetPerson(
                    User.UserIdentifier, Organization.OrganizationIdentifier);

                person = UserFactory.CreatePerson(Organization.OrganizationIdentifier);
                person.UserIdentifier = user.UserIdentifier;
                person.EmailEnabled = true;
                person.EmployerGroupIdentifier = currentPerson.EmployerGroupIdentifier;

                if (autoGroupJoinId.HasValue)
                {
                    person.UserAccessGranted = DateTimeOffset.UtcNow;
                    person.UserAccessGrantedBy = User.FullName;
                }
            }

            if (isNewUser)
                UserStore.Insert(user, person);
            else if (isNewPerson)
                PersonStore.Insert(person);

            if (isNewPerson)
            {
                AddMembership(person.UserIdentifier, autoGroupJoinId);
                AddMembership(person.UserIdentifier, person.EmployerGroupIdentifier);

                if (person.UserAccessGranted.HasValue)
                {
                    PersonHelper.SendAccountCreated(Organization.OrganizationIdentifier, Organization.LegalName, user, person);

                    if (!Organization.Toolkits.Portal.NotSendWelcomeMessage)
                        PersonHelper.SendWelcomeMessage(Organization.OrganizationIdentifier, user.UserIdentifier);
                }
            }

            return (user, person);
        }

        private static void AddMembership(Guid userId, Guid? group)
        {
            if (!group.HasValue || !MembershipPermissionHelper.CanModifyMembership(group.Value))
                return;

            MembershipHelper.Save(new Membership
            {
                UserIdentifier = userId,
                GroupIdentifier = group.Value,
                Assigned = DateTimeOffset.UtcNow
            });
        }

        #endregion
    }
}