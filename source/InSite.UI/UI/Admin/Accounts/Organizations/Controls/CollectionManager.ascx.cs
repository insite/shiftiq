using System;

using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Constant;

namespace InSite.UI.Admin.Accounts.Organizations.Controls
{
    public partial class CollectionManager : BaseUserControl
    {
        protected Guid? OrganizationId
        {
            get => (Guid?)ViewState[nameof(OrganizationId)];
            set => ViewState[nameof(OrganizationId)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            CollectionSelector.AutoPostBack = true;
            CollectionSelector.ValueChanged += CollectionSelector_ValueChanged;

            AddItemButton.Click += AddItemButton_Click;
            AddFolderButton.Click += AddFolderButton_Click;
            AddPersonButton.Click += AddPersonButton_Click;

            base.OnInit(e);
        }

        public void LoadData(Guid organizationId)
        {
            OrganizationId = organizationId;

            ItemRepeater.ClearStorage();
            FolderRepeater.ClearStorage();
            PersonRepeater.ClearStorage();
        }

        public void SaveData()
        {
            ItemRepeater.SaveCollections();
            FolderRepeater.SaveCollections();
            PersonRepeater.SaveCollections();

            TCollectionItemCache.Refresh();
        }

        #region Event handlers

        private void CollectionSelector_ValueChanged(object sender, EventArgs e) => OnCollectionChanged();

        private void OnCollectionChanged()
        {
            ItemsContainer.Visible = false;
            FoldersContainer.Visible = false;
            PersonsContainer.Visible = false;

            ItemRepeater.UnloadCollection();
            FolderRepeater.UnloadCollection();
            PersonRepeater.UnloadCollection();

            if (!CollectionSelector.Value.HasValue)
                return;

            var collection = TCollectionSearch.Select(CollectionSelector.Value.Value)
                ?? throw new ArgumentException($"Collection with id = {CollectionSelector.Value} is not found");
            var collectionName = collection.CollectionName;

            var isFolder = collectionName == CollectionName.Standards_Organizations_Classification_Flag
                || collectionName == CollectionName.Standards_Document_Content
                || collectionName == CollectionName.Standards_Standards_Category_Name;
            if (isFolder)
            {
                FoldersContainer.Visible = true;
                FolderRepeater.LoadCollection(OrganizationId.Value, collection.CollectionIdentifier);
                return;
            }

            var isPerson = collectionName == CollectionName.Messages_Communications_Office;
            if (isPerson)
            {
                PersonsContainer.Visible = true;
                PersonRepeater.LoadCollection(OrganizationId.Value, collection.CollectionIdentifier);
                return;
            }

            ItemsContainer.Visible = true;
            ItemRepeater.LoadCollection(OrganizationId.Value, collection.CollectionIdentifier);
        }

        private void AddItemButton_Click(object sender, EventArgs e) => ItemRepeater.AddItem();

        private void AddFolderButton_Click(object sender, EventArgs e) => FolderRepeater.AddFolder();

        private void AddPersonButton_Click(object sender, EventArgs e) => PersonRepeater.AddPerson();

        #endregion
    }
}