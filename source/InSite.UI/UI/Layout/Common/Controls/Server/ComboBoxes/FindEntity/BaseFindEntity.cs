using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Humanizer;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Common.Events;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Common.Web.UI
{
    [ValidationProperty(nameof(Values))]
    public abstract class BaseFindEntity<TFilter> : BaseControl, IBaseFindEntity, IPostBackDataHandler, IHasEmptyMessage where TFilter : Filter
    {
        #region Events

        public event FindEntityValueChangedEventHandler ValueChanged;

        private void OnValueChanged(FindEntityValueChangedEventArgs e)
        {
            ValueChanged?.Invoke(this, e);
        }

        public event EventHandler ItemsRequested;

        private void OnItemsRequested()
        {
            ItemsRequested?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Constants

        private const int DefaultPageSize = 10;
        private const int MinPageSize = 5;

        #endregion

        #region Classes

        [Serializable]
        [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
        public class DataItem
        {
            [JsonProperty(PropertyName = "value")]
            public virtual Guid Value { get; set; }

            [JsonProperty(PropertyName = "text")]
            public virtual string Text { get; set; }

            public ReadOnlyDataItem AsReadOnly() => new ReadOnlyDataItem(this);

            public DataItem Clone()
            {
                return (DataItem)MemberwiseClone();
            }
        }

        [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
        public class GroupItem : DataItem
        {
            [JsonIgnore]
            public override Guid Value
            {
                get => throw new InvalidOperationException();
                set => throw new InvalidOperationException();
            }

            [JsonProperty(PropertyName = "group")]
            public override string Text { get => base.Text; set => base.Text = value; }

            [JsonProperty(PropertyName = "items", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public DataItem[] DataItems { get; set; }
        }

        public class ReadOnlyDataItem
        {
            public Guid Value => _item.Value;

            public string Text => _item.Text;

            private DataItem _item;

            public ReadOnlyDataItem(DataItem dataItem)
            {
                _item = dataItem;
            }
        }

        [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
        protected class ClientSideSettings
        {
            [JsonProperty(PropertyName = "id")]
            public string ClientID { get; }

            [JsonProperty(PropertyName = "name")]
            public string UniqueID { get; }

            [DefaultValue(true)]
            [JsonProperty(PropertyName = "showHeader", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public bool ShowHeader { get; set; }

            [DefaultValue(true)]
            [JsonProperty(PropertyName = "showFooter", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public bool ShowFooter { get; set; }

            [DefaultValue(true)]
            [JsonProperty(PropertyName = "showClear", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public bool ShowClear { get; set; }

            [DefaultValue(true)]
            [JsonProperty(PropertyName = "autoSave", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public bool CloseOnSelect { get; set; }

            [DefaultValue(1)]
            [JsonProperty(PropertyName = "maxSelect", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public int MaxSelectionCount { get; set; }

            [JsonProperty(PropertyName = "output", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public string OutputType { get; set; }

            [JsonProperty(PropertyName = "disabled", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public bool IsDisabled { get; set; }

            [JsonProperty(PropertyName = "placeholder", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public string EmptyMessage { get; set; }

            [DefaultValue(DefaultPageSize)]
            [JsonProperty(PropertyName = "pageSize", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public int PageSize { get; set; }

            [JsonProperty(PropertyName = "maxPageCount", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public int MaxPageCount { get; set; }

            [JsonProperty(PropertyName = "url", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public string EditorUrl { get; set; }

            [JsonProperty(PropertyName = "callback")]
            public ClientSideCallbackSettings Callback { get; } = new ClientSideCallbackSettings();

            [JsonProperty(PropertyName = "strings")]
            public ClientSideStringSettings Strings { get; } = new ClientSideStringSettings();

            public ClientSideSettings(string clientId, string uniqueId)
            {
                ClientID = clientId;
                UniqueID = uniqueId;
            }

            public bool ShouldSerializeCallback() => !Callback.IsEmpty;

            public bool ShouldSerializeStrings() => !Strings.IsEmpty;
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

        [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
        protected class ClientSideStringSettings
        {
            [JsonProperty(PropertyName = "modalHeader")]
            public string ModalHeader { get; set; }

            [JsonProperty(PropertyName = "tableHeader")]
            public string TableHeader { get; set; }

            [JsonProperty(PropertyName = "entityPlural")]
            public string EntityPlural { get; set; }

            [JsonProperty(PropertyName = "clearTitle")]
            public string ClearTitle { get; set; }

            [JsonProperty(PropertyName = "cancelTitle")]
            public string CancelTitle { get; set; }

            [JsonProperty(PropertyName = "keywordText")]
            public string KeywordText { get; set; }

            [JsonProperty(PropertyName = "clearFilterText")]
            public string ClearFilterText { get; set; }

            [JsonProperty(PropertyName = "applyFilterText")]
            public string ApplyFilterText { get; set; }

            [JsonProperty(PropertyName = "noItems")]
            public string NoItemsMessage { get; set; }

            public bool IsEmpty =>
                ModalHeader.IsEmpty()
                && TableHeader.IsEmpty()
                && EntityPlural.IsEmpty()
                && ClearTitle.IsEmpty()
                && CancelTitle.IsEmpty()
                && KeywordText.IsEmpty()
                && ClearFilterText.IsEmpty()
                && ApplyFilterText.IsEmpty()
                && NoItemsMessage.IsEmpty();
        }

        [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
        private class JsonRequestData
        {
            [JsonProperty(PropertyName = "page", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public int? PageNumber { get; set; }

            [JsonProperty(PropertyName = "filter", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public Dictionary<string, string> CriteriaParameters { get; set; }
        }

        [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
        private class JsonResponseData
        {
            [JsonProperty(PropertyName = "count", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public int TotalCount { get; set; }

            [JsonProperty(PropertyName = "items", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public DataItem[] DataItems { get; set; }
        }

        #endregion

        #region Properties

        public string ModalHeader
        {
            get => (string)ViewState[nameof(ModalHeader)];
            set => ViewState[nameof(ModalHeader)] = value;
        }

        public FindEntityOutput Output
        {
            get => (FindEntityOutput)(ViewState[nameof(Output)] ?? FindEntityOutput.Button);
            set => ViewState[nameof(Output)] = value;
        }

        public bool Enabled
        {
            get => (bool)(ViewState[nameof(Enabled)] ?? true);
            set => ViewState[nameof(Enabled)] = value;
        }

        public string EmptyMessage
        {
            get => (string)ViewState[nameof(EmptyMessage)] ?? string.Empty;
            set => ViewState[nameof(EmptyMessage)] = value;
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

        public int PageSize
        {
            get => (int)(ViewState[nameof(PageSize)] ?? DefaultPageSize);
            set => ViewState[nameof(PageSize)] = Number.CheckRange(value, MinPageSize);
        }

        public int MaxPageCount
        {
            get => (int)(ViewState[nameof(MaxPageCount)] ?? 0);
            set => ViewState[nameof(MaxPageCount)] = Number.CheckRange(value, 0);
        }

        public bool ShowHeader
        {
            get => (bool)(ViewState[nameof(ShowHeader)] ?? true);
            set => ViewState[nameof(ShowHeader)] = value;
        }

        public bool ShowFooter
        {
            get => (bool)(ViewState[nameof(ShowFooter)] ?? true);
            set => ViewState[nameof(ShowFooter)] = value;
        }

        public bool AllowClear
        {
            get => (bool)(ViewState[nameof(AllowClear)] ?? true);
            set => ViewState[nameof(AllowClear)] = value;
        }

        public bool CloseOnSelect
        {
            get => (bool)(ViewState[nameof(CloseOnSelect)] ?? true);
            set => ViewState[nameof(CloseOnSelect)] = value;
        }

        public int MaxSelectionCount
        {
            get => (int)(ViewState[nameof(MaxSelectionCount)] ?? 1);
            set => ViewState[nameof(MaxSelectionCount)] = Number.CheckRange(value, 0);
        }

        public bool HasValue => ValuesInternal != null;

        public int SelectedCount => ValuesInternal != null ? ValuesInternal.Length : 0;

        public Guid? Value
        {
            get
            {
                if (MaxSelectionCount == 1)
                {
                    var values = ValuesInternal;
                    if (values != null && values.Length == 1)
                        return values[0];
                }

                return null;
            }
            set
            {
                ValuesInternal = value.HasValue ? new[] { value.Value } : null;
            }
        }

        public Guid[] Values
        {
            get => ValuesInternal.EmptyIfNull();
            set => ValuesInternal = CheckRange(value);
        }

        public ReadOnlyDataItem Item
        {
            get
            {
                if (MaxSelectionCount == 1)
                {
                    var items = ItemsInternal;
                    if (items != null && items.Length == 1)
                        return items[0].AsReadOnly();
                }

                return null;
            }
        }

        public IEnumerable<ReadOnlyDataItem> Items
        {
            get
            {
                EnsureItemsPopulated();

                return ItemsInternal.Select(x => x.AsReadOnly());
            }
        }

        private Guid[] ValuesInternal
        {
            get => (Guid[])ViewState[nameof(Values)];
            set => ViewState[nameof(Values)] = value;
        }

        private DataItem[] ItemsInternal
        {
            get => (DataItem[])ViewState[nameof(ItemsInternal)];
            set => ViewState[nameof(ItemsInternal)] = value;
        }

        [PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public FindEntityClientEvents ClientEvents => _clientEvents;

        private Func<string, string> _translate;
        private bool _isTranslateSet;
        protected Func<string, string> Translate
        {
            get
            {
                if (!_isTranslateSet)
                {
                    if (Page is InSite.UI.Layout.Lobby.LobbyBasePage lobbyPage)
                        _translate = lobbyPage.Translate;
                    else
                        _translate = null;

                    _isTranslateSet = true;
                }

                return _translate;
            }
        }

        #endregion

        #region Fields

        private readonly FindEntityClientEvents _clientEvents;

        private FindEntityValueChangedEventArgs _changedArgs;

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

        public BaseFindEntity()
        {
            _clientEvents = new FindEntityClientEvents(nameof(ClientEvents), ViewState);
        }

        #endregion

        #region Loading

        protected override void OnLoad(EventArgs e)
        {
            if (HttpRequestHelper.IsAjaxRequest && Page.Request.Form["__FINDENTITYID"] == UniqueID)
            {
                OnItemsRequested();

                var requestJson = Page.Request.Form["__FINDENTITYPR"];
                var requestData = JsonConvert.DeserializeObject<JsonRequestData>(requestJson);
                var responseData = new JsonResponseData();

                var filter = GetFilter(requestData.CriteriaParameters);

                if (requestData.PageNumber.HasValue)
                {
                    var page = Number.CheckRange(requestData.PageNumber.Value, 1, MaxPageCount > 0 ? MaxPageCount : (int?)null);

                    if (filter != null)
                        filter.Paging = Paging.SetPage(page, PageSize);


                    responseData.DataItems = Select(filter);
                }
                else
                {
                    responseData.TotalCount = Count(filter);

                    if (filter != null)
                        filter.Paging = Paging.SetPage(1, PageSize);

                    responseData.DataItems = responseData.TotalCount > 0 ? Select(filter) : new DataItem[0];
                }

                var responseJson = JsonConvert.SerializeObject(responseData);

                var response = Context.Response;
                response.Clear();
                response.Write(responseJson);
                response.End();
            }

            base.OnLoad(e);
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

            EnsureItemsPopulated();

            var settings = CreateClientSideSettings(ClientID, UniqueID);

            PageFooterContentRenderer.RegisterScript(
                typeof(BaseFindEntity<>),
                "init_" + ClientID,
                $"inSite.common.findEntity.init({JsonHelper.SerializeJsObject(settings)});");

            base.OnPreRender(e);
        }

        protected virtual ClientSideSettings CreateClientSideSettings(string clientId, string uniqueId)
        {
            var emptyMessage = EmptyMessage.NullIfEmpty();
            if (emptyMessage.HasValue() && EnableTranslation)
                emptyMessage = Translate(emptyMessage);

            var settings = new ClientSideSettings(clientId, uniqueId)
            {
                ShowHeader = ShowHeader,
                ShowFooter = ShowFooter,
                ShowClear = AllowClear,
                CloseOnSelect = CloseOnSelect,
                MaxSelectionCount = MaxSelectionCount,
                IsDisabled = !Enabled,
                EmptyMessage = emptyMessage,
                PageSize = PageSize,
                MaxPageCount = MaxPageCount,
            };

            if (Output != FindEntityOutput.Button)
                settings.OutputType = Output.GetName().ToLower();

            if (Output == FindEntityOutput.List)
                settings.EditorUrl = GetEditorUrl().NullIfEmpty();

            var entityName = GetEntityName();
            var entityNamePluralized = entityName.Pluralize();

            settings.Strings.EntityPlural = EnableTranslation
                ? Translate(entityNamePluralized)
                : entityNamePluralized;

            settings.Strings.ModalHeader = string.IsNullOrEmpty(ModalHeader)
                ? settings.Strings.EntityPlural
                : (EnableTranslation ? Translate(ModalHeader) : ModalHeader);

            var tableHeader = entityName.Singularize().Titleize() + " Name";

            settings.Strings.TableHeader = EnableTranslation
                ? Translate(tableHeader)
                : tableHeader;

            settings.Strings.ClearTitle = EnableTranslation ? HttpUtility.HtmlEncode(Translate("Clear")) : "Clear";
            settings.Strings.CancelTitle = EnableTranslation ? HttpUtility.HtmlEncode(Translate("Cancel")) : "Cancel";
            settings.Strings.KeywordText = EnableTranslation ? Translate("Keyword") : "Keyword";
            settings.Strings.ClearFilterText = EnableTranslation ? HttpUtility.HtmlEncode(Translate("Clear Filter")) : "Clear Filter";
            settings.Strings.ApplyFilterText = EnableTranslation ? HttpUtility.HtmlEncode(Translate("Apply Filter")) : "Apply Filter";
            settings.Strings.NoItemsMessage = EnableTranslation ? HttpUtility.HtmlEncode(Translate("No Items")) : "No Items";

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

            return settings;
        }

        protected virtual string GetEditorUrl() => null;

        #endregion

        #region Abstract methods

        protected abstract string GetEntityName();

        protected abstract int Count(TFilter filter);

        protected abstract DataItem[] Select(TFilter filter);

        protected virtual TFilter GetFilter(IDictionary<string, string> values)
        {
            return GetFilter(values.GetOrDefault("keyword").NullIfEmpty());
        }

        protected abstract TFilter GetFilter(string keyword);

        protected abstract IEnumerable<DataItem> GetItems(Guid[] ids);

        #endregion

        #region IPostBackDataHandler

        bool IPostBackDataHandler.LoadPostData(string postDataKey, NameValueCollection postCollection)
        {
            Page.ClientScript.ValidateEvent(postDataKey);

            return LoadPostData(postCollection[postDataKey]);
        }

        protected virtual bool LoadPostData(string value)
        {
            if (!Visible || !Enabled)
                return false;

            var oldValues = ValuesInternal.EmptyIfNull();
            var newValues = value.IsNotEmpty()
                ? JsonConvert.DeserializeObject<DataItem[]>(value)
                    .Select(x => x.Value).ToArray()
                : new Guid[0];

            if (newValues.Length > 0)
                newValues = CheckRange(newValues);

            var isChanged = Visible
                && (oldValues.Length != newValues.Length
                    || oldValues.Any(ov => !newValues.Any(nv => ov == nv)));

            if (isChanged)
            {
                ValuesInternal = newValues.NullIfEmpty();
                _changedArgs = new FindEntityValueChangedEventArgs(newValues, oldValues, MaxSelectionCount == 1);
            }

            return isChanged;
        }

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

        protected virtual void RaisePostDataChangedEvent()
        {
            if (_changedArgs != null)
            {
                OnValueChanged(_changedArgs);
                _changedArgs = null;
            }
            else
            {
                OnValueChanged(new FindEntityValueChangedEventArgs(ValuesInternal.NullIfEmpty(), new Guid[0], MaxSelectionCount == 1));
            }
        }

        #endregion

        #region Rendering

        protected override void Render(HtmlTextWriter writer)
        {
            AddAttributesToRender(writer);

            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            writer.AddAttribute(HtmlTextWriterAttribute.Id, ClientID);
            writer.AddAttribute(HtmlTextWriterAttribute.Name, UniqueID);
            writer.AddAttribute(HtmlTextWriterAttribute.Type, "hidden");

            if (ItemsInternal.IsNotEmpty())
                writer.AddAttribute(HtmlTextWriterAttribute.Value, JsonConvert.SerializeObject(ItemsInternal));

            writer.RenderBeginTag(HtmlTextWriterTag.Input);
            writer.RenderEndTag();

            writer.RenderEndTag();
        }

        protected override void AddAttributesToRender(HtmlTextWriter writer)
        {
            var attrStyle = Attributes["style"];
            if (attrStyle.IsNotEmpty())
                Attributes.Remove("style");

            var styles = Shift.Sdk.UI.CssStyleCollection.Parse(attrStyle);

            if (!Width.IsEmpty)
                styles["width"] = Width.ToString();

            if (!styles.IsEmpty)
                writer.AddAttribute(HtmlTextWriterAttribute.Style, styles.ToString());

            base.AddAttributesToRender(writer);

            writer.AddAttribute(HtmlTextWriterAttribute.Class, ControlHelper.MergeCssClasses(
                "insite-findentity",
                Output == FindEntityOutput.None ? "no-output" : null,
                CssClass));
        }

        #endregion

        #region Helper methods

        private Guid[] CheckRange(IEnumerable<Guid> value)
        {
            if (value == null)
                return null;

            var query = value.Distinct();
            if (MaxSelectionCount > 0)
                query = query.Take(MaxSelectionCount);

            return query.ToArray().NullIfEmpty();
        }

        private void EnsureItemsPopulated()
        {
            var items = ItemsInternal.EmptyIfNull();
            var values = ValuesInternal.EmptyIfNull();
            var isChanged = items.Length != values.Length
                || items.Length > 0 && items.Any(x => !values.Contains(x.Value));

            if (!isChanged)
                return;

            var loadedItems = values.Length > 0
                ? GetItems(values).Select(x => x.Clone()).OrderBy(x => x.Text).ToArray()
                : null;

            if (loadedItems.IsNotEmpty())
            {
                if (loadedItems.Length != values.Length)
                    ValuesInternal = loadedItems.Select(x => x.Value).ToArray();

                ItemsInternal = loadedItems;
            }
            else
            {
                ItemsInternal = null;
            }
        }

        #endregion
    }
}