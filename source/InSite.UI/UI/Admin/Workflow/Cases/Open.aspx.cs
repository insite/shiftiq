using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Application.Cases.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Admin.Issues.Forms
{
    public partial class Open : AdminBasePage
    {
        #region Properties

        private string Action => Request.QueryString["action"];
        protected Guid? CaseIdentifier => Guid.TryParse(Request.QueryString["case"], out var result) ? result : (Guid?)null;

        #endregion

        #region Initialization and loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DuplicateAssigneeID.AutoPostBack = TopicIdentifier.AutoPostBack = true;
            DuplicateAssigneeID.ValueChanged += DuplicateAssigneeID_ValueChanged;

            TopicIdentifier.AutoPostBack = true;
            TopicIdentifier.ValueChanged += AssigneeID_ValueChanged;

            ExistingCaseIdentifier.AutoPostBack = true;
            ExistingCaseIdentifier.ValueChanged += ExistingCaseIdentifier_ValueChanged;

            IssueType.AutoPostBack = true;
            IssueType.ValueChanged += IssueType_ValueChanged;

            DuplicateEmployerGroup.Filter.OrganizationIdentifier = EmployerIdentifier.Filter.OrganizationIdentifier = Organization.OrganizationIdentifier;
            DuplicateEmployerGroup.Filter.GroupType = EmployerIdentifier.Filter.GroupType = GroupTypes.Employer;

            CreationType.AutoPostBack = true;
            CreationType.ValueChanged += CreationType_ValueChanged;

            JsonFileUploadExtensionValidator.ServerValidate += JsonFileUploadExtensionValidator_ServerValidate;
            JsonFileUploadButton.Click += JsonFileUploadButton_Click;

            SaveButton.Click += SaveButton_Click;

            DuplicateIssueType.Settings.CollectionName = IssueType.Settings.CollectionName = CollectionName.Cases_Classification_Type;
            DuplicateIssueType.Settings.OrganizationIdentifier = IssueType.Settings.OrganizationIdentifier = Organization.OrganizationIdentifier;
            DuplicateIssueType.Settings.UseAlphabeticalOrder = IssueType.Settings.UseAlphabeticalOrder = true;
        }

        private void IssueType_ValueChanged(object sender, ComboBoxValueChangedEventArgs e)
        {
            IssueStatus.IssueType = IssueType.Value;
            IssueStatus.RefreshData();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            DateReported.Value = TimeZones.ConvertFromUtc(DateTimeOffset.UtcNow, User.TimeZone);

            CancelButton.NavigateUrl = $"/ui/admin/workflow/cases/search";

            PageHelper.AutoBindHeader(this);

            CreationType.EnsureDataBound();
            CreationType.SetVisibleOptions(CreationTypeEnum.One, CreationTypeEnum.Duplicate, CreationTypeEnum.Upload);

            OwnerIdentifier.Filter.GroupIdentifier = Organization.AdministratorGroupIdentifier;
            OwnerIdentifier.Filter.IsAdministrator = true;

            SetupAdministratorID();
            SetDefaultValues();

            if (Action == "duplicate")
            {
                CreationType.ValueAsEnum = CreationTypeEnum.Duplicate;

                if (CaseIdentifier.HasValue)
                {
                    CancelButton.NavigateUrl = $"/ui/admin/workflow/cases/outline?case={CaseIdentifier.Value}";
                    ExistingCaseIdentifier.Value = CaseIdentifier.Value;
                    OnExistingCaseIdentifierSelectedIndexChanged();
                    AssigneeSelected(true);
                }
            }

            OnCreationTypeSelected();
        }

        private void SetupAdministratorID()
        {
            DuplicateAdministratorID.Filter.IncludeUserIdentifiers = AdministratorIdentifier.Filter.IncludeUserIdentifiers =
                MembershipSearch.Select(
                    x => (x.Group.OrganizationIdentifier == Organization.OrganizationIdentifier && x.Group.GroupName == "Issue Administrators")
                       || x.User.UserIdentifier == User.UserIdentifier)
                .Select(x => x.UserIdentifier)
                .ToArray();

            // DuplicateAdministratorID.ListFilter.IncludePersons = AdministratorID.ListFilter.IncludePersons;
        }

        private void SetDefaultValues()
        {
            var unspecified = "Unspecified";

            IssueType.Value = unspecified;
            DuplicateIssueType.Value = unspecified;

            AdministratorIdentifier.Value = User.UserIdentifier;
            DuplicateAdministratorID.Value = User.UserIdentifier;
        }

        private void AssigneeSelected(bool isDuplicate = false)
        {
            DuplicateAssigneeHelp.InnerText = TopicHelp.InnerText = "The person to whom the issue is assigned.";

            EmployerIdentifier.Value = null;
            DuplicateEmployerGroup.Value = null;

            Guid? AssigneeIDGuid = null;
            if (isDuplicate)
                AssigneeIDGuid = DuplicateAssigneeID.Value;
            else
                AssigneeIDGuid = TopicIdentifier.Value;

            if (AssigneeIDGuid.HasValue)
            {
                var assignee = PersonCriteria.BindFirst(
                    x => new { x.MembershipStatus, x.User.Email, x.EmployerGroupIdentifier },
                    new PersonFilter
                    {
                        OrganizationIdentifier = Organization.Identifier,
                        UserIdentifier = AssigneeIDGuid.Value
                    });

                if (assignee != null)
                {
                    DuplicateAssigneeHelp.InnerText = TopicHelp.InnerText = $"{assignee.Email} - {assignee.MembershipStatus?.ItemName}";
                    if (assignee.EmployerGroupIdentifier.HasValue)
                    {
                        DuplicateEmployerGroup.Value = assignee.EmployerGroupIdentifier;
                        EmployerIdentifier.Value = assignee.EmployerGroupIdentifier;
                    }
                }
            }
        }

        #endregion

        #region Event handlers

        private void AssigneeID_ValueChanged(object sender, EventArgs e)
        {
            AssigneeSelected();
        }

        private void DuplicateAssigneeID_ValueChanged(object sender, EventArgs e)
        {
            AssigneeSelected(true);
        }

        private void ExistingCaseIdentifier_ValueChanged(object sender, EventArgs e)
        {
            OnExistingCaseIdentifierSelectedIndexChanged();
        }

        private void OnExistingCaseIdentifierSelectedIndexChanged()
        {
            if (ExistingCaseIdentifier.HasValue)
            {
                var issue = ServiceLocator.IssueSearch.GetIssue(ExistingCaseIdentifier.Value.Value);

                DuplicateIssueType.Value = issue?.IssueType;
                DuplicateIssueTitle.Text = issue?.IssueTitle;
                DuplicateIssueDescription.Text = issue?.IssueDescription;
                DuplicateAdministratorID.Value = issue?.AdministratorUserIdentifier;
                DuplicateAssigneeID.Value = issue?.TopicUserIdentifier;
                DuplicateEmployerGroup.Value = issue?.IssueEmployerGroupIdentifier;
                DuplicateDateReported.Value = issue?.IssueReported?.DateTime;
            }
            else
            {
                DuplicateIssueType.Value = null;
                DuplicateIssueTitle.Text = null;
                DuplicateIssueDescription.Text = null;
                DuplicateAdministratorID.Value = null;
                DuplicateAssigneeID.Value = null;
                DuplicateEmployerGroup.Value = null;
                DuplicateDateReported.Value = null;
            }
        }

        private void JsonFileUploadExtensionValidator_ServerValidate(object source, ServerValidateEventArgs e)
        {
            e.IsValid = e.Value == null || e.Value.EndsWith(".json", StringComparison.OrdinalIgnoreCase);
        }

        private void JsonFileUploadButton_Click(object sender, EventArgs e)
        {
            if (JsonFileUpload.PostedFile == null || JsonFileUpload.PostedFile.ContentLength == 0)
            {
                EditorStatus.AddMessage(AlertType.Error, "Uploaded file is empty");
                return;
            }

            string text;
            using (var reader = new StreamReader(JsonFileUpload.FileContent, Encoding.UTF8))
                text = reader.ReadToEnd();

            JsonInput.Text = text;
        }

        private void CreationType_ValueChanged(object sender, EventArgs e)
        {
            OnCreationTypeSelected();
        }

        private void OnCreationTypeSelected()
        {
            var value = CreationType.ValueAsEnum;

            if (value == CreationTypeEnum.One)
                MultiView.SetActiveView(OneView);
            if (value == CreationTypeEnum.Duplicate)
                MultiView.SetActiveView(CopyView);
            if (value == CreationTypeEnum.Upload)
                MultiView.SetActiveView(UploadView);

        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var value = CreationType.ValueAsEnum;

            if (value == CreationTypeEnum.One)
                SaveOne();
            if (value == CreationTypeEnum.Duplicate)
                SaveCopy();
            if (value == CreationTypeEnum.Upload)
                SaveUpload();
        }

        public void SaveOne()
        {
            if (Validate())
            {
                var issue = UniqueIdentifier.Create();
                SendCommands(issue);
                RedirectToOutline(issue);
            }
        }

        public void SaveCopy()
        {
            if (!Page.IsValid)
                return;

            if (!ExistingCaseIdentifier.HasValue)
                return;

            if (Validate(true))
            {
                var issue = UniqueIdentifier.Create();
                SendCommands(issue, true);
                RedirectToOutline(issue);
            }
        }

        public void SaveUpload()
        {
            if (!Page.IsValid)
                return;

            try
            {
                var json = JsonInput.Text;
                var result = CaseHelper.Deserialize(json);

                if (JsonInput.Text.IsEmpty() || result == null)
                {
                    EditorStatus.AddMessage(AlertType.Error, $"Wrong JSON file uploaded");
                    return;
                }

                result.OrganizationIdentifier = Organization.OrganizationIdentifier;

                SendCommands(result, true);
                RedirectToOutline(result.IssueIdentifier);
            }
            catch (JsonReaderException ex)
            {
                EditorStatus.AddMessage(AlertType.Error, $"Your uploaded file has an unexpected format. {ex.Message}");
            }
            catch (Exception ex)
            {
                EditorStatus.AddMessage(AlertType.Error, "Error during import: " + ex.Message);
            }
        }

        private bool Validate(bool isDuplication = false)
        {
            if (isDuplication)
            {
                if (!DuplicateAdministratorID.HasValue)
                {
                    EditorStatus.AddMessage(AlertType.Error, "Please select an Administrator.");
                    return false;
                }

                if (!DuplicateAssigneeID.HasValue)
                {
                    EditorStatus.AddMessage(AlertType.Error, "Please select a Member.");
                    return false;
                }
            }
            else
            {
                if (!AdministratorIdentifier.HasValue)
                {
                    EditorStatus.AddMessage(AlertType.Error, "Please select an Administrator.");
                    return false;
                }

                if (!TopicIdentifier.HasValue)
                {
                    EditorStatus.AddMessage(AlertType.Error, "Please select a Member.");
                    return false;
                }
            }
            return Page.IsValid;
        }

        private void SendCommands(Application.Issues.Read.VIssue result, bool v)
        {
            var number = ServiceLocator.IssueSearch.GetNextIssueNumber(Organization.Identifier);
            OpenIssue open = null;
            AssignUser admin = null;
            AssignUser assignee = null;

            open = new OpenIssue(result.IssueIdentifier,
                Organization.OrganizationIdentifier,
                number, result.IssueTitle,
                result.IssueDescription,
                DateTimeOffset.UtcNow,
                null, result.IssueType,
                result.IssueReported);
            admin = new AssignUser(result.IssueIdentifier, result.AdministratorUserIdentifier.Value, "Administrator");
            assignee = new AssignUser(result.IssueIdentifier, result.TopicUserIdentifier.Value, "Topic");

            ServiceLocator.SendCommand(open);
            ServiceLocator.SendCommand(admin);
            ServiceLocator.SendCommand(assignee);
        }

        private void SendCommands(Guid issue, bool isDuplication = false)
        {
            var number = ServiceLocator.IssueSearch.GetNextIssueNumber(Organization.Identifier);

            if (isDuplication)
            {
                ServiceLocator.SendCommand(new OpenIssue(issue, Organization.OrganizationIdentifier, number, DuplicateIssueTitle.Text, DuplicateIssueDescription.Text, DateTimeOffset.UtcNow, null, DuplicateIssueType.Value, DuplicateDateReported.Value));
                ServiceLocator.SendCommand(new AssignUser(issue, DuplicateAdministratorID.Value.Value, "Administrator"));
                ServiceLocator.SendCommand(new AssignUser(issue, DuplicateAssigneeID.Value.Value, "Topic"));
            }
            else
            {
                ServiceLocator.SendCommand(new OpenIssue(issue, Organization.OrganizationIdentifier, number, IssueTitle.Text, IssueDescription.Text, DateTimeOffset.UtcNow, null, IssueType.Value, DateReported.Value));
                ServiceLocator.SendCommand(new AssignUser(issue, AdministratorIdentifier.Value.Value, "Administrator"));
                ServiceLocator.SendCommand(new AssignUser(issue, TopicIdentifier.Value.Value, "Topic"));

                if (IssueStatus.ValueAsGuid.HasValue)
                    ServiceLocator.SendCommand(new ChangeIssueStatus(issue, IssueStatus.ValueAsGuid.Value, DateTimeOffset.UtcNow));

                if (OwnerIdentifier.Value.HasValue)
                    ServiceLocator.SendCommand(new AssignUser(issue, OwnerIdentifier.Value.Value, "Owner"));
            }

            if (isDuplication)
            {
                if (DuplicateEmployerGroup.HasValue)
                {
                    var assignEmployer = new AssignGroup(issue, DuplicateEmployerGroup.Value.Value, "Employer");
                    ServiceLocator.SendCommand(assignEmployer);
                }
            }
            else
            {
                if (EmployerIdentifier.HasValue)
                {
                    var assignEmployer = new AssignGroup(issue, EmployerIdentifier.Value.Value, "Employer");
                    ServiceLocator.SendCommand(assignEmployer);
                }
            }
        }

        #endregion

        #region Methods (redirect)

        private void RedirectToOutline(Guid issue)
        {
            HttpResponseHelper.Redirect($"/ui/admin/workflow/cases/outline?case={issue}");
        }

        #endregion
    }
}