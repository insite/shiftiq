using System;

using InSite.Common.Web.UI;

namespace InSite.Admin.Sites.Pages.Controls
{
    public partial class PagePopupSelector : BaseUserControl
    {
        public Guid? SiteId
        {
            get => (Guid?)ViewState[nameof(SiteId)];
            set => ViewState[nameof(SiteId)] = value;
        }

        public string FixedPageType
        {
            get => FixedTypeInput.Value;
            set => FixedTypeInput.Value = value;
        }

        public Guid? Value
        {
            get => string.IsNullOrEmpty(ValueInput.Value) ? (Guid?)null : Guid.Parse(ValueInput.Value);
            set
            {
                var isValid = value.HasValue;

                if (isValid)
                {
                    var parent = ServiceLocator.PageSearch.Select(value.Value);

                    if (isValid = parent != null)
                    {
                        TextInput.Value = $"{parent.Title}";
                        ValueInput.Value = parent.PageIdentifier.ToString();
                        TypeInput.Value = parent.PageType;
                    }
                }

                if (!isValid)
                {
                    TextInput.Value = null;
                    ValueInput.Value = null;
                    TypeInput.Value = null;
                }
            }
        }

        public bool AllowEdit
        {
            get => (bool)(ViewState[nameof(AllowEdit)] ?? true);
            set => ViewState[nameof(AllowEdit)] = value;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            StyleLiteral.ContentKey = GetType().FullName;
        }

        protected override void OnPreRender(EventArgs e)
        {
            SiteInput.Value = SiteId.ToString();
            OpenButton.Visible = AllowEdit;
            ClearButton.Visible = AllowEdit;

            base.OnPreRender(e);
        }
    }
}