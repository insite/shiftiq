using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

using Shift.Common.Timeline.Commands;

using InSite.Application.Contacts.Read;
using InSite.Application.Groups.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Contacts;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;
using Shift.Contract;

namespace InSite.Admin.Contacts.Groups.Forms
{
    public partial class Edit : AdminBasePage, IOverrideWebRouteParent
    {
        #region Constants

        private const string GroupSearchUrl = "/ui/admin/contacts/groups/search";

        #endregion

        #region Classes

        [Serializable]
        private class EntityInfo
        {
            public Guid Identifier { get; }
            public string Subtype { get; }
            public string Label { get; }

            public EntityInfo(QGroup group)
            {
                Identifier = group.GroupIdentifier;
                Subtype = group.GroupType;
                Label = group.GroupLabel;
            }
        }

        #endregion

        #region Properties

        private EntityInfo Entity
        {
            get => (EntityInfo)ViewState[nameof(Entity)];
            set => ViewState[nameof(Entity)] = value;
        }

        private bool IsEmployer => Request.QueryString["employer"] == "1";

        #endregion

        #region Initialization and Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            GroupType.AutoPostBack = true;
            GroupType.ValueChanged += (s, a) => SetFieldVisibility();

            GroupRegion.Settings.CollectionName = CollectionName.Contacts_People_Location_Region;
            GroupRegion.Settings.OrganizationIdentifier = Organization.Key;

            GroupStatusItemIdentifier.ListFilter.OrganizationIdentifier = Organization.Identifier;
            GroupStatusItemIdentifier.ListFilter.CollectionName = CollectionName.Contacts_Groups_Status_Name;

            GroupOccupations.InitDelegates(SelectProfiles, SelectProfilesToAdd, DeleteProfiles, InsertProfiles, null);

            AchievementEditor.InitDelegates(
                Organization.Identifier,
                GetAssignedAchievements,
                DeleteAchievements,
                InsertAchievements,
                "group");

            SaveButton.Click += SaveButton_Click;
        }

        public override void ApplyAccessControl()
        {
            base.ApplyAccessControl();

            SaveButton.Visible = CanEdit;
            DeleteButton.Visible = CanDelete;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            var status = Request.QueryString["status"];
            if (status == "saved")
                SetStatus(ScreenStatus, StatusType.Saved);

            var panel = Request.QueryString["panel"]?.ToLower();
            if (panel == "permission")
                PermissionSection.IsSelected = true;
            else if (panel == "people")
                RoleSection.IsSelected = true;

            GroupParentID.Filter.OrganizationIdentifier = CurrentSessionState.Identity.Organization.Identifier;

            AddNewUsersAutomatically.Visible = Identity.IsOperator;

            Open();

            var deleteUrl = $"/ui/admin/contacts/groups/delete?id={Entity.Identifier}";
            if (IsEmployer)
                deleteUrl += "&employer=1";

            DeleteButton.NavigateUrl = deleteUrl;
            CancelButton.NavigateUrl = GroupSearchUrl;

            var returnUrl = $"/ui/admin/contacts/groups/edit?contact={Entity.Identifier}";
            HistoryButton.NavigateUrl = $"/ui/admin/logs/aggregates/outline?aggregate={Entity.Identifier}&returnURL=" + HttpUtility.UrlEncode(returnUrl);
        }

        #endregion

        #region Event handlers

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!IsValid || !Save())
                return;

            Open();

            SetStatus(ScreenStatus, StatusType.Saved);
        }

        #endregion

        #region Methods (open)

        private void Open()
        {
            var group = Guid.TryParse(Request.QueryString["contact"], out var groupId)
                ? ServiceLocator.GroupSearch.GetGroup(groupId)
                : null;
            if (group == null || group.OrganizationIdentifier != Organization.OrganizationIdentifier)
                HttpResponseHelper.Redirect(GroupSearchUrl);

            Entity = new EntityInfo(group);

            var createUrl = "/ui/admin/contacts/groups/create?type=" + Entity.Subtype;
            if (Entity.Label != null)
                createUrl += "&label=" + Entity.Label;

            string expired = string.Empty;

            if (group.GroupExpiry.HasValue && group.GroupExpiry.Value < DateTimeOffset.UtcNow)
                expired = "<span class=\"badge bg-danger\">Expired</span>";

            PageHelper.AutoBindHeader(
                Page,
                new BreadcrumbItem("Add New Group", createUrl, null, null),
                $"{group.GroupName} <span class='form-text'>{group.GroupType}</span> {expired}");

            SetInputValues(group);

            GroupOccupations.LoadProfiles(Organization.Identifier, group.GroupIdentifier);

            if (group.GroupCapacity.HasValue)
            {
                var capacity = group.GroupCapacity.Value;
                var countPeople = RoleGrid.GetAllRowsCount();

                if (capacity < countPeople)
                    ScreenStatus.AddMessage(
                        AlertType.Warning,
                        $"This group contains {countPeople} people but has a capacity of {capacity}. " +
                        $"Please remove {countPeople - capacity}.");
            }

            AchievementEditor.SetEditable(true, true);
            AchievementEditor.LoadAchievements();
        }

        private void SetInputValues(QGroup group)
        {
            GroupType.Value = group.GroupType;
            GroupLabel.Text = group.GroupLabel;
            GroupName.Text = group.GroupName;
            GroupDescription.Text = group.GroupDescription;
            GroupCode.Text = group.GroupCode;
            GroupRegion.Value = group.GroupRegion;
            GroupURL.Text = group.GroupWebSiteUrl;
            GroupEmail.Text = group.GroupEmail;
            Capacity.ValueAsInt = group.GroupCapacity;

            GroupExpiry.Value = group.GroupExpiry;
            LifetimeQuantity.ValueAsInt = group.LifetimeQuantity;
            LifetimeUnit.Value = group.LifetimeUnit;

            AddNewUsersAutomatically.Checked = group.AddNewUsersAutomatically;
            AllowSelfSubscription.Checked = group.AllowSelfSubscription;
            AllowJoinGroupUsingLink.Checked = group.AllowJoinGroupUsingLink;

            OnlyOperatorCanAddUserField.Visible = Organization.Toolkits.Contacts.EnableOperatorGroup && Identity.IsOperator;
            OnlyOperatorCanAddUser.Checked = group.OnlyOperatorCanAddUser;

            MessageToAdminWhenMembershipStarted.Filter.Type = MessageTypeName.Notification;
            MessageToAdminWhenMembershipStarted.Value = group.MessageToAdminWhenMembershipStarted;

            MessageToAdminWhenMembershipEnded.Filter.Type = MessageTypeName.Notification;
            MessageToAdminWhenMembershipEnded.Value = group.MessageToAdminWhenMembershipEnded;

            MessageToUserWhenMembershipStarted.Filter.Type = MessageTypeName.Notification;
            MessageToUserWhenMembershipStarted.Value = group.MessageToUserWhenMembershipStarted;

            MessageToUserWhenMembershipEnded.Filter.Type = MessageTypeName.Notification;
            MessageToUserWhenMembershipEnded.Value = group.MessageToUserWhenMembershipEnded;

            MessageToAdminWhenEventVenueChanged.Filter.Type = MessageTypeName.Alert;
            MessageToAdminWhenEventVenueChanged.Value = group.MessageToAdminWhenEventVenueChanged;

            MandatorySurveyFormIdentifier.Value = group.SurveyFormIdentifier;

            MembershipProduct.Value = group.MembershipProductIdentifier;

            GroupCategory.Text = group.GroupCategory;

            GroupStatusItemSelectorView.IsActive = true;
            GroupStatusItemIdentifier.RefreshData();
            GroupStatusItemIdentifier.ValueAsGuid = group.GroupStatusItemIdentifier;
            GroupStatusItemText.Text = null;

            GroupOffice.Text = group.GroupOffice;
            GroupPhone.Text = group.GroupPhone;

            GroupParentID.Filter.ExcludeGroupIdentifiers = ServiceLocator.GroupSearch.GetGroupDescendentRelationships(group.GroupIdentifier)
                .Select(x => x.ChildGroupIdentifier).ToArray();

            GroupParentID.Value = group.ParentGroupIdentifier;

            var hasParent = group.ParentGroupIdentifier.HasValue;

            if (hasParent)
            {
                var parent = ServiceLocator.GroupSearch.GetGroup(group.ParentGroupIdentifier.Value);

                hasParent = parent != null;

                if (hasParent)
                    ParentLink.NavigateUrl = $"/ui/admin/contacts/groups/edit?contact={parent.GroupIdentifier}";
            }

            ParentLink.Visible = hasParent;
            OrganizeLink.NavigateUrl = $"/ui/admin/contacts/groups/organize?group={group.GroupIdentifier}";

            BindParentConnections();
            BindChildren();
            BindChildConnections();
            BindTagList(ServiceLocator.GroupSearch.GetGroupTags(group.GroupIdentifier).ToArray());

            GroupIndustry.Value = group.GroupIndustry;
            NumberOfEmployees.Value = group.GroupSize;

            AddressList.SetInputValues(group);

            RoleGrid.LoadData(group);
            PermissionGrid.LoadData(group.GroupIdentifier, "contact&panel=permission");

            Photos.LoadData(group.GroupIdentifier);

            SetFieldVisibility();
        }

        private void BindParentConnections()
        {
            var group = Entity.Identifier;

            var parentContainments = ServiceLocator.GroupSearch
                .GetParentConnections(group, x => x.ParentGroup)
                .Select(x => new { x.ParentGroup.GroupIdentifier, x.ParentGroup.GroupName })
                .OrderBy(x => x.GroupName)
                .ToArray();

            var hasParentContainments = parentContainments.Length > 0;

            ParentContainmentRepeater.Visible = hasParentContainments;
            ParentContainmentRepeater.DataSource = parentContainments;
            ParentContainmentRepeater.DataBind();

            if (!hasParentContainments)
            {
                ParentContainmentMessage.Visible = true;
                ParentContainmentMessage.Text = $"<p>There are no parent groups contains the {Entity.Subtype.ToLower()}.</p>";
            }
        }

        private void BindTagList(string[] selectedTags)
        {
            var tags = TCollectionItemCache.Select(new TCollectionItemFilter
            {
                OrganizationIdentifier = Organization.OrganizationIdentifier,
                CollectionName = CollectionName.Contacts_Groups_Classification_Tag
            });

            GroupTagList.DataSource = tags;
            GroupTagList.DataBind();
            GroupTagsField.Visible = tags.Count > 0;

            if (selectedTags.Length > 0)
            {
                foreach (var selectedTag in selectedTags)
                {
                    var item = GroupTagList.Items.FindByText(selectedTag.ToString());
                    if (item != null)
                        item.Selected = true;
                }
            }
        }

        private void BindChildren()
        {
            var group = Entity.Identifier;

            var childrenGroups = ServiceLocator.GroupSearch
                .GetGroups(new QGroupFilter { ParentGroupIdentifier = group })
                .Select(x => new { x.GroupIdentifier, x.GroupName })
                .OrderBy(x => x.GroupName)
                .ToArray();

            ChildrenRepeater.DataSource = childrenGroups;
            ChildrenRepeater.DataBind();

            ChildrenField.Visible = childrenGroups.Length > 0;
        }

        private void BindChildConnections()
        {
            var group = Entity.Identifier;

            var childContainments = ServiceLocator.GroupSearch
                .GetChildConnections(group, x => x.ChildGroup)
                .Select(x => new { x.ChildGroup.GroupIdentifier, x.ChildGroup.GroupName })
                .OrderBy(x => x.GroupName)
                .ToArray();

            ChildrenContainmentRepeater.DataSource = childContainments;
            ChildrenContainmentRepeater.DataBind();
            ChildrenContainmentField.Visible = childContainments.Length > 0;
        }

        private void SetFieldVisibility()
        {
            var isRole = GroupType.Value == GroupTypes.Role;
            PermissionSection.Visible = isRole && CurrentSessionState.Identity.IsGranted(PermissionIdentifiers.Admin_Settings);
            GroupContactCard.Visible = !isRole;
            AddressSection.Visible = !isRole;
        }

        #endregion

        #region Methods (save)

        private bool Save()
        {
            if (GroupParentID.Value.HasValue && ServiceLocator.GroupSearch.CausesCycle(Entity.Identifier, GroupParentID.Value.Value))
            {
                ScreenStatus.AddMessage(
                    AlertType.Error,
                    "The selected parent is not valid because this relationship creates dependency cycle in hierarchy.");
                return false;
            }

            var g = ServiceLocator.GroupSearch.GetGroup(Entity.Identifier);
            var commands = new List<Command>();

            GetInputValues(g, commands);

            ServiceLocator.SendCommands(commands);

            return true;
        }

        private void GetInputValues(QGroup g, List<Command> commands)
        {
            var id = Entity.Identifier;
            var isRole = GroupType.Value == GroupTypes.Role;
            var isEmployer = GroupType.Value == GroupTypes.Employer;
            var label = GroupHelper.CleanGroupLabel(GroupLabel.Text);
            var region = GroupContactRegionField.Visible ? GroupRegion.Value : g.GroupRegion;
            var capacity = Capacity.ValueAsInt;
            var phone = Phone.Format(GroupPhone.Text);

            commands.Add(new RenameGroup(id, GroupType.Value, GroupName.Text));
            commands.Add(new DescribeGroup(id, GroupCategory.Text, GroupCode.Text, GroupDescription.Text, label));
            commands.Add(new ChangeGroupLocation(id, GroupOffice.Text, region, g.ShippingPreference, g.GroupWebSiteUrl));
            commands.Add(new ChangeGroupCapacity(id, capacity));
            commands.Add(new ChangeGroupSettings(id, AddNewUsersAutomatically.Checked, AllowSelfSubscription.Checked));
            commands.Add(new ModifyAllowJoinGroupUsingLink(id, AllowJoinGroupUsingLink.Checked));

            if (Organization.Toolkits.Contacts.EnableOperatorGroup && Identity.IsOperator)
                commands.Add(new ModifyGroupOnlyOperatorCanAddUser(id, OnlyOperatorCanAddUser.Checked));

            commands.Add(new ConfigureGroupNotifications(id,
                MessageToAdminWhenEventVenueChanged.Value,
                MessageToAdminWhenMembershipEnded.Value,
                MessageToAdminWhenMembershipStarted.Value,
                MessageToUserWhenMembershipEnded.Value,
                MessageToUserWhenMembershipStarted.Value
                ));

            commands.Add(new ChangeGroupSurvey(id, MandatorySurveyFormIdentifier.Value, Necessity.Required));
            commands.Add(new ModifyGroupMembershipProduct(id, MembershipProduct.Value));
            commands.Add(new ModifyGroupStatus(id, GetGroupStatusItemId()));
            commands.Add(new ChangeGroupPhone(id, phone));
            commands.Add(new ChangeGroupParent(id, GroupParentID.Value));

            foreach (System.Web.UI.WebControls.ListItem item in GroupTagList.Items)
            {
                if (item.Selected)
                    commands.Add(new AddGroupTag(id, item.Text));
                else
                    commands.Add(new RemoveGroupTag(id, item.Text));
            }

            commands.Add(new ChangeGroupExpiry(id, GroupExpiry.Value));
            commands.Add(new ChangeGroupLifetime(id, LifetimeQuantity.ValueAsInt, LifetimeUnit.Value));

            commands.Add(new ChangeGroupEmail(id, GroupEmail.Text));
            commands.Add(new ChangeGroupWebSiteUrl(id, GroupURL.Text));

            if (isEmployer)
            {
                commands.Add(new ChangeGroupIndustry(id, GroupIndustry.Value, ""));
                commands.Add(new ChangeGroupSize(id, NumberOfEmployees.Value));
            }

            if (!isRole)
            {
                var addresses = new Dictionary<AddressType, GroupAddress>();

                AddressList.GetInputValues(addresses);

                foreach (var entry in addresses)
                    commands.Add(new ChangeGroupAddress(id, entry.Key, entry.Value));
            }
        }

        private Guid? GetGroupStatusItemId()
        {
            return TryCreateGroupStatusItem() ?? GroupStatusItemIdentifier.ValueAsGuid;
        }

        private Guid? TryCreateGroupStatusItem()
        {
            if (!GroupStatusItemTextView.IsActive)
                return null;

            var text = GroupStatusItemText.Text;
            if (text.IsEmpty())
                return null;

            var existItems = TCollectionItemSearch.Select(new TCollectionItemFilter
            {
                OrganizationIdentifier = Organization.Identifier,
                CollectionName = CollectionName.Contacts_Groups_Status_Name,
                ItemName = text
            });

            if (existItems.Count > 0)
                return existItems[0].ItemIdentifier;

            var collectionId = TCollectionSearch.BindFirst(
                x => (Guid?)x.CollectionIdentifier,
                new TCollectionFilter { CollectionName = CollectionName.Contacts_Groups_Status_Name });
            if (!collectionId.HasValue)
                return null;

            var nextSequence = TCollectionItemSearch.GetNextSequence(collectionId.Value, Organization.Identifier);
            var entity = new TCollectionItem
            {
                ItemName = text,
                ItemSequence = nextSequence,
                OrganizationIdentifier = Organization.Identifier,
                ItemIdentifier = UniqueIdentifier.Create(),
                CollectionIdentifier = collectionId.Value
            };

            TCollectionItemStore.Insert(entity);
            TCollectionItemCache.Refresh();

            return entity.ItemIdentifier;
        }

        #endregion

        #region Methods (occupations)

        private DataTable SelectProfiles()
        {
            return TDepartmentStandardSearch.SelectDepartmentProfilesOnly(Entity.Identifier);
        }

        private DataTable SelectProfilesToAdd(string searchText)
        {
            return TDepartmentStandardSearch.SelectNewDepartmentProfiles(Organization.Identifier, Entity.Identifier, searchText);
        }

        private bool DeleteProfiles(IList<Guid> profiles)
        {
            TDepartmentStandardStore.DeleteByDepartment(Entity.Identifier, profiles);
            return true;
        }

        private int InsertProfiles(IList<Guid> profiles)
        {
            var newProfiles = new List<Guid>();

            foreach (var profileStandardIdentifier in profiles)
                if (!TDepartmentStandardSearch.Exists(x => x.DepartmentIdentifier == Entity.Identifier && x.StandardIdentifier == profileStandardIdentifier))
                    newProfiles.Add(profileStandardIdentifier);

            TDepartmentStandardStore.InsertPermissions(Entity.Identifier, profiles);

            return newProfiles.Count;
        }

        #endregion

        #region IHasParentLinkParameters & IOverrideWebRouteParent

        IWebRoute IOverrideWebRouteParent.GetParent()
            => GetParent();

        protected override string GetReturnUrl()
            => GroupSearchUrl;

        #endregion

        #region Achievement Editor

        private List<AchievementListGridItem> GetAssignedAchievements(List<AchievementListGridItem> list)
        {
            var groupId = Entity.Identifier;

            var groupAchievementIds = TAchievementDepartmentSearch
                .Bind(x => x.AchievementIdentifier, x => x.DepartmentIdentifier == groupId);

            var assignedAchievementIds = list.Select(x => x.AchievementIdentifier).ToList();

            return list
                .Where(x => assignedAchievementIds.Contains(x.AchievementIdentifier))
                .ToList();
        }

        private void DeleteAchievements(IEnumerable<Guid> achievements)
        {
            var list = TAchievementOrganizationSearch.Select(x => x.OrganizationIdentifier == Organization.Identifier && achievements.Contains(x.AchievementIdentifier));

            TAchievementOrganizationStore.Delete(list);
        }

        private int InsertAchievements(IEnumerable<Guid> achievements)
        {
            var count = 0;

            foreach (var achievementID in achievements)
            {
                if (!TAchievementOrganizationSearch.Exists(x => x.OrganizationIdentifier == Organization.Identifier && x.AchievementIdentifier == achievementID))
                {
                    TAchievementOrganizationStore.InsertOrganizationAchievement(Organization.Identifier, achievementID);
                    count++;
                }
            }

            return count;
        }

        #endregion
    }
}