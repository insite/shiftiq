using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Application.Standards.Read;
using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;

using Shift.Common;
using Shift.Common.Events;
using Shift.Constant;

namespace InSite.UI.Admin.Contacts.Groups.Controls
{
    public partial class GroupOccupations : UserControl
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

        private List<Guid> AddedProfilesDataKeys
        {
            get => (List<Guid>)ViewState[nameof(AddedProfilesDataKeys)];
            set => ViewState[nameof(AddedProfilesDataKeys)] = value;
        }

        private List<Guid> NewProfilesDataKeys
        {
            get => (List<Guid>)ViewState[nameof(NewProfilesDataKeys)];
            set => ViewState[nameof(NewProfilesDataKeys)] = value;
        }

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            AddedProfilesDeleteButton.Click += AddedProfilesDeleteButton_Click;
            NewProfileAddButton.Click += NewProfileAddButton_Click;
            NewProfileAddCopyButton.Click += NewProfileAddCopyButton_Click;
            NewBulkAddButton.Click += NewBulkAddButton_Click;

            NewProfileFilterButton.Click += NewProfileFilterButton_Click;
            NewProfileClearButton.Click += NewProfileClearButton_Click;

            AddedProfiles.DataBinding += AddedProfiles_DataBinding;
            AddedProfiles.ItemDataBound += AddedProfiles_ItemDataBound;

            NewProfiles.DataBinding += NewProfiles_DataBinding;
            NewProfiles.ItemDataBound += NewProfiles_ItemDataBound;
        }

        protected override void OnPreRender(EventArgs e)
        {
            AddedProfilesSelectAllButton.OnClientClick = string.Format("return setCheckboxes('{0}', true);", ProfileTable.ClientID);
            AddedProfilesUnselectAllButton.OnClientClick = string.Format("return setCheckboxes('{0}', false);", ProfileTable.ClientID);
            AddedProfilesDeleteButton.OnClientClick = "return confirm('Are you sure you want to delete selected profiles?');";

            NewProfileSelectAllButton.OnClientClick = string.Format("return setCheckboxes('{0}', true);", NewProfileTable.ClientID);
            NewProfileUnselectAllButton.OnClientClick = string.Format("return setCheckboxes('{0}', false);", NewProfileTable.ClientID);

            base.OnPreRender(e);
        }

        #endregion

        #region Event handlers

        private void NewBulkAddButton_Click(Object sender, EventArgs e)
        {
            var text = NewBulkProfileNumbers.Text;

            if (String.IsNullOrEmpty(text))
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

            //_editorStatus.SetMessage(AlertType.Success, string.Format("{0:n0} profiles have been added to this organization", count));

            NewBulkProfileNumbers.Text = String.Empty;

            var totalCount = LoadProfiles();
            OnRefreshed(totalCount);
        }

        private void AddedProfilesDeleteButton_Click(object sender, EventArgs e)
        {
            var profiles = GetSelectedProfiles(AddedProfiles, AddedProfilesDataKeys);

            if (_deleteProfiles(profiles))
            {
                var totalCount = LoadProfiles();
                OnRefreshed(totalCount);
            }
        }

        private void NewProfileAddButton_Click(object sender, EventArgs e)
        {
            var profiles = GetSelectedProfiles(NewProfiles, NewProfilesDataKeys);

            _insertProfiles(profiles);

            var totalCount = LoadProfiles();
            OnRefreshed(totalCount);
        }

        private void NewProfileAddCopyButton_Click(Object sender, EventArgs e)
        {
            var profiles = CopyProfiles();

            _insertProfiles(profiles);

            var totalCount = LoadProfiles();
            OnRefreshed(totalCount);
        }

        private void NewProfileFilterButton_Click(object sender, EventArgs e)
        {
            IsNewProfilesSearched = true;
            LoadProfiles();
        }

        private void NewProfileClearButton_Click(object sender, EventArgs e)
        {
            IsNewProfilesSearched = false;
            NewProfileSearchText.Text = null;
            LoadProfiles();
        }

        private void AddedProfiles_DataBinding(object sender, EventArgs e)
        {
            AddedProfilesDataKeys = new List<Guid>();
        }

        private void AddedProfiles_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var standardId = (Guid)DataBinder.Eval(e.Item.DataItem, "StandardIdentifier");

            AddedProfilesDataKeys.Add(standardId);
        }

        private void NewProfiles_DataBinding(object sender, EventArgs e)
        {
            NewProfilesDataKeys = new List<Guid>();
        }

        private void NewProfiles_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var standardId = (Guid)DataBinder.Eval(e.Item.DataItem, "StandardIdentifier");

            NewProfilesDataKeys.Add(standardId);
        }

        #endregion

        #region Public methods

        public void InitDelegates(
            SelectProfiles selectProfiles,
            SelectProfilesToAdd selectProfilesToAdd,
            DeleteProfiles deleteProfiles,
            InsertProfiles insertProfiles,
            InitProfileCopy initProfileCopy)
        {
            _selectProfiles = selectProfiles;
            _selectProfilesToAdd = selectProfilesToAdd;
            _deleteProfiles = deleteProfiles;
            _insertProfiles = insertProfiles;
            _initProfileCopy = initProfileCopy;

            NewProfileAddCopyButton.Visible = _initProfileCopy != null;
        }

        public int LoadProfiles(Guid? organization, Guid? department)
        {
            OrganizationIdentifier = organization;
            DepartmentIdentifier = department;

            var data = _selectProfiles();

            AddedProfiles.DataSource = data;
            AddedProfiles.DataBind();

            ProfileTab.Visible = data.Rows.Count > 0;
            ProfileTab.Title = $"Profiles <span class=\"badge rounded-pill bg-info ms-1\">{data.Rows.Count:n0}</span>";

            if (IsNewProfilesSearched)
            {
                var newProfiles = _selectProfilesToAdd(NewProfileSearchText.Text);

                NewProfiles.DataSource = newProfiles;
                NewProfiles.DataBind();

                NewProfileResultColumn.Visible = newProfiles.Rows.Count > 0;

                NewProfileResultMessage.Visible = true;
                NewProfileResultMessage.InnerHtml = newProfiles.Rows.Count > 0
                    ? $"Found {newProfiles.Rows.Count:n0} profiles:"
                    : "No profiles found";
            }
            else
            {
                NewProfiles.DataSource = null;
                NewProfiles.DataBind();

                NewProfileResultColumn.Visible = false;
                NewProfileResultMessage.Visible = false;
            }

            return data.Rows.Count;
        }

        #endregion

        #region Helper methods

        private int LoadProfiles()
        {
            return LoadProfiles(OrganizationIdentifier, DepartmentIdentifier);
        }

        private static IList<Guid> GetSelectedProfiles(Repeater repeater, List<Guid> dataKeys)
        {
            var result = new List<Guid>();

            for (var i = 0; i < repeater.Items.Count; i++)
            {
                var item = repeater.Items[i];
                var checkbox = (ICheckBoxControl)item.FindControl("OccupationSelected");
                if (checkbox.Checked)
                    result.Add(dataKeys[i]);
            }

            return result;
        }

        #endregion

        #region Copy profiles

        private IList<Guid> CopyProfiles()
        {
            var profiles = GetSelectedProfiles(NewProfiles, NewProfilesDataKeys);

            var copies = new List<Guid>();

            foreach (var profileStandardIdentifier in profiles)
                copies.Add(CopyProfile(profileStandardIdentifier));

            return copies;
        }

        private Guid CopyProfile(Guid profile)
        {
            var info = ServiceLocator.StandardSearch.GetStandard(profile);
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