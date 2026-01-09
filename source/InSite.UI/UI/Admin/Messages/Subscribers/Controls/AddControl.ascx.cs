using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using Humanizer;

using InSite.Admin.Contacts.People.Forms;
using InSite.Application.Contacts.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Constant;

using CreateControl = InSite.Admin.Contacts.People.Forms.Create;

namespace InSite.UI.Admin.Messages.Subscribers.Controls
{
    public partial class AddControl : BaseUserControl
    {
        #region Classes

        public class InitData
        {
            public bool IsUserCreated { get; set; }
            public string CreateUserUrl { get; set; }
            public string UploadUserUrl { get; set; }
            public string ParentUrl { get; set; }
            public bool AllowNewContact { get; set; }
            public bool AllowUploadContact { get; set; }
            public string SaveProgressHeaderText { get; set; }
            public string DefaultContactType { get; set; }
        }

        public class ContactForSave
        {
            public Guid Identifier { get; set; }
            public bool IsGroup { get; set; }
        }

        private class DataItem
        {
            public Guid Identifier { get; set; }
            public string Code { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
            public string EmailAlternate { get; set; }
            public int Size { get; set; }
        }

        #endregion

        #region Properties

        public Func<ContactForSave, bool> SaveContact;

        private string ParentUrl
        {
            get => (string)ViewState[nameof(ParentUrl)];
            set => ViewState[nameof(ParentUrl)] = value;
        }

        private List<Guid> SearchResultDataKeys
        {
            get => (List<Guid>)ViewState[nameof(SearchResultDataKeys)];
            set => ViewState[nameof(SearchResultDataKeys)] = value;
        }

        private HashSet<Guid> SearchSelectedEntities
        {
            get => (HashSet<Guid>)ViewState[nameof(SearchSelectedEntities)];
            set => ViewState[nameof(SearchSelectedEntities)] = value;
        }

        private Guid[] SavedIdentifiers
        {
            get => (Guid[])ViewState[nameof(SavedIdentifiers)];
            set => ViewState[nameof(SavedIdentifiers)] = value;
        }

        private static readonly string _defaultContactType = typeof(AddControl) + "." + nameof(DefaultContactType);
        private string DefaultContactType
        {
            get => (string)Session[_defaultContactType];
            set => Session[_defaultContactType] = value;
        }

        protected string PreviousContactType
        {
            get => (string)ViewState[nameof(PreviousContactType)];
            set => ViewState[nameof(PreviousContactType)] = value;
        }

        protected bool IsGroup
        {
            get => (bool)ViewState[nameof(IsGroup)];
            set => ViewState[nameof(IsGroup)] = value;
        }

        protected bool IsPerson
        {
            get => (bool)ViewState[nameof(IsPerson)];
            set => ViewState[nameof(IsPerson)] = value;
        }

        #endregion

        #region Fields

        private DateTime? _processStartedOn;

        #endregion

        #region Initialization and Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            CriteriaContactType.AutoPostBack = true;
            CriteriaContactType.ValueChanged += (s, a) => OnCriteriaContactTypeChanged();

            CriteriaSearchButton.Click += (s, a) => Search();
            CriteriaClearButton.Click += (s, a) => { CriteriaClear(); Search(); };

            SearchResultPagination.PageChanged += SearchResultPagination_PageChanged;

            SearchResultRepeater.DataBinding += SearchResultRepeater_DataBinding;
            SearchResultRepeater.ItemDataBound += SearchResultRepeater_ItemDataBound;

            SearchResultSaveButton.Click += SearchResultSaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                GetCriteriaResultSelections();
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            var selectedCount = SearchSelectedEntities != null ? SearchSelectedEntities.Count : 0;

            ScriptManager.RegisterStartupScript(
                this,
                typeof(CreateUserConnection),
                "set_count",
                $"$('#{SearchSelectCount.ClientID}').text('{selectedCount.ToString("n0")}');",
                true);
        }

        #endregion

        #region Event handlers

        private void OnCriteriaContactTypeChanged()
        {
            var contactType = CriteriaContactType.Value;

            CriteriaGroupTypeField.Visible = contactType == "Group";
            CriteriaEmailField.Visible = contactType == "Person";
        }

        private void SearchResultPagination_PageChanged(object sender, Pagination.PageChangedEventArgs e)
        {
            SearchResultRepeater.DataBind();
        }

        private void SearchResultRepeater_DataBinding(object sender, EventArgs e)
        {
            SearchResultDataKeys = new List<Guid>();

            SearchResultRepeater.DataSource = GetCriteriaResultData();
        }

        private void SearchResultRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var id = (Guid)DataBinder.Eval(e.Item.DataItem, "Identifier");

            var selectedCheckBox = (ICheckBoxControl)e.Item.FindControl("Selected");
            selectedCheckBox.Checked = SearchSelectedEntities.Contains(id);
            SearchResultDataKeys.Add(id);
        }

        private void SearchResultSaveButton_Click(object sender, EventArgs e)
        {
            if (SearchSelectedEntities.Count == 0)
            {
                ScreenStatus.AddMessage(AlertType.Error, "There are no selected contacts");
                return;
            }

            if (Save())
                HttpResponseHelper.Redirect(ParentUrl);
        }

        #endregion

        #region Methods (open)

        public void LoadData(InitData initData)
        {
            ParentUrl = initData.ParentUrl;

            SaveProgress.HeaderText = initData.SaveProgressHeaderText;

            if (initData.IsUserCreated && CreateControl.SavedIdentifiers.IsNotEmpty())
                LoadDataFromUserCreated();
            else
                LoadDataFromInit(initData);

            OnCriteriaContactTypeChanged();

            Search();

            SearchResultCloseButton.NavigateUrl = initData.ParentUrl;
        }

        private void LoadDataFromUserCreated()
        {
            SavedIdentifiers = CreateControl.SavedIdentifiers;
            SearchSelectedEntities = SavedIdentifiers.ToHashSet();

            CreateControl.SavedIdentifiers = null;

            CriteriaContactType.Value = "Person";
            CriteriaContactType.Enabled = false;
        }

        private void LoadDataFromInit(InitData initData)
        {
            NewUserCard.Visible = initData.AllowNewContact || initData.AllowUploadContact;

            CreateContactLink.Visible = initData.AllowNewContact;
            CreateContactLink.NavigateUrl = initData.CreateUserUrl;

            UploadContactLink.Visible = initData.AllowUploadContact;
            UploadContactLink.NavigateUrl = initData.UploadUserUrl;

            DefaultContactType = initData.DefaultContactType;

            CriteriaClear();

            if (DefaultContactType.IsNotEmpty())
            {
                var option = CriteriaContactType.FindOptionByValue(DefaultContactType);
                if (option != null)
                    option.Selected = true;
            }

            CriteriaGroupType.EnsureDataBound();
            CriteriaGroupType.Value = GroupTypes.List;
        }

        private void CriteriaClear()
        {
            if (CriteriaContactType.Enabled)
                CriteriaContactType.ClearSelection();

            CriteriaGroupType.Value = null;
            CriteriaContactName.Text = null;
            CriteriaEmail.Text = null;
            CriteriaContactCode.Text = null;

            OnCriteriaContactTypeChanged();

            SearchSelectedEntities = new HashSet<Guid>();
        }

        #endregion

        #region Methods (save)

        private bool Save()
        {
            ProgressCallback("Prepare data", 0, 1, false);

            var function = SettingsAddFunction.SelectedValue;
            var isGroup = CriteriaContactType.Value == "Group";

            var contactsForSave = PrepareForSave(SearchSelectedEntities, function, isGroup);

            return SaveContactConnections(contactsForSave);
        }

        private static List<ContactForSave> PrepareForSave(ICollection<Guid> selectedContacts, string function, bool isGroup)
        {
            var hashSet = new HashSet<Guid>();
            var list = new List<ContactForSave>();

            foreach (var contact in selectedContacts)
            {
                if (isGroup)
                {
                    if (function == "Group" || function == "GroupAndPerson")
                    {
                        if (hashSet.Add(contact))
                            list.Add(new ContactForSave { Identifier = contact, IsGroup = true });
                    }

                    if (function == "GroupAndPerson" || function == "Person")
                    {
                        var people = MembershipSearch.Bind(x => x.User.UserIdentifier, x => x.Group.GroupIdentifier == contact);
                        foreach (var person in people)
                        {
                            if (hashSet.Add(person))
                                list.Add(new ContactForSave { Identifier = person, IsGroup = false });
                        }
                    }
                }
                else
                {
                    if (hashSet.Add(contact))
                        list.Add(new ContactForSave { Identifier = contact, IsGroup = false });
                }
            }

            return list;
        }

        private bool SaveContactConnections(List<ContactForSave> contactsForSave)
        {
            if (SaveContact == null)
                throw new ArgumentNullException(nameof(SaveContact));

            for (int i = 0; i < contactsForSave.Count; i++)
            {
                if (!SaveContact(contactsForSave[i]))
                    return false;

                ProgressCallback($"Adding contact {i + 1}", i + 1, contactsForSave.Count, true);
            }

            return true;
        }

        private void ProgressCallback(string status, int currentPosition, int positionMax, bool enableTimeRemaining)
        {
            SaveProgress.UpdateContext(context =>
            {
                var progressBar = (ProgressIndicator.ContextData)context.Items["Progress"];
                progressBar.Total = positionMax;
                progressBar.Value = currentPosition;

                context.Variables["status"] = status;

                if (enableTimeRemaining)
                {
                    if (!_processStartedOn.HasValue)
                        _processStartedOn = DateTime.UtcNow;

                    context.Variables["time_remaining"] = string.Format(
                        "{0:hh}:{0:mm}:{0:ss}s",
                        Clock.TimeRemaining(positionMax, currentPosition, _processStartedOn.Value));
                }
                else
                {
                    context.Variables["time_remaining"] = "00:00:00s";
                }
            });
        }

        #endregion

        #region Methods (search results)

        private void Search()
        {
            var contactType = CriteriaContactType.Value;

            DefaultContactType = contactType;

            if (PreviousContactType != contactType)
            {
                PreviousContactType = contactType;
                SearchSelectedEntities = new HashSet<Guid>();
            }

            IsGroup = contactType == "Group";
            IsPerson = contactType == "Person";

            SearchResultHeaderGroup.Visible = IsGroup;
            SearchResultHeaderPerson.Visible = IsPerson;

            int count;

            if (IsGroup)
            {
                EntityName.Text = "Groups";

                count = ServiceLocator.GroupSearch.CountGroups(GetGroupFilter());
            }
            else if (IsPerson)
            {
                EntityName.Text = "People";

                count = ServiceLocator.PersonSearch.CountPersons(GetPersonFilter());
            }
            else
                throw ApplicationError.Create("Not expected contact type: " + contactType);

            var hasData = count > 0;

            SearchResultPagination.ItemsCount = count;
            SearchResultPagination.PageIndex = 0;
            SearchResultFooter.Visible = SearchResultPagination.PageCount > 1;
            SearchResultCount.InnerText = count.ToString("n0");

            SearchResultUpdatePanel.Visible = hasData;
            SearchResultCount.Visible = hasData;
            SearchNoResultContainer.Visible = !hasData;

            SearchResultRepeater.DataBind();
        }

        private object GetCriteriaResultData()
        {
            if (IsGroup)
            {
                var filter = GetGroupFilter();

                filter.Paging = Paging.SetSkipTake(SearchResultPagination.ItemsSkip, SearchResultPagination.ItemsTake);

                var groups = ServiceLocator.GroupSearch.SearchGroupDetails(filter);

                return groups
                    .Select(x => new DataItem
                    {
                        Identifier = x.GroupIdentifier,
                        Code = x.GroupCode,
                        Name = x.GroupName,
                        Size = x.MembershipCount,
                    })
                    .ToList()
                    .ToSearchResult();
            }

            if (IsPerson)
            {
                var filter = GetPersonFilter();
                filter.OrderBy = "User.FullName,User.Email";
                filter.Paging = Paging.SetStartEnd(SearchResultPagination.StartItem, SearchResultPagination.EndItem);

                return ServiceLocator.PersonSearch.GetPersons(filter, x => x.User).Select(x => new
                {
                    Identifier = x.UserIdentifier,
                    Code = x.PersonCode,
                    Name = x.User.FullName,
                    Email = x.User.Email,
                    EmailAlternate = x.User.EmailAlternate
                });
            }

            return null;
        }

        private void GetCriteriaResultSelections()
        {
            if (!SearchResultRepeater.Visible)
                return;

            foreach (RepeaterItem item in SearchResultRepeater.Items)
            {
                var checkBox = (ICheckBoxControl)item.FindControl("Selected");
                var userId = SearchResultDataKeys[item.ItemIndex];

                if (!checkBox.Checked && SearchSelectedEntities.Contains(userId))
                    SearchSelectedEntities.Remove(userId);
                else if (checkBox.Checked && !SearchSelectedEntities.Contains(userId))
                    SearchSelectedEntities.Add(userId);
            }
        }

        private QGroupFilter GetGroupFilter()
        {
            return new QGroupFilter
            {
                OrganizationIdentifier = Organization.Identifier,
                GroupNameLike = CriteriaContactName.Text,
                GroupCodes = GetCriteriaContactCodes(),
                GroupType = CriteriaGroupType.Value
            };
        }

        private QPersonFilter GetPersonFilter()
        {
            return new QPersonFilter
            {
                OrganizationIdentifier = Organization.OrganizationIdentifier,
                UserIdentifiers = SavedIdentifiers,
                UserNameContains = CriteriaContactName.Text,
                UserEmailContains = CriteriaEmail.Text,
                PersonCodes = GetCriteriaContactCodes()
            };
        }

        #endregion

        #region Helpers

        private string[] GetCriteriaContactCodes()
        {
            return CriteriaContactCode.Text.IsEmpty()
                ? new string[0]
                : StringHelper.Split(CriteriaContactCode.Text);
        }

        protected static string GetContactName(object dataItem)
            => string.Format("<a href=\"/ui/admin/contacts/people/edit?contact={0}\">{1}</a> <span class='form-text'>{2}</span>", (Guid)DataBinder.Eval(dataItem, "Identifier"), (string)DataBinder.Eval(dataItem, "Name"), (string)DataBinder.Eval(dataItem, "Code"));

        protected static string GetContactEmail(object dataItem)
            => string.Format("<a href='mailto:{0}'>{0}</a>", (string)DataBinder.Eval(dataItem, "Email"));

        protected static string GetContactEmailAlternate(object dataItem)
            => string.Format("{0}", (string)DataBinder.Eval(dataItem, "EmailAlternate"));

        protected static string GetContactSize(object dataItem)
            => "Person".ToQuantity((int)DataBinder.Eval(dataItem, "Size"));

        protected static string GetContactSize(object dataItem, bool isGroup)
        {
            if (isGroup)
                return "Person".ToQuantity((int)DataBinder.Eval(dataItem, "Size"));

            return null;
        }

        #endregion
    }
}