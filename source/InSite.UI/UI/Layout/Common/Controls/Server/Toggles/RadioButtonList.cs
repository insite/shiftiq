using System.Web.UI;
using System.Web.UI.WebControls;

namespace InSite.Common.Web.UI
{
    [SupportsEventValidation]
    [ValidationProperty("SelectedItem")]
    public class RadioButtonList : BaseToggleList<RadioButton>
    {
        public bool DisableTranslation
        {
            get => (bool?)ViewState[nameof(DisableTranslation)] ?? false;
            set => ViewState[nameof(DisableTranslation)] = value;
        }

        private RadioButton _radioButton;

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
            if (!GetItemIndex(value, out var index) || index < 0 || index >= Items.Count)
                return null;

            var item = Items[index];
            if (!item.Enabled)
                return null;

            Page.ClientScript.ValidateEvent(key, value);

            if (index == selectedIndex)
                return null;

            SetPostDataSelection(index);

            return index;
        }

        protected override RadioButton GetToggleToRepeat()
        {
            if (_radioButton == null)
            {
                _radioButton = new RadioButton { EnableViewState = false };

                Controls.Add(_radioButton);

                _radioButton.AutoPostBack = AutoPostBack;
                _radioButton.CausesValidation = CausesValidation;
                _radioButton.ValidationGroup = ValidationGroup;
                _radioButton.DisableTranslation = DisableTranslation;
                _radioButton.GroupName = UniqueID;
            }

            return _radioButton;
        }

        protected override void SetupToggleToRepeat(RadioButton radio, ListItem item, int index)
        {
            radio.Value = item.Value;
        }
    }
}