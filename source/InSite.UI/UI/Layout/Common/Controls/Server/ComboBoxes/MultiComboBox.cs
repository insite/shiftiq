using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.UI;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

using static InSite.Common.Web.UI.ComboBoxMultiple;

namespace InSite.Common.Web.UI
{
    [ValidationProperty(nameof(Values))]
    public class MultiComboBox : BaseComboBox
    {
        #region Events

        public event EventHandler ValueChanged;

        private void OnValueChanged(EventArgs e)
        {
            ValueChanged?.Invoke(this, e);
        }

        #endregion

        #region Classes

        [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
        protected class ClientSideMultiSettings : ClientSideSettings
        {
            [JsonProperty(PropertyName = "multi")]
            public ClientSideMultipleSettings Multiple { get; } = new ClientSideMultipleSettings();

            public ClientSideMultiSettings(string id)
                : base(id)
            {

            }

            public bool ShouldSerializeMultiple() => !Multiple.IsEmpty;
        }

        [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
        protected class ClientSideMultipleSettings
        {
            [JsonProperty(PropertyName = "max", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public int? Max { get; internal set; }

            [JsonProperty(PropertyName = "text", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public string TextFormat { get; set; }

            [JsonProperty(PropertyName = "actions", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public bool Actions { get; set; } = false;

            [JsonProperty(PropertyName = "selectAll", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public string SelectAllText { get; internal set; }

            [JsonProperty(PropertyName = "deselectAll", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public string DeselectAllText { get; internal set; }

            [JsonProperty(PropertyName = "countText", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public string CountSingularFormat { get; internal set; }

            [JsonProperty(PropertyName = "countManyText", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public string CountPluralFormat { get; internal set; }

            [JsonProperty(PropertyName = "countAllText", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public string CountAllFormat { get; internal set; }

            [JsonProperty(PropertyName = "maxAllText", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public string MaxTotalSingularFormat { get; internal set; }

            [JsonProperty(PropertyName = "maxAllManyText", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public string MaxTotalPluralFormat { get; internal set; }

            [JsonProperty(PropertyName = "maxGroupText", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public string MaxGroupSingularFormat { get; internal set; }

            [JsonProperty(PropertyName = "maxGroupManyText", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public string MaxGroupPluralFormat { get; internal set; }

            public bool IsEmpty =>
                !Max.HasValue
                && TextFormat.IsEmpty()
                && !Actions
                && SelectAllText.IsEmpty()
                && DeselectAllText.IsEmpty()
                && CountSingularFormat.IsEmpty()
                && CountPluralFormat.IsEmpty()
                && CountAllFormat.IsEmpty()
                && MaxTotalSingularFormat.IsEmpty()
                && MaxTotalPluralFormat.IsEmpty()
                && MaxGroupSingularFormat.IsEmpty()
                && MaxGroupPluralFormat.IsEmpty();
        }

        #endregion

        #region Properties

        protected override string ControlCssClass => base.ControlCssClass + " insite-multiple";

        public virtual IEnumerable<string> Values
        {
            get
            {
                foreach (var option in this.FlattenOptions())
                    if (option.Selected)
                        yield return option.Value;
            }
            set
            {
                ClearSelection();

                _values = null;

                if (value != null)
                {
                    _values = new List<string>(value.Where(x => x.IsNotEmpty()).Distinct());
                    if (_values.Count == 0)
                        _values = null;
                }

                if (_values == null || Items.Count == 0)
                    return;

                var isFound = false;

                foreach (var v in _values)
                {
                    var item = this.FindOptionByValue(v);
                    if (item != null)
                    {
                        item.Selected = true;
                        isFound = true;
                    }
                }

                if (isFound)
                    _values = null;
            }
        }

        public IEnumerable<int> ValuesAsInt
        {
            get => Values.Select(x => int.Parse(x));
            set => Values = value == null ? null : value.Select(x => x.ToString());
        }

        public IEnumerable<Guid> ValuesAsGuid
        {
            get => Values.Select(x => Guid.Parse(x));
            set => Values = value == null ? null : value.Select(x => x.ToString());
        }

        public string[] ValuesArray => Values.ToArray();

        public int[] ValuesAsIntArray => ValuesAsInt.ToArray();

        public Guid[] ValuesAsGuidArray => ValuesAsGuid.ToArray();

        [PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ComboBoxMultiple Multiple => _multiple;

        public override bool HasValue => Values.Any();

        #endregion

        #region Fields

        private List<string> _values;
        private ComboBoxMultiple _multiple;

        #endregion

        #region Construction

        public MultiComboBox()
        {
            _multiple = new ComboBoxMultiple(nameof(Multiple), ViewState);
            _multiple.Format = FormatType.Count;
            _multiple.FormatCountMax = 1;
        }

        #endregion

        #region Loading

        protected override ClientSideSettings CreateClientSideSettings(string clientId)
        {
            var settings = new ClientSideMultiSettings(clientId);

            if (Multiple.Max > 0)
                settings.Multiple.Max = Multiple.Max;

            if (Multiple.ActionsBox)
                settings.Multiple.Actions = Multiple.ActionsBox;

            if (Multiple.Format != FormatType.Values)
            {
                string format;

                if (Multiple.Format == FormatType.Count)
                {
                    format = "count";

                    if (Multiple.FormatCountMax > 0)
                        format += $" > {Multiple.FormatCountMax}";
                }
                else if (Multiple.Format == FormatType.Static)
                {
                    format = "static";
                }
                else
                    throw ApplicationError.Create("Unexpected format type: {0}", Multiple.Format.GetName());

                settings.Multiple.TextFormat = format;
            }

            if (Multiple.SelectAllText != DefaultSelectAllText)
                settings.Multiple.SelectAllText = Multiple.SelectAllText;

            if (Multiple.DeselectAllText != DefaultDeselectAllText)
                settings.Multiple.DeselectAllText = Multiple.DeselectAllText;

            if (Multiple.CountSingularFormat != DefaultCountSingularFormat)
                settings.Multiple.CountSingularFormat = Multiple.CountSingularFormat;

            if (Multiple.CountPluralFormat != DefaultCountPluralFormat)
                settings.Multiple.CountPluralFormat = Multiple.CountPluralFormat;

            if (Multiple.CountAllFormat.IsNotEmpty())
                settings.Multiple.CountAllFormat = Multiple.CountAllFormat;

            if (Multiple.MaxTotalSingularFormat != DefaultMaxTotalSingularFormat)
                settings.Multiple.MaxTotalSingularFormat = Multiple.MaxTotalSingularFormat;

            if (Multiple.MaxTotalPluralFormat != DefaultMaxTotalPluralFormat)
                settings.Multiple.MaxTotalPluralFormat = Multiple.MaxTotalPluralFormat;

            if (Multiple.MaxGroupSingularFormat != DefaultMaxGroupSingularFormat)
                settings.Multiple.MaxGroupSingularFormat = Multiple.MaxGroupSingularFormat;

            if (Multiple.MaxGroupPluralFormat != DefaultMaxGroupPluralFormat)
                settings.Multiple.MaxGroupPluralFormat = Multiple.MaxGroupPluralFormat;

            return settings;
        }

        #endregion

        #region Methods

        public void SelectAll()
        {
            foreach (var option in this.FlattenOptions())
                option.Selected = true;
        }

        #endregion

        #region Event handlers

        protected override void OnItemAssigned(ComboBoxItem item)
        {
            if (_values != null && item is IComboBoxOption option)
            {
                var index = _values.IndexOf(option.Value);

                if (index >= 0)
                {
                    _values.RemoveAt(index);

                    if (_values.Count == 0)
                        _values = null;

                    option.Selected = true;
                }
            }
        }

        protected override void OnItemSelected(ComboBoxItem item)
        {

        }

        protected override void OnPreRender(EventArgs e)
        {
            if (Enabled)
                Page.RegisterRequiresPostBack(this);

            base.OnPreRender(e);
        }

        #endregion

        #region IPostBackDataHandler

        protected override bool LoadPostData(string value)
        {
            var oldValues = ValuesArray;
            var newValues = value.IsEmpty()
                ? new string[0]
                : value.Split(',').Where(x => x.IsNotEmpty()).Distinct().ToArray();
            var isChanged = Visible
                && (oldValues.Length != newValues.Length
                    || oldValues.Any(ov => !newValues.Any(nv => ov.Equals(nv, StringComparison.Ordinal))));

            if (isChanged)
                Values = newValues;

            return isChanged;
        }

        protected override void RaisePostDataChangedEvent()
        {
            OnValueChanged(EventArgs.Empty);
        }

        #endregion

        #region Rendering

        protected override void AddAttributesToRender(HtmlTextWriter writer)
        {
            base.AddAttributesToRender(writer);

            writer.AddAttribute(HtmlTextWriterAttribute.Multiple, "multiple");
        }

        #endregion
    }
}