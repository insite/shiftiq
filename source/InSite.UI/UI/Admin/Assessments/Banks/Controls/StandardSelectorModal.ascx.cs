using System;
using System.ComponentModel;
using System.Web.UI;

using InSite.Common.Web.UI;

namespace InSite.Admin.Assessments.Banks.Controls
{
    public partial class StandardSelectorModal : BaseUserControl
    {
        public EventHandler AssetSelected { get; set; }

        public string AssetType
        {
            get
            {
                return (string)ViewState[nameof(AssetType)];
            }
            set
            {
                ViewState[nameof(AssetType)] = value;
                Filter.AssetType = value;
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
                Filter.AssetSubtype = value;
            }
        }

        [Serializable]
        public class FilterSettings
        {
            public string AssetType { get; set; }
            public string AssetSubtype { get; set; }
            public bool? IsPublished { get; set; }
        }

        [PersistenceMode(PersistenceMode.Attribute), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public FilterSettings Filter => (FilterSettings)(ViewState[nameof(Filter)] ?? (ViewState[nameof(Filter)] = new FilterSettings()));

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            AssetSelectedButton.Click += OnAssetSelected;

            StyleLiteral.ContentKey = GetType().FullName;
        }

        private void OnAssetSelected(object sender, EventArgs e)
        {
            AssetSelected?.Invoke(sender, e);
        }
    }
}