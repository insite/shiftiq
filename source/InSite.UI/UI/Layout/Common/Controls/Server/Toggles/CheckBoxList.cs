using System;

using Shift.Common;

namespace InSite.Common.Web.UI
{
    public class CheckBoxList : BaseToggleList<CheckBox>
    {
        public bool DisableTranslation
        {
            get => (bool?)ViewState[nameof(DisableTranslation)] ?? false;
            set => ViewState[nameof(DisableTranslation)] = value;
        }

        private CheckBox _checkBox;
        private bool _isChangeEventTriggered;

        public CheckBoxList()
        {
            _checkBox = new CheckBox { EnableViewState = false };
            _checkBox.ID = "0";
            Controls.Add(_checkBox);
        }

        protected override void OnPreRender(EventArgs e)
        {
            _checkBox.AutoPostBack = AutoPostBack;
            _checkBox.CausesValidation = CausesValidation;
            _checkBox.ValidationGroup = ValidationGroup;
            _checkBox.DisableTranslation = DisableTranslation;

            for (var i = 0; i < Items.Count; i++)
            {
                _checkBox.ID = i.ToString();
                Page.RegisterRequiresPostBack(_checkBox);
            }

            base.OnPreRender(e);
        }

        private bool GetItemIndex(string value, out int index)
        {
            index = -1;

            if (!value.StartsWith(UniqueID))
                return false;

            var result = value.Substring(UniqueID.Length + 1);

            return int.TryParse(result, out index);
        }

        protected override int? LoadPostData(string key, string value, int selectedIndex)
        {
            if (!GetItemIndex(key, out var index) || index < 0 || index >= Items.Count)
                return null;

            var item = Items[index];
            if (!item.Enabled)
                return null;

            var isSelected = value != null && (item.Value.IsEmpty() || value == item.Value);
            if (isSelected == item.Selected)
                return null;

            item.Selected = isSelected;

            if (!_isChangeEventTriggered)
            {
                _isChangeEventTriggered = true;
                return index;
            }

            return null;
        }

        protected override CheckBox GetToggleToRepeat()
        {
            return _checkBox;
        }

        protected override void SetupToggleToRepeat(CheckBox toggle, System.Web.UI.WebControls.ListItem item, int index)
        {
            if (RepeatDirection == System.Web.UI.WebControls.RepeatDirection.Horizontal)
                toggle.CssClass += " d-inline-block me-3";

            toggle.Value = item.Value;
        }
    }
}