using System;
using System.Collections.Specialized;
using System.Data;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Cmds.Actions.BulkTool.Assign;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;

using Shift.Common;
using Shift.Constant;
using Shift.Common.Events;

using CheckBox = System.Web.UI.WebControls.CheckBox;
using Label = System.Web.UI.WebControls.Label;
using MembershipType = Shift.Constant.MembershipType;
using UserProfileRepository = InSite.Persistence.Plugin.CMDS.UserProfileRepository;

namespace InSite.Cmds.Controls.BulkTool.Assign
{
    public partial class AssignCompanyProfilesToEmployee : UserControl
    {
        #region Events

        public event AlertHandler Alert;

        private void OnAlert(AlertType type, string message) =>
            Alert?.Invoke(this, new AlertArgs(type, message));

        #endregion

        #region Public methods

        public void LoadData(PersonFinderSecurityInfoWrapper finderSecurityInfo)
        {
            PersonIdentifier.Filter.OrganizationIdentifier = CurrentIdentityFactory.ActiveOrganizationIdentifier;

            if (!finderSecurityInfo.CanSeeAllCompanyPeople && !CurrentSessionState.Identity.HasAccessToAllCompanies)
            {
                var user = CurrentSessionState.Identity.User;

                PersonIdentifier.Filter.ParentUserIdentifier = user.UserIdentifier;
                PersonIdentifier.Filter.DepartmentsForParentId = user.UserIdentifier;
            }

            PersonIdentifier.Filter.RoleType = new[] { MembershipType.Organization, MembershipType.Department };

            DepartmentIdentifier.Filter.OrganizationIdentifier = CurrentIdentityFactory.ActiveOrganizationIdentifier;

            LoadDepartments();
        }

        #endregion

        #region Initialization and loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PersonIdentifier.AutoPostBack = true;
            PersonIdentifier.ValueChanged += (s, a) => LoadDepartments();

            DepartmentIdentifier.AutoPostBack = true;
            DepartmentIdentifier.ValueChanged += (s, a) => { LoadLevels(); LoadProfiles(); };

            ProfileRepeater.ItemCreated += ProfileRepeater_ItemCreated;
            ProfileRepeater.ItemDataBound += ProfileRepeater_ItemDataBound;

            SaveButton.Click += SaveButton_Click;

            PrimaryProfileValidator.ServerValidate += PrimaryProfileValidator_ServerValidate;
        }

        private void PrimaryProfileValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            var alreadyHasPrimary = false;

            args.IsValid = true;

            foreach (RepeaterItem item in ProfileRepeater.Items)
            {
                var profileStandardIdentifier = Guid.Parse(((Label)item.FindControl("ProfileStandardIdentifier")).Text);
                var profileType = ((ComboBox)item.FindControl("ProfileType")).Value;

                if (profileType != "Primary")
                    continue;

                if (alreadyHasPrimary)
                {
                    args.IsValid = false;
                    break;
                }

                alreadyHasPrimary = true;
            }
        }

        #endregion

        #region Event handlers

        private void ProfileRepeater_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.AlternatingItem && e.Item.ItemType != ListItemType.Item)
                return;

            var profileType = (ComboBox)e.Item.FindControl("ProfileType");
            profileType.AutoPostBack = true;
            profileType.ValueChanged += ProfileType_ValueChanged;
        }

        private void ProfileType_ValueChanged(object sender, EventArgs e)
        {
            var combo = (ComboBox)sender;
            var required = (CheckBox)combo.NamingContainer.FindControl("ComplianceRequired");
            var isPrimary = combo.Value == "Primary";
            required.Enabled = !isPrimary;
            required.Checked = isPrimary;
        }

        private void ProfileRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.AlternatingItem && e.Item.ItemType != ListItemType.Item)
                return;

            var row = (DataRowView)e.Item.DataItem;
            var isPrimary = row["IsPrimary"] as bool?;
            var isComplianceRequired = row["IsComplianceRequired"] as bool?;

            var profileType = (ComboBox)e.Item.FindControl("ProfileType");
            profileType.Value = !isPrimary.HasValue ? null : isPrimary.Value ? "Primary" : "Secondary";

            var IsComplianceRequired = (CheckBox)e.Item.FindControl("ComplianceRequired");
            IsComplianceRequired.Checked = isComplianceRequired ?? false;
            IsComplianceRequired.Enabled = !(isPrimary ?? false);
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            SaveData();

            LoadProfiles();
        }

        #endregion

        #region Load & save methods

        private void LoadDepartments()
        {
            DepartmentField.Visible = PersonIdentifier.Value.HasValue;
            ProfileRepeater.Visible = false;
            SaveButton.Visible = false;

            if (PersonIdentifier.Value == null)
                return;

            DepartmentIdentifier.Filter.UserIdentifier = PersonIdentifier.Value.Value;
            DepartmentIdentifier.Filter.RoleType = new[] { MembershipType.Organization, MembershipType.Department };
            DepartmentIdentifier.Value = null;
        }

        private void LoadLevels()
        {
            ProfileRepeater.Visible = false;
            SaveButton.Visible = false;
        }

        private void LoadProfiles()
        {
            var hasSelection = PersonIdentifier.Value.HasValue && DepartmentIdentifier.HasValue;

            ProfileRepeater.Visible = hasSelection;
            SaveButton.Visible = hasSelection;

            if (!hasSelection)
                return;

            var table = ProfileRepository.SelectProfilesForPerson(
                DepartmentIdentifier.Value.Value,
                PersonIdentifier.Value.Value);

            ProfileRepeater.DataSource = table;
            ProfileRepeater.DataBind();
        }

        private void SaveData()
        {
            if (ProfileRepeater.Items.Count == 0 || PersonIdentifier.Value == null)
                return;

            var errors = new StringCollection();

            SaveProfiles(errors);

            if (errors.Count > 0)
            {
                var html = new StringBuilder();
                html.Append("<ul>");
                foreach (var error in errors)
                    html.Append("<li>" + error + "</li>");
                html.Append("</ul>");
                OnAlert(AlertType.Warning, html.ToString());
            }
            else
            {
                OnAlert(AlertType.Success, "Your changes have been saved.");
            }
        }

        private void SaveProfiles(StringCollection errors)
        {
            var userKey = PersonIdentifier.Value.Value;
            var departmentIdentifier = DepartmentIdentifier.Value.Value;

            foreach (RepeaterItem item in ProfileRepeater.Items)
            {
                var profileStandardIdentifier = Guid.Parse(((Label)item.FindControl("ProfileStandardIdentifier")).Text);
                var profileType = ((ComboBox)item.FindControl("ProfileType")).Value;
                var isPrimaryProfile = profileType == "Primary";
                var isComplianceRequired = ((CheckBox)item.FindControl("ComplianceRequired")).Checked;

                var info = DepartmentProfileUserSearch.SelectFirst(
                    x => x.UserIdentifier == userKey
                      && x.DepartmentIdentifier == departmentIdentifier
                      && x.ProfileStandardIdentifier == profileStandardIdentifier);

                if (profileType.IsNotEmpty())
                {
                    if (info == null)
                    {
                        try
                        {
                            UserProfileRepository.RegisterNewProfile(
                                isPrimaryProfile,
                                departmentIdentifier,
                                userKey,
                                profileStandardIdentifier,
                                false,
                                false,
                                isPrimaryProfile || isComplianceRequired);
                        }
                        catch (Exception ex)
                        {
                            if (ex.Message.Contains("Cannot insert duplicate key row in object"))
                            {
                                var person = UserSearch.Select(userKey);
                                errors.Add($"{person.FullName} is already assigned another primary profile");
                            }
                            else
                            {
                                throw;
                            }
                        }
                    }
                    else
                    {
                        info.IsPrimary = isPrimaryProfile;
                        info.IsRequired = isPrimaryProfile || isComplianceRequired;

                        DepartmentProfileUserStore.Update(info);
                    }
                }
                else if (info != null)
                {
                    DepartmentProfileUserStore.Delete(
                        new DepartmentProfileUser[] { info },
                        CurrentSessionState.Identity.User.UserIdentifier,
                        CurrentSessionState.Identity.Organization.OrganizationIdentifier);
                }
            }
        }

        #endregion
    }
}