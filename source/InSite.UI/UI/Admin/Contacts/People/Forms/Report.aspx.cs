using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;

using InSite.Admin.Contacts.People.Controls;
using InSite.Application.Attempts.Read;
using InSite.Application.Contacts.Read;
using InSite.Application.Issues.Read;
using InSite.Application.Records.Read;
using InSite.Application.Registrations.Read;
using InSite.Application.Surveys.Read;
using InSite.Common.Web;
using InSite.Persistence;
using InSite.UI.Admin.Events.Classes.Controls;
using InSite.UI.Layout.Admin;
using InSite.Web.Helpers;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Constant;

namespace InSite.Admin.Contacts.People.Forms
{
    public partial class Report : AdminBasePage, IHasParentLinkParameters
    {
        #region Classes

        private class UserAddress
        {
            public string AddressType { get; set; }
            public QPersonAddress Address { get; set; }
            public string GmapsLink { get; set; }
        }

        [Serializable]
        private class CertificateDataItem
        {
            public Guid ProgressionIdentifier { get; set; }
            public string ResourceTitle { get; set; }
            public DateTimeOffset? Assigned { get; set; }
            public DateTimeOffset? Completed { get; set; }
            public DateTimeOffset? Expired { get; set; }
            public DateTimeOffset? Validated { get; set; }
            public decimal ScoreScaled { get; set; }
            public int? AttendanceDaysAbsent { get; set; }
            public string CreditIdentifier { get; set; }
            public string ApprenticeshipNumber { get; set; }
            public string QualificationNumber { get; set; }
        }

        [Serializable]
        public class AssesmentDataItem
        {
            public Guid AttemptIdentifier { get; set; }
            public Guid BankIdentifier { get; set; }
            public Guid FormIdentifier { get; set; }

            public string FormAsset { get; set; }
            public string FormName { get; set; }
            public string FormTime { get; set; }
            public string FormTitle { get; set; }
            public string GradingAssessor { get; set; }

            public bool IsAdmin { get; set; }
        }

        [Serializable]
        private class SurveyDataItem
        {
            public Guid ResponseSessionIdentifier { get; }
            public Guid SurveyFormIdentifier { get; }
            public DateTimeOffset? Started { get; }
            public DateTimeOffset? Completed { get; }
            public bool IsLocked { get; }

            public string SurveyName { get; }
            public bool EnableUserConfidentiality { get; }

            public string GroupName { get; set; }
            public string PeriodName { get; set; }

            public SurveyDataItem(ISurveyResponse entity)
            {
                ResponseSessionIdentifier = entity.ResponseSessionIdentifier;
                SurveyFormIdentifier = entity.SurveyFormIdentifier;
                Started = entity.ResponseSessionStarted;
                Completed = entity.ResponseSessionCompleted;
                IsLocked = entity.ResponseIsLocked;

                SurveyName = entity.SurveyName;
                EnableUserConfidentiality = entity.SurveyIsConfidential;

                if (entity.GroupIdentifier.HasValue)
                    GroupName = entity.GroupName;

                if (entity.PeriodIdentifier.HasValue)
                    PeriodName = entity.PeriodName;
            }
        }

        [Serializable]
        private class AchievementDataItem
        {
            public string AchievementTitle { get; set; }
            public string CredentialStatus { get; set; }
            public DateTimeOffset? CredentialGranted { get; set; }
            public DateTimeOffset? CredentialRevoked { get; set; }
            public DateTimeOffset? CredentialExpired { get; set; }
        }

        [Serializable]
        private class CommentDataItem
        {
            public Guid Identifier { get; set; }
            public string AuthorName { get; set; }
            public DateTimeOffset Posted { get; set; }
            public string Description { get; set; }
        }

        [Serializable]
        private class MessageDataItem
        {
            public Guid MailoutIdentifier { get; set; }
            public string SenderEmail { get; set; }
            public string SenderName { get; set; }
            public string ContentSubject { get; set; }
            public DateTimeOffset? DeliveryCompleted { get; set; }
            public string ViewEmailUrl { get; set; }
        }

        [Serializable]
        private class CaseDataItem
        {
            public Guid IssueIdentifier { get; set; }
            public string IssueStatusName { get; set; }
            public string IssueStatusCategoryHtml { get; set; }
            public string IssueTitle { get; set; }
            public int AttachmentCount { get; set; }
            public int CommentCount { get; set; }
            public DateTimeOffset? IssueOpened { get; set; }
            public DateTimeOffset? IssueClosed { get; set; }
        }

        private enum MessageEntityType { Email, Delivery }

        #endregion

        #region Properties

        private const string SearchUrl = "/ui/admin/contacts/people/search";

        protected Guid? UserIdentifier => Guid.TryParse(Request.QueryString["contact"], out var contact) ? contact : (Guid?)null;

        private AchievementDataItem[] AchievementDataSource
        {
            get => (AchievementDataItem[])ViewState[nameof(AchievementDataSource)];
            set => ViewState[nameof(AchievementDataSource)] = value;
        }

        private AssesmentDataItem[] AssesmentDataSource
        {
            get => (AssesmentDataItem[])ViewState[nameof(AssesmentDataSource)];
            set => ViewState[nameof(AssesmentDataSource)] = value;
        }

        private CaseDataItem[] CaseDataSource
        {
            get => (CaseDataItem[])ViewState[nameof(CaseDataSource)];
            set => ViewState[nameof(CaseDataSource)] = value;
        }

        private SurveyDataItem[] SurveyDataSource
        {
            get => (SurveyDataItem[])ViewState[nameof(SurveyDataSource)];
            set => ViewState[nameof(SurveyDataSource)] = value;
        }

        private CommentDataItem[] CommentDataSource
        {
            get => (CommentDataItem[])ViewState[nameof(CommentDataSource)];
            set => ViewState[nameof(CommentDataSource)] = value;
        }

        private MessageDataItem[] MessageDataSource
        {
            get => (MessageDataItem[])ViewState[nameof(MessageDataSource)];
            set => ViewState[nameof(MessageDataSource)] = value;
        }

        #endregion

        #region Fields

        private ReturnUrl _returnUrl;

        public static readonly (string Text, string Anchor)[] MoreInfoItems = new[]
        {
            ("Person", "person"),
            ("Membership", "membership"),
            ("Addresses", "addresses"),
            ("Groups", "groups"),
            ("Registrations", "registrations"),
            ("Invoices", "invoices"),
            ("Gradebooks", "gradebooks"),
            ("Cases", "cases"),
            ("Forms", "forms"),
            ("Assessment Attempts", "attempts"),
            ("Achievements", "achievements"),
            ("Comments", "comments"),
            ("Messages", "messages")
        };

        #endregion

        #region Initialization and Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            AchievementGrid.DataBinding += AchievementGrid_DataBinding;
            AssesmentGrid.DataBinding += AssesmentGrid_DataBinding;
            CaseGrid.DataBinding += CaseGrid_DataBinding;
            SurveyGrid.DataBinding += SurveyGrid_DataBinding;
            CommentGrid.DataBinding += CommentGrid_DataBinding;
            MessageGrid.DataBinding += MessageGrid_DataBinding;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            AddressList.EnablePaging = false;
            RoleGrid.EnablePaging = false;
            Registrations.EnablePaging = false;

            if (!IsPostBack)
                LoadData();

            ScriptManager.RegisterStartupScript(
                Page,
                GetType(),
                "initTreeViews",
                "(function () { initGradeTreeView(); })();",
                true);

            PrintButton.Click += OnPrint;
            TrainingRecordsPrintButton.Click += OnTrainingRecordsPrint;
        }

        #endregion

        #region Event handlers

        private void AchievementGrid_DataBinding(object sender, EventArgs e)
        {
            AchievementGrid.DataSource = AchievementDataSource.ApplyPaging(AchievementGrid.Paging).ToList();
        }

        private void AssesmentGrid_DataBinding(object sender, EventArgs e)
        {
            AssesmentGrid.DataSource = AssesmentDataSource.ApplyPaging(AssesmentGrid.Paging).ToList();
        }

        private void CaseGrid_DataBinding(object sender, EventArgs e)
        {
            CaseGrid.DataSource = CaseDataSource.ApplyPaging(CaseGrid.Paging).ToList();
        }

        private void SurveyGrid_DataBinding(object sender, EventArgs e)
        {
            SurveyGrid.DataSource = SurveyDataSource.ApplyPaging(SurveyGrid.Paging).ToList();
        }

        private void CommentGrid_DataBinding(object sender, EventArgs e)
        {
            CommentGrid.DataSource = CommentDataSource.ApplyPaging(CommentGrid.Paging).ToList();
        }

        private void MessageGrid_DataBinding(object sender, EventArgs e)
        {
            MessageGrid.DataSource = MessageDataSource.ApplyPaging(MessageGrid.Paging).ToList();
        }

        private void OnPrint(object sender, EventArgs e)
        {
            var report = (BCPVPAMembershipSummaryReport)LoadControl("BCPVPAMembershipSummaryReport.ascx");
            var person = ServiceLocator.PersonSearch.GetPerson(
                UserIdentifier.Value, Organization.Identifier,
                x => x.User,
                x => x.HomeAddress
            );
            report.LoadReport(person);

            var siteContent = new StringBuilder();
            using (var stringWriter = new StringWriter(siteContent))
            {
                using (var htmlWriter = new HtmlTextWriter(stringWriter))
                    report.RenderControl(htmlWriter);
            }

            var settings = new HtmlConverterSettings(ServiceLocator.AppSettings.Application.WebKitHtmlToPdfExePath)
            {
                Viewport = new HtmlConverterSettings.ViewportSize(980, 1400),
                MarginTop = 22,
                HeaderUrl = "",
                HeaderSpacing = 7,
            };

            var data = HtmlConverter.HtmlToPdf(siteContent.ToString(), settings);

            Response.SendFile($"MembershipSummary-{person.User.FullName}", "pdf", data);
        }

        private void OnTrainingRecordsPrint(object sender, EventArgs e)
        {
            var report = (TrainingRecordsReport)LoadControl("TrainingRecordsReport.ascx");
            var person = ServiceLocator.PersonSearch.GetPerson(UserIdentifier.Value, Organization.Identifier,
                x => x.User,
                x => x.HomeAddress
            );
            report.LoadReport(person);

            var siteContent = new StringBuilder();
            using (var stringWriter = new StringWriter(siteContent))
            {
                using (var htmlWriter = new HtmlTextWriter(stringWriter))
                    report.RenderControl(htmlWriter);
            }

            var settings = new HtmlConverterSettings(ServiceLocator.AppSettings.Application.WebKitHtmlToPdfExePath)
            {
                Viewport = new HtmlConverterSettings.ViewportSize(980, 1400),
                MarginTop = 22,
                HeaderUrl = "",
                HeaderSpacing = 7,
            };

            var data = HtmlConverter.HtmlToPdf(siteContent.ToString(), settings);

            Response.SendFile($"TrainingRecords-{person.User.FullName}", "pdf", data);
        }

        #endregion

        #region Binding

        private void LoadData()
        {
            if (UserIdentifier == null)
            {
                HttpResponseHelper.Redirect(SearchUrl);
                return;
            }

            var person = ServiceLocator.PersonSearch.GetPerson(UserIdentifier.Value, Organization.Identifier,
                x => x.User,
                x => x.HomeAddress,
                x => x.WorkAddress,
                x => x.BillingAddress,
                x => x.ShippingAddress
                );

            if (person == null)
                HttpResponseHelper.Redirect(SearchUrl);

            var employer = person.EmployerGroupIdentifier.HasValue
                ? ServiceLocator.GroupSearch.GetGroup(person.EmployerGroupIdentifier.Value, x => x.Parent)
                : null;

            var employerShippingAddress = employer != null
                ? ServiceLocator.GroupSearch.GetAddress(employer.GroupIdentifier, AddressType.Shipping)
                : null;

            var title = person.User.FullName;
            if (person.PersonCode != null)
                title += $" <span class='form-text'>Account #{person.PersonCode}</span>";

            PageHelper.AutoBindHeader(Page, null, title);

            FullName.Text = person.User.FullName;
            Honorific.Text = person.User.Honorific.HasValue() ? person.User.Honorific + " " : "";

            if (employer != null)
            {
                EmployerPhone.Text = employer.GroupPhone.HasValue() ? employer.GroupPhone : "None";
                EmployerNumber.Text = employer.GroupCode.HasValue() ? employer.GroupCode : "None";
                EmployerName.Text = employer.GroupName.HasValue() ? employer.GroupName : "None";
                if (employerShippingAddress != null)
                {
                    EmployerAddress.Text = ClassVenueAddressInfo.GetAddress(employerShippingAddress);
                    EmployerAddress.NavigateUrl = employerShippingAddress.GetGMapsAddressLink();
                }
                DistrictName.Text = TCollectionItemCache.GetName(employer.GroupStatusItemIdentifier);
            }
            else
            {
                EmployerNumber.Text = "None";
                EmployerName.Text = "None";
                EmployerPhone.Text = "None";
            }

            ShippingPreference.Text = person.ShippingPreference.HasValue() ? person.ShippingPreference : "None";

            var membershipStatus = TCollectionItemCache.GetName(person.MembershipStatusItemIdentifier);

            if (membershipStatus.IsNotEmpty())
            {
                MembershipStatus1.Text = membershipStatus;
                if (person.MemberStartDate.HasValue)
                    MembershipStatus1.Text += $" from {person.MemberStartDate:MMM d, yyyy}";
                if (person.MemberEndDate.HasValue)
                    MembershipStatus1.Text += $" till {person.MemberEndDate:MMM d, yyyy}";
            }
            else
                MembershipStatus1.Text = "None";

            JobTitle.Text = person.JobTitle.HasValue() ? person.JobTitle : "None";

            Email.Text = person.User.Email.HasValue() ? person.User.Email.ToLower() : null;
            Phone.Text = person.Phone.HasValue() ? person.Phone : "None";
            PhoneHome.Text = person.PhoneHome.HasValue() ? person.PhoneHome : "None";
            PhoneWork.Text = person.PhoneWork.HasValue() ? person.PhoneWork : "None";
            PhoneMobile.Text = person.User.PhoneMobile.HasValue() ? person.User.PhoneMobile : "None";
            PhoneOther.Text = person.PhoneOther.HasValue() ? person.PhoneOther : "None";
            ContactCode.Text = person.PersonCode.HasValue() ? person.PersonCode : "None";
            Birthdate.Text = $"{person.Birthdate:MMM d, yyyy}";
            Region.Text = person.Region.HasValue() ? person.Region : "None";
            WebSiteUrl.Text = person.WebSiteUrl.HasValue() ? person.WebSiteUrl : "None";
            TradeworkerNumber.Text = person.TradeworkerNumber.HasValue() ? person.TradeworkerNumber : "None";
            Gender.Text = person.Gender.HasValue() ? person.Gender : "None";
            UnionInfo.Text = person?.EmployeeUnion;
            if (UnionInfo.Text == "")
                UnionInfo.Text = "None";

            EmergencyContactName.Text = person.EmergencyContactName;
            EmergencyContactPhoneNumber.Text = person.EmergencyContactPhone;
            EmergencyContactRelationship.Text = person.EmergencyContactRelationship;
            if (EmergencyContactRelationship.Text == "" && EmergencyContactPhoneNumber.Text == "" && EmergencyContactRelationship.Text == "")
                EmergencyContactName.Text = "None";
            ESL.Text = string.Equals(person.FirstLanguage, "Not English", StringComparison.OrdinalIgnoreCase) ? "Yes" : "No";

            LoadMembershipSection(person.User, person, employer, employerShippingAddress, membershipStatus);

            UserInvoice.LoadData(UserIdentifier.Value);
            CaseSection.Visible = (Identity.Organization.Toolkits.Issues?.EnableWorkflowManagement ?? false) && CurrentSessionState.Identity.IsGranted("Admin/Workflow");

            BindAddresses(person);
            BindGroups(person.User);
            BindRegistrations();
            BindGrades();
            if (CaseSection.Visible)
                BindCases(person.User);
            BindSurveys(person.User);
            BindAttempts(person.User);
            BindAchievements(person.User);
            BindComments(person);
            BindMessages(person);
        }

        private void LoadMembershipSection(QUser user, QPerson person, QGroup employer, QGroupAddress employerShippingAddress, string membershipStatus)
        {
            var isAssociation = Organization.IsAssociation;

            MembershipSection.Visible = isAssociation;
            PrintButton.Visible = isAssociation;

            if (isAssociation)
            {
                MembershipNumber.Text = person.PersonCode.HasValue() ? person.PersonCode : "None";
                MembershipFullName.Text = user.FullName;
                MembershipHonorific.Text = user.Honorific.HasValue() ? user.Honorific + " " : "";

                if (person.HomeAddress != null)
                    MembershipHomeAddress.Text = ClassVenueAddressInfo.GetAddress(person.HomeAddress);
                else
                    MembershipHomeAddress.Text = "None";

                var phones = new StringBuilder();

                if (person.Phone.HasValue())
                    phones.AppendLine($"<div>Preferred: {person.Phone}</div>");
                if (person.PhoneHome.HasValue())
                    phones.AppendLine($"<div>Home: {person.PhoneHome}</div>");
                if (person.PhoneWork.HasValue())
                    phones.AppendLine($"<div>Work: {person.PhoneWork}</div>");
                if (user.PhoneMobile.HasValue())
                    phones.AppendLine($"<div>Cell: {user.PhoneMobile}</div>");
                if (person.PhoneOther.HasValue())
                    phones.AppendLine($"<div>Other: {person.PhoneOther}</div>");


                if (phones.Length > 0)
                    MembershipPhoneNumbers.Text = phones.ToString();
                else
                    MembershipPhoneNumbers.Text = "None";

                if (employer != null)
                {
                    var pN = new StringBuilder();

                    if (employer.GroupPhone.HasValue())
                        pN.AppendLine($"<div>Phone: {employer?.GroupPhone}</div>");

                    if (pN.Length > 0)
                        MembershipSchoolPhoneNumbers.Text = pN.ToString();
                    else
                        MembershipSchoolPhoneNumbers.Text = "None";

                    MembershipSchoolNumber.Text = employer.GroupCode.HasValue() ? "#" + employer.GroupCode : "";
                    MembershipSchoolName.Text = employer.GroupName.HasValue() ? employer.GroupName : "None";
                    if (employerShippingAddress != null)
                        MembershipSchoolAddress.Text = ClassVenueAddressInfo.GetAddress(employerShippingAddress);
                }
                else
                {
                    MembershipSchoolName.Text = "None";
                    MembershipSchoolPhoneNumbers.Text = "None";
                }
                MembershipSchoolDisctrict.Text = employer?.Parent?.GroupName;
                MembershipShippingPreference.Text = person.ShippingPreference.HasValue() ? person.ShippingPreference : "None";

                if (membershipStatus.IsNotEmpty())
                {
                    MembershipStatus2.Text = membershipStatus;
                    if (person.MemberStartDate.HasValue)
                        MembershipStatus2.Text += $" from {person.MemberStartDate:MMM d, yyyy}";
                    if (person.MemberEndDate.HasValue)
                        MembershipStatus2.Text += $" till {person.MemberEndDate:MMM d, yyyy}";
                }
                else
                    MembershipStatus2.Text = "None";

                MembershipPosition.Text = person.JobTitle.HasValue() ? person.JobTitle : "None";

                BindRoles(user);
            }
        }

        private void BindAddresses(QPerson person)
        {
            var list = new List<UserAddress>();

            if (person.HomeAddress != null)
                list.Add(new UserAddress { AddressType = "Home", Address = person.HomeAddress, GmapsLink = person.HomeAddress.GetGMapsAddressLink() });

            if (person.WorkAddress != null)
                list.Add(new UserAddress { AddressType = "Work", Address = person.WorkAddress, GmapsLink = person.WorkAddress.GetGMapsAddressLink() });

            if (person.BillingAddress != null)
                list.Add(new UserAddress { AddressType = "Billing", Address = person.BillingAddress, GmapsLink = person.BillingAddress.GetGMapsAddressLink() });

            if (person.ShippingAddress != null)
                list.Add(new UserAddress { AddressType = "Shipping", Address = person.ShippingAddress, GmapsLink = person.ShippingAddress.GetGMapsAddressLink() });

            AddressList.DataSource = list;
            AddressList.DataBind();

            AddressSectionTitle.Text = $"Addresses ({list.Count:n0})";
        }

        private void BindRoles(QUser user)
        {
            var roles = MembershipSearch
                .Select(x => x.UserIdentifier == user.UserIdentifier, x => x.Group)
                .Where(x => x.Group.GroupType == GroupTypes.Team)
                .OrderBy(x => x.Group.GroupType)
                .ThenBy(x => x.Group.GroupName)
                .ToList();

            ParticipationsRepeater.DataSource = roles;
            ParticipationsRepeater.DataBind();
        }

        private void BindGroups(QUser user)
        {
            var accessibleOrgs = Identity.Organizations.Select(x => x.Identifier).ToArray();
            var roles = MembershipSearch
                .Select(x => x.UserIdentifier == user.UserIdentifier && accessibleOrgs.Contains(x.Group.OrganizationIdentifier), x => x.Group)
                .OrderBy(x => x.Group.GroupType)
                .ThenBy(x => x.Group.GroupName)
                .ToList();

            RoleGrid.DataSource = roles;
            RoleGrid.DataBind();

            RoleSectionTitle.Text = $"Groups ({roles.Count:n0})";
        }

        private void BindRegistrations()
        {
            var filter = new QRegistrationFilter
            {
                CandidateIdentifier = UserIdentifier,
                OrganizationIdentifier = Organization.Identifier
            };

            var registrations = ServiceLocator.RegistrationSearch
                .GetRegistrations(filter, x => x.Event)
                .OrderByDescending(x => x.Event.EventScheduledStart)
                .ThenBy(x => x.RegistrationRequestedOn)
                .Select(x => new
                {
                    x.Event,
                    x.RegistrationRequestedOn,
                    x.WorkBasedHoursToDate,
                    x.ApprovalStatus,
                    Customer = x.CustomerIdentifier.HasValue ? ServiceLocator.GroupSearch.GetGroup(x.CustomerIdentifier.Value) : null,
                    Employer = x.EmployerIdentifier.HasValue ? ServiceLocator.GroupSearch.GetGroup(x.EmployerIdentifier.Value) : null,
                    x.RegistrationFee,
                    x.AttendanceStatus,
                    x.Score,
                    x.RegistrationComment
                })
                .OrderByDescending(x => x.RegistrationRequestedOn)
                .ThenBy(x => x.Event.EventTitle)
                .ToList();

            Registrations.DataSource = registrations;
            Registrations.DataBind();

            NoRegistrations.Visible = registrations.Count == 0;

            RegistrationSectionTitle.Text = $"Registrations ({registrations.Count:n0})";
        }

        private void BindGrades()
        {
            int gradeCount;
            var gradeHierarchy = GetGradeHierarchy(out gradeCount);

            NoGrades.Visible = gradeHierarchy.Count == 0;
            GradePanel.Visible = gradeHierarchy.Count > 0;

            Grades.LoadData(gradeHierarchy);

            GradeSectionTitle.Text = $"Gradebooks ({gradeCount:n0})";
        }

        private List<GradeTreeViewNode.Grade> GetGradeHierarchy(out int gradeCount)
        {
            gradeCount = 0;

            var filter = new QProgressFilter
            {
                StudentUserIdentifier = UserIdentifier,
                OrganizationIdentifier = Organization.Identifier
            };

            var allScores = ServiceLocator.RecordSearch.GetGradebookScores(filter, null, null);

            var groupedScores = allScores
                .GroupBy(x => x.GradebookIdentifier)
                .ToDictionary(x => x.Key, x => x.ToList());

            var gradebookIds = allScores.Select(x => x.GradebookIdentifier).Distinct().ToArray();

            var groupedGradeItems = ServiceLocator.RecordSearch
                .GetGradeItems(new QGradeItemFilter { GradebookIdentifiers = gradebookIds }, x => x.Gradebook)
                .GroupBy(x => x.GradebookIdentifier)
                .ToDictionary(x => x.Key, x => x.ToList());

            var treeViewItems = new List<GradeTreeViewNode.Grade>();

            foreach (var gradebookId in gradebookIds)
            {
                if (!groupedScores.TryGetValue(gradebookId, out var gradebookScores))
                    throw new ArgumentException($"Not scores for GradebookId = '{gradebookId}'");

                if (!groupedGradeItems.TryGetValue(gradebookId, out var gradebookItems))
                    throw new ArgumentException($"Not gradebook items for GradebookId = '{gradebookId}'");

                var (treeViewItem, currentCount) = CreateGradeRootItem(gradebookScores, gradebookItems);

                treeViewItems.Add(treeViewItem);

                gradeCount += currentCount;
            }

            return treeViewItems;
        }

        private (GradeTreeViewNode.Grade, int) CreateGradeRootItem(List<QProgress> scores, List<QGradeItem> gradebookItems)
        {
            var gradebook = gradebookItems[0].Gradebook;

            var treeViewItem = new GradeTreeViewNode.Grade
            {
                ID = gradebook.GradebookIdentifier.ToString(),
                Name = gradebook.GradebookTitle,
                ClassName = gradebook.Event?.EventTitle,
                ClassStartDate = gradebook.Event != null ? gradebook.Event.EventScheduledStart : (DateTimeOffset?)null,
                ClassEndDate = gradebook.Event?.EventScheduledEnd,
                Level = 0,
                ScoreValue = null,
                Comment = null,
                Children = new List<GradeTreeViewNode.Grade>()
            };

            var currentItems = gradebookItems.FindAll(x => x.ParentGradeItemIdentifier == null);

            var gradeCount = AddItems(scores, treeViewItem, currentItems, 1, gradebookItems);

            return (treeViewItem, gradeCount);
        }

        private int AddItems(List<QProgress> scores, GradeTreeViewNode.Grade parent, List<QGradeItem> currentItems, int level, List<QGradeItem> gradebookItems)
        {
            if (currentItems.Count == 0)
                return 0;

            var gradeCount = 0;

            foreach (var inputItem in currentItems)
            {
                if (!inputItem.GradeItemIsReported)
                    continue;

                var score = scores.Find(x => x.GradeItemIdentifier == inputItem.GradeItemIdentifier && x.UserIdentifier == UserIdentifier);
                var scoreValue = GradebookHelper.GetScoreValue(score, inputItem.GradeItemFormat);

                var outputItem = new GradeTreeViewNode.Grade
                {
                    ID = $"{parent.ID}-{inputItem.GradeItemIdentifier}",
                    Name = inputItem.GradeItemName,
                    Level = level,
                    ScoreValue = scoreValue,
                    Comment = score?.ProgressComment,
                    Children = new List<GradeTreeViewNode.Grade>()
                };

                parent.Children.Add(outputItem);

                if (scoreValue != null)
                    gradeCount++;

                var children = gradebookItems.FindAll(x => x.ParentGradeItemIdentifier == inputItem.GradeItemIdentifier);

                gradeCount += AddItems(scores, outputItem, children, level + 1, gradebookItems);
            }

            return gradeCount;
        }

        private void BindCases(QUser user)
        {
            var filter = new QIssueFilter
            {
                OrganizationIdentifier = Organization.Identifier,
                TopicUserIdentifier = user.UserIdentifier
            };

            CaseDataSource = ServiceLocator.IssueSearch.GetIssues(filter)
                .Select(x => new CaseDataItem
                {
                    AttachmentCount = x.AttachmentCount,
                    CommentCount = x.CommentCount,
                    IssueClosed = x.IssueClosed,
                    IssueIdentifier = x.IssueIdentifier,
                    IssueOpened = x.IssueOpened,
                    IssueStatusCategoryHtml = x.IssueStatusCategoryHtml,
                    IssueTitle = x.IssueTitle,
                    IssueStatusName = x.IssueStatusName
                }
                ).ToArray();

            CaseGrid.VirtualItemCount = CaseDataSource.Length;
            CaseGrid.DataBind();

            NoCases.Visible = CaseDataSource.Length == 0;

            CaseSectionTitle.Text = $"Cases ({CaseDataSource.Length:n0})";
        }

        private void BindSurveys(QUser user)
        {
            var filter = new QResponseSessionFilter
            {
                OrganizationIdentifier = Organization.Identifier,
                RespondentUserIdentifier = user.UserIdentifier
            };

            var sessions = ServiceLocator.SurveySearch
                .GetResponseSessions(filter)
                .Select(x => new SurveyDataItem(x)).ToList();

            SurveyDataSource = sessions.ToArray();

            SurveyGrid.VirtualItemCount = SurveyDataSource.Length;
            SurveyGrid.DataBind();

            NoSurveys.Visible = SurveyDataSource.Length == 0;

            SurveySectionTitle.Text = $"Forms ({SurveyDataSource.Length:n0})";
        }

        private void BindAttempts(QUser user)
        {
            var filter = new QAttemptFilter
            {
                FormOrganizationIdentifier = Organization.Identifier,
                LearnerUserIdentifier = user.UserIdentifier
            };

            bool isAssesmentAttemptsAdmin = Identity.IsGranted("Admin/Assessments/Attempts");

            AssesmentDataSource = ServiceLocator.AttemptSearch
                .GetAttempts(filter, x => x.AssessorPerson, x => x.LearnerPerson, x => x.Form, x => x.GradingAssessor)
                .Select(x => new AssesmentDataItem
                {
                    AttemptIdentifier = x.AttemptIdentifier,
                    BankIdentifier = x.Form.BankIdentifier,
                    FormIdentifier = x.Form.FormIdentifier,
                    FormName = x.Form.FormName,
                    FormTitle = x.Form.FormTitle,
                    FormAsset = GetFormAsset(x.Form, x.AttemptStarted),
                    FormTime = GetFormatTime(x.AttemptStarted, x.AttemptGraded),
                    GradingAssessor = (x.GradingAssessor != null ? x.GradingAssessor.UserFullName : ""),
                    IsAdmin = isAssesmentAttemptsAdmin
                })
                .ToArray();

            AssesmentGrid.VirtualItemCount = AssesmentDataSource.Length;
            AssesmentGrid.DataBind();

            NoAssesments.Visible = AssesmentDataSource.Length == 0;

            AssesmentSectionTitle.Text = $"Assessment Attempts ({AssesmentDataSource.Length:n0})";
        }

        private void BindAchievements(QUser user)
        {
            var filter = new VCredentialFilter
            {
                OrganizationIdentifier = Organization.Identifier,
                UserIdentifier = user.UserIdentifier
            };

            AchievementDataSource = ServiceLocator.AchievementSearch.GetCredentials(filter).Select(x => new AchievementDataItem
            {
                AchievementTitle = x.AchievementTitle,
                CredentialStatus = x.CredentialStatus,
                CredentialExpired = x.CredentialExpired,
                CredentialGranted = x.CredentialGranted,
                CredentialRevoked = x.CredentialRevoked
            }).ToArray();

            AchievementGrid.VirtualItemCount = AchievementDataSource.Length;
            AchievementGrid.DataBind();

            NoAchievements.Visible = AchievementDataSource.Length == 0;

            AchievementSectionTitle.Text = $"Achievements ({AchievementDataSource.Length:n0})";
        }

        private void BindComments(QPerson person)
        {
            CommentDataSource = PersonCommentSummarySearch
                .SelectForCommentRepeater(person.UserIdentifier, person.OrganizationIdentifier)
                .Select(x => new CommentDataItem
                {
                    Identifier = x.CommentIdentifier,
                    AuthorName = x.AuthorUserName,
                    Posted = x.CommentPosted,
                    Description = Markdown.ToHtml(x.CommentText)
                })
                .ToArray();

            CommentGrid.VirtualItemCount = CommentDataSource.Length;
            CommentGrid.DataBind();

            NoComments.Visible = CommentDataSource.Length == 0;

            CommentSectionTitle.Text = $"Comments ({CommentDataSource.Length:n0})";
        }

        private void BindMessages(QPerson person)
        {
            var returnUrl = new ReturnUrl($"/ui/admin/contacts/people/report?contact={UserIdentifier}#messages");

            MessageDataSource = TEmailSearch
                .GetMyMessages(person.UserIdentifier, Organization.OrganizationIdentifier)
                .Select(x => new MessageDataItem
                {
                    SenderEmail = x.SenderEmail,
                    SenderName = x.SenderName,
                    ContentSubject = x.ContentSubject,
                    DeliveryCompleted = x.DeliveryCompleted,
                    ViewEmailUrl = returnUrl
                        .GetRedirectUrl($"/ui/admin/messages/deliveries/view?mailout={x.MailoutIdentifier}&recipient={HttpUtility.UrlEncode(x.RecipientEmail)}")
                })
                .ToArray();

            MessageGrid.VirtualItemCount = MessageDataSource.Length;
            MessageGrid.DataBind();

            NoMessages.Visible = MessageDataSource.Length == 0;

            MessageSectionTitle.Text = $"Messages ({MessageDataSource.Length:n0})";
        }

        #endregion

        #region Helpers

        protected string IndentHtml(int indent)
            => $"<span style='display:inline-block;width:{20 * indent}px;'></span>";

        protected string GetLocalDate(string name)
        {
            var dataItem = Page.GetDataItem();
            var when = (DateTimeOffset?)DataBinder.Eval(dataItem, name);
            return when.FormatDateOnly(User.TimeZone, nullValue: string.Empty);
        }

        protected string GetLocalDate(object item)
        {
            var when = (DateTimeOffset?)item;
            return when.FormatDateOnly(User.TimeZone, nullValue: string.Empty);
        }

        protected string GetLocalTime(string name)
        {
            var dataItem = Page.GetDataItem();
            var when = (DateTimeOffset?)DataBinder.Eval(dataItem, name);
            return when.Format(User.TimeZone, nullValue: string.Empty);
        }

        protected string GetRedirectUrl(string format, params object[] args)
        {
            var url = string.Format(format, args);

            if (_returnUrl == null)
                _returnUrl = new ReturnUrl("contact");

            return _returnUrl.GetRedirectUrl(url);
        }

        private string GetFormatTime(DateTimeOffset? attemptStarted, DateTimeOffset? attemptGraded)
        {
            var html = new StringBuilder();

            if (attemptStarted.HasValue)
                html.Append("<div>Started " + attemptStarted.Value.Format(User.TimeZone, true) + "</div>");

            if (attemptGraded.HasValue)
                html.Append("<div>Completed " + attemptGraded.Value.Format(User.TimeZone, true) + "</div>");

            return html.ToString();
        }

        private string GetFormAsset(Application.Banks.Read.QBankForm form, DateTimeOffset? attemptStarted)
        {
            if (form == null)
                return string.Empty;

            var assetVersion = form.FormAssetVersion;
            if (form.FormFirstPublished.HasValue && attemptStarted.HasValue && attemptStarted.Value < form.FormFirstPublished.Value)
                assetVersion = 0;

            return $"{form.FormAsset}.{assetVersion}";
        }

        #endregion

        #region IHasParentLinkParameters

        public string GetParentLinkParameters(IWebRoute parent)
        {
            if (parent.Name.EndsWith("/edit"))
                return $"contact={UserIdentifier}";

            return null;
        }

        #endregion
    }
}