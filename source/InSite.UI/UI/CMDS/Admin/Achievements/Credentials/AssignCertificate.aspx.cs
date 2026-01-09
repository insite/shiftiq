using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Shift.Common.Timeline.Commands;

using Humanizer;

using InSite.Application.Credentials.Write;
using InSite.Cmds.Infrastructure;
using InSite.Common.Web;
using InSite.Common.Web.Cmds;
using InSite.Common.Web.UI;
using InSite.Custom.CMDS.Common.Controls.Server;
using InSite.Domain.Records;
using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

using AlertType = Shift.Constant.AlertType;
using CmdsPersonFilter = InSite.Persistence.Plugin.CMDS.CmdsPersonFilter;
using IconButton = InSite.Custom.CMDS.Common.Controls.Server.IconButton;
using Path = System.IO.Path;

namespace InSite.Cmds.Actions.BulkTool.Assign
{
    public partial class AssignCertificate : AdminBasePage
    {
        #region Constants

        private const string DateFormat = @"M\/d\/yyyy";
        private const string DateValidationExpression = "^(0?[1-9]|1[012])/(0?[1-9]|[12][0-9]|3[01])/[0-9]{4}$";

        protected const int MaxFileSize = 1; // in MB

        private static readonly string TempFolderPath = System.IO.Path.Combine(ServiceLocator.FilePaths.TempFolderPath, "TssCertificateTemp");

        private const int TempFileLiveTime = 7 * 24 * 60; // in minutes

        #endregion

        #region Classes

        [Serializable]
        public class Upload
        {
            #region Properties

            public Guid? UploadID { get; set; }
            public Guid ContainerID { get; set; }
            public string Description { get; set; }
            public string FilePath { get; set; }
            public string Name { get; set; }
            public Guid PersonID { get; set; }

            #endregion
        }

        [Serializable]
        private class UploadCollection : IEnumerable<Upload>
        {
            #region Properties

            public int Count => _list.Count;

            #endregion

            #region Fields

            private readonly List<Upload> _list = new List<Upload>();

            #endregion

            #region Public methods

            public void Add(Upload item)
            {
                _list.Add(item);
            }

            public void Remove(string filePath)
            {
                var item = _list.Find(a => StringHelper.Equals(a.FilePath, filePath));

                Remove(item);
            }

            public void Remove(Guid uploadId)
            {
                var item = _list.Find(a => a.UploadID.HasValue && a.UploadID.Value == uploadId);

                Remove(item);
            }

            public void Remove(Upload item)
            {
                if (item != null && _list.Remove(item) && !item.UploadID.HasValue)
                    DeleteTempFile(item.FilePath);
            }

            public void Clear()
            {
                while (_list.Count > 0)
                    Remove(_list[0]);
            }

            #endregion

            #region IEnumerable

            public IEnumerator<Upload> GetEnumerator() => _list.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            #endregion
        }

        [Serializable]
        private class PersonUploadCollection
        {
            #region Fields

            private readonly Dictionary<Guid, UploadCollection> _dict;

            #endregion

            #region Construction

            public PersonUploadCollection()
            {
                _dict = new Dictionary<Guid, UploadCollection>();
            }

            #endregion

            #region Public methods

            public UploadCollection Get(Guid personId)
            {
                return _dict.TryGetValue(personId, out UploadCollection result) ? result : null;
            }

            public UploadCollection GetOrCreate(Guid personId)
            {
                if (!_dict.TryGetValue(personId, out UploadCollection result))
                    _dict.Add(personId, result = new UploadCollection());

                return result;
            }

            public void Remove(Guid personId, string filePath)
            {
                if (_dict.TryGetValue(personId, out var collection))
                {
                    collection.Remove(filePath);

                    if (collection.Count == 0)
                        _dict.Remove(personId);
                }
            }

            public void Remove(Guid personId, Guid uploadId)
            {
                if (_dict.TryGetValue(personId, out var collection))
                {
                    collection.Remove(uploadId);

                    if (collection.Count == 0)
                        _dict.Remove(personId);
                }
            }

            public void Clear()
            {
                foreach (Guid personId in _dict.Keys)
                    _dict[personId].Clear();

                _dict.Clear();
            }

            public List<UploadCollection> GetAllUploads()
            {
                return _dict.Values.ToList();
            }

            #endregion
        }

        private class CredentialInfo
        {
            public List<Command> Commands { get; set; }
            public Guid CredentialIdentifier { get; set; }
        }

        #endregion

        #region Fields

        private PersonFinderSecurityInfoWrapper _finderSecurityInfo;

        #endregion

        #region Security

        public override void ApplyAccessControl()
        {
            if (!Identity.IsGranted("cmds/users/assign-certificates"))
                CreateAccessDeniedException();

            FinderSecurityInfo.LoadPermissions();
        }

        #endregion

        #region Properties

        private PersonUploadCollection Uploads => (PersonUploadCollection)(ViewState[nameof(Uploads)]
            ?? (ViewState[nameof(Uploads)] = new PersonUploadCollection()));

        private PersonFinderSecurityInfoWrapper FinderSecurityInfo => _finderSecurityInfo
            ?? (_finderSecurityInfo = new PersonFinderSecurityInfoWrapper(ViewState));

        private string DownloadUrl => Request.QueryString["download"];

        #endregion

        #region Initialization and loading

        protected override void OnInit(EventArgs e)
        {
            SendFile();

            base.OnInit(e);

            SubType.AutoPostBack = true;
            SubType.ValueChanged += SubType_ValueChanged;

            Category.AutoPostBack = true;
            Category.ValueChanged += Category_ValueChanged;

            AchievementIdentifier.AutoPostBack = true;
            AchievementIdentifier.ValueChanged += AchievementIdentifier_ValueChanged;

            Department.AutoPostBack = true;
            Department.ValueChanged += Department_ValueChanged;

            RoleTypeSelector.AutoPostBack = true;
            RoleTypeSelector.SelectedIndexChanged += RoleTypeSelector_SelectedIndexChanged;

            EmployeeRequired.ServerValidate += EmployeeRequired_ServerValidate;

            Employees.ItemCreated += Employees_ItemCreated;
            Employees.ItemDataBound += Employees_ItemDataBound;

            UnassignCertificates.Click += UnassignCertificates_Click;
            UnassignCertificates.OnClientClick = "return confirm('Are you sure you want to unassign all of the time-sensitive certificates from all of the people you have selected?');";

            AttachFileButton.Click += AttachFileButton_Click;
            SaveDownloadButton.Click += SaveDownloadButton_Click;
            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                PageHelper.AutoBindHeader(this);

                SubType.LoadItems(new[] { AchievementTypes.TimeSensitiveSafetyCertificate, AchievementTypes.TrainingGuide });

                Category.ListFilter.OrganizationIdentifier = OrganizationSearch.Select(CurrentIdentityFactory.ActiveOrganizationIdentifier).OrganizationIdentifier;
                Category.RefreshData();

                LoadAchievements();
                Open();
            }
            else
                SetEnabledStateForValidators();
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            trValidFor.Style["display"] = IsTimeSensitive.Checked ? "" : "none";

            Page.ClientScript.RegisterClientScriptBlock(GetType(), "Constants", "var _dateValidationExpression='" + DateValidationExpression + "'", true);
        }

        #endregion

        #region Event handlers

        private void SubType_ValueChanged(object sender, EventArgs e)
        {
            LoadAchievements();
            LoadEmployees();
            SetEnableSignOffAvailability();
        }

        private void Category_ValueChanged(object sender, EventArgs e)
        {
            LoadAchievements();
            LoadEmployees();
        }

        private void AchievementIdentifier_ValueChanged(object sender, EventArgs e)
        {
            LoadEmployees();
        }

        private void Department_ValueChanged(object sender, EventArgs e)
        {
            LoadEmployees();
        }

        private void RoleTypeSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadEmployees();
        }

        private void EmployeeRequired_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = false;

            foreach (RepeaterItem item in Employees.Items)
            {
                var assignedCtrl = (ICheckBoxControl)item.FindControl("Assigned");

                if (assignedCtrl.Checked)
                {
                    args.IsValid = true;

                    break;
                }
            }
        }

        private void Employees_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.AlternatingItem && e.Item.ItemType != ListItemType.Item)
                return;

            var repeater = (Repeater)e.Item.FindControl("AttachmentRepeater");
            repeater.ItemCommand += AttachmentRepeater_ItemCommand;

            var dateCompletedValidator = (Common.Web.UI.CustomValidator)e.Item.FindControl("DateCompletedValidator");
            dateCompletedValidator.ServerValidate += DateValidator_ServerValidate;
        }

        private void DateValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            var regex = new Regex(DateValidationExpression);

            args.IsValid = regex.IsMatch(args.Value)
                           && DateTime.TryParseExact(args.Value, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date);
        }

        private void Employees_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.AlternatingItem && e.Item.ItemType != ListItemType.Item)
                return;

            var row = (DataRowView)e.Item.DataItem;
            var personID = (Guid)row["UserIdentifier"];

            var statusCtrl = (EmployeeAchievementStatusSelector2)e.Item.FindControl("Status");

            if (AchievementIdentifier.HasValue)
            {
                var completedCtrl = (ICheckBoxControl)e.Item.FindControl("Assigned");
                completedCtrl.Checked = (bool)row["IsAssigned"];

                var documentNumberCtrl = (ITextControl)e.Item.FindControl("DocumentNumber");
                documentNumberCtrl.Text = row["ResourceNumber"] as string;

                statusCtrl.Value = row["ResourceValidationStatus"] as string;

                var statusRequiredCtrl = (Common.Web.UI.RequiredValidator)e.Item.FindControl("StatusRequired");
                statusRequiredCtrl.Enabled = completedCtrl.Checked;

                var dateCompletedCtrl = (ITextControl)e.Item.FindControl("DateCompleted");
                dateCompletedCtrl.Text = GetTextFromDate(row["ResourceDateCompleted"] as DateTime?);

                var expirationDateCtrl = (ITextControl)e.Item.FindControl("ExpirationDate");
                expirationDateCtrl.Text = GetTextFromDate(row["ResourceExpirationDate"] as DateTime?) ?? "None";
            }

            BindUploads(personID, e.Item);
        }

        private void AttachmentRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            var control = (HiddenField)e.Item.FindControl("UserIdentifier");
            var userId = Guid.Parse(control.Value);

            if (StringHelper.Equals(e.CommandName, "DeleteTempUpload"))
            {
                var filePath = (string)e.CommandArgument;

                Uploads.Remove(userId, filePath);
            }
            else if (StringHelper.Equals(e.CommandName, "DeleteUpload"))
            {
                var uploadId = Guid.Parse(e.CommandArgument.ToString());

                CmdsUploadProvider.Current.Delete(uploadId);

                Uploads.Remove(userId, uploadId);
            }

            BindUploadsByPersonID(userId);
        }

        private void UnassignCertificates_Click(object sender, EventArgs e)
        {
            var deleteList = new List<DeleteCredential>();

            foreach (RepeaterItem item in Employees.Items)
            {
                var assigned = (ICheckBoxControl)item.FindControl("Assigned");
                if (!assigned.Checked)
                    continue;

                var userIdentifier = (ITextControl)item.FindControl("UserIdentifier");
                var user = Guid.Parse(userIdentifier.Text);
                var list = VCmdsCredentialSearch.Select(x => x.AchievementIdentifier == AchievementIdentifier.Value && x.UserIdentifier == user);

                foreach (var credential in list)
                    deleteList.Add(new DeleteCredential(credential.CredentialIdentifier));
            }

            foreach (var command in deleteList)
                ServiceLocator.SendCommand(command);

            LoadEmployees();
        }

        private void SaveDownloadButton_Click(object sender, EventArgs e)
        {
            if (!Guid.TryParse(UploadedPersonID.Value, out var personID) || string.IsNullOrEmpty(UploadedFilePath.Value))
                return;

            Uploads.GetOrCreate(personID).Add(new Upload
            {
                PersonID = personID,
                FilePath = UploadedFilePath.Value,
                Name = UploadedName.Value,
                Description = UploadedDescription.Value
            });

            BindUploadsByPersonID(personID);
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!IsValid)
                return;

            Save();
            Open();
            SetStatus(EditorStatus, StatusType.Saved);
        }

        private void AttachFileButton_Click(object sender, EventArgs e)
        {
            var fileBytes = File.FileBytes;
            if (fileBytes.Length > 0 && fileBytes.Length <= MaxFileSize * 1000 * 1000)
            {
                var fileName = CmdsUploadProvider.AdjustFileName(File.FileName);
                var filePath = SaveTempFile(fileName, fileBytes);

                var dataObj = new
                {
                    filePath = filePath,
                    name = TitleInput.Text,
                    description = Description.Text
                };
                var dataJson = JsonHelper.SerializeJsObject(dataObj);

                Page.ClientScript.RegisterStartupScript(
                    GetType(),
                    "Close",
                    $"closeFileUploader({HttpUtility.JavaScriptStringEncode(dataJson, true)});",
                    true);
            }
        }

        private static string SaveTempFile(string name, byte[] file)
        {
            DeleteOldTempFiles();

            var folderName = UniqueIdentifier.Create().ToString().Replace("-", "");

            var folderPath = Path.Combine(TempFolderPath, folderName);
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var filePath = Path.Combine(folderPath, name);
            System.IO.File.WriteAllBytes(filePath, file);

            return Path.Combine(folderName, name);
        }

        private static void DeleteOldTempFiles()
        {
            if (!Directory.Exists(TempFolderPath))
                return;

            foreach (var dirPath in Directory.GetDirectories(TempFolderPath))
            {
                var dirInfo = new DirectoryInfo(dirPath);
                if (dirInfo.CreationTimeUtc.AddMinutes(TempFileLiveTime) < DateTime.UtcNow)
                    dirInfo.Delete(true);
            }
        }

        #endregion

        #region Load and save

        private void Open()
        {
            IsRequired.Checked = true;
            IsTimeSensitive.Checked = false;

            Department.Filter.OrganizationIdentifier = CurrentIdentityFactory.ActiveOrganizationIdentifier;

            if (!FinderSecurityInfo.CanSeeAllDepartments && !Identity.HasAccessToAllCompanies)
                Department.Filter.UserIdentifier = User.UserIdentifier;

            Department.Value = null;

            LoadEmployees();

            SubType.Value = AchievementTypes.TimeSensitiveSafetyCertificate;

            SetEnableSignOffAvailability();
        }

        private void Save()
        {
            if (!CheckUploads())
            {
                EditorStatus.AddMessage(AlertType.Error, "Session is expired. You need to re-enter data for bulk update.");

                LoadEmployees();

                return;
            }

            if (IsTimeSensitive.Checked && !ValidForCount.ValueAsInt.HasValue)
            {
                EditorStatus.AddMessage(AlertType.Error, "You have checked the Time-Sensitive box. Please input a <strong>Valid For</strong> interval.");
                return;
            }

            try
            {
                foreach (RepeaterItem item in Employees.Items)
                    SaveItem(item);
            }
            catch (IOException ioex)
            {
                if (ioex is FileNotFoundException || ioex is DirectoryNotFoundException)
                {
                    EditorStatus.AddMessage(AlertType.Error, "Error occurred. You need to re-enter data for bulk update.");
                    LoadEmployees();
                    return;
                }

                throw;
            }
            finally
            {
                Uploads.Clear();
            }
        }

        #endregion

        #region Setting and getting input values

        private CredentialInfo GetInputValues(RepeaterItem item, VCmdsCredential info)
        {
            var commands = new List<Command>();

            var userKeyCtrl = (ITextControl)item.FindControl("UserIdentifier");
            var documentNumberCtrl = (ITextControl)item.FindControl("DocumentNumber");
            var dateCompletedCtrl = (ITextControl)item.FindControl("DateCompleted");

            var completed = GetDateFromText(dateCompletedCtrl.Text);
            var userIdentifier = Guid.Parse(userKeyCtrl.Text);

            Expiration expiration = null;

            if (IsTimeSensitive.Checked && ValidForCount.ValueAsInt.HasValue)
                expiration = new Expiration(ExpirationType.Relative, null, ValidForCount.ValueAsInt.Value, "Month");

            var person = ServiceLocator.ContactSearch.GetPerson(userIdentifier, Organization.OrganizationIdentifier);

            Guid credentialIdentifier;

            if (info == null)
            {
                var necessity = IsRequired.Checked ? "Mandatory" : "Optional";
                var priority = "Planned";
                var authorityType = EmployeeAchievementHelper.TypeAllowsSignOff(SubType.Value) ? "Self" : null;

                var achievement = AchievementIdentifier.Value.Value;
                credentialIdentifier = ServiceLocator.AchievementSearch.GetCredentialIdentifier(null, achievement, userIdentifier);

                commands.Add(new CreateCredential(credentialIdentifier, Organization.OrganizationIdentifier, achievement, userIdentifier, DateTimeOffset.UtcNow));
                commands.Add(new ChangeCredentialExpiration(credentialIdentifier, expiration));
                commands.Add(new TagCredential(credentialIdentifier, necessity, priority));

                var documentNumber = StringHelper.StripHtml(documentNumberCtrl.Text, string.Empty, false);

                commands.Add(new ChangeCredentialAuthority(credentialIdentifier, null, null, authorityType, null, documentNumber, null));

                if (completed.HasValue)
                {
                    commands.Add(new GrantCredential(
                        credentialIdentifier,
                        completed.Value,
                        "Bulk assigned",
                        null,
                        person?.EmployerGroupIdentifier,
                        person?.EmployerGroupStatus));
                }
            }
            else
            {
                credentialIdentifier = info.CredentialIdentifier;

                commands.Add(new ChangeCredentialExpiration(credentialIdentifier, expiration));
                commands.Add(new TagCredential(credentialIdentifier, info.CredentialNecessity, info.CredentialPriority));
                commands.Add(new ChangeCredentialAuthority(credentialIdentifier, info.AuthorityIdentifier, info.AuthorityName, info.AuthorityType, info.AuthorityLocation, documentNumberCtrl.Text, info.CredentialHours));

                if (info.CredentialGranted != completed)
                {
                    if (completed.HasValue)
                    {
                        commands.Add(new GrantCredential(
                            credentialIdentifier,
                            completed.Value,
                            "Bulk assigned",
                            null,
                            person?.EmployerGroupIdentifier,
                            person?.EmployerGroupStatus));
                    }
                    else
                        commands.Add(new DeleteCredential(credentialIdentifier));
                }
            }

            return new CredentialInfo { Commands = commands, CredentialIdentifier = credentialIdentifier };
        }

        private static DateTime? GetDateFromText(string text)
        {
            return string.IsNullOrWhiteSpace(text) ? (DateTime?)null : DateTime.ParseExact(text, DateFormat, CultureInfo.InvariantCulture);
        }

        private static string GetTextFromDate(DateTime? date)
        {
            return date == null ? null : date.Value.ToString(DateFormat, CultureInfo.InvariantCulture);
        }

        #endregion

        #region Load employees

        private DataTable CreateDataSourceForEmployees(CmdsPersonFilter filter)
        {
            DataTable table = AchievementIdentifier.Value.HasValue
                ? ContactRepository3.SelectPersonsWithCertificationInfo(filter, AchievementIdentifier.Value.Value, Organization.Identifier)
                : ContactRepository3.SelectPersons(filter, Organization.Identifier);

            return table;
        }

        private DataTable CreateDataSourceForUploads(CmdsPersonFilter filter)
        {
            return ContactRepository3.SelectAchievementUploadsForPersons(filter, AchievementIdentifier.Value.Value);
        }

        private CmdsPersonFilter CreateDataFilter()
        {
            var filter = new CmdsPersonFilter
            {
                OrganizationIdentifier = CurrentIdentityFactory.ActiveOrganizationIdentifier,
                DepartmentIdentifier = Department.Value,
                KeyeraRoles = new[] { CmdsRole.Workers }
            };

            if (!FinderSecurityInfo.CanSeeAllCompanyPeople && !Identity.HasAccessToAllCompanies)
                filter.ParentUserIdentifier = User.UserIdentifier;

            if (!FinderSecurityInfo.CanSeeAllCompanyPeople && !Identity.HasAccessToAllCompanies && !Department.HasValue)
                filter.DepartmentsForParentId = User.UserIdentifier;

            filter.RoleType = GetRoleTypes();

            return filter;
        }

        private void LoadEmployees()
        {
            EmployeesPanel.Visible = false;
            SaveButton.Visible = false;
            UnassignCertificates.Visible = false;

            var filter = CreateDataFilter();
            if (filter.DepartmentIdentifier == null || AchievementIdentifier.Value == null)
                return;

            var table = CreateDataSourceForEmployees(filter);
            var count = table.Rows.Count;

            LoadEmployeeUploads(filter);

            Employees.DataSource = table;
            Employees.DataBind();

            Employees.Visible = count > 0;
            UserHeading.InnerText = "User".ToQuantity(count);
            EmployeesPanel.Visible = true;
            SaveButton.Visible = true;
            UnassignCertificates.Visible = true;
        }

        private void LoadEmployeeUploads(CmdsPersonFilter filter)
        {
            Uploads.Clear();

            if (AchievementIdentifier.Value == null)
                return;

            var table = CreateDataSourceForUploads(filter);

            foreach (DataRow row in table.Rows)
            {
                var upload = new Upload
                {
                    UploadID = (Guid)row["UploadIdentifier"],
                    ContainerID = (Guid)row["ContainerIdentifier"],
                    Name = (string)row["Name"],
                    Description = row["Description"] as string,
                    PersonID = (Guid)row["UserIdentifier"],
                };

                Uploads.GetOrCreate(upload.PersonID).Add(upload);
            }
        }

        private void BindUploadsByPersonID(Guid personID)
        {
            foreach (RepeaterItem item in Employees.Items)
            {
                var userIdentifierCtrl = (ITextControl)item.FindControl("UserIdentifier");
                var curPersonID = Guid.Parse(userIdentifierCtrl.Text);

                if (personID == curPersonID)
                {
                    BindUploads(personID, item);
                    break;
                }
            }
        }

        private void BindUploads(Guid personId, RepeaterItem item)
        {
            var personUploads = Uploads.Get(personId);

            var attachUploadButton = (IconButton)item.FindControl("AttachUploadButton");
            attachUploadButton.OnClientClick = string.Format("showFileUploader('{0}'); return false;", personId);

            var repeater = (Repeater)item.FindControl("AttachmentRepeater");
            repeater.Visible = personUploads != null && personUploads.Count > 0;
            repeater.DataSource = personUploads;
            repeater.DataBind();
        }

        #endregion

        #region Save

        private void SaveItem(RepeaterItem item)
        {
            var assignedCtrl = (ICheckBoxControl)item.FindControl("Assigned");

            if (assignedCtrl.Checked)
                UpdateItem(item);
            else
                DeleteItem(item);
        }

        private void UpdateItem(RepeaterItem item)
        {
            var userKeyCtrl = (ITextControl)item.FindControl("UserIdentifier");
            var userIdentifier = Guid.Parse(userKeyCtrl.Text);

            var info = VCmdsCredentialSearch.SelectFirst(x => x.UserIdentifier == userIdentifier && x.AchievementIdentifier == AchievementIdentifier.Value);
            var result = GetInputValues(item, info);

            foreach (var command in result.Commands)
                ServiceLocator.SendCommand(command);

            SaveUploads(userIdentifier, result.CredentialIdentifier);
        }

        private void DeleteItem(RepeaterItem item)
        {
            var userIdentifierCtrl = (ITextControl)item.FindControl("UserIdentifier");
            var userIdentifier = Guid.Parse(userIdentifierCtrl.Text);

            EmployeeAchievementHelper.DeleteUserAchievement(userIdentifier, AchievementIdentifier.Value.Value);
        }

        private void SaveUploads(Guid userIdentifier, Guid credentialIdentifier)
        {
            var personUploads = Uploads.Get(userIdentifier);
            if (personUploads == null)
                return;

            foreach (Upload upload in personUploads)
            {
                if (upload.UploadID.HasValue)
                    continue;

                var physicalFilePath = GetTempFilePath(upload.FilePath);
                var fileName = Path.GetFileName(physicalFilePath);

                using (var stream = System.IO.File.Open(physicalFilePath, FileMode.Open, FileAccess.Read))
                {
                    CmdsUploadProvider.Current.Update(UploadContainerType.Workflow, credentialIdentifier, fileName, model =>
                    {
                        model.Title = upload.Name;
                        model.Description = upload.Description;

                        model.Write(stream);
                    });
                }

                DeleteTempFile(upload.FilePath);
            }
        }

        private bool CheckUploads()
        {
            var list = Uploads.GetAllUploads();

            foreach (var collection in list)
            {
                foreach (var upload in collection)
                {
                    if (upload.UploadID.HasValue)
                        continue;

                    var physPath = GetTempFilePath(upload.FilePath);
                    if (!System.IO.File.Exists(physPath))
                        return false;
                }
            }

            return true;
        }

        #endregion

        #region Helper methods

        private void LoadAchievements()
        {
            AchievementIdentifier.Filter.AchievementType = SubType.Value;
            AchievementIdentifier.Filter.OrganizationIdentifier = CurrentIdentityFactory.ActiveOrganizationIdentifier;
            AchievementIdentifier.Filter.CategoryIdentifier = Category.ValueAsGuid;
            AchievementIdentifier.Value = null;
        }

        private void SetEnableSignOffAvailability()
        {
            EnableSignOff.Visible = EmployeeAchievementHelper.TypeAllowsSignOff(SubType.Value);
            EnableSignOff.Checked = false;
        }

        private string[] GetRoleTypes()
        {
            var list = new List<string>();

            foreach (System.Web.UI.WebControls.ListItem item in RoleTypeSelector.Items)
            {
                if (item.Selected)
                    list.Add(item.Value);
            }

            if (list.Count == 0)
                list.Add("NA");

            return list.ToArray();
        }

        private void SetEnabledStateForValidators()
        {
            foreach (RepeaterItem item in Employees.Items)
            {
                var assignedCtrl = (ICheckBoxControl)item.FindControl("Assigned");
                var statusRequiredCtrl = (Common.Web.UI.RequiredValidator)item.FindControl("StatusRequired");

                statusRequiredCtrl.Enabled = assignedCtrl.Checked;
            }
        }

        protected static string GetUploadUrl(Upload info)
        {
            return info.UploadID.HasValue
                ? CmdsUploadProvider.GetFileRelativeUrl(info.ContainerID, info.Name)
                : GetTempFileUrl(info.FilePath);
        }

        private static string GetTempFileUrl(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));

            return "/ui/cmds/admin/achievements/credentials/assign-certificate?download=" + HttpUtility.UrlEncode(path);
        }

        private void SendFile()
        {
            if (string.IsNullOrEmpty(DownloadUrl))
                return;

            var physicalPath = GetTempFilePath(DownloadUrl);
            if (!System.IO.File.Exists(physicalPath))
                HttpResponseHelper.SendHttp404();

            Response.SendFile(null, physicalPath, "application/octet-stream");
        }

        private static string GetTempFilePath(string path)
            => Path.Combine(TempFolderPath, path);

        private static void DeleteTempFile(string filePath)
        {
            var physicalPath = GetTempFilePath(filePath);
            var dirName = Path.GetDirectoryName(physicalPath);

            if (Directory.Exists(dirName))
                Directory.Delete(dirName, true);
        }

        #endregion
    }
}
