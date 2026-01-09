using System;
using System.Linq;

using InSite.Application.Contacts.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;
using InSite.UI.Layout.Admin;
using InSite.Web.Data;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

using PersonFilter = InSite.Persistence.Plugin.CMDS.CmdsPersonFilter;

namespace InSite.Cmds.Admin.People.Forms
{
    public partial class Create : AdminBasePage, ICmdsUserControl
    {
        #region Constants

        private const string SearchUrl = "/ui/cmds/admin/users/search";
        private const string EditUrl = "/ui/cmds/admin/users/edit";

        #endregion

        #region Security

        public override void ApplyAccessControlForCmds()
        {
            base.ApplyAccessControlForCmds();

            if (Access.Read && !Access.Administrate && !Access.Configure)
                Access = Access.SetAll(false);
        }

        #endregion

        #region Initialization and Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            AddNewPersonButton.Click += (s, a) => Save();
            BackToEditButton.Click += (s, a) => MultiView.SetActiveView(CreatorView);

            OrganizationIdentifier.AutoPostBack = true;
            OrganizationIdentifier.ValueChanged += OrganizationIdentifier_ValueChanged;

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            PageHelper.AutoBindHeader(this);

            MultiView.SetActiveView(CreatorView);

            FilterCompanies();

            PersonDetails.SetInputValues(new Persistence.User(), new Person());

            CancelButton.NavigateUrl = SearchUrl;
        }

        private void FilterCompanies()
        {
            var seeAllCompanies = Identity.IsInRole(CmdsRole.Programmers) ||
                                  Identity.IsInRole(CmdsRole.SystemAdministrators);

            if (seeAllCompanies)
                return;

            var seeMyCompanies = seeAllCompanies || Identity.IsInRole(CmdsRole.OfficeAdministrators) ||
                                 Identity.IsInRole(CmdsRole.FieldAdministrators);

            if (seeMyCompanies)
            {
                OrganizationIdentifier.Filter.RequireMembershipForUserEmail = User.Email;
                OrganizationIdentifier.Value = null;
            }
            else
            {
                OrganizationIdentifier.Value = CurrentIdentityFactory.ActiveOrganizationIdentifier;
                OrganizationIdentifier.Enabled = false;
            }
        }

        #endregion

        #region Event handlers

        private void OrganizationIdentifier_ValueChanged(object sender, EventArgs e)
        {
            DepartmentIdentifier.Filter.OrganizationIdentifier = OrganizationIdentifier.Value ?? Guid.Empty;
            DepartmentIdentifier.Value = null;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
                Save();
        }

        #endregion

        #region Methods (save)

        private void Save()
        {
            var user = UserFactory.Create();
            var person = UserFactory.CreatePerson(Organization.Identifier);

            PersonDetails.GetInputValues(user, person);

            user.AccessGrantedToCmds = true;
            user.UserIdentifier = UniqueIdentifier.Create();

            user.SetDefaultPassword(Default.CmdsPassword);

            if (UserSearch.IsEmailDuplicate(user.UserIdentifier, user.Email))
            {
                ScreenStatus.AddMessage(AlertType.Error, $"There is another user already registered with the email address <strong>{user.Email}</strong>.");
                return;
            }

            if (MultiView.GetActiveView() != DuplicateView && BindDuplicates(user))
                return;

            UserStore.Insert(user, person);

            if (DepartmentIdentifier.HasValue)
            {
                var organization = OrganizationIdentifier.Value ?? Organization.Identifier;

                MembershipStore.Save(MembershipFactory.Create(user.UserIdentifier, DepartmentIdentifier.Value.Value, organization, RoleType.SelectedValue));
                
                if (organization != Organization.Identifier)
                    PersonStore.Insert(PersonFactory.Create(user.UserIdentifier, organization, null, true, User.FullName));
            }

            var learnerGroupId = ServiceLocator.GroupSearch
                .GetGroups(new QGroupFilter { GroupName = "CMDS Workers" })
                .FirstOrDefault()
                .GroupIdentifier;

            MembershipHelper.Save(learnerGroupId, user.UserIdentifier, "Membership");

            InSite.Admin.Contacts.People.Utilities.PersonHelper.SendAccountCreated(OrganizationIdentifiers.CMDS, "CMDS", user, person);

            PersonDetails.SaveRoles(user.UserIdentifier);

            HttpResponseHelper.Redirect(EditUrl + $"?userID={user.UserIdentifier}&new=true");
        }

        private bool BindDuplicates(QUser user)
        {
            MultiView.SetActiveView(DuplicateView);

            Duplicates.SetVisibleColumns(new[] { "Name", "City", "EmailWork" });

            var filter = new PersonFilter
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                IsArchived = null,
                NameFilterType = "Similar"
            };

            Duplicates.LoadData(filter);

            if (!Duplicates.HasRows)
            {
                MultiView.SetActiveView(CreatorView);

                return false;
            }

            filter.NameFilterType = "Exact";
            var exactDuplicateCount = ContactRepository3.CountPeople(filter);
            filter.NameFilterType = "Similar";
            var similarDuplicateCount = ContactRepository3.CountPeople(filter);

            var text1 = exactDuplicateCount == 1
                ? "is <strong>1</strong> person"
                : $"are <strong>{exactDuplicateCount}</strong> people";

            var text2 = similarDuplicateCount == 1
                ? "is <strong>1</strong> person"
                : $"are <strong>{similarDuplicateCount}</strong> people";

            DuplicateWarningMessage.Text = $@"There {text2} in the system who {(similarDuplicateCount == 1 ? "has" : "have")} a name with similar spelling or pronounciation. (Of these, there {text1} with the same spelling.)<br/>Are you sure you want to create a new record for this person?";

            if (exactDuplicateCount == 0 || Identity.IsInRole(CmdsRole.Programmers) ||
                Identity.IsInRole(CmdsRole.SystemAdministrators))
            {
                AddNewPersonButton.Visible = true;
                AddNewPersonInstruction.Visible = false;
            }
            else
            {
                AddNewPersonButton.Visible = false;
                AddNewPersonInstruction.Visible = true;
            }

            return true;
        }

        #endregion
    }
}