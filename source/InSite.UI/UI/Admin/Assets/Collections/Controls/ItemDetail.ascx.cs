using System;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Persistence;

using Shift.Common;

namespace InSite.Admin.Utilities.Collections.Controls
{
    public partial class ItemDetail : UserControl
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

            ItemNameUniqueValidator.ServerValidate += ItemNameUniqueValidator_ServerValidate;
        }

        private void ItemNameUniqueValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            var name = ItemName.Text;
            if (name.IsEmpty())
                return;

            var filter = new TCollectionItemFilter
            {
                CollectionIdentifier = CollectionIdentifier,
                ExcludeItemIdentifier = ItemIdentifier,
                ItemName = name
            };

            var organizationId = OrganizationIdentifier.Value;
            if (organizationId.HasValue)
                filter.OrganizationIdentifier = organizationId.Value;
            else
                filter.HasOrganization = false;

            var folder = ItemFolderTextView.IsActive ? ItemFolderText.Text : ItemFolderSelector.Value;
            if (folder.IsNotEmpty())
                filter.ItemFolder = folder;
            else
                filter.HasFolder = false;

            args.IsValid = !TCollectionItemSearch.Exists(filter);
        }

        internal void SetDefaultInputValues(Guid collectionId)
        {
            CollectionIdentifier = collectionId;
        }

        public void SetInputValues(TCollectionItem entity)
        {
            CollectionIdentifier = entity.CollectionIdentifier;
            ItemIdentifier = entity.ItemIdentifier;

            OrganizationIdentifier.Value = entity.OrganizationIdentifier;

            ItemFolderSelector.CollectionIdentifier = entity.CollectionIdentifier;
            ItemFolderSelector.RefreshData();
            ItemFolderSelector.Value = entity.ItemFolder;
            ItemFolderText.Text = null;
            ItemFolderSelectorView.IsActive = true;

            ItemName.Text = entity.ItemName;
            ItemDescription.Text = entity.ItemDescription;
            ItemIsDisabled.Checked = entity.ItemIsDisabled;

            ItemColor.EnsureDataBound();
            ItemColor.Value = entity.ItemColor;

            ItemIcon.Text = entity.ItemIcon;

            if (!string.IsNullOrEmpty(entity.ItemIcon))
                IconExample.Text = $"<i class='{entity.ItemIcon}'></i>";

            ItemHours.ValueAsDecimal = entity.ItemHours;
        }

        public void GetInputValues(TCollectionItem entity)
        {
            entity.OrganizationIdentifier = OrganizationIdentifier.Value;
            entity.ItemFolder = ItemFolderTextView.IsActive
                ? ItemFolderText.Text
                : ItemFolderSelector.Value;
            entity.ItemName = ItemName.Text;
            entity.ItemDescription = ItemDescription.Text;
            entity.ItemIsDisabled = ItemIsDisabled.Checked;
            entity.ItemColor = ItemColor.Value;
            entity.ItemIcon = ItemIcon.Text;
            entity.ItemHours = ItemHours.ValueAsDecimal;
        }
    }
}