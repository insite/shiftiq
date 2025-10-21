using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;

using Shift.Common.Events;
using Shift.Sdk.UI;

namespace InSite.Common.Web.UI
{
    [SupportsEventValidation]
    public abstract class BaseInputBox : BaseControl, IPostBackDataHandler, IHasEmptyMessage
    {
        #region Properties

        public bool AutoPostBack
        {
            get => (bool)(ViewState[nameof(AutoPostBack)] ?? false);
            set => ViewState[nameof(AutoPostBack)] = value;
        }

        public bool Enabled
        {
            get => (bool)(ViewState[nameof(Enabled)] ?? true);
            set => ViewState[nameof(Enabled)] = value;
        }

        public Unit Width
        {
            get => (Unit)(ViewState[nameof(Width)] ?? Unit.Empty);
            set => ViewState[nameof(Width)] = value;
        }

        public Unit Height
        {
            get => (Unit)(ViewState[nameof(Height)] ?? Unit.Empty);
            set => ViewState[nameof(Height)] = value;
        }

        public bool CausesValidation
        {
            get => (bool)(ViewState[nameof(CausesValidation)] ?? false);
            set => ViewState[nameof(CausesValidation)] = value;
        }

        public string ValidationGroup
        {
            get { return (string)(ViewState[nameof(ValidationGroup)] ?? string.Empty); }
            set { ViewState[nameof(ValidationGroup)] = value; }
        }

        [PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public InputClientEvents ClientEvents => _clientEvents;

        public string EmptyMessage
        {
            get { return (string)ViewState[nameof(EmptyMessage)] ?? string.Empty; }
            set { ViewState[nameof(EmptyMessage)] = value; }
        }

        public bool ReadOnly
        {
            get => (bool)(ViewState[nameof(ReadOnly)] ?? false);
            set => ViewState[nameof(ReadOnly)] = value;
        }

        public int TabIndex
        {
            get => (int)(ViewState[nameof(TabIndex)] ?? 0);
            set => ViewState[nameof(TabIndex)] = value;
        }

        public abstract bool HasValue { get; }

        #endregion

        #region Fields

        private InputClientEvents _clientEvents;
        private static readonly HashSet<string> _redundantAttributes = new HashSet<string>(new[]
        {
            "id",
            "class",
            "type",
            "name",
            "rows",
            "cols",
            "value",
            "maxlength",
            "disabled",
            "readonly",
            "width",
            "height",
            "placeholder",

            "onblur",
            "onfocus",
            "onchange",
            "onclick",
            "onkeydown",
            "onkeypress",
            "onkeyup"
        }, StringComparer.OrdinalIgnoreCase);

        #endregion

        #region Construction

        public BaseInputBox()
        {
            _clientEvents = new InputClientEvents(nameof(ClientEvents), ViewState);
        }

        #endregion

        #region Loading

        protected override void OnAttributeSet(AttributeEventArgs e)
        {
            if (!e.Cancel)
                e.Cancel = _redundantAttributes.Contains(e.Name);
        }

        protected override void OnPreRender(EventArgs e)
        {
            if (!AutoPostBack)
                Page.ClientScript.RegisterForEventValidation(UniqueID);

            PageFooterContentRenderer.RegisterScript(
                typeof(BaseInputBox),
                "init_" + ClientID,
                $"inSite.common.baseInput.init('{ClientID}');");

            base.OnPreRender(e);
        }

        #endregion

        #region IPostBackDataHandler

        bool IPostBackDataHandler.LoadPostData(string postDataKey, NameValueCollection postCollection)
        {
            return LoadPostData(postDataKey, postCollection);
        }

        protected abstract bool LoadPostData(string postDataKey, NameValueCollection postCollection);

        void IPostBackDataHandler.RaisePostDataChangedEvent()
        {
            RaisePostDataChangedEvent();
        }

        protected abstract void RaisePostDataChangedEvent();

        #endregion

        #region Rendering

        protected void AddStyleAttributes(HtmlTextWriter writer)
        {
            var attrStyle = Attributes["style"];
            if (!string.IsNullOrEmpty(attrStyle))
                Attributes.Remove("style");

            var styles = Shift.Sdk.UI.CssStyleCollection.Parse(attrStyle);

            if (!Width.IsEmpty)
                styles["width"] = Width.ToString();

            if (!Height.IsEmpty)
                styles["height"] = Height.ToString();

            if (!styles.IsEmpty)
                writer.AddAttribute(HtmlTextWriterAttribute.Style, styles.ToString());
        }

        protected void AddClientEventAttributes(HtmlTextWriter writer)
        {
            AddClientEventAttribute(writer, "onblur", ClientEvents.OnBlur);

            AddClientEventAttribute(writer, "onchange", script =>
            {
                if (!string.IsNullOrEmpty(ClientEvents.OnChange))
                    ControlHelper.AddScript(script, ClientEvents.OnChange);

                if (!AutoPostBack)
                    return;

                var postBackOptions = GetPostBackOptions();
                var postBackEventReference = Page.ClientScript.GetPostBackEventReference(postBackOptions, true);

                if (string.IsNullOrEmpty(postBackEventReference))
                    return;

                ControlHelper.AddScript(script, postBackEventReference);
                if (postBackOptions.ClientSubmit)
                    ControlHelper.AddScript(script, "return false;");
            });

            AddClientEventAttribute(writer, "onclick", ClientEvents.OnClick);
            AddClientEventAttribute(writer, "onfocus", ClientEvents.OnFocus);
            AddClientEventAttribute(writer, "onkeydown", ClientEvents.OnKeyDown);
            AddClientEventAttribute(writer, "onkeypress", ClientEvents.OnKeyPress);
            AddClientEventAttribute(writer, "onkeyup", ClientEvents.OnKeyUp);
        }

        #endregion

        #region Helpers

        private PostBackOptions GetPostBackOptions()
        {
            var result = new PostBackOptions(this, string.Empty)
            {
                AutoPostBack = AutoPostBack
            };

            if (CausesValidation && Page.GetValidators(ValidationGroup).Count > 0)
            {
                result.PerformValidation = true;
                result.ValidationGroup = ValidationGroup;
            }

            return result;
        }

        #endregion
    }
}