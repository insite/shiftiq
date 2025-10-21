using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;

namespace InSite.Admin.Accounts.Organizations.Controls
{
    public partial class CollectionFolderRepeater : BaseUserControl
    {
        #region Classes

        [Serializable]
        private class FolderInfo
        {
            public string Name { get; set; }

            public List<ItemInfo> Items { get; } = new List<ItemInfo>();

            public FolderInfo()
                : this(string.Empty)
            {

            }

            public FolderInfo(string name)
            {
                Name = name;
            }
        }

        [Serializable]
        private class ItemInfo
        {
            public int Number { get; private set; }
            public string Name { get; set; }

            public ItemInfo(int num, string name)
            {
                Number = num;
                Name = name;
            }
        }

        private class ValidationInfo
        {
            public string Name { get; }
            public bool IsValid { get; set; }
            public List<ValidationInfo> Children { get; }

            public ValidationInfo(string name)
            {
                Name = name;
                IsValid = true;
                Children = new List<ValidationInfo>();
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

        private Dictionary<MultiKey<Guid, Guid>, List<FolderInfo>> CollectionFolders
        {
            get => (Dictionary<MultiKey<Guid, Guid>, List<FolderInfo>>)ViewState[nameof(CollectionFolders)];
            set => ViewState[nameof(CollectionFolders)] = value;
        }

        #endregion

        #region Fields

        private Dictionary<Guid, TCollection> _loadedCollections = null;

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            CommonStyle.ContentKey = CommonScript.ContentKey = typeof(CollectionFolderRepeater).FullName;

            FolderNameRequiredValidator.ServerValidate += FolderNameRequiredValidator_ServerValidate;
            FolderNameDuplicateValidator.ServerValidate += FolderNameDuplicateValidator_ServerValidate;
            FolderItemRequiredValidator.ServerValidate += FolderItemRequiredValidator_ServerValidate;
            ItemNameRequiredValidator.ServerValidate += ItemNameRequiredValidator_ServerValidate;
            ItemNameDuplicateValidator.ServerValidate += ItemNameDuplicateValidator_ServerValidate;

            FolderRepeater.ItemCreated += FolderRepeater_ItemCreated;
            FolderRepeater.ItemDataBound += FolderRepeater_ItemDataBound;
            FolderRepeater.ItemCommand += FolderRepeater_ItemCommand;
        }

        protected override void OnLoad(EventArgs e)
        {
            ReadRepeaterData();

            base.OnLoad(e);
        }

        protected override void OnPreRender(EventArgs e)
        {
            ScriptManager.RegisterStartupScript(
                Page, GetType(), "init", $"collectionFolderRepeater.init('{ClientID}','{ReorderData.ClientID}');", true);

            base.OnPreRender(e);
        }

        #endregion

        #region Event handlers

        private void FolderRepeater_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var itemRepeater = (Repeater)e.Item.FindControl("ItemRepeater");
            itemRepeater.ItemCommand += ItemRepeater_ItemCommand;
        }

        private void FolderRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            BindItemRepeater(e.Item, (FolderInfo)e.Item.DataItem);
        }

        private void FolderRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "RemoveFolder")
            {
                var folders = GetCollectionFolders();

                folders.RemoveAt(e.Item.ItemIndex);

                BindFolderRepeater();
            }
            else if (e.CommandName == "AddItem")
            {
                var folders = GetCollectionFolders();
                var folder = folders[e.Item.ItemIndex];

                folder.Items.Add(new ItemInfo(NextItemNum, string.Empty));

                BindItemRepeater(e.Item, folder);
            }
        }

        private void ItemRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "RemoveItem")
            {
                var folderItem = (RepeaterItem)e.Item.NamingContainer.NamingContainer;
                var number = int.Parse((string)e.CommandArgument);
                var folders = GetCollectionFolders();
                var folder = folders[folderItem.ItemIndex];

                folder.Items.RemoveAll(x => x.Number == number);

                BindItemRepeater(folderItem, folder);
            }
        }

        private void FolderNameRequiredValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            var collections = ValidateCollections(x => x.All(y => !string.IsNullOrEmpty(y.Name)), null, null);

            args.IsValid = collections.Length == 0;

            if (!args.IsValid)
                FolderNameRequiredValidator.ErrorMessage =
                    "Each collection folder must have a name:<ul><li>" +
                    string.Join("</li><li>", collections.OrderBy(x => x).Select(x => HttpUtility.HtmlEncode(x.Name))) +
                    "</li></ul>";
        }

        private void FolderNameDuplicateValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            HashSet<string> names = null;

            var collections = ValidateCollections(
                x => { names = new HashSet<string>(StringComparer.OrdinalIgnoreCase); return true; },
                x => string.IsNullOrEmpty(x.Name) || names.Add(x.Name),
                null);

            args.IsValid = collections.Length == 0;

            if (!args.IsValid)
                FolderNameDuplicateValidator.ErrorMessage =
                    "Each collection folder must have unique name within its collection:" + ToHtmlList(collections);
        }

        private void FolderItemRequiredValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            var collections = ValidateCollections(null, x => string.IsNullOrEmpty(x.Name) || x.Items.Count > 0, null);

            args.IsValid = collections.Length == 0;

            if (!args.IsValid)
                FolderItemRequiredValidator.ErrorMessage =
                    "Each сollection folders must contain at least one item:" + ToHtmlList(collections);
        }

        private void ItemNameRequiredValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            var collections = ValidateCollections(null, x => x.Items.All(y => !string.IsNullOrEmpty(x.Name)), null);

            args.IsValid = collections.Length == 0;

            if (!args.IsValid)
                FolderItemRequiredValidator.ErrorMessage =
                    "Each folder item must have a name" + ToHtmlList(collections);
        }

        private void ItemNameDuplicateValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            HashSet<string> names = null;

            var collections = ValidateCollections(
                null,
                x => { names = new HashSet<string>(StringComparer.OrdinalIgnoreCase); return true; },
                x => string.IsNullOrEmpty(x.Name) || names.Add(x.Name));

            args.IsValid = collections.Length == 0;

            if (!args.IsValid)
                FolderNameDuplicateValidator.ErrorMessage =
                    "Each folder item must have unique name within its folder" + ToHtmlList(collections);
        }

        #endregion

        #region Methods (public)

        public void ClearStorage()
        {
            CollectionFolders = new Dictionary<MultiKey<Guid, Guid>, List<FolderInfo>>();
        }

        public void UnloadCollection()
        {
            CollectionId = null;
            BindFolderRepeater();
        }

        public void LoadCollection(Guid organizationId, Guid collectionId)
        {
            if (CollectionFolders == null)
                ClearStorage();

            CollectionId = new MultiKey<Guid, Guid>(organizationId, collectionId);

            BindFolderRepeater();
        }

        public void AddFolder()
        {
            var items = GetCollectionFolders();

            items.Add(new FolderInfo());

            BindFolderRepeater();
        }

        public void SaveCollections()
        {
            foreach (var kv in CollectionFolders)
            {
                var organizationId = kv.Key.Key1;
                var collectionId = kv.Key.Key2;
                var folders = kv.Value;

                var organization = OrganizationSearch.Select(organizationId);
                var dbItems = TCollectionItemSearch.Select(new TCollectionItemFilter
                {
                    CollectionIdentifier = collectionId,
                    OrganizationIdentifier = organization.Identifier
                });

                var itemSequence = 1;

                foreach (var folder in folders)
                {
                    foreach (var item in folder.Items)
                    {
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

                        dbItem.ItemFolder = folder.Name;
                        dbItem.ItemName = item.Name;
                        dbItem.ItemSequence = itemSequence++;

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
                }

                if (dbItems.Count > 0)
                    TCollectionItemStore.Delete(dbItems);
            }

            ClearStorage();
            BindFolderRepeater();
        }

        #endregion

        #region Methods (other)

        private List<FolderInfo> GetCollectionFolders()
        {
            List<FolderInfo> folders = null;

            if (CollectionId != null && !CollectionFolders.TryGetValue(CollectionId, out folders))
            {
                var organizationId = CollectionId.Key1;
                var collectionId = CollectionId.Key2;

                folders = new List<FolderInfo>();

                var folderIndex = new Dictionary<string, FolderInfo>(StringComparer.OrdinalIgnoreCase);
                var dbItems = TCollectionItemSearch.Select(new TCollectionItemFilter
                {
                    CollectionIdentifier = collectionId,
                    OrganizationIdentifier = organizationId
                });

                foreach (var dbItem in dbItems.OrderBy(x => x.ItemSequence))
                {
                    var folderName = dbItem.ItemFolder;
                    if (string.IsNullOrEmpty(folderName))
                        folderName = "Common";

                    if (!folderIndex.TryGetValue(folderName, out var folder))
                    {
                        folder = new FolderInfo(folderName);
                        folderIndex.Add(folderName, folder);
                        folders.Add(folder);
                    }

                    folder.Items.Add(new ItemInfo(dbItem.ItemNumber, dbItem.ItemName));
                }

                CollectionFolders.Add(CollectionId, folders);
            }

            return folders;
        }

        private void BindFolderRepeater()
        {
            var folders = GetCollectionFolders();
            var hasFolders = folders.IsNotEmpty();

            NoFoldersMessage.Visible = !hasFolders;
            FolderRepeater.Visible = hasFolders;

            FolderRepeater.DataSource = folders;
            FolderRepeater.DataBind();
        }

        private void BindItemRepeater(RepeaterItem item, FolderInfo folder)
        {
            var itemRepeater = (Repeater)item.FindControl("ItemRepeater");
            itemRepeater.DataSource = folder.Items;
            itemRepeater.DataBind();
        }

        protected override void SetupValidationGroup(string groupName)
        {
            FolderNameRequiredValidator.ValidationGroup = groupName;
            FolderNameDuplicateValidator.ValidationGroup = groupName;
            FolderItemRequiredValidator.ValidationGroup = groupName;
            ItemNameRequiredValidator.ValidationGroup = groupName;
            ItemNameDuplicateValidator.ValidationGroup = groupName;
        }

        private void ReadRepeaterData()
        {
            if (CollectionId == null || !IsPostBack)
                return;

            var reload = false;
            var rebind = false;

            var folders = GetCollectionFolders();

            if (FolderRepeater.Items.Count == folders.Count)
            {
                foreach (RepeaterItem folderItem in FolderRepeater.Items)
                {
                    var folderInfo = folders[folderItem.ItemIndex];

                    var folderName = (ITextBox)folderItem.FindControl("FolderName");
                    folderInfo.Name = folderName.Text;

                    var itemRepeater = (Repeater)folderItem.FindControl("ItemRepeater");
                    if (itemRepeater.Items.Count == folderInfo.Items.Count)
                    {
                        foreach (RepeaterItem itemItem in itemRepeater.Items)
                        {
                            var itemInfo = folderInfo.Items[itemItem.ItemIndex];

                            var itemName = (ITextBox)itemItem.FindControl("ItemName");
                            itemInfo.Name = itemName.Text;
                        }
                    }
                    else
                    {
                        reload = true;
                        break;
                    }
                }
            }
            else
            {
                reload = true;
            }

            if (!string.IsNullOrEmpty(ReorderData.Value))
            {
                if (!reload)
                    rebind = ReorderFolders(folders, ReorderData.Value);

                ReorderData.Value = string.Empty;
            }

            if (reload)
                CollectionFolders.Remove(CollectionId);

            if (reload || rebind)
                BindFolderRepeater();
        }

        private static bool ReorderFolders(List<FolderInfo> folders, string inputValue)
        {
            var hasChanges = false;

            if (!string.IsNullOrEmpty(inputValue))
            {
                var foldersData = inputValue.Split(';');
                if (foldersData.Length != folders.Count)
                    throw new ApplicationError("Invalid collections count: " + inputValue);

                foreach (var folderData in foldersData)
                {
                    var folderDataParts = folderData.Split(':');
                    if (folderDataParts.Length != 2)
                        throw new ApplicationError("Invalid collection group: " + inputValue);

                    var folderIndex = int.Parse(folderDataParts[0]);
                    var itemNums = string.IsNullOrEmpty(folderDataParts[1]) ? new int[0] : folderDataParts[1].Split(',').Select(x => int.Parse(x)).ToArray();

                    var folderInfo = folders[folderIndex];
                    if (folderInfo.Items.Count != itemNums.Length)
                        throw new ApplicationError("Invalid collection tags count: " + inputValue);

                    var itemMapping = folderInfo.Items.Select((x, i) => new Tuple<int, ItemInfo>(i, x)).ToDictionary(x => x.Item2.Number);

                    folderInfo.Items.Clear();

                    foreach (var num in itemNums)
                    {
                        var itemMapInfo = itemMapping[num];

                        if (itemMapInfo.Item1 != folderInfo.Items.Count)
                            hasChanges = true;

                        folderInfo.Items.Add(itemMapInfo.Item2);
                    }
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

        private ValidationInfo[] ValidateCollections(Func<IEnumerable<FolderInfo>, bool> isCollectionValid, Func<FolderInfo, bool> isFolderValid, Func<ItemInfo, bool> isItemValid)
        {
            var result = new List<ValidationInfo>();

            foreach (var kv in CollectionFolders)
            {
                var vCollection = new ValidationInfo(GetCollection(kv.Key.Key2).CollectionName)
                {
                    IsValid = isCollectionValid == null || isCollectionValid(kv.Value)
                };

                if (vCollection.IsValid && (isFolderValid != null || isItemValid != null))
                {
                    foreach (var folder in kv.Value)
                    {
                        var vFolder = new ValidationInfo(folder.Name)
                        {
                            IsValid = isFolderValid == null || isFolderValid(folder)
                        };

                        if (vFolder.IsValid && isItemValid != null)
                        {
                            foreach (var item in folder.Items)
                            {
                                if (!isItemValid(item))
                                    vFolder.Children.Add(new ValidationInfo(item.Name));
                            }
                        }

                        if (!vFolder.IsValid || vFolder.Children.Count > 0)
                            vCollection.Children.Add(vFolder);
                    }
                }

                if (!vCollection.IsValid || vCollection.Children.Count > 0)
                    result.Add(vCollection);
            }

            return result.OrderBy(x => x.Name).ToArray();
        }

        private static string ToHtmlList(IEnumerable<ValidationInfo> items)
        {
            var builder = new StringBuilder();

            builder.Append("<ul>");

            foreach (var item in items)
                ToHtmlList(builder, item, 1);

            builder.Append("</ul>");

            return builder.ToString();
        }

        private static void ToHtmlList(StringBuilder builder, ValidationInfo item, int level)
        {
            builder.Append("<li>");

            var itemHtml = HttpUtility.HtmlEncode(item.Name);

            if (item.Children.Count > 0)
            {
                if (level == 1)
                    builder.AppendFormat("<strong>{0}</strong>", itemHtml);
                else if (level == 2)
                    builder.AppendFormat("<i>{0}</i>", itemHtml);
                else
                    builder.Append(itemHtml);

                builder.Append("<ul>");

                foreach (var child in item.Children)
                    ToHtmlList(builder, child, level + 1);

                builder.Append("</ul>");
            }
            else
            {
                builder.Append(itemHtml);
            }

            builder.Append("</li>");
        }

        #endregion
    }
}