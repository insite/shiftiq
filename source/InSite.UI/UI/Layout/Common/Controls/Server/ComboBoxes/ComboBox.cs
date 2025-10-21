using System;
using System.Web.UI;

using Shift.Common;
using Shift.Sdk.UI;

namespace InSite.Common.Web.UI
{
    [ValidationProperty(nameof(Value))]
    public class ComboBox : BaseComboBox
    {
        #region Events

        public event ComboBoxValueChangedEventHandler ValueChanged;

        private void OnValueChanged(ComboBoxValueChangedEventArgs e)
        {
            ValueChanged?.Invoke(this, e);
        }

        #endregion

        #region Properties

        public virtual string Value
        {
            get
            {
                var option = GetSelectedOption();

                return option != null ? option.Value : string.Empty;
            }
            set
            {
                ClearSelection();

                _value = value.NullIfEmpty();

                if (_value == null || Items.Count == 0)
                    return;

                var option = this.FindOptionByValue(_value, StringComparison.InvariantCultureIgnoreCase);
                if (option != null)
                {
                    option.Selected = true;
                    _value = null;
                }
            }
        }

        public int? ValueAsInt
        {
            get
            {
                var value = Value;
                return value.IsEmpty() ? (int?)null : int.Parse(value);
            }
            set => Value = !value.HasValue ? null : value.Value.ToString();
        }

        public Guid? ValueAsGuid
        {
            get
            {
                var value = Value;
                return value.IsEmpty() ? (Guid?)null : Guid.Parse(value);
            }
            set => Value = !value.HasValue ? null : value.Value.ToString();
        }

        public bool? ValueAsBoolean
        {
            get
            {
                var value = Value;
                return value.IsEmpty() ? (bool?)null : bool.Parse(value);
            }
            set => Value = !value.HasValue ? null : value.Value ? bool.TrueString : bool.FalseString;
        }

        public virtual bool AllowBlank
        {
            get => (bool)(ViewState[nameof(AllowBlank)] ?? true);
            set => ViewState[nameof(AllowBlank)] = value;
        }

        public override bool HasValue => Value.IsNotEmpty();

        #endregion

        #region Fields

        private string _value;
        private ComboBoxValueChangedEventArgs _changedArgs;

        #endregion

        #region Methods

        internal IComboBoxOption GetSelectedOption()
        {
            IComboBoxOption defaultOption = null;

            foreach (var option in this.FlattenOptions())
            {
                if (option.Selected)
                    return option;
                else if (defaultOption == null && option.AllowSelect)
                    defaultOption = option;
            }

            if (defaultOption != null)
            {
                defaultOption.Selected = true;
                return defaultOption;
            }

            return null;
        }

        protected virtual ComboBoxOption GetEmptyOption() => new ComboBoxOption();

        #endregion

        #region Event handlers

        protected override void OnItemAssigned(ComboBoxItem item)
        {
            if (_value != null && item is IComboBoxOption option && option.Value == _value)
            {
                _value = null;
                option.Selected = true;
            }
        }

        protected override void OnItemSelected(ComboBoxItem item)
        {
            if (item is IComboBoxOption option && option.Selected)
            {
                foreach (var o in this.FlattenOptions())
                    if (o != option)
                        o.Selected = false;
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            // Ensure the combo box has selection
            GetSelectedOption();

            base.OnPreRender(e);
        }

        protected override void OnPostLoadItems()
        {
            base.OnPostLoadItems();

            if (AllowBlank && Items.Count > 0)
                Items.Insert(0, GetEmptyOption());
        }

        #endregion

        #region IPostBackDataHandler

        protected override bool LoadPostData(string value)
        {
            if (!Visible)
                return false;

            var oldValue = Value;

            Value = value;

            var newValue = Value;

            var isChanged = oldValue != Value;

            if (isChanged)
                _changedArgs = new ComboBoxValueChangedEventArgs(newValue, oldValue);

            return isChanged;
        }

        protected override void RaisePostDataChangedEvent()
        {
            OnValueChanged(_changedArgs ?? new ComboBoxValueChangedEventArgs(Value, null));

            _changedArgs = null;
        }

        #endregion
    }
}