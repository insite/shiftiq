using System;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;

namespace InSite.UI.Admin.Learning.Categories.Controls
{
    public partial class CategoryDetail : BaseUserControl
    {
        private Guid CollectionIdentifier
        {
            get => (Guid)ViewState[nameof(CollectionIdentifier)];
            set => ViewState[nameof(CollectionIdentifier)] = value;
        }

        private Guid? ItemIdentifier
        {
            get => (Guid?)ViewState[nameof(ItemIdentifier)];
            set => ViewState[nameof(ItemIdentifier)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            CategoryNameUniqueValidator.ServerValidate += CategoryNameUniqueValidator_ServerValidate;
        }

        private void CategoryNameUniqueValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            var name = CategoryName.Text;
            if (name.IsEmpty())
                return;

            var folder = GetFolderName();
            if (folder.IsEmpty())
                return;

            var filter = new TCollectionItemFilter
            {
                OrganizationIdentifier = Organization.Identifier,
                CollectionIdentifier = CollectionIdentifier,
                ExcludeItemIdentifier = ItemIdentifier,
                ItemFolder = folder,
                ItemName = name
            };

            args.IsValid = !TCollectionItemSearch.Exists(filter);
        }

        internal void SetDefaultInputValues(Guid collectionId)
        {
            CollectionIdentifier = collectionId;

            SetCategoryFolder(null);
        }

        public void SetInputValues(TCollectionItem entity)
        {
            CollectionIdentifier = entity.CollectionIdentifier;

            ItemIdentifier = entity.ItemIdentifier;

            SetCategoryFolder(entity.ItemFolder);

            CategoryName.Text = entity.ItemName;

            DescriptionInput.Text = entity.ItemDescription;
        }

        private void SetCategoryFolder(string value)
        {
            CategoryFolderSelector.OrganizationIdentifier = Organization.Identifier;
            CategoryFolderSelector.CollectionIdentifier = CollectionIdentifier;
            CategoryFolderSelector.RefreshData();
            CategoryFolderSelector.Value = value;
            CategoryFolderText.Text = null;
            CategoryFolderSelectorView.IsActive = true;
        }

        public void GetInputValues(TCollectionItem entity)
        {
            entity.OrganizationIdentifier = Organization.Identifier;
            entity.ItemFolder = CategoryFolderTextView.IsActive
                ? CategoryFolderText.Text
                : CategoryFolderSelector.Value;
            entity.ItemName = CategoryName.Text;
            entity.ItemDescription = DescriptionInput.Text;
        }

        private string GetFolderName()
        {
            return CategoryFolderTextView.IsActive ? CategoryFolderText.Text : CategoryFolderSelector.Value;
        }
    }
}