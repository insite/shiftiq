using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Common.Events;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Common.Web.UI
{
    [ParseChildren(true), PersistChildren(true)]
    public abstract class BaseComboBox : BaseControl, IPostBackDataHandler, IComboBoxItemOwner, IComboBoxItemContainer, IHasEmptyMessage
    {
        #region Constants

        public const string DefaultSearchEmptyFormat = "No results matched {0}";

        #endregion

        #region Enums

        protected enum BindingType
        {
            Code,
            Database
        }

        #endregion

        #region Classes

        [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
        protected class ClientSideSettings
        {
            [JsonProperty(PropertyName = "id")]
            public string ControlID { get; set; }

            [JsonProperty(PropertyName = "noSelect", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public string EmptyText { get; set; }

            [JsonProperty(PropertyName = "noSearch", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public string SearchEmptyText { get; set; }

            [JsonProperty(PropertyName = "search", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public bool EnableSearch { get; set; }

            [DefaultValue(false), JsonProperty(PropertyName = "showContent", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public bool ShowOptionHtml { get; set; }

            [JsonProperty(PropertyName = "style", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public string ButtonStyle { get; set; }

            [JsonProperty(PropertyName = "width", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public string Width { get; set; }

            [JsonProperty(PropertyName = "menu")]
            public ClientSideDropDownSettings DropDown { get; } = new ClientSideDropDownSettings();

            [JsonProperty(PropertyName = "callback")]
            public ClientSideCallbackSettings Callback { get; } = new ClientSideCallbackSettings();

            public ClientSideSettings(string id)
            {
                ControlID = id;
            }

            public bool ShouldSerializeDropDown() => !DropDown.IsEmpty;

            public bool ShouldSerializeCallback() => !Callback.IsEmpty;
        }

        [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
        protected class ClientSideDropDownSettings
        {
            [JsonProperty(PropertyName = "header", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public string Header { get; set; }

            [JsonProperty(PropertyName = "size", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public int? Size { get; set; }

            [JsonProperty(PropertyName = "container", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public string Container { get; set; }

            [JsonProperty(PropertyName = "dropDir", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public string Direction { get; set; }

            [JsonProperty(PropertyName = "width", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public string Width { get; set; }

            public bool IsEmpty =>
                Header.IsEmpty()
                && !Size.HasValue
                && Container.IsEmpty()
                && Direction.IsEmpty()
                && Width.IsEmpty();
        }

        [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
        protected class ClientSideCallbackSettings
        {
            [JsonProperty(PropertyName = "postBack", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public string PostBack { get; internal set; }

            [JsonProperty(PropertyName = "onChange", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public string OnChange { get; internal set; }

            public bool IsEmpty =>
                PostBack.IsEmpty()
                && OnChange.IsEmpty();
        }

        #endregion

        #region Properties

        protected virtual string ControlCssClass => "insite-combobox";

        public bool Enabled
        {
            get => (bool)(ViewState[nameof(Enabled)] ?? true);
            set => ViewState[nameof(Enabled)] = value;
        }

        public bool EnableSearch
        {
            get => (bool)(ViewState[nameof(EnableSearch)] ?? false);
            set => ViewState[nameof(EnableSearch)] = value;
        }

        public string EmptyMessage
        {
            get => (string)ViewState[nameof(EmptyMessage)] ?? string.Empty;
            set => ViewState[nameof(EmptyMessage)] = value;
        }

        public string SearchEmptyMessage
        {
            get => (string)ViewState[nameof(SearchEmptyMessage)] ?? DefaultSearchEmptyFormat;
            set => ViewState[nameof(SearchEmptyMessage)] = value;
        }

        public bool ShowOptionHtml
        {
            get => (bool)(ViewState[nameof(ShowOptionHtml)] ?? false);
            set => ViewState[nameof(ShowOptionHtml)] = value;
        }

        public ButtonSize ButtonSize
        {
            get => (ButtonSize)(ViewState[nameof(ButtonSize)] ?? ButtonSize.Default);
            set => ViewState[nameof(ButtonSize)] = value;
        }

        public ButtonStyle ButtonStyle
        {
            get => (ButtonStyle)(ViewState[nameof(ButtonStyle)] ?? ButtonStyle.None);
            set => ViewState[nameof(ButtonStyle)] = value;
        }

        public Unit Width
        {
            get => (Unit)(ViewState[nameof(Width)] ?? Unit.Percentage(100));
            set => ViewState[nameof(Width)] = value;
        }

        public bool AutoPostBack
        {
            get => (bool)(ViewState[nameof(AutoPostBack)] ?? false);
            set => ViewState[nameof(AutoPostBack)] = value;
        }

        public bool CausesValidation
        {
            get => (bool)(ViewState[nameof(CausesValidation)] ?? false);
            set => ViewState[nameof(CausesValidation)] = value;
        }

        public string ValidationGroup
        {
            get => (string)(ViewState[nameof(ValidationGroup)] ?? string.Empty);
            set => ViewState[nameof(ValidationGroup)] = value;
        }

        public bool EnableTranslation
        {
            get => Translate != null && (bool)(ViewState[nameof(EnableTranslation)] ?? false);
            set => ViewState[nameof(EnableTranslation)] = value;
        }

        [PersistenceMode(PersistenceMode.InnerProperty)]
        public ComboBoxItemCollection<ComboBoxItem> Items => _items;

        [PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ComboBoxDropDown DropDown => _dropDown;

        [PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ComboBoxClientEvents ClientEvents => _clientEvents;

        protected virtual BindingType ControlBinding => BindingType.Database;

        protected bool IsBindingDone
        {
            get => (bool)(ViewState[nameof(IsBindingDone)] ?? false);
            set => ViewState[nameof(IsBindingDone)] = value;
        }

        protected Func<string, string> Translate { get; private set; }

        public abstract bool HasValue { get; }

        #endregion

        #region Fields

        private readonly ComboBoxDropDown _dropDown;
        private readonly ComboBoxItemCollection<ComboBoxItem> _items;
        private readonly ComboBoxClientEvents _clientEvents;

        private static readonly ListItemArray _nullDataSource = new ListItemArray();

        private static readonly HashSet<string> _redundantAttributes = new HashSet<string>(new[]
        {
            "id",
            "name",
            "title",
            "disabled",
            "multiple",
            "class",
            "width",
            "height",

            "data-max-options",
            "data-actions-box",
            "data-selected-text-format",
            "data-live-search",
            "data-style",
            "data-width",
            "data-header",
            "data-size",
            "data-container",
            "data-dropup-auto",
        }, StringComparer.OrdinalIgnoreCase);

        #endregion

        #region Construction

        public BaseComboBox()
        {
            _dropDown = new ComboBoxDropDown(nameof(DropDown), ViewState);
            _items = new ComboBoxItemCollection<ComboBoxItem>(this, false);
            _clientEvents = new ComboBoxClientEvents(nameof(ClientEvents), ViewState);
        }

        #endregion

        #region Loading

        protected override void OnInit(EventArgs e)
        {
            if (Page is InSite.UI.Layout.Lobby.LobbyBasePage lobbyPage)
                Translate = lobbyPage.Translate;
            else
                Translate = null;

            base.OnInit(e);

            if (ControlBinding == BindingType.Code)
            {
                EnsureDataBound();
            }
            else if (!Page.IsPostBack && EnableTranslation && Items.Count > 0)
            {
                foreach (var item in Items)
                    TranslateItem(item);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (ControlBinding == BindingType.Database)
                EnsureDataBound();
        }

        protected override void OnAttributeSet(AttributeEventArgs e)
        {
            if (!e.Cancel)
                e.Cancel = _redundantAttributes.Contains(e.Name);
        }

        protected override void OnPreRender(EventArgs e)
        {
            if (!AutoPostBack)
                Page.ClientScript.RegisterForEventValidation(UniqueID);

            var settings = CreateClientSideSettings(ClientID);

            SetupClientSideSettings(settings);

            PageFooterContentRenderer.RegisterScript(
                typeof(BaseComboBox),
                "init_" + ClientID,
                $"inSite.common.comboBox.init({JsonHelper.SerializeJsObject(settings)});");

            base.OnPreRender(e);
        }

        #endregion

        #region Methods

        public virtual void LoadItems(IEnumerable<string> textValueData)
        {
            OnPreLoadItems();

            foreach (var item in textValueData)
                Items.Add(LoadItem(item, item));

            OnPostLoadItems();
        }

        public virtual void LoadItems<T>(params T[] items) where T : Enum
        {
            OnPreLoadItems();

            foreach (var item in items)
                Items.Add(LoadItem(item.GetDescription(), item.GetName()));

            OnPostLoadItems();
        }

        public virtual void LoadItems(IEnumerable<Shift.Common.ListItem> data)
        {
            OnPreLoadItems();

            foreach (var item in data)
                Items.Add(LoadItem(item));

            OnPostLoadItems();
        }

        internal void LoadItems(IEnumerable data, string valueField, string textField)
        {
            var hasValue = valueField.IsNotEmpty();
            var hasText = textField.IsNotEmpty();

            OnPreLoadItems();

            foreach (var item in data)
            {
                var option = new ComboBoxOption();

                if (item != null)
                {
                    if (hasText)
                        option.Text = DataBinder.Eval(item, textField)?.ToString();

                    if (hasValue)
                        option.Value = DataBinder.Eval(item, valueField)?.ToString();
                }

                Items.Add(option);
            }

            OnPostLoadItems();
        }

        public virtual void ClearSelection()
        {
            foreach (var item in this.FlattenOptions())
                item.Selected = false;
        }

        public void EnsureDataBound()
        {
            if (!IsBindingDone)
                RefreshData();
        }

        public void RefreshData()
        {
            var list = CreateDataSource();
            if (list != _nullDataSource)
                LoadItems(list);
        }

        #endregion

        #region Methods (internal)

        protected virtual ClientSideSettings CreateClientSideSettings(string clientId) => new ClientSideSettings(clientId);

        protected virtual void SetupClientSideSettings(ClientSideSettings settings)
        {
            settings.EmptyText = EmptyMessage.Trim().NullIfEmpty();

            if (SearchEmptyMessage != DefaultSearchEmptyFormat)
                settings.SearchEmptyText = SearchEmptyMessage;

            settings.EnableSearch = EnableSearch;
            settings.ShowOptionHtml = ShowOptionHtml;
            settings.ButtonStyle = ControlHelper.MergeCssClasses(
                ButtonSize.GetContextualClass(),
                (ButtonStyle != ButtonStyle.None ? ButtonStyle.GetContextualClass() : "btn-combobox"));

            if (!Width.IsEmpty)
                settings.Width = Width.ToString();

            if (DropDown.Header.IsNotEmpty())
                settings.DropDown.Header = DropDown.Header;

            if (DropDown.Size > 0)
                settings.DropDown.Size = DropDown.Size;

            if (DropDown.Container.IsNotEmpty())
                settings.DropDown.Container = DropDown.Container;

            if (DropDown.Direction != ComboBoxDropDown.DirectionType.Auto)
                settings.DropDown.Direction = DropDown.Direction.GetName().ToLower();

            if (!DropDown.Width.IsEmpty)
                settings.DropDown.Width = DropDown.Width.ToString();

            if (ClientEvents.OnChange.IsNotEmpty())
                settings.Callback.OnChange = ClientEvents.OnChange;

            if (AutoPostBack)
            {
                var postBackOptions = new PostBackOptions(this, string.Empty)
                {
                    AutoPostBack = AutoPostBack
                };

                if (CausesValidation && Page.GetValidators(ValidationGroup).Count > 0)
                {
                    postBackOptions.PerformValidation = true;
                    postBackOptions.ValidationGroup = ValidationGroup;
                }

                var script = Page.ClientScript.GetPostBackEventReference(postBackOptions, true);

                if (script.IsNotEmpty())
                {
                    if (postBackOptions.ClientSubmit)
                        script = ControlHelper.MergeScripts(script, "return false;");

                    settings.Callback.PostBack = script;
                }
            }
        }

        protected virtual void OnPreLoadItems()
        {
            Items.Clear();
        }

        protected virtual ComboBoxItem LoadItem(string text, string value)
        {
            var option = new ComboBoxOption(text, value);

            if (EnableTranslation)
                TranslateItem(option);

            return option;
        }

        protected virtual ComboBoxItem LoadItem(Shift.Common.ListItem item)
        {
            var option = (ComboBoxOption)LoadItem(item.Text, item.Value);

            if (item.Enabled.HasValue)
                option.Enabled = item.Enabled.Value;

            if (item.Description.IsNotEmpty())
                option.SubText = item.Description;

            if (item.Icon.IsNotEmpty())
                option.Icon = item.Icon;

            return option;
        }

        protected virtual void OnPostLoadItems()
        {
            IsBindingDone = true;
        }

        protected virtual ListItemArray CreateDataSource() => _nullDataSource;

        protected virtual void TranslateItem(ComboBoxItem item)
        {
            item.Text = Translate(item.Text);
        }

        #endregion

        #region IPostBackDataHandler

        bool IPostBackDataHandler.LoadPostData(string postDataKey, NameValueCollection postCollection)
        {
            Page.ClientScript.ValidateEvent(postDataKey);

            return LoadPostData(postCollection[postDataKey]);
        }

        protected abstract bool LoadPostData(string value);

        void IPostBackDataHandler.RaisePostDataChangedEvent()
        {
            if (AutoPostBack && !Page.IsPostBackEventControlRegistered)
            {
                Page.AutoPostBackControl = this;

                if (CausesValidation)
                    Page.Validate(ValidationGroup);
            }

            RaisePostDataChangedEvent();
        }

        protected abstract void RaisePostDataChangedEvent();

        #endregion

        #region IStateManager

        protected override void LoadViewState(object savedState)
        {
            if (savedState != null)
            {
                var pair = (Pair)savedState;
                savedState = pair.First;
                ((IStateManager)Items).LoadViewState(pair.Second);
            }

            base.LoadViewState(savedState);
        }

        protected override object SaveViewState()
        {
            var state1 = base.SaveViewState();
            var state2 = ((IStateManager)Items).SaveViewState();

            return state1 != null || state2 != null
                ? new Pair(state1, state2)
                : null;
        }

        protected override void TrackViewState()
        {
            base.TrackViewState();
            ((IStateManager)Items).TrackViewState();
        }

        #endregion

        #region IComboBoxItemContainer

        IEnumerable<ComboBoxItem> IComboBoxItemContainer.Items => _items;

        #endregion

        #region IComboBoxItemOwner

        void IComboBoxItemOwner.ItemAssigned(ComboBoxItem item) => OnItemAssigned(item);

        protected abstract void OnItemAssigned(ComboBoxItem item);

        void IComboBoxItemOwner.ItemSelected(ComboBoxItem item) => OnItemSelected(item);

        protected abstract void OnItemSelected(ComboBoxItem item);

        #endregion

        #region Rendering

        protected override void Render(HtmlTextWriter writer)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Id, ClientID);
            writer.AddAttribute(HtmlTextWriterAttribute.Name, UniqueID);

            AddAttributesToRender(writer);

            writer.RenderBeginTag(HtmlTextWriterTag.Select);

            foreach (IComboBoxItem item in Items)
                item.Render(writer);

            writer.RenderEndTag();
        }

        protected override void AddAttributesToRender(HtmlTextWriter writer)
        {
            base.AddAttributesToRender(writer);

            var style = GetStyle();
            if (style != null)
                writer.AddAttribute(HtmlTextWriterAttribute.Style, style);

            if (!Enabled)
                writer.AddAttribute(HtmlTextWriterAttribute.Disabled, "disabled");

            writer.AddAttribute(
                HtmlTextWriterAttribute.Class,
                ControlHelper.MergeCssClasses(
                    ControlCssClass,
                    CssClass,
                    "form-select"));
        }

        protected string GetStyle()
        {
            var sb = new StringBuilder();

            if (!Width.IsEmpty)
                sb.Append("width:").Append(Width.ToString()).Append(";");

            return sb.Length == 0 ? null : sb.ToString();
        }

        #endregion
    }
}