using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;

namespace InSite.Admin.Accounts.Organizations.Controls
{
    public partial class CollectionPersonRepeater : BaseUserControl
    {
        #region Classes

        [Serializable]
        private class PersonInfo
        {
            public int ItemNumber { get; private set; }
            public Guid? UserIdentifier { get; set; }

            public PersonInfo(int itemNum, Guid? user)
            {
                ItemNumber = itemNum;
                UserIdentifier = user;
            }
        }

        #endregion

        #region Properties

        private int NextItemNum
        {
            get
            {
                var value = (int)(ViewState[nameof(NextItemNum)] ?? -1);

                ViewState[nameof(NextItemNum)] = value - 1;

                return value;
            }
        }

        private MultiKey<Guid, Guid> CollectionId
        {
            get => (MultiKey<Guid, Guid>)ViewState[nameof(CollectionId)];
            set => ViewState[nameof(CollectionId)] = value;
        }

        private Dictionary<MultiKey<Guid, Guid>, List<PersonInfo>> CollectionItems
        {
            get => (Dictionary<MultiKey<Guid, Guid>, List<PersonInfo>>)ViewState[nameof(CollectionItems)];
            set => ViewState[nameof(CollectionItems)] = value;
        }

        #endregion

        #region Fields

        private Dictionary<Guid, TCollection> _loadedCollections = null;

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            CommonStyle.ContentKey = CommonScript.ContentKey = typeof(CollectionPersonRepeater).FullName;

            RequiredValidator.ServerValidate += RequiredValidator_ServerValidate;
            DuplicateValidator.ServerValidate += DuplicateValidator_ServerValidate;

            Repeater.ItemCommand += Repeater_ItemCommand;
            Repeater.ItemDataBound += Repeater_ItemDataBound;
        }

        protected override void OnLoad(EventArgs e)
        {
            ReadRepeaterData();

            base.OnLoad(e);
        }

        protected override void OnPreRender(EventArgs e)
        {
            ScriptManager.RegisterStartupScript(
                Page, GetType(), "init", $"collectionPersonRepeater.init('{ClientID}','{ReorderData.ClientID}');", true);

            base.OnPreRender(e);
        }

        #endregion

        #region Event handlers

        private void RequiredValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            var collections = CollectionItems
                .Where(x => x.Value.Any(y => !y.UserIdentifier.HasValue))
                .Select(x => GetCollection(x.Key.Key2).CollectionName)
                .ToArray();

            args.IsValid = collections.Length == 0;

            if (!args.IsValid)
                RequiredValidator.ErrorMessage =
                    "Each combobox in contacts collection must have a selected value:<ul><li>" +
                    string.Join("</li><li>", collections.OrderBy(x => x).Select(x => HttpUtility.HtmlEncode(x))) +
                    "</li></ul>";
        }

        private void DuplicateValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            var collections = new List<Tuple<string, string[]>>();

            foreach (var kv in CollectionItems)
            {
                var items = new List<Guid>();
                var ids = new HashSet<Guid>();

                foreach (var item in kv.Value)
                {
                    if (item.UserIdentifier.HasValue && !ids.Add(item.UserIdentifier.Value))
                        items.Add(item.UserIdentifier.Value);
                }

                if (items.Count > 0)
                {
                    var names = UserSearch.Bind(x => x.FullName, new UserFilter { IncludeUserIdentifiers = items.ToArray() });
                    collections.Add(new Tuple<string, string[]>(GetCollection(kv.Key.Key2).CollectionName, names));
                }
            }

            args.IsValid = collections.Count == 0;

            if (!args.IsValid)
                DuplicateValidator.ErrorMessage = "Each item must have unique name within its collection:<ul><li>" +
                    string.Join("</li><li>", collections.OrderBy(x => x.Item1).Select(x =>
                    {
                        var items = string.Join("</li><li>", x.Item2.OrderBy(y => y).Distinct(StringComparer.OrdinalIgnoreCase).Select(y => HttpUtility.HtmlEncode(y)));
                        return $"<strong>{HttpUtility.HtmlEncode(x.Item1)}</strong>:<ul><li>{items}</li></ul>";
                    })) +
                    "</li></ul>";
        }

        private void Repeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var personInfo = (PersonInfo)e.Item.DataItem;
            var personId = (FindPerson)e.Item.FindControl("PersonID");

            personId.Value = personInfo.UserIdentifier;
        }

        private void Repeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                var num = int.Parse((string)e.CommandArgument);
                var items = GetCollectionItems();

                items.RemoveAll(x => x.ItemNumber == num);

                BindRepeater();
            }
        }

        #endregion

        #region Methods (public)

        public void ClearStorage()
        {
            CollectionItems = new Dictionary<MultiKey<Guid, Guid>, List<PersonInfo>>();
        }

        public void UnloadCollection()
        {
            CollectionId = null;
            BindRepeater();
        }

        public void LoadCollection(Guid organizationId, Guid collectionId)
        {
            if (CollectionItems == null)
                ClearStorage();

            CollectionId = new MultiKey<Guid, Guid>(organizationId, collectionId);

            BindRepeater();
        }

        private List<PersonInfo> GetCollectionItems()
        {
            List<PersonInfo> items = null;

            if (CollectionId != null && !CollectionItems.TryGetValue(CollectionId, out items))
            {
                var organizationId = CollectionId.Key1;
                var collectionId = CollectionId.Key2;

                items = TCollectionItemSearch
                    .Select(new TCollectionItemFilter
                    {
                        CollectionIdentifier = collectionId,
                        OrganizationIdentifier = organizationId
                    })
                    .OrderBy(x => x.ItemSequence)
                    .Select(x => new PersonInfo(x.ItemNumber, Guid.TryParse(x.ItemName, out var personId) ? personId : (Guid?)null))
                    .Where(x => x.UserIdentifier.HasValue)
                    .ToList();

                CollectionItems.Add(CollectionId, items);
            }

            return items;
        }

        public void AddPerson()
        {
            var items = GetCollectionItems();

            items.Add(new PersonInfo(NextItemNum, null));

            BindRepeater();
        }

        public void SaveCollections()
        {
            foreach (var kv in CollectionItems)
            {
                var organizationId = kv.Key.Key1;
                var collectionId = kv.Key.Key2;
                var items = kv.Value;

                var organization = OrganizationSearch.Select(organizationId);
                var dbItems = TCollectionItemSearch.Select(new TCollectionItemFilter
                {
                    CollectionIdentifier = collectionId,
                    OrganizationIdentifier = organization.Identifier
                });

                for (var i = 0; i < items.Count; i++)
                {
                    var item = items[i];
                    var dbIndex = dbItems.FindIndex(x => x.ItemNumber == item.ItemNumber);
                    var isNew = dbIndex == -1;
                    var dbItem = isNew
                        ? new TCollectionItem
                        {
                            OrganizationIdentifier = organization.OrganizationIdentifier,
                            CollectionIdentifier = collectionId,
                            ItemIdentifier = UniqueIdentifier.Create()
                        }
                        : dbItems[dbIndex];

                    dbItem.ItemName = item.UserIdentifier.Value.ToString();
                    dbItem.ItemSequence = i + 1;

                    if (!isNew)
                    {
                        TCollectionItemStore.Update(dbItem);

                        dbItems.RemoveAt(dbIndex);
                    }
                    else
                    {
                        TCollectionItemStore.Insert(dbItem);
                    }
                }

                if (dbItems.Count > 0)
                    TCollectionItemStore.Delete(dbItems);
            }

            ClearStorage();
        }

        #endregion

        #region Methods (other)

        private void BindRepeater()
        {
            var items = GetCollectionItems();
            var hasItems = items.IsNotEmpty();

            NoItemsMessage.Visible = !hasItems;
            Repeater.Visible = hasItems;

            Repeater.DataSource = items;
            Repeater.DataBind();
        }

        protected override void SetupValidationGroup(string groupName)
        {
            RequiredValidator.ValidationGroup = ValidationGroup;
            DuplicateValidator.ValidationGroup = ValidationGroup;
        }

        private void ReadRepeaterData()
        {
            if (CollectionId == null || !IsPostBack)
                return;

            var reload = false;
            var rebind = false;

            var items = GetCollectionItems();

            if (items.Count == Repeater.Items.Count)
            {
                foreach (RepeaterItem item in Repeater.Items)
                {
                    var personId = (FindPerson)item.FindControl("PersonID");

                    items[item.ItemIndex].UserIdentifier = personId.Value;
                }
            }
            else
            {
                reload = true;
            }

            if (!string.IsNullOrEmpty(ReorderData.Value))
            {
                if (!reload)
                    rebind = ReorderPersons(items, ReorderData.Value);

                ReorderData.Value = string.Empty;
            }

            if (reload)
                CollectionItems.Remove(CollectionId);

            if (reload || rebind)
                BindRepeater();
        }

        private static bool ReorderPersons(List<PersonInfo> persons, string inputValue)
        {
            var hasChanges = false;

            if (!string.IsNullOrEmpty(inputValue))
            {
                var itemsOrder = inputValue.Split(',')
                    .Select(x => int.Parse(x))
                    .Where(x => x >= 0 && x < persons.Count)
                    .Distinct()
                    .ToArray();

                if (persons.Count != itemsOrder.Length)
                    throw new ApplicationError("Invalid collection tags count: " + inputValue);

                var itemMapping = new Dictionary<int, PersonInfo>();
                for (var i = 0; i < persons.Count; i++)
                    itemMapping.Add(i, persons[i]);

                persons.Clear();

                foreach (var index in itemsOrder)
                {
                    var info = itemMapping[index];

                    if (index != persons.Count)
                        hasChanges = true;

                    persons.Add(info);
                }
            }

            return hasChanges;
        }

        private TCollection GetCollection(Guid collectionId)
        {
            if (_loadedCollections == null)
                _loadedCollections = new Dictionary<Guid, TCollection>();

            if (!_loadedCollections.TryGetValue(collectionId, out var result))
                _loadedCollections.Add(collectionId, result = TCollectionSearch.Select(collectionId));

            return result;
        }

        #endregion
    }
}