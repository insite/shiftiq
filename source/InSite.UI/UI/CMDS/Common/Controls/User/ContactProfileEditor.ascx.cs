using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Application.Standards.Read;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;

using Shift.Common;
using Shift.Common.Events;
using Shift.Constant;

using AlertType = Shift.Constant.AlertType;
using Label = System.Web.UI.WebControls.Label;

namespace InSite.Cmds.Controls.Contacts.ListEditors
{
    public partial class ContactProfileEditor : UserControl
    {
        #region Delegates

        public delegate DataTable SelectProfiles();
        public delegate DataTable SelectProfilesToAdd(string searchText);
        public delegate bool DeleteProfiles(IList<Guid> profiles);
        public delegate int InsertProfiles(IList<Guid> profiles);
        public delegate void InitProfileCopy(QStandard profile);

        #endregion

        #region Events

        public event IntValueHandler Refreshed;
        private void OnRefreshed(int count) =>
            Refreshed?.Invoke(this, new IntValueArgs(count));

        #endregion

        #region Fields

        private SelectProfiles _selectProfiles;
        private SelectProfilesToAdd _selectProfilesToAdd;
        private DeleteProfiles _deleteProfiles;
        private InsertProfiles _insertProfiles;
        private InitProfileCopy _initProfileCopy;

        private Alert _editorStatus;

        #endregion

        #region Properties

        private bool IsNewProfilesSearched
        {
            get { return ViewState[nameof(IsNewProfilesSearched)] != null && (bool)ViewState[nameof(IsNewProfilesSearched)]; }
            set { ViewState[nameof(IsNewProfilesSearched)] = value; }
        }

        protected Guid? OrganizationIdentifier
        {
            get => (Guid?)ViewState[nameof(OrganizationIdentifier)];
            set => ViewState[nameof(OrganizationIdentifier)] = value;
        }

        protected Guid? DepartmentIdentifier
        {
            get => (Guid?)ViewState[nameof(DepartmentIdentifier)];
            set => ViewState[nameof(DepartmentIdentifier)] = value;
        }

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DeleteProfileButton.Click += DeleteProfileButton_Click;
            AddProfileButton.Click += AddProfileButton_Click;
            AddProfileCopyButton.Click += AddProfileCopyButton_Click;
            AddMultipleButton.Click += AddMultipleButton_Click;

            FilterButton.Click += FilterButton_Click;
            ClearButton.Click += ClearButton_Click;

            AddedProfiles.ItemDataBound += Profiles_ItemDataBound;
            NewProfiles.ItemDataBound += Profiles_ItemDataBound;
        }

        protected override void OnPreRender(EventArgs e)
        {
            SelectAllButton.OnClientClick = $"return setCheckboxes('{AddedProfiles.ClientID}', true);";
            UnselectAllButton.OnClientClick = $"return setCheckboxes('{AddedProfiles.ClientID}', false);";

            DeleteProfileButton.OnClientClick = "return confirm('Are you sure you want to delete selected profiles?');";

            SelectAllButton2.OnClientClick = $"return setCheckboxes('{NewProfiles.ClientID}', true);";
            UnselectAllButton2.OnClientClick = $"return setCheckboxes('{NewProfiles.ClientID}', false);";

            base.OnPreRender(e);
        }

        #endregion

        #region Event handlers

        private void AddMultipleButton_Click(object sender, EventArgs e)
        {
            var text = MultipleProfileNumbers.Text;

            if (string.IsNullOrEmpty(text))
                return;


            var profiles = new List<Guid>();

            var numbers = Shift.Common.StringHelper.Split(text);

            foreach (var number in numbers)
            {
                var profile = StandardSearch.SelectFirst(x => x.StandardType == "Profile" && x.Code == number);

                if (profile != null)
                    profiles.Add(profile.StandardIdentifier);
            }

            var count = _insertProfiles(profiles);

            _editorStatus.AddMessage(AlertType.Success, string.Format("{0:n0} profiles have been added to this organization", count));

            MultipleProfileNumbers.Text = string.Empty;

            var totalCount = LoadProfiles();
            OnRefreshed(totalCount);
        }

        private void DeleteProfileButton_Click(object sender, EventArgs e)
        {
            var profiles = GetSelectedProfiles(AddedProfiles);

            if (_deleteProfiles(profiles))
            {
                var totalCount = LoadProfiles();
                OnRefreshed(totalCount);
            }
        }

        private void AddProfileButton_Click(object sender, EventArgs e)
        {
            var profiles = GetSelectedProfiles(NewProfiles);

            _insertProfiles(profiles);

            var totalCount = LoadProfiles();
            OnRefreshed(totalCount);
        }

        private void AddProfileCopyButton_Click(object sender, EventArgs e)
        {
            var profiles = CopyProfiles();

            _insertProfiles(profiles);

            var totalCount = LoadProfiles();
            OnRefreshed(totalCount);
        }

        private void FilterButton_Click(object sender, EventArgs e)
        {
            IsNewProfilesSearched = true;
            LoadProfiles();
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            IsNewProfilesSearched = false;
            SearchText.Text = null;
            LoadProfiles();
        }

        private void Profiles_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var row = (DataRowView)e.Item.DataItem;
            var title = row["ProfileTitle"] as string;

            var titleLabel = (Label)e.Item.FindControl("Title");
            titleLabel.Text = title != null && title.Length > 50 ? title.Substring(0, 47) + "..." : title;
            titleLabel.ToolTip = title != null && title.Length > 50 ? title : null;
        }

        #endregion

        #region Public methods

        public void InitDelegates(
            SelectProfiles selectProfiles,
            SelectProfilesToAdd selectProfilesToAdd,
            DeleteProfiles deleteProfiles,
            InsertProfiles insertProfiles,
            InitProfileCopy initProfileCopy,
            Alert editorStatus,
            string entityName)
        {
            _selectProfiles = selectProfiles;
            _selectProfilesToAdd = selectProfilesToAdd;
            _deleteProfiles = deleteProfiles;
            _insertProfiles = insertProfiles;
            _editorStatus = editorStatus;
            _initProfileCopy = initProfileCopy;

            EntityName1.Text = entityName;
            EntityName2.Text = entityName;

            AddProfileCopyButton.Visible = _initProfileCopy != null;
            CopyMessage.Visible = _initProfileCopy != null;
        }

        public int LoadProfiles(Guid? organizationId, Guid? department)
        {
            OrganizationIdentifier = organizationId;
            DepartmentIdentifier = department;

            var data = _selectProfiles();

            AddedProfiles.DataSource = data;
            AddedProfiles.DataBind();

            ProfileTab.SetTitle("Profiles", data.Rows.Count);
            ProfileTab.Visible = data.Rows.Count > 0;

            if (IsNewProfilesSearched)
            {
                var newProfiles = _selectProfilesToAdd(SearchText.Text);

                NewProfiles.DataSource = newProfiles;
                NewProfiles.DataBind();

                ProfileList.Visible = newProfiles.Rows.Count > 0;

                FoundProfile.Visible = true;

                FoundProfile.InnerHtml = newProfiles.Rows.Count > 0
                    ? string.Format("Found {0} profiles:", newProfiles.Rows.Count)
                    : "No profiles found";
            }
            else
            {
                NewProfiles.DataSource = null;
                NewProfiles.DataBind();

                ProfileList.Visible = false;
                FoundProfile.Visible = false;
            }

            return data.Rows.Count;
        }

        #endregion

        #region Helper methods

        private int LoadProfiles()
        {
            return LoadProfiles(OrganizationIdentifier, DepartmentIdentifier);
        }

        private static IList<Guid> GetSelectedProfiles(Repeater repeater)
        {
            var profiles = new List<Guid>();

            foreach (RepeaterItem item in repeater.Items)
            {
                var profile = (ICheckBoxControl)item.FindControl("ProfileSelected");
                if (!profile.Checked)
                    continue;

                var profileStandardIdentifierControl = (ITextControl)item.FindControl("ProfileStandardIdentifier");
                var profileStandardIdentifier = Guid.Parse(profileStandardIdentifierControl.Text);

                profiles.Add(profileStandardIdentifier);
            }

            return profiles;
        }

        #endregion

        #region Copy profiles

        private IList<Guid> CopyProfiles()
        {
            var profiles = GetSelectedProfiles(NewProfiles);

            var copies = new List<Guid>();

            foreach (var profileStandardIdentifier in profiles)
                copies.Add(CopyProfile(profileStandardIdentifier));

            return copies;
        }

        private Guid CopyProfile(Guid profile)
        {
            var info = ServiceLocator.StandardSearch.GetStandard(profile); // , x => x.Departments, x => x.StandardValidations);
            info.StandardIdentifier = UniqueIdentifier.Create();
            info.ParentStandardIdentifier = profile;
            info.ContentTitle = $"Copy of {info.ContentTitle}";

            _initProfileCopy(info);

            info.Code = ProfileHelper.InitNumber(info, CurrentSessionState.Identity.User.Email);
            info.AssetNumber = Sequence.Increment(CurrentSessionState.Identity.Organization.Identifier, SequenceType.Asset);

            StandardStore.Insert(info);

            StandardContainmentStore.CopyByFrom(profile, info.StandardIdentifier);

            return info.StandardIdentifier;
        }

        #endregion
    }
}