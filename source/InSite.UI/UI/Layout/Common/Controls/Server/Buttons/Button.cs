using System;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Shift.Common;
using Shift.Constant;

namespace InSite.Common.Web.UI
{
    public class Button : Control, IPostBackEventHandler, IAttributeAccessor, IButton
    {
        #region Events

        private static readonly object EventClick = new object();

        private static readonly object EventCommand = new object();

        public event EventHandler Click
        {
            add => Events.AddHandler(EventClick, value);
            remove => Events.RemoveHandler(EventClick, value);
        }

        public event CommandEventHandler Command
        {
            add => Events.AddHandler(EventCommand, value);
            remove => Events.RemoveHandler(EventCommand, value);
        }

        private void OnClick() =>
            ((EventHandler)Events[EventClick])?.Invoke(this, EventArgs.Empty);

        private void OnCommand(string name, string argument)
        {
            var args = new CommandEventArgs(name, argument);
            ((EventHandler)Events[EventClick])?.Invoke(this, args);
            RaiseBubbleEvent(this, args);
        }

        #endregion

        #region Properties

        public ButtonStyle ButtonStyle
        {
            get => (ButtonStyle)(ViewState[nameof(ButtonStyle)] ?? ButtonStyle.Primary);
            set => ViewState[nameof(ButtonStyle)] = value;
        }

        public ButtonSize Size
        {
            get => (ButtonSize)(ViewState[nameof(Size)] ?? ButtonSize.Small);
            set => ViewState[nameof(Size)] = value;
        }

        public string CssClass
        {
            get => (string)ViewState[nameof(CssClass)];
            set => ViewState[nameof(CssClass)] = value;
        }

        public string Icon
        {
            get => (string)ViewState[nameof(Icon)];
            set => ViewState[nameof(Icon)] = value;
        }

        public IconPositionType IconPosition
        {
            get => (IconPositionType)(ViewState[nameof(IconPosition)] ?? IconPositionType.BeforeText);
            set => ViewState[nameof(IconPosition)] = value;
        }

        public string Text
        {
            get => (string)ViewState[nameof(Text)];
            set => ViewState[nameof(Text)] = value;
        }

        public string OnClientClick
        {
            get => (string)ViewState[nameof(OnClientClick)];
            set => ViewState[nameof(OnClientClick)] = value;
        }

        public string ConfirmText
        {
            get => (string)ViewState[nameof(ConfirmText)];
            set => ViewState[nameof(ConfirmText)] = value;
        }

        public bool DisableAfterClick
        {
            get => (bool)(ViewState[nameof(DisableAfterClick)] ?? false);
            set => ViewState[nameof(DisableAfterClick)] = value;
        }

        public int? EnableAfter // in milliseconds
        {
            get => (int?)ViewState[nameof(EnableAfter)];
            set => ViewState[nameof(EnableAfter)] = value;
        }

        public virtual bool CausesValidation
        {
            get => (bool)(ViewState[nameof(CausesValidation)] ?? true);
            set => ViewState[nameof(CausesValidation)] = value;
        }

        public string ValidationGroup
        {
            get => (string)ViewState[nameof(ValidationGroup)];
            set => ViewState[nameof(ValidationGroup)] = value;
        }

        public string NavigateUrl
        {
            get => (string)ViewState[nameof(NavigateUrl)];
            set => ViewState[nameof(NavigateUrl)] = value;
        }

        public string NavigateTarget
        {
            get => (string)ViewState[nameof(NavigateTarget)];
            set => ViewState[nameof(NavigateTarget)] = value;
        }

        public bool PostBackEnabled
        {
            get => (bool)(ViewState[nameof(PostBackEnabled)] ?? true);
            set => ViewState[nameof(PostBackEnabled)] = value;
        }

        public string CommandName
        {
            get => (string)ViewState[nameof(CommandName)];
            set => ViewState[nameof(CommandName)] = value;
        }

        public string CommandArgument
        {
            get => (string)ViewState[nameof(CommandArgument)];
            set => ViewState[nameof(CommandArgument)] = value;
        }

        public bool Enabled
        {
            get => (bool?)ViewState[nameof(Enabled)] ?? true;
            set => ViewState[nameof(Enabled)] = value;
        }

        public string ToolTip
        {
            get => (string)ViewState[nameof(ToolTip)];
            set => ViewState[nameof(ToolTip)] = value;
        }

        public string GroupName
        {
            get => (string)ViewState[nameof(GroupName)];
            set => ViewState[nameof(GroupName)] = value?.Trim().NullIfEmpty();
        }

        string IButtonControl.PostBackUrl
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        #endregion

        #region Properties (attributes)

        public System.Web.UI.AttributeCollection Attributes
        {
            get
            {
                if (_attributes == null)
                {
                    if (_attributesState == null)
                    {
                        _attributesState = new StateBag(ignoreCase: true);
                        if (IsTrackingViewState)
                            ((IStateManager)_attributesState).TrackViewState();
                    }

                    _attributes = new System.Web.UI.AttributeCollection(_attributesState);
                }

                return _attributes;
            }
        }

        public System.Web.UI.CssStyleCollection Style => Attributes.CssStyle;

        public bool HasAttributes => _attributes != null && _attributes.Count > 0
            || _attributesState.IsNotEmpty();

        #endregion

        #region Fields

        private System.Web.UI.AttributeCollection _attributes;
        private StateBag _attributesState;

        #endregion

        #region Loading

        protected override void LoadViewState(object savedState)
        {
            if (savedState == null)
                return;

            var pair = (Pair)savedState;

            base.LoadViewState(pair.First);

            if (pair.Second != null)
            {
                IStateManager attrStateManager;

                if (_attributesState == null)
                {
                    attrStateManager = _attributesState = new StateBag(ignoreCase: true);
                    attrStateManager.TrackViewState();
                }
                else
                {
                    attrStateManager = _attributesState;
                }

                attrStateManager.LoadViewState(pair.Second);
            }
        }

        protected override object SaveViewState()
        {
            var state1 = base.SaveViewState();
            var state2 = _attributesState != null
                ? ((IStateManager)_attributesState).SaveViewState()
                : null;

            return state1 == null && state2 == null
                ? null
                : new Pair(state1, state2);
        }

        protected override void TrackViewState()
        {
            base.TrackViewState();

            if (_attributesState != null)
                ((IStateManager)_attributesState).TrackViewState();
        }

        #endregion

        #region IPostBackEventHandler

        void IPostBackEventHandler.RaisePostBackEvent(string eventArgument)
        {
            if (!Visible || !Enabled)
                return;

            Page.ClientScript.ValidateEvent(UniqueID, eventArgument);

            if (CausesValidation)
                Page.Validate(ValidationGroup);

            OnClick();

            if (CommandName.IsNotEmpty())
                OnCommand(CommandName, CommandArgument);

            if (DisableAfterClick && GroupName != null)
                ScriptManager.RegisterStartupScript(
                    Page,
                    typeof(Button),
                    "enable_group." + ClientID,
                    $"inSite.common.enableButtonGroup({HttpUtility.JavaScriptStringEncode(GroupName, true)});",
                    true);
        }

        #endregion

        #region Rendering

        public override void RenderControl(HtmlTextWriter writer)
        {
            if (!Visible)
                return;

            var hasText = Text.IsNotEmpty();
            var hasIcon = Icon.IsNotEmpty();

            var cssClass = ControlHelper.MergeCssClasses(
                "btn",
                Size.GetContextualClass(),
                ButtonStyle.GetContextualClass(),
                Enabled ? null : "disabled",
                CssClass,
                !hasText && hasIcon ? "btn-icon" : null
            );

            writer.AddAttribute(HtmlTextWriterAttribute.Id, ClientID);
            writer.AddAttribute(HtmlTextWriterAttribute.Name, UniqueID);
            writer.AddAttribute(HtmlTextWriterAttribute.Class, cssClass);

            if (ToolTip.IsNotEmpty())
                writer.AddAttribute(HtmlTextWriterAttribute.Title, ToolTip);

            if (GroupName.IsNotEmpty())
                writer.AddAttribute("data-btn-group", GroupName);

            if (_attributesState != null)
                foreach (string key in Attributes.Keys)
                    writer.AddAttribute(key, Attributes[key]);

            AttachJavaScript(writer);

            writer.RenderBeginTag(HtmlTextWriterTag.A);

            if (hasText)
            {
                if (hasIcon && IconPosition == IconPositionType.BeforeText)
                    RenderIcon(writer, "me-2");

                writer.Write(Text);

                if (hasIcon && IconPosition == IconPositionType.AfterText)
                    RenderIcon(writer, "ms-2");
            }
            else if (hasIcon)
            {
                RenderIcon(writer, null);
            }

            writer.RenderEndTag();
        }

        private void RenderIcon(HtmlTextWriter writer, string cssClass)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Class, ControlHelper.MergeCssClasses(Icon, cssClass));

            writer.RenderBeginTag(HtmlTextWriterTag.I);
            writer.RenderEndTag();
        }

        private void AttachJavaScript(HtmlTextWriter writer)
        {
            var script = new StringBuilder();

            if (OnClientClick.IsNotEmpty())
                ControlHelper.AddScript(script, OnClientClick);

            if (ConfirmText.IsNotEmpty())
                ControlHelper.AddScript(script, string.Format("if (!confirm({0})) {{ return false; }}", HttpUtility.JavaScriptStringEncode(ConfirmText, true)));

            var postBackOptions = new PostBackOptions(this, string.Empty)
            {
                RequiresJavaScriptProtocol = true
            };

            if (CausesValidation && Page.GetValidators(ValidationGroup).Count > 0)
            {
                postBackOptions.PerformValidation = true;
                postBackOptions.ValidationGroup = ValidationGroup;
            }

            if (DisableAfterClick)
            {
                var validationGroup = postBackOptions.PerformValidation
                    ? HttpUtility.JavaScriptStringEncode(postBackOptions.ValidationGroup.EmptyIfNull(), true)
                    : "null";
                var groupName = GroupName != null
                    ? HttpUtility.JavaScriptStringEncode(GroupName, true)
                    : "null";
                var enableAfter = EnableAfter.HasValue ? EnableAfter.Value : 0;
                var disableScript = $"if (!inSite.common.disableButton(this,{validationGroup},{groupName},{enableAfter})) return false;";

                ControlHelper.AddScript(script, disableScript);
            }

            if (NavigateUrl.IsNotEmpty())
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Href, NavigateUrl);

                if (NavigateTarget.IsNotEmpty())
                    writer.AddAttribute(HtmlTextWriterAttribute.Target, NavigateTarget);

                if (postBackOptions.PerformValidation)
                    ControlHelper.AddScript(script, $"Page_ClientValidate('{postBackOptions.ValidationGroup}'); if (!Page_IsValid) return false;");
            }
            else if (PostBackEnabled)
            {
                var href = Page.ClientScript.GetPostBackEventReference(postBackOptions, true);
                if (href.IsNotEmpty())
                    writer.AddAttribute(HtmlTextWriterAttribute.Href, href);
            }

            if (script.Length > 0)
                writer.AddAttribute(HtmlTextWriterAttribute.Onclick, script.ToString());
        }

        #endregion

        #region IAttributeAccessor

        string IAttributeAccessor.GetAttribute(string key)
        {
            return (string)_attributesState?[key];
        }

        void IAttributeAccessor.SetAttribute(string key, string value)
        {
            Attributes[key] = value;
        }

        #endregion
    }

    public sealed class AddButton : Button
    {
        public AddButton()
        {
            ButtonStyle = ButtonStyle.Default;
            Icon = "fas fa-plus-circle";
            Text = "Add";
        }
    }

    public sealed class CancelButton : Button
    {
        public override bool CausesValidation
        {
            get => (bool)(ViewState[nameof(CausesValidation)] ?? false);
            set => ViewState[nameof(CausesValidation)] = value;
        }

        public CancelButton()
        {
            Text = "Cancel";
            Icon = "fas fa-ban";
            ButtonStyle = ButtonStyle.Default;
        }
    }

    public sealed class CloseButton : Button
    {
        public override bool CausesValidation
        {
            get => (bool)(ViewState[nameof(CausesValidation)] ?? false);
            set => ViewState[nameof(CausesValidation)] = value;
        }

        public CloseButton()
        {
            Text = "Close";
            Icon = "fas fa-ban";
            ButtonStyle = ButtonStyle.Default;
        }
    }

    public sealed class ClearButton : Button
    {
        public override bool CausesValidation
        {
            get => (bool)(ViewState[nameof(CausesValidation)] ?? false);
            set => ViewState[nameof(CausesValidation)] = value;
        }

        public ClearButton()
        {
            Text = "Clear";
            Icon = "fas fa-times";
            ButtonStyle = ButtonStyle.Primary;
        }
    }

    public sealed class ResetButton : Button
    {
        public override bool CausesValidation
        {
            get => (bool)(ViewState[nameof(CausesValidation)] ?? false);
            set => ViewState[nameof(CausesValidation)] = value;
        }

        public ResetButton()
        {
            Text = "Reset";
            Icon = "fas fa-undo";
            ButtonStyle = ButtonStyle.Primary;
        }
    }

    public sealed class FilterButton : Button
    {
        public override bool CausesValidation
        {
            get => (bool)(ViewState[nameof(CausesValidation)] ?? false);
            set => ViewState[nameof(CausesValidation)] = value;
        }

        public FilterButton()
        {
            Text = "Search";
            Icon = "fas fa-search";
            ButtonStyle = ButtonStyle.Primary;
        }
    }

    public sealed class SearchButton : Button
    {
        public override bool CausesValidation
        {
            get => (bool)(ViewState[nameof(CausesValidation)] ?? false);
            set => ViewState[nameof(CausesValidation)] = value;
        }

        public SearchButton()
        {
            Text = "Search";
            Icon = "fas fa-search";
            ButtonStyle = ButtonStyle.Primary;
        }
    }

    public sealed class DownloadButton : Button
    {
        public DownloadButton()
        {
            Text = "Download";
            Icon = "fas fa-download";
            ButtonStyle = ButtonStyle.Default;
        }
    }

    public sealed class UploadButton : Button
    {
        public UploadButton()
        {
            Text = "Upload";
            Icon = "fas fa-upload";
            ButtonStyle = ButtonStyle.Default;
        }
    }

    public sealed class NextButton : Button
    {
        public NextButton()
        {
            Text = "Next";
            ButtonStyle = ButtonStyle.Primary;
            Icon = "fas fa-arrow-alt-right";
            IconPosition = IconPositionType.AfterText;
        }
    }

    public sealed class SaveButton : Button
    {
        public SaveButton()
        {
            Text = "Save";
            Icon = "fas fa-save";
            ButtonStyle = ButtonStyle.Success;
        }
    }

    public sealed class DeleteButton : Button
    {
        public DeleteButton()
        {
            Text = "Delete";
            Icon = "fas fa-trash-alt";
            ButtonStyle = ButtonStyle.Danger;
        }
    }

    public sealed class ArchiveButton : Button
    {
        public ArchiveButton()
        {
            Text = "Archive";
            Icon = "fas fa-archive";
            ButtonStyle = ButtonStyle.Danger;
        }
    }

    public sealed class UnarchiveButton : Button
    {
        public UnarchiveButton()
        {
            Text = "Unarchive";
            Icon = "fas fa-archive";
            ButtonStyle = ButtonStyle.Success;
        }
    }
}