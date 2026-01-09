using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

using InSite.Application.Standards.Read;
using InSite.Application.Standards.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Standards;
using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;
using InSite.UI.Layout.Admin;
using InSite.Web.Data;

using Shift.Common;
using Shift.Constant;
using Shift.Contract;
using Shift.Sdk.UI;

namespace InSite.Custom.CMDS.Admin.Standards.Profiles
{
    public partial class Edit : AdminBasePage, ICmdsUserControl, IHasParentLinkParameters, IOverrideWebRouteParent
    {
        #region Constants

        private const string SearchUrl = "/ui/cmds/admin/standards/profiles/search";
        private const string CreateUrl = "/ui/cmds/admin/standards/profiles/create";
        private const string SelfUrl = "/ui/cmds/admin/standards/profiles/edit";

        private const string OrganizationEditUrl = "/ui/cmds/admin/organizations/edit";
        private const string DepartmentEditUrl = "/ui/cmds/admin/departments/edit";

        #endregion

        #region Properties

        private Guid StandardIdentifier => Guid.TryParse(Request.QueryString["id"], out var value) ? value : Guid.Empty;

        private Guid? ParentOrganizationIdentifier => Guid.TryParse(Request.QueryString["organization"], out var value) ? value : (Guid?)null;

        private Guid? ParentDepartmentIdentifier => Guid.TryParse(Request.QueryString["department"], out var value) ? value : (Guid?)null;

        private bool IsLockAllowed
        {
            get => (bool)ViewState[nameof(IsLockAllowed)];
            set => ViewState[nameof(IsLockAllowed)] = value;
        }

        private bool? _isLocked;
        private bool IsLocked => _isLocked
            ?? (_isLocked = StandardSearch.Select(StandardIdentifier)?.IsLocked == true).Value;

        #endregion

        #region Initialization & Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ProfileOwnership.AllowToAddProfilesFromCompany = true;
            ProfileOwnership.Enabled = false;

            UniqueNumber.ServerValidate += UniqueNumber_ServerValidate;

            CompetencyList.Alert += (s, a) => ScreenStatus.AddMessage(a);
            CompetencyList.Refreshed += (s, a) => CompetencyTab.SetTitle("Competencies", a.Value);

            LockButton.Click += (s, a) => LockUnlock(true);
            UnlockButton.Click += (s, a) => LockUnlock(false);
            CopyButton.Click += CopyButton_Click;
            MoveButton.Click += MoveButton_Click;
            ConfirmMoveButton.Click += ConfirmMoveButton_Click;
            SaveButton.Click += SaveButton_Click;
            DeleteButton.Click += DeleteButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            var status = Request.QueryString["status"];

            if (status == "copied")
                ScreenStatus.AddMessage(AlertType.Information, $"A copy of this competency has been made. You are now editing the copy.");
            else if (status == "saved")
                SetStatus(ScreenStatus, StatusType.Saved);

            Open();
            SetupCommandButtons();

            EditCertificateButton.NavigateUrl = $"/ui/cmds/admin/standards/profiles/edit-certificate?id={StandardIdentifier}";
            ViewDifferencesButton.NavigateUrl = $"/ui/cmds/admin/standards/profiles/compare?id={StandardIdentifier}";
            PrintReportButton.NavigateUrl = $"/ui/cmds/admin/reports/competencies-per-profile?id={StandardIdentifier}";
            CancelButton.NavigateUrl = ParentOrganizationIdentifier.HasValue
                ? OrganizationEditUrl + $"?id={ParentOrganizationIdentifier}"
                : ParentDepartmentIdentifier.HasValue
                    ? DepartmentEditUrl + $"?id={ParentDepartmentIdentifier}"
                    : SearchUrl;
        }

        private void SetupCommandButtons()
        {
            var isLocked = IsLocked;
            var isLockAllowed = IsLockAllowed && Access.Write;

            LockButton.Visible = !isLocked && isLockAllowed;
            UnlockButton.Visible = isLocked && isLockAllowed;
            LockButtonSpacer.Visible = isLockAllowed;

            CopyButton.Visible = !isLocked && Access.Write;
            CopyButtonSpacer.Visible = !isLocked && Access.Write;

            EditCertificateButton.Visible = !isLocked;

            SaveButton.Visible = !isLocked && Access.Write;
            DeleteButton.Visible = !isLocked && Access.Delete;
        }

        public override void ApplyAccessControlForCmds()
        {
            base.ApplyAccessControlForCmds();

            IsLockAllowed = Identity.IsInRole(CmdsRole.Programmers) || Identity.IsInRole(CmdsRole.SystemAdministrators);

            SetupCommandButtons();
        }

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
        {
            if (OrganizationEditUrl.EndsWith(parent.Name))
                return "id=" + (ParentOrganizationIdentifier
                    ?? DepartmentSearch.Select(ParentDepartmentIdentifier.Value)?.OrganizationIdentifier);

            if (DepartmentEditUrl.EndsWith(parent.Name))
                return "id=" + ParentDepartmentIdentifier;

            return null;
        }

        IWebRoute IOverrideWebRouteParent.GetParent() => GetParent();

        protected override string GetReturnUrl()
        {
            if (ParentOrganizationIdentifier.HasValue)
                return OrganizationEditUrl;

            if (ParentDepartmentIdentifier.HasValue)
                return DepartmentEditUrl;

            return null;
        }

        #endregion

        #region Event handlers

        private void UniqueNumber_ServerValidate(object source, ServerValidateEventArgs args)
        {
            var list = StandardSearch.Select(x => x.StandardType == "Profile" && x.Code == NumberInput.Text);

            args.IsValid = list.Count == 0 || list[0].StandardIdentifier == StandardIdentifier;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            if (!Save())
                return;

            Open();

            SetStatus(ScreenStatus, StatusType.Saved);
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (Delete())
                HttpResponseHelper.Redirect(SearchUrl);
        }

        private void CopyButton_Click(object sender, EventArgs e)
        {
            var id = Copy();

            if (id == Guid.Empty)
                return;

            var editUrl = SelfUrl + $"?id={id}&status=copied";

            HttpResponseHelper.Redirect(editUrl);
        }

        private void MoveButton_Click(object sender, EventArgs e)
        {
            var help = "Select the organization you want to become the new owner of this profile, then click <strong>Confirm</strong>.";

            ProfileOwnershipHeading.InnerText = "Move Profile";
            ProfileOwnership.SwitchToMoveMode(help);
            ProfileOwnershipConfirm.Visible = true;
        }

        private void ConfirmMoveButton_Click(object sender, EventArgs e)
        {
            var id = Copy();

            if (Delete())
                HttpResponseHelper.Redirect($"{SelfUrl}?id={id}");
        }

        #endregion

        #region Load & Save

        private void Open()
        {
            var info = StandardSearch.Select(StandardIdentifier, x => x.Organization);
            if (info == null || !string.Equals(info.StandardType, "Profile", StringComparison.OrdinalIgnoreCase))
                HttpResponseHelper.Redirect(SearchUrl);

            PageHelper.AutoBindHeader(
                this,
                new BreadcrumbItem("Add New Profile", CreateUrl, null, null),
                "#" + info.Code);

            if (IsLocked)
                ScreenStatus.AddMessage(AlertType.Information, "Changes to this profile cannot be made because it is currently locked.");

            SetInputValues(info);

            var competencyCount = CompetencyList.LoadData(info);
            CompetencyTab.SetTitle("Competencies", competencyCount);

            LoadPersons();
        }

        private void LoadPersons()
        {
            PersonGrid.SetVisibleColumns(new[] { "Name", "Organization", "EmailWork" });
            PersonGrid.LoadData(new CmdsPersonFilter { ProfileStandardIdentifier = StandardIdentifier });

            NoPersonPanel.Visible = !PersonGrid.HasRows;
            PersonGrid.Visible = PersonGrid.HasRows;

            PeopleTab.SetTitle("People", PersonGrid.RowCount);
        }

        private bool Save()
        {
            StandardStore.Update(StandardIdentifier, asset => GetInputValues(asset));

            return true;
        }

        private Guid Copy()
        {
            var profile = StandardFactory.Create("Profile");

            profile.StandardIdentifier = UniqueIdentifier.Create();

            GetInputValues(profile);

            profile.OrganizationIdentifier = ProfileOwnership.GetSelectedOrganizationId();

            profile.ContentTitle = $"{profile.ContentTitle} - Copy";

            if (profile.OrganizationIdentifier == OrganizationIdentifiers.CMDS)
                profile.ParentStandardIdentifier = StandardIdentifier;

            profile.Code = ProfileHelper.InitNumber(profile, User.Email);

            StandardStore.Insert(profile);

            StandardContainmentStore.CopyByFrom(StandardIdentifier, profile.StandardIdentifier);

            return profile.StandardIdentifier;
        }

        private bool Delete()
        {
            if (!CheckDeleteAllowed())
                return false;

            StandardStore.Delete(StandardIdentifier);

            return true;
        }

        private void LockUnlock(bool isLock)
        {
            ServiceLocator.SendCommand(new ModifyStandardFieldBool(StandardIdentifier, StandardField.IsLocked, isLock));

            _isLocked = isLock;

            Open();
            SetupCommandButtons();
        }

        #endregion

        #region Setting and getting input values

        private void SetInputValues(Standard info)
        {
            var isLocked = info.IsLocked;

            NumberInputField.Visible = !isLocked;
            NumberOutputField.Visible = isLocked;

            TitleInputField.Visible = !isLocked;
            TitleOutputField.Visible = isLocked;

            CertificationHoursPercentCoreInputField.Visible = !isLocked;
            CertificationHoursPercentCoreOutputField.Visible = isLocked;

            CertificationHoursPercentNonCoreInputField.Visible = !isLocked;
            CertificationHoursPercentNonCoreOutputField.Visible = isLocked;

            DescriptionInputField.Visible = !isLocked;
            DescriptionOutputField.Visible = isLocked;

            if (!isLocked)
            {
                NumberInput.Text = info.Code;

                CertificationHoursPercentCoreInput.ValueAsDecimal = info.CertificationHoursPercentCore;
                CertificationHoursPercentNonCoreInput.ValueAsDecimal = info.CertificationHoursPercentNonCore;

                TitleInput.Text = info.ContentTitle;
                DescriptionInput.Text = info.ContentDescription;
            }
            else
            {
                NumberOutput.Text = info.Code;

                CertificationHoursPercentCoreOutput.Text = info.CertificationHoursPercentCore?.ToString("n2") ?? "&nbsp;";
                CertificationHoursPercentNonCoreOutput.Text = info.CertificationHoursPercentNonCore?.ToString("n2") ?? "&nbsp;";

                TitleOutput.Text = info.ContentTitle ?? "&nbsp;";
                DescriptionOutput.Text = info.ContentDescription ?? "&nbsp;";
            }

            NumberOutput.Text = info.Code;

            ProfileOwnership.SetInputValues(info);
        }

        private void GetInputValues(QStandard info)
        {
            if (info.IsLocked)
                return;

            info.Code = NumberInput.Text;
            info.CertificationHoursPercentCore = CertificationHoursPercentCoreInput.ValueAsDecimal;
            info.CertificationHoursPercentNonCore = CertificationHoursPercentNonCoreInput.ValueAsDecimal;
            info.ContentTitle = TitleInput.Text;
            info.ContentDescription = DescriptionInput.Text;

            ProfileOwnership.GetInputValues(info);
        }

        #endregion

        #region Check delete allowed

        private bool CheckDeleteAllowed()
        {
            var groups = ProfileRepository.SelectRelatedGroups(StandardIdentifier);
            var employees = ProfileRepository.SelectRelatedEmployees(StandardIdentifier);

            var childProfiles = StandardSearch.Select(x => x.ParentStandardIdentifier == StandardIdentifier).OrderBy(x => x.Code).ToList();

            if (groups.Rows.Count == 0 && employees.Rows.Count == 0 && childProfiles.Count == 0)
                return true;

            var profile = StandardSearch.Select(StandardIdentifier);

            var names = new List<string>();

            if (groups.Rows.Count > 0)
                names.Add("organizations");

            if (employees.Rows.Count > 0)
                names.Add("employees");

            if (childProfiles.Count > 0)
                names.Add("child profiles");

            var nameList = BuildNameList(names);

            var message = new StringBuilder();
            message.AppendFormat(
                "You can't delete this profile ({0}) because it is referenced by the following {1}:<br/>",
                profile.ContentTitle, nameList);

            var showHeader = names.Count > 1;

            if (showHeader)
                message.Append("<br/>");

            if (groups.Rows.Count > 0)
                AddRelatedGroups(message, groups, showHeader);

            if (employees.Rows.Count > 0)
                AddRelatedEmployees(message, employees, showHeader);

            if (childProfiles.Count > 0)
                AddChildProfiles(message, childProfiles, showHeader);

            message.AppendFormat(
                "You must remove the profile from the {0} that reference it before you can delete the profile.",
                nameList);

            ScreenStatus.AddMessage(AlertType.Error, message.ToString());

            return false;
        }

        private static string BuildNameList(IList<string> names)
        {
            var nameList = new StringBuilder();

            for (var i = 0; i < names.Count; i++)
            {
                if (i > 0)
                {
                    if (i == names.Count - 1)
                        nameList.Append(" and ");
                    else
                        nameList.Append(", ");
                }

                nameList.Append(names[i]);
            }

            return nameList.ToString();
        }

        private void AddRelatedGroups(StringBuilder message, DataTable groups, bool showHeader)
        {
            if (showHeader)
                message.Append("<strong>Organizations</strong>");

            message.Append("<ul>");

            var prevOrganizationIdentifier = Guid.Empty;
            var isDepartmentAdded = false;

            foreach (DataRow row in groups.Rows)
            {
                var organizationId = (Guid)row["OrganizationIdentifier"];
                var companyName = (string)row["CompanyName"];
                var departmentName = row["DepartmentName"] as string;

                if (prevOrganizationIdentifier != organizationId)
                {
                    if (prevOrganizationIdentifier != Guid.Empty)
                    {
                        if (isDepartmentAdded)
                        {
                            message.Append(" )");
                            isDepartmentAdded = false;
                        }

                        message.Append("</li>");
                    }

                    prevOrganizationIdentifier = organizationId;

                    message.AppendFormat("<li>{0}", companyName);
                }
                else
                {
                    if (isDepartmentAdded)
                    {
                        message.Append(", ");
                    }
                    else
                    {
                        message.Append("&nbsp;&nbsp;&nbsp;( ");
                        isDepartmentAdded = true;
                    }

                    message.Append(departmentName);
                }
            }

            if (isDepartmentAdded)
                message.Append(" )");

            message.Append("</li></ul>");
        }

        private void AddRelatedEmployees(StringBuilder message, DataTable users, bool showHeader)
        {
            if (showHeader)
                message.Append("<strong>Employees</strong>");

            message.Append("<ul>");

            var key = Guid.Empty;

            foreach (DataRow row in users.Rows)
            {
                var userKey = (Guid)row["UserIdentifier"];
                var fullName = (string)row["FullName"];
                var companyName = (string)row["CompanyName"];

                if (key != userKey)
                {
                    if (key != Guid.Empty)
                    {
                        message.Append(" )");
                        message.Append("</li>");
                    }

                    key = userKey;

                    message.AppendFormat("<li>{0}&nbsp;&nbsp;&nbsp;( {1}", fullName, companyName);
                }
                else
                {
                    message.AppendFormat(", {0}", companyName);
                }
            }

            message.Append(" )</li></ul>");
        }

        private void AddChildProfiles(StringBuilder message, List<Standard> profiles, bool showHeader)
        {
            if (showHeader)
                message.Append("<strong>Child Profiles</strong>");

            message.Append("<ul>");

            foreach (var profile in profiles) message.AppendFormat("<li>{0}: {1}</li>", profile.Code, profile.ContentTitle);

            message.Append("</ul>");
        }

        #endregion
    }
}