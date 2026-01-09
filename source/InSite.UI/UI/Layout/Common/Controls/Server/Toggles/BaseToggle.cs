using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Web.UI;

using Shift.Common;
using Shift.Sdk.UI;

namespace InSite.Common.Web.UI
{
    [ParseChildren(true), PersistChildren(false), ControlValueProperty(nameof(Checked)), ValidationProperty(nameof(Checked))]
    public abstract class BaseToggle : Control, IPostBackDataHandler, IAttributeAccessor, IHasText, IHasToolTip, IMultilingualDisabled
    {
        #region Events

        public event EventHandler CheckedChanged;

        #endregion

        #region Properties

        public Dictionary<string, string> Attributes
            => (Dictionary<string, string>)(ViewState[nameof(Attributes)]
            ?? (ViewState[nameof(Attributes)] = new Dictionary<string, string>()));

        public bool AutoPostBack
        {
            get => (bool)(ViewState[nameof(AutoPostBack)] ?? false);
            set => ViewState[nameof(AutoPostBack)] = value;
        }

        public bool Checked
        {
            get => (bool)(ViewState[nameof(Checked)] ?? false);
            set => ViewState[nameof(Checked)] = value;
        }

        public string CssClass
        {
            get => (string)ViewState[nameof(CssClass)];
            set => ViewState[nameof(CssClass)] = value;
        }

        public bool CausesValidation
        {
            get => (bool)(ViewState[nameof(CausesValidation)] ?? false);
            set => ViewState[nameof(CausesValidation)] = value;
        }

        public bool Disabled
        {
            get => (bool)(ViewState[nameof(Disabled)] ?? false);
            set => ViewState[nameof(Disabled)] = value;
        }

        public bool DisableTranslation
        {
            get => (bool?)ViewState[nameof(DisableTranslation)] ?? false;
            set => ViewState[nameof(DisableTranslation)] = value;
        }

        public bool Enabled
        {
            get => ViewState[nameof(Enabled)] as bool? ?? true;
            set => ViewState[nameof(Enabled)] = value;
        }

        public string OnClientChange
        {
            get => ViewState[nameof(OnClientChange)] as string;
            set => ViewState[nameof(OnClientChange)] = value;
        }

        public string SubText
        {
            get => (string)ViewState[nameof(SubText)];
            set => ViewState[nameof(SubText)] = value;
        }

        [Bindable(true)]
        public string Text
        {
            get => (string)ViewState[nameof(Text)];
            set => ViewState[nameof(Text)] = value;
        }

        public string ToolTip
        {
            get => (string)ViewState[nameof(ToolTip)];
            set => ViewState[nameof(ToolTip)] = value;
        }

        public string ValidationGroup
        {
            get => (string)ViewState[nameof(ValidationGroup)];
            set => ViewState[nameof(ValidationGroup)] = value;
        }

        #endregion

        #region Fields

        private string _inputType;

        #endregion

        #region Construction

        protected BaseToggle(string inputType)
        {
            _inputType = inputType;
        }

        #endregion

        #region IPostBackDataHandler

        bool IPostBackDataHandler.LoadPostData(string postDataKey, NameValueCollection postCollection)
        {
            if (!Visible || !Enabled)
                return false;

            var isChecked = IsChecked(postDataKey, postCollection);
            if (isChecked == Checked)
                return false;

            Checked = isChecked;

            return true;
        }

        protected abstract bool IsChecked(string postDataKey, NameValueCollection postCollection);

        void IPostBackDataHandler.RaisePostDataChangedEvent()
        {
            if (AutoPostBack && !Page.IsPostBackEventControlRegistered)
            {
                Page.AutoPostBackControl = this;

                if (CausesValidation)
                    Page.Validate(ValidationGroup);
            }

            CheckedChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region PreRender

        protected override void OnPreRender(EventArgs e)
        {
            if (Enabled)
                Page.RegisterRequiresPostBack(this);

            if (!AutoPostBack)
                Page.ClientScript.RegisterForEventValidation(UniqueID);

            base.OnPreRender(e);
        }

        #endregion

        #region IAttributeAccessor

        string IAttributeAccessor.GetAttribute(string key)
        {
            return Attributes.ContainsKey(key) ? Attributes[key] : null;
        }

        void IAttributeAccessor.SetAttribute(string key, string value)
        {
            if (Attributes.ContainsKey(key))
                Attributes[key] = value;
            else
                Attributes.Add(key, value);
        }

        #endregion

        #region Rendering

        protected virtual void AddInputAttributes(HtmlTextWriter writer)
        {
            foreach (KeyValuePair<string, string> p in Attributes)
                writer.AddAttribute(p.Key, p.Value);

            writer.AddAttribute(HtmlTextWriterAttribute.Id, ClientID);
            writer.AddAttribute(HtmlTextWriterAttribute.Type, _inputType);

            string onChangeScript;

            if (AutoPostBack)
            {
                var script1 = OnClientChange;
                var script2 = GetPostBackScript();

                onChangeScript = ControlHelper.MergeScripts(script1, script2);
            }
            else
                onChangeScript = OnClientChange;

            if (onChangeScript.IsNotEmpty())
                writer.AddAttribute(HtmlTextWriterAttribute.Onchange, onChangeScript);

            if (Checked)
                writer.AddAttribute(HtmlTextWriterAttribute.Checked, "checked");

            if (!Enabled || Disabled)
                writer.AddAttribute(HtmlTextWriterAttribute.Disabled, "disabled");
        }

        protected virtual string GetPostBackScript()
        {
            var postBackOptions = new PostBackOptions(this, string.Empty) { AutoPostBack = true };

            if (CausesValidation && Page.GetValidators(ValidationGroup).Count > 0)
            {
                postBackOptions.PerformValidation = true;
                postBackOptions.ValidationGroup = ValidationGroup;
            }

            return Page.ClientScript.GetPostBackEventReference(postBackOptions, true);
        }

        protected void AddToolTipAttributes(HtmlTextWriter writer)
        {
            if (ToolTip.IsNotEmpty())
                writer.AddAttribute("title", ToolTip);
        }

        protected void RenderBlock(HtmlTextWriter writer, string name, string value, string cssClass)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Class, ControlHelper.MergeCssClasses("form-check", CssClass, cssClass));

            AddToolTipAttributes(writer);

            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            RenderInput(writer, name, value, false);
            RenderLabel(writer);

            writer.RenderEndTag();
        }

        protected void RenderInput(HtmlTextWriter writer, string name, string value, bool isStandalone)
        {
            AddInputAttributes(writer);

            var cssClass = GetInputCssClass();

            if (isStandalone)
            {
                AddToolTipAttributes(writer);

                writer.AddAttribute("aria-label", Text.IfNullOrEmpty("..."));

                cssClass = ControlHelper.MergeCssClasses(cssClass, CssClass);
            }

            if (cssClass.IsNotEmpty())
                writer.AddAttribute(HtmlTextWriterAttribute.Class, cssClass);

            writer.AddAttribute(HtmlTextWriterAttribute.Name, name);

            if (value.IsNotEmpty())
                writer.AddAttribute(HtmlTextWriterAttribute.Value, value);

            writer.RenderBeginTag(HtmlTextWriterTag.Input);
            writer.RenderEndTag();
        }

        protected void RenderLabel(HtmlTextWriter writer)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Class, GetLabelCssClass());
            writer.AddAttribute(HtmlTextWriterAttribute.For, ClientID);

            writer.RenderBeginTag(HtmlTextWriterTag.Label);

            if (Text.IsNotEmpty())
                writer.Write(Text);

            if (SubText.IsNotEmpty())
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Class, "fs-xs text-body-secondary d-block");

                writer.RenderBeginTag(HtmlTextWriterTag.Span);

                writer.Write(SubText);

                writer.RenderEndTag();
            }

            writer.RenderEndTag();
        }

        protected virtual string GetInputCssClass() => "form-check-input";

        protected virtual string GetLabelCssClass() => "form-check-label";

        #endregion
    }
}