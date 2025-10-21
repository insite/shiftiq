using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;
using Shift.Common.Events;
using Shift.Sdk.UI;

namespace InSite.Admin.Accounts.Organizations.Controls
{
    public partial class CollectionItemRepeater : BaseUserControl
    {
        #region Classes

        [Serializable]
        private class ItemInfo
        {
            public int Number { get; private set; }
            public int Sequence { get; set; }
            public string Name { get; set; }

            public ItemInfo(int number, string name)
            {
                Number = number;
                Name = name;
            }

            public ItemInfo(int number, string name, int sequence)
            {
                Number = number;
                Name = name;
                Sequence = sequence;
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

        private Dictionary<MultiKey<Guid, Guid>, List<ItemInfo>> CollectionItems
        {
            get => (Dictionary<MultiKey<Guid, Guid>, List<ItemInfo>>)ViewState[nameof(CollectionItems)];
            set => ViewState[nameof(CollectionItems)] = value;
        }

        public bool EnableAJAX
        {
            get => (bool)(ViewState[nameof(EnableAJAX)] ?? true);
            set => ViewState[nameof(EnableAJAX)] = value;
        }

        #endregion

        #region Fields

        private Dictionary<Guid, TCollection> _loadedCollections = null;

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            RequiredValidator.ServerValidate += RequiredValidator_ServerValidate;
            DuplicateValidator.ServerValidate += DuplicateValidator_ServerValidate;

            Repeater.ItemCommand += Repeater_ItemCommand;

            UpdatePanel.Request += UpdatePanel_Request;

            ReorderButton.Click += ReorderButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            ReadRepeaterData();

            CommandButtons.Visible = true;

            ReorderButton.OnClientClick = $"inSite.common.gridReorderHelper.startReorder('{UniqueID}'); return false;";

            base.OnLoad(e);
        }

        protected override void OnPreRender(EventArgs e)
        {
            UpdatePanel.ChildrenAsTriggers = EnableAJAX;

            ScriptManager.RegisterStartupScript(
                Page,
                typeof(CollectionItemRepeater),
                $"register_reorder_{UniqueID}",
                $@"
                inSite.common.gridReorderHelper.registerReorder({{
                    id:'{UniqueID}',
                    selector:'#{SectionControl.ClientID} div.res-cont-list > table:first',
                    items:'tbody > tr',
                    {(EnableAJAX ? $"updatePanelId:'{UpdatePanel.ClientID}'" : $"callbackControlId:'{ReorderButton.UniqueID}'")},
                }});", true);


            base.OnPreRender(e);
        }

        #endregion

        #region Event handlers

        private void ReorderButton_Click(object sender, EventArgs e)
        {
            OnReorderCommand(Request.Form["__EVENTARGUMENT"]);
        }

        private void UpdatePanel_Request(object sender, StringValueArgs e)
        {
            OnReorderCommand(e.Value);
        }


        public void OnReorderCommand(string argument)
        {
            if (string.IsNullOrEmpty(argument))
                return;

            var args = argument.Split('&');
            var command = args[0];
            var value = args.Length > 1 ? args[1] : null;

            if (command == "save-reorder")
            {
                var changes = JavaScriptHelper.GridReorder.Parse(value);
                var items = GetCollectionItems();
                var reorder = new ItemInfo[items.Count];

                for (int i = 0; i < items.Count; i++)
                    reorder[i] = items[i];

                foreach (var c in changes)
                    reorder[c.Destination.ItemIndex] = items[c.Source.ItemIndex];

                Reorder(reorder);

                BindRepeater();
            }
        }

        private void RequiredValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            var collections = CollectionItems
                .Where(x => x.Value.Any(y => string.IsNullOrEmpty(y.Name)))
                .Select(x => GetCollection(x.Key.Key2).CollectionName)
                .ToArray();

            args.IsValid = collections.Length == 0;

            if (!args.IsValid)
                RequiredValidator.ErrorMessage =
                    "Each collection item must have a name:<ul><li>" +
                    string.Join("</li><li>", collections.OrderBy(x => x).Select(x => HttpUtility.HtmlEncode(x))) +
                    "</li></ul>";
        }

        private void DuplicateValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            var collections = new List<Tuple<string, List<string>>>();

            foreach (var kv in CollectionItems)
            {
                var items = new List<string>();
                var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                foreach (var item in kv.Value)
                {
                    if (!string.IsNullOrEmpty(item.Name) && !names.Add(item.Name))
                        items.Add(item.Name);
                }

                if (items.Count > 0)
                    collections.Add(new Tuple<string, List<string>>(GetCollection(kv.Key.Key2).CollectionName, items));
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

        private void Repeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                var num = int.Parse((string)e.CommandArgument);
                var items = GetCollectionItems();

                items.RemoveAll(x => x.Number == num);

                BindRepeater();
            }
        }

        #endregion

        #region Methods (public)

        public void ClearStorage()
        {
            CollectionItems = new Dictionary<MultiKey<Guid, Guid>, List<ItemInfo>>();
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

        private List<ItemInfo> GetCollectionItems()
        {
            List<ItemInfo> items = null;

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
                    .Select(x => new ItemInfo(x.ItemNumber, x.ItemName, x.ItemSequence))
                    .ToList();

                CollectionItems.Add(CollectionId, items);
            }

            return items;
        }

        public void AddItem()
        {
            var items = GetCollectionItems();

            items.Add(new ItemInfo(NextItemNum, string.Empty));

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
                    var dbIndex = dbItems.FindIndex(x => x.ItemNumber == item.Number);
                    var isNew = dbIndex == -1;
                    var dbItem = isNew
                        ? new TCollectionItem
                        {
                            OrganizationIdentifier = organization.OrganizationIdentifier,
                            CollectionIdentifier = collectionId,
                            ItemIdentifier = UniqueIdentifier.Create()
                        }
                        : dbItems[dbIndex];

                    dbItem.ItemName = item.Name;
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
            CommandButtons.Visible = hasItems;

            Repeater.Visible = hasItems;

            Repeater.DataSource = items;
            Repeater.DataBind();
        }

        private void Reorder(ItemInfo[] toReorder)
        {
            int sequence = 0;

            foreach (var c in toReorder)
                c.Sequence = ++sequence;

            CollectionItems.Remove(CollectionId);
            CollectionItems.Add(CollectionId, toReorder.ToList());
        }

        protected override void SetupValidationGroup(string groupName)
        {
            RequiredValidator.ValidationGroup = groupName;
            DuplicateValidator.ValidationGroup = groupName;
        }

        private void ReadRepeaterData()
        {
            if (CollectionId == null || !IsPostBack)
                return;

            var items = GetCollectionItems();

            if (items.Count == Repeater.Items.Count)
            {
                foreach (RepeaterItem item in Repeater.Items)
                {
                    var itemName = (ITextBox)item.FindControl("ItemName");

                    items[item.ItemIndex].Name = itemName.Text;
                }
            }
            else
            {
                CollectionItems.Remove(CollectionId);
                BindRepeater();
            }
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