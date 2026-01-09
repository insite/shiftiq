using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Web.UI;

using Shift.Common;
using Shift.Sdk.UI;

namespace InSite.Common.Web.UI
{
    [Serializable, ParseChildren(true, "Items")]
    public sealed class ComboBoxOptionGroup : ComboBoxItem, IComboBoxItemContainer
    {
        #region Properties

        public override string Text { get; set; }

        public bool Enabled { get; set; } = true;

        [PersistenceMode(PersistenceMode.InnerDefaultProperty)]
        public Collection<ComboBoxOption> Items => _items;

        [PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ComboBoxGroupMultiple Multiple => _multiple;

        #endregion

        #region Fields

        private Collection<ComboBoxOption> _items = new Collection<ComboBoxOption>();
        private ComboBoxGroupMultiple _multiple = new ComboBoxGroupMultiple();

        #endregion

        #region Constrution

        public ComboBoxOptionGroup()
            : base()
        {

        }

        public ComboBoxOptionGroup(string text)
            : this()
        {
            Text = text;
        }

        #endregion

        #region ComboBoxItem

        protected override bool SetOwner(BaseComboBox comboBox, bool isTrackingViewState)
        {
            var isAssigned = base.SetOwner(comboBox, isTrackingViewState);

            if (isAssigned)
            {
                for (var i = 0; i < _items.Count; i++)
                    ((IComboBoxItem)_items[i]).SetOwner(comboBox, isTrackingViewState);

                _items = new ComboBoxItemCollection<ComboBoxOption>(comboBox, isTrackingViewState, _items);
            }

            return isAssigned;
        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (Text.IsNotEmpty())
                writer.AddAttribute("label", Text);

            if (!Enabled)
                writer.AddAttribute(HtmlTextWriterAttribute.Disabled, "disabled");

            if (ComboBox is MultiComboBox)
            {
                if (Multiple.Max > 0)
                    writer.AddAttribute("data-max-options", Multiple.Max.ToString());
            }

            writer.RenderBeginTag("optgroup");

            foreach (IComboBoxItem item in Items)
                item.Render(writer);

            writer.RenderEndTag();
        }

        protected override void LoadState(IStateReader reader)
        {
            base.LoadState(reader);

            reader.Get<string>(x => Text = x);
            reader.Get<bool>(x => Enabled = x);
            reader.Get<int>(x => Multiple.Max = x);
        }

        protected override void SaveState(IStateWriter writer)
        {
            base.SaveState(writer);

            writer.Add(Text);
            writer.Add(Enabled);
            writer.Add(Multiple.Max);
        }

        #endregion

        #region IStateManager

        protected override object SaveViewState()
        {
            var state1 = base.SaveViewState();
            var state2 = ((IStateManager)Items).SaveViewState();

            return state1 != null || state2 != null
                ? new Pair(state1, state2)
                : null;
        }

        protected override void LoadViewState(object state)
        {
            if (state != null)
            {
                var pair = (Pair)state;
                state = pair.First;
                ((IStateManager)Items).LoadViewState(pair.Second);
            }

            base.LoadViewState(state);
        }

        protected override void TrackViewState()
        {
            base.TrackViewState();

            ((IStateManager)_items).TrackViewState();
        }

        #endregion

        #region IComboBoxItemContainer

        IEnumerable<ComboBoxItem> IComboBoxItemContainer.Items => _items;

        #endregion
    }
}