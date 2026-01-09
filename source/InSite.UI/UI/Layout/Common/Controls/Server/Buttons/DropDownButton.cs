using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Common.Web.UI
{
    [ParseChildren(true), PersistChildren(true)]
    public class DropDownButton : Control, IPostBackEventHandler, IAttributeAccessor, IHasToolTip, IHasText
    {
        #region Events

        public event CommandEventHandler Click;

        private void OnClick(string commandName, object argument) =>
            Click?.Invoke(this, new CommandEventArgs(commandName, argument));

        #endregion

        #region Properties

        public string Text
        {
            get { return (string)ViewState[nameof(Text)]; }
            set { ViewState[nameof(Text)] = value; }
        }

        public string ToolTip
        {
            get { return (string)ViewState[nameof(ToolTip)]; }
            set { ViewState[nameof(ToolTip)] = value; }
        }

        public string CssClass
        {
            get { return (string)ViewState[nameof(CssClass)]; }
            set { ViewState[nameof(CssClass)] = value; }
        }

        public string MenuCssClass
        {
            get { return (string)ViewState[nameof(MenuCssClass)]; }
            set { ViewState[nameof(MenuCssClass)] = value; }
        }

        public bool CausesValidation
        {
            get { return ViewState[nameof(CausesValidation)] == null || (bool)ViewState[nameof(CausesValidation)]; }
            set { ViewState[nameof(CausesValidation)] = value; }
        }

        public string ValidationGroup
        {
            get { return (string)ViewState[nameof(ValidationGroup)]; }
            set { ViewState[nameof(ValidationGroup)] = value; }
        }

        public DefaultButtonAction DefaultAction
        {
            get { return (DefaultButtonAction?)ViewState[nameof(DefaultAction)] ?? DefaultButtonAction.None; }
            set { ViewState[nameof(DefaultAction)] = value; }
        }

        public ButtonStyle ButtonStyle
        {
            get => (ButtonStyle)(ViewState[nameof(ButtonStyle)] ?? ButtonStyle.Default);
            set => ViewState[nameof(ButtonStyle)] = value;
        }

        public string IconName
        {
            get => (string)ViewState[nameof(IconName)];
            set => ViewState[nameof(IconName)] = value;
        }

        public IconType IconType
        {
            get => (IconType?)ViewState[nameof(IconType)] ?? IconType.Solid;
            set => ViewState[nameof(IconType)] = value;
        }

        [PersistenceMode(PersistenceMode.InnerProperty)]
        public DropDownButtonItemCollection Items
        {
            get => (DropDownButtonItemCollection)(ViewState[nameof(Items)] ??
                (ViewState[nameof(Items)] = new DropDownButtonItemCollection()));
            set => ViewState[nameof(Items)] = value;
        }

        private Dictionary<string, string> Attributes => (Dictionary<string, string>)(ViewState[nameof(Attributes)]
            ?? (ViewState[nameof(Attributes)] = new Dictionary<string, string>()));

        #endregion

        #region IPostBackEventHandler

        void IPostBackEventHandler.RaisePostBackEvent(string eventArgument)
        {
            if (!Visible)
                return;

            Page.ClientScript.ValidateEvent(UniqueID, eventArgument);

            var commandName = eventArgument;

            if (eventArgument != "default")
            {
                var index = int.Parse(eventArgument);

                var baseItem = Items[index];
                if (!baseItem.Visible || baseItem.Name.IsEmpty())
                    return;

                commandName = baseItem.Name;

                if ((baseItem is DropDownButtonItem item) && (item.CausesValidation ?? CausesValidation))
                    Page.Validate(item.ValidationGroup.IfNullOrEmpty(ValidationGroup));
            }

            OnClick(commandName, EventArgs.Empty);
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

        public override void RenderControl(HtmlTextWriter writer)
        {
            if (!Visible)
                return;

            foreach (KeyValuePair<string, string> p in Attributes)
                writer.AddAttribute(p.Key, p.Value);

            var isPostBack = DefaultAction == DefaultButtonAction.PostBack;
            var cssClass = !isPostBack ? "dropdown" : "btn-group";

            writer.AddAttribute(HtmlTextWriterAttribute.Id, ClientID);
            writer.AddAttribute(HtmlTextWriterAttribute.Class, ControlHelper.MergeCssClasses(cssClass, CssClass));
            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            if (isPostBack)
                RenderSplitButton(writer);
            else
                RenderButton(writer);

            RenderDropDown(writer);

            writer.RenderEndTag();
        }

        #endregion

        #region Rendering (button)

        private void RenderButton(HtmlTextWriter writer)
        {
            AddDropDownAttributes(writer, false);
            AddToolTipAttribute(writer, ToolTip);

            writer.AddAttribute(HtmlTextWriterAttribute.Type, "button");
            writer.RenderBeginTag(HtmlTextWriterTag.Button);

            RenderIconAndText(writer, IconName, IconType, Text);

            writer.RenderEndTag();
        }

        private void RenderSplitButton(HtmlTextWriter writer)
        {
            #region PostBack button

            AddToolTipAttribute(writer, ToolTip);
            AddOnClickAttribute(writer, "default");

            writer.AddAttribute(HtmlTextWriterAttribute.Class, GetButtonClass());
            writer.AddAttribute(HtmlTextWriterAttribute.Type, "button");
            writer.RenderBeginTag(HtmlTextWriterTag.Button);

            RenderIconAndText(writer, IconName, IconType, Text);

            writer.RenderEndTag();

            #endregion

            #region DropDown button

            AddDropDownAttributes(writer, true);

            writer.AddAttribute(HtmlTextWriterAttribute.Type, "button");
            writer.RenderBeginTag(HtmlTextWriterTag.Button);
            writer.Write("<span class='visually-hidden'>Toggle Dropdown</span>");
            writer.RenderEndTag();

            #endregion
        }

        private void RenderButtonCaret(HtmlTextWriter writer)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "caret");

            writer.RenderBeginTag(HtmlTextWriterTag.Span);
            writer.RenderEndTag();
        }

        private string GetButtonClass() => "btn " + ButtonStyle.GetContextualClass() + " btn-sm";

        private void AddDropDownAttributes(HtmlTextWriter writer, bool isSplit)
        {
            var cssClass = GetButtonClass() + " dropdown-toggle";

            if (isSplit)
                cssClass += " dropdown-toggle-split";

            writer.AddAttribute("data-bs-toggle", "dropdown");
            writer.AddAttribute("aria-expanded", "false");
            writer.AddAttribute(HtmlTextWriterAttribute.Class, cssClass);
        }

        #endregion

        #region Rendering (dropdown)

        private void RenderDropDown(HtmlTextWriter writer)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Class, ControlHelper.MergeCssClasses("dropdown-menu", MenuCssClass));
            writer.RenderBeginTag(HtmlTextWriterTag.Ul);

            for (var index = 0; index < Items.Count; index++)
            {
                var baseItem = Items[index];
                if (!baseItem.Visible)
                    continue;

                if (baseItem is DropDownButtonSeparatorItem)
                {
                    writer.RenderBeginTag(HtmlTextWriterTag.Li);

                    writer.AddAttribute(HtmlTextWriterAttribute.Class, "dropdown-divider");
                    writer.RenderBeginTag(HtmlTextWriterTag.Hr);
                    writer.RenderEndTag();

                    writer.RenderEndTag();
                }
                else if (baseItem is DropDownButtonItem item)
                {
                    writer.RenderBeginTag(HtmlTextWriterTag.Li);

                    #region a

                    AddIdAttribute(item);

                    var cssClass = "dropdown-item";

                    if (!item.Enabled)
                        cssClass += " disabled";

                    if (cssClass.IsNotEmpty())
                        writer.AddAttribute(HtmlTextWriterAttribute.Class, cssClass);

                    if ((item is DropDownButtonLinkItem linkItem) && linkItem.NavigateUrl.IsNotEmpty())
                    {
                        writer.AddAttribute(HtmlTextWriterAttribute.Href, linkItem.NavigateUrl);

                        if (linkItem.Target.IsNotEmpty())
                            writer.AddAttribute(HtmlTextWriterAttribute.Target, linkItem.Target);
                    }
                    else
                    {
                        AddOnClickAttribute(writer, index.ToString());

                        writer.AddAttribute(HtmlTextWriterAttribute.Href, "javascript:void(0);");
                    }

                    AddToolTipAttribute(writer, item.ToolTip);

                    writer.RenderBeginTag(HtmlTextWriterTag.A);

                    RenderIconAndText(writer, item.IconName, item.IconType, item.Text);

                    writer.RenderEndTag();

                    #endregion

                    writer.RenderEndTag();
                }
                else
                {
                    throw new ApplicationError("Unexpected item type: " + baseItem.GetType().FullName);
                }
            }

            writer.RenderEndTag();

            void AddIdAttribute(DropDownButtonBaseItem item)
            {
                if (item.Name.IsEmpty())
                    return;

                writer.AddAttribute(HtmlTextWriterAttribute.Id, $"{ClientID}_{item.Name}");
                writer.AddAttribute("data-id", item.Name);
            }
        }

        #endregion

        #region Rendering (common)

        private string GetJavaScript(string eventArgument, bool? causesValidation, string validationGroup)
        {
            var script = new StringBuilder();

            var postBackOptions = GetPostBackOptions(eventArgument, causesValidation, validationGroup);
            var postBackEventReference = Page.ClientScript.GetPostBackEventReference(postBackOptions, true);

            if (postBackEventReference.IsNotEmpty())
            {
                ControlHelper.AddScript(script, postBackEventReference);
                if (postBackOptions.ClientSubmit)
                    ControlHelper.AddScript(script, "return false;");
            }

            return script.ToString();
        }

        private PostBackOptions GetPostBackOptions(string argument, bool? causesValidation, string validationGroup)
        {
            var options = new PostBackOptions(this, argument)
            {
                AutoPostBack = true
            };

            if (validationGroup.IsEmpty())
                validationGroup = ValidationGroup;

            if (causesValidation ?? CausesValidation && Page.GetValidators(validationGroup).Count > 0)
            {
                options.PerformValidation = true;
                options.ValidationGroup = ValidationGroup;
            }

            return options;
        }

        private void RenderIconAndText(HtmlTextWriter writer, string iconName, IconType iconType, string text)
        {
            RenderIcon(writer, iconName, iconType);
            RenderText(writer, text);
        }

        private void RenderIcon(HtmlTextWriter writer, string name, IconType type)
        {
            if (name.IsEmpty())
                return;

            var iconClass = type.GetContextualClass() + " fa-" + name + " me-1";

            writer.AddAttribute(HtmlTextWriterAttribute.Class, iconClass);

            writer.RenderBeginTag(HtmlTextWriterTag.I);
            writer.RenderEndTag();
        }

        private static void RenderText(HtmlTextWriter writer, string text)
        {
            if (text.IsEmpty())
                return;

            var translatedText = Persistence.LabelSearch.GetTranslation(text, CookieTokenModule.Current.Language, CurrentSessionState.Identity.Organization.Identifier);

            writer.AddAttribute(HtmlTextWriterAttribute.Class, "text");

            writer.RenderBeginTag(HtmlTextWriterTag.Span);

            writer.Write(translatedText);

            writer.RenderEndTag();
        }

        private void AddOnClickAttribute(HtmlTextWriter writer, string eArgument)
        {
            var js = GetJavaScript(eArgument, CausesValidation, ValidationGroup);
            if (js.IsNotEmpty())
                writer.AddAttribute(HtmlTextWriterAttribute.Onclick, js);
        }

        private static void AddToolTipAttribute(HtmlTextWriter writer, string text)
        {
            if (text.IsNotEmpty())
                writer.AddAttribute(HtmlTextWriterAttribute.Title, text);
        }

        #endregion

        #region Helper methods

        internal void CopyFrom(DropDownButton other)
        {
            other.ShallowCopyTo(this, BindingFlags.DeclaredOnly);

            Items.Clear();
            foreach (var item in other.Items)
                Items.Add(item.Clone());
        }

        #endregion
    }
}