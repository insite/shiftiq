using System;
using System.Web.UI;

using InSite.Persistence;

using Shift.Constant;

namespace InSite.Admin.Assessments.Banks.Controls
{
    public partial class StandardSelector : UserControl
    {
        public event EventHandler AssetSelected;

        public string AssetType
        {
            get
            {
                return (string)ViewState[nameof(AssetType)];
            }
            set
            {
                ViewState[nameof(AssetType)] = value;
                StandardSelectorModal.AssetType = value;
            }
        }

        public string AssetSubtype
        {
            get
            {
                return (string)ViewState[nameof(AssetSubtype)];
            }
            set
            {
                ViewState[nameof(AssetSubtype)] = value;
                StandardSelectorModal.AssetSubtype = value;
            }
        }

        public Guid? Value
        {
            get => string.IsNullOrEmpty(ValueInput.Value) ? (Guid?)null : Guid.Parse(ValueInput.Value);
            set
            {
                var isValid = value.HasValue;

                if (isValid)
                {
                    var parent = StandardSearch.Bind(value.Value, x => new
                    {
                        x.StandardIdentifier,
                        x.AssetNumber,
                        x.Code,
                        Title = CoreFunctions.GetContentTextEn(x.StandardIdentifier, ContentLabel.Title) ?? "end"
                    });

                    if (isValid = parent != null)
                    {
                        TextInput.Value = $"{parent.Code}. {parent.Title}";
                        ValueInput.Value = parent.StandardIdentifier.ToString();
                    }
                }

                if (!isValid)
                {
                    TextInput.Value = null;
                    ValueInput.Value = null;
                }
            }
        }

        public bool AllowEdit
        {
            get => OpenButton.Visible;
            set => OpenButton.Visible = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            StandardSelectorModal.AssetSelected += OnAssetSelected;
        }

        private void OnAssetSelected(object sender, EventArgs e)
        {
            AssetSelected?.Invoke(sender, e);
        }
    }
}