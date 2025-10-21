using System;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Persistence;

namespace InSite.Admin.Utilities.Collections.Controls
{
    public partial class Detail : UserControl
    {
        #region Properties

        private Guid? CollectionIdentifier
        {
            get => (Guid?)ViewState[nameof(CollectionIdentifier)];
            set => ViewState[nameof(CollectionIdentifier)] = value;
        }

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            CollectionNameUniqueValidator.ServerValidate += CollectionNameUniqueValidator_ServerValidate;
        }

        #endregion

        #region Event handlers

        private void CollectionNameUniqueValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = !TCollectionSearch.Exists(new TCollectionFilter
            {
                ExcludeCollectionIdentifier = CollectionIdentifier,
                CollectionName = CollectionName.Text
            });
        }

        #endregion

        public void SetDefaultInputValues()
        {

        }

        public void SetInputValues(TCollection entity)
        {
            CollectionIdentifier = entity.CollectionIdentifier;

            CollectionName.Text = entity.CollectionName;
            CollectionReferences.Text = entity.CollectionReferences;

            CollectionToolSelector.RefreshData();
            CollectionToolSelector.Value = entity.CollectionTool;
            CollectionToolText.Text = string.Empty;
            CollectionToolSelectorView.IsActive = true;

            CollectionPackageSelector.RefreshData();
            CollectionPackageSelector.Value = entity.CollectionPackage;
            CollectionPackageText.Text = string.Empty;
            CollectionPackageSelectorView.IsActive = true;

            CollectionProcessSelector.RefreshData();
            CollectionProcessSelector.Value = entity.CollectionProcess;
            CollectionProcessText.Text = string.Empty;
            CollectionProcessSelectorView.IsActive = true;

            CollectionTypeSelector.RefreshData();
            CollectionTypeSelector.Value = entity.CollectionType;
            CollectionTypeText.Text = string.Empty;
            CollectionTypeSelectorView.IsActive = true;
        }

        public void GetInputValues(TCollection entity)
        {
            entity.CollectionName = CollectionName.Text;
            entity.CollectionReferences = CollectionReferences.Text;
            entity.CollectionTool = CollectionToolTextView.IsActive
                ? CollectionToolText.Text
                : CollectionToolSelector.Value;
            entity.CollectionPackage = CollectionPackageTextView.IsActive
                ? CollectionPackageText.Text
                : CollectionPackageSelector.Value;
            entity.CollectionProcess = CollectionProcessTextView.IsActive
                ? CollectionProcessText.Text
                : CollectionProcessSelector.Value;
            entity.CollectionType = CollectionTypeTextView.IsActive
                ? CollectionTypeText.Text
                : CollectionTypeSelector.Value;
        }
    }
}