using System;
using System.Text;
using System.Web.UI;

using Shift.Common;
using Shift.Common.Events;

namespace InSite.Common.Web.UI
{
    [ParseChildren(true), PersistChildren(false)]
    public abstract class BaseControl : Control, IAttributeAccessor
    {
        #region Properties

        public AttributeCollection Attributes
        {
            get
            {
                if (_attributeCollection == null)
                {
                    if (_attributeState == null)
                        _attributeState = CreateAttributeState(IsTrackingViewState);

                    _attributeCollection = new AttributeCollection(_attributeState);
                    _attributeCollection.AttributeAdd += Attributes_AttributeSet;
                    _attributeCollection.AttributeSet += Attributes_AttributeSet;
                }

                return _attributeCollection;
            }
        }

        public bool HasAttributes => _attributeCollection != null && _attributeCollection.Count > 0
            || _attributeState.IsNotEmpty();

        public string CssClass
        {
            get { return (string)ViewState[nameof(CssClass)]; }
            set { ViewState[nameof(CssClass)] = value; }
        }

        #endregion

        #region Fields

        private AttributeCollection _attributeCollection;
        private StateBag _attributeState;

        #endregion

        #region Event handlers

        private void Attributes_AttributeSet(object sender, AttributeEventArgs e)
        {
            OnAttributeSet(e);
        }

        protected virtual void OnAttributeSet(AttributeEventArgs e)
        {

        }

        #endregion

        #region ViewState

        protected override void TrackViewState()
        {
            base.TrackViewState();

            if (_attributeState != null)
                ((IStateManager)_attributeState).TrackViewState();
        }

        protected override void LoadViewState(object savedState)
        {
            var pair = (Pair)savedState;
            if (pair == null)
                return;

            base.LoadViewState(pair.First);

            if (pair.Second == null)
                return;

            if (_attributeState == null)
                _attributeState = CreateAttributeState(true);

            ((IStateManager)_attributeState).LoadViewState(pair.Second);
        }

        protected override object SaveViewState()
        {
            var ctrlState = base.SaveViewState();
            var attrState = _attributeState != null ? ((IStateManager)_attributeState).SaveViewState() : null;

            return ctrlState != null || attrState != null ? new Pair(ctrlState, attrState) : null;
        }

        #endregion

        #region IAttributeAccessor

        string IAttributeAccessor.GetAttribute(string name)
        {
            return Attributes[name];
        }

        void IAttributeAccessor.SetAttribute(string name, string value)
        {
            Attributes[name] = value;
        }

        #endregion

        #region Rendering

        protected virtual void AddAttributesToRender(HtmlTextWriter writer)
        {
            if (_attributeState == null)
                return;

            var attr = Attributes;
            foreach (string key in attr.Keys)
                writer.AddAttribute(key, attr[key]);
        }

        #endregion

        #region Helper methods

        private static StateBag CreateAttributeState(bool trackChanges)
        {
            var result = new StateBag(true);

            if (trackChanges)
                ((IStateManager)result).TrackViewState();

            return result;
        }

        protected static void AddClientEventAttribute(HtmlTextWriter writer, HtmlTextWriterAttribute key, Action<StringBuilder> action)
        {
            var script = new StringBuilder();

            action(script);

            AddClientEventAttribute(writer, key, script.ToString());
        }

        protected static void AddClientEventAttribute(HtmlTextWriter writer, HtmlTextWriterAttribute key, string value)
        {
            if (string.IsNullOrEmpty(value))
                return;

            writer.AddAttribute(key, value);
        }

        protected static void AddClientEventAttribute(HtmlTextWriter writer, string key, Action<StringBuilder> action)
        {
            var script = new StringBuilder();

            action(script);

            AddClientEventAttribute(writer, key, script.ToString());
        }

        protected static void AddClientEventAttribute(HtmlTextWriter writer, string key, string value)
        {
            if (string.IsNullOrEmpty(value))
                return;

            writer.AddAttribute(key, value);
        }

        #endregion
    }
}