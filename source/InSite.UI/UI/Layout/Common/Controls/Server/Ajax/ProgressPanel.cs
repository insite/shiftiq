using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Common.Web.UI
{
    [ParseChildren(true), PersistChildren(true)]
    public class ProgressPanel : Control, IPostBackDataHandler
    {
        #region Events

        public event EventHandler RequestCancelled;

        private void OnRequestCancelled() =>
            RequestCancelled?.Invoke(this, EventArgs.Empty);

        #endregion

        #region Classes

        [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
        private class ClientSideSettings
        {
            [JsonProperty(PropertyName = "id")]
            public string ClientID { get; set; }

            [JsonProperty(PropertyName = "name")]
            public string UniqueID { get; set; }

            [JsonProperty(PropertyName = "context")]
            public string ContextID { get; set; }

            [JsonProperty(PropertyName = "width", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public string Width { get; set; }

            [JsonProperty(PropertyName = "title", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public string HeaderText { get; set; }

            [JsonProperty(PropertyName = "cancel", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public string CancelMode { get; set; }

            [DefaultValue(true), JsonProperty(PropertyName = "enabled", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public bool Enabled { get; set; }

            [JsonProperty(PropertyName = "postBack", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public string PostBackScript { get; set; }

            [JsonProperty(PropertyName = "triggers")]
            public ClientSideTriggers Triggers { get; set; }

            [JsonProperty(PropertyName = "callbacks")]
            public ClientSideCallbacks Callbacks { get; set; }

            [JsonProperty(PropertyName = "items")]
            public ProgressPanelItemClientData[] Items { get; set; }

            public ClientSideSettings(string id, string name, string context)
            {
                ClientID = id;
                UniqueID = name;
                ContextID = context;
            }

            public bool ShouldSerializeTriggers() => Triggers.IsNotEmpty();

            public bool ShouldSerializeCallbacks() => Callbacks != null && Callbacks.IsNotEmpty();
        }

        [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
        private class ClientSideTriggers
        {
            [DefaultValue(true), JsonProperty(PropertyName = "postBack", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public bool TriggerOnPostBack { get; set; }

            [DefaultValue(true), JsonProperty(PropertyName = "ajaxRequest", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public bool TriggerOnAjaxRequest { get; set; }

            [JsonProperty(PropertyName = "items")]
            public ProgressPanelTriggerClientData[] Items { get; set; }

            public bool ShouldSerializeItems() => Items.IsNotEmpty();

            public bool IsNotEmpty() => ShouldSerializeItems()
                || !TriggerOnPostBack
                || !TriggerOnAjaxRequest;
        }

        [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
        private class ClientSideCallbacks
        {
            [JsonProperty(PropertyName = "pollStart", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public string OnPollingStart { get; set; }

            [JsonProperty(PropertyName = "pollError", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public string OnPollingError { get; set; }

            [JsonProperty(PropertyName = "pollStopped", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public string OnPollingStopped { get; set; }

            [JsonProperty(PropertyName = "cancelled", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public string OnCancelled { get; set; }

            [JsonProperty(PropertyName = "submit", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public string OnSubmitDetected { get; set; }

            [JsonProperty(PropertyName = "submitError", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public string OnSubmitError { get; set; }

            public bool IsNotEmpty() => OnPollingStart.IsNotEmpty()
                || OnPollingError.IsNotEmpty()
                || OnPollingStopped.IsNotEmpty()
                || OnCancelled.IsNotEmpty()
                || OnSubmitDetected.IsNotEmpty()
                || OnSubmitError.IsNotEmpty();
        }

        private class ProgressContext : ProgressPanelContext
        {
            public ProgressContext(ProgressPanelItemCollection items)
            {
                Items = new ProgressPanelContextItemCollection(
                    items.Where(x => x.HasContextItem).Select(x => x.GetContextData()));
            }
        }

        private class ProgressExternalContext : IProgressExternalContext
        {
            public string ID { get; set; }
            public ProgressContext Context { get; set; }
        }

        public interface IProgressExternalContext
        {
        }

        #endregion

        #region Properties

        public string ContextID
        {
            get => (string)ViewState[nameof(ContextID)];
            private set => ViewState[nameof(ContextID)] = value;
        }

        public ProgressPanelPosition Position
        {
            get => (ProgressPanelPosition)(ViewState[nameof(Position)] ?? ProgressPanelPosition.Fixed);
            set => ViewState[nameof(Position)] = value;
        }

        public ProgressPanelCancel Cancel
        {
            get => (ProgressPanelCancel)(ViewState[nameof(Cancel)] ?? ProgressPanelCancel.Disabled);
            set => ViewState[nameof(Cancel)] = value;
        }

        public string CssClass
        {
            get => (string)(ViewState[nameof(CssClass)] ?? string.Empty);
            set => ViewState[nameof(CssClass)] = value;
        }

        public string HeaderText
        {
            get => (string)(ViewState[nameof(HeaderText)] ?? string.Empty);
            set => ViewState[nameof(HeaderText)] = value;
        }

        public bool Enabled
        {
            get => (bool)(ViewState[nameof(Enabled)] ?? true);
            set => ViewState[nameof(Enabled)] = value;
        }

        public bool PostBackAsTrigger
        {
            get => (bool)(ViewState[nameof(PostBackAsTrigger)] ?? true);
            set => ViewState[nameof(PostBackAsTrigger)] = value;
        }

        public bool AjaxRequestAsTrigger
        {
            get => (bool)(ViewState[nameof(AjaxRequestAsTrigger)] ?? true);
            set => ViewState[nameof(AjaxRequestAsTrigger)] = value;
        }

        public Unit Width
        {
            get => (Unit)(ViewState[nameof(Width)] ?? Unit.Empty);
            set => ViewState[nameof(Width)] = value;
        }

        [PersistenceMode(PersistenceMode.InnerProperty)]
        public ProgressPanelItemCollection Items => _items;

        [PersistenceMode(PersistenceMode.InnerProperty)]
        public ProgressPanelTriggerCollection Triggers => _triggers;

        [PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ProgressPanelClientEvents ClientEvents => _clientEvents;

        /// <summary>
        /// Returns True if the request was cancelled manually by clicking Cancel button
        /// </summary>
        public bool IsRequestCancelled => CurrentContext.IsCancelled;

        private ProgressContext CurrentContext
            => _context ?? (_context = new ProgressContext(Items));

        #endregion

        #region Fields

        private readonly ProgressPanelItemCollection _items;
        private readonly ProgressPanelTriggerCollection _triggers;
        private readonly ProgressPanelClientEvents _clientEvents;

        private ProgressContext _context;
        private ProgressExternalContext _overridenContext;
        private bool _allowUpdateContext = true;

        private string _requestValue;

        #endregion

        #region Construction

        public ProgressPanel()
        {
            _items = new ProgressPanelItemCollection(IsTrackingViewState);
            _triggers = new ProgressPanelTriggerCollection(IsTrackingViewState);
            _clientEvents = new ProgressPanelClientEvents(nameof(ClientEvents), ViewState);
        }

        #endregion

        #region Methods

        public void UpdateContext(Action<ProgressPanelContext> update)
        {
            if (!_allowUpdateContext)
                return;

            update(CurrentContext);

            InSite.Web.Persistence.ProgressStatus.SetContext(ContextID, CurrentContext);

            if (CurrentContext.IsComplete)
                _allowUpdateContext = false;
        }

        public void RemoveContext()
        {
            InSite.Web.Persistence.ProgressStatus.RemoveContext(ContextID);
        }

        public IProgressExternalContext GetExternalContext()
        {
            return new ProgressExternalContext
            {
                ID = ContextID,
                Context = CurrentContext
            };
        }

        public void SetExternalContext(IProgressExternalContext context)
        {
            if (_overridenContext != null || !(context is ProgressExternalContext extContext))
                throw new ApplicationError("Can't override the progress context: " + ClientID);

            _overridenContext = extContext;
            ContextID = _overridenContext.ID;
            _context = _overridenContext.Context;
        }

        public static void UpdateExternalContext(HttpApplicationState app, IProgressExternalContext context, Action<ProgressPanelContext> update)
        {
            if (!(context is ProgressExternalContext extContext) || extContext.Context.IsComplete)
                return;

            update(extContext.Context);

            InSite.Web.Persistence.ProgressStatus.SetContext(app, extContext.ID, extContext.Context);
        }

        #endregion

        #region Loading and Inialization

        protected override void OnLoad(EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (ContextID == null)
                    ContextID = HttpServerUtility.UrlTokenEncode(Guid.NewGuid().ToByteArray());
            }

            base.OnLoad(e);

            ScriptManager.RegisterClientScriptBlock(
                Page,
                typeof(ProgressPanel),
                "init",
                "inSite.common.progressPanel.register();",
                true);
        }

        protected override void OnPreRender(EventArgs e)
        {
            _allowUpdateContext = false;

            if (_context != null)
                InSite.Web.Persistence.ProgressStatus.RemoveContext(ContextID);

            if (Items.Count == 0)
                throw ApplicationError.Create(ClientID + " has no items.");

            var settings = new ClientSideSettings(ClientID, UniqueID, ContextID)
            {
                HeaderText = HeaderText.Trim().NullIfEmpty(),
                CancelMode = Cancel.GetName(ProgressPanelCancel.Disabled),
                Enabled = Enabled,
                PostBackScript = Page.ClientScript.GetPostBackEventReference(
                    new PostBackOptions(this) { AutoPostBack = true }, true),
                Items = Items.Select(x => x.GetClientData()).ToArray(),
                Triggers = new ClientSideTriggers
                {
                    TriggerOnPostBack = PostBackAsTrigger,
                    TriggerOnAjaxRequest = AjaxRequestAsTrigger,
                    Items = Triggers.Select(x => x.GetClientData(NamingContainer)).ToArray()
                },
                Callbacks = new ClientSideCallbacks
                {
                    OnPollingStart = ClientEvents.OnPollingStart,
                    OnPollingError = ClientEvents.OnPollingError,
                    OnPollingStopped = ClientEvents.OnPollingStopped,
                    OnCancelled = ClientEvents.OnCancelled,
                    OnSubmitDetected = ClientEvents.OnSubmitDetected,
                    OnSubmitError = ClientEvents.OnSubmitError
                }
            };

            if (!Width.IsEmpty)
                settings.Width = Width.ToString();

            ScriptManager.RegisterStartupScript(
                Page,
                typeof(ProgressPanel),
                "init_" + ClientID,
                $"inSite.common.progressPanel.init({JsonHelper.SerializeJsObject(settings)});", true);

            base.OnPreRender(e);
        }

        #endregion

        #region IStateManager

        protected override void LoadViewState(object savedState)
        {
            if (savedState != null)
            {
                var triplet = (Triplet)savedState;
                savedState = triplet.First;
                ((IStateManager)Items).LoadViewState(triplet.Second);
                ((IStateManager)Triggers).LoadViewState(triplet.Third);
            }

            base.LoadViewState(savedState);
        }

        protected override object SaveViewState()
        {
            var state1 = base.SaveViewState();
            var state2 = ((IStateManager)Items).SaveViewState();
            var state3 = ((IStateManager)Triggers).SaveViewState();

            return state1 != null || state2 != null || state3 != null
                ? new Triplet(state1, state2, state3)
                : null;
        }

        protected override void TrackViewState()
        {
            base.TrackViewState();

            ((IStateManager)Items).TrackViewState();
            ((IStateManager)Triggers).TrackViewState();
        }

        #endregion

        #region IPostBackEventHandler

        bool IPostBackDataHandler.LoadPostData(string postDataKey, NameValueCollection postCollection) =>
            LoadPostData(postDataKey, postCollection);

        void IPostBackDataHandler.RaisePostDataChangedEvent() =>
            RaisePostDataChangedEvent();

        private bool LoadPostData(string postDataKey, NameValueCollection postCollection)
        {
            Page.ClientScript.ValidateEvent(postDataKey);

            _requestValue = postCollection[postDataKey];

            return true;
        }

        private void RaisePostDataChangedEvent()
        {
            if (!Page.IsPostBackEventControlRegistered)
                Page.AutoPostBackControl = this;

            if (_requestValue == "cancel")
                OnRequestCancelled();
        }

        #endregion

        #region Rendering

        protected override void Render(HtmlTextWriter writer)
        {
            var position = Position == ProgressPanelPosition.Fixed
                ? "fixed"
                : Position == ProgressPanelPosition.Absolute
                    ? "absolute"
                    : null;

            writer.AddAttribute(HtmlTextWriterAttribute.Id, ClientID);
            writer.AddAttribute(HtmlTextWriterAttribute.Class, ControlHelper.MergeCssClasses("progress-panel", position, CssClass));

            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            writer.Write("<div><div></div></div>");
            writer.RenderEndTag();
        }

        #endregion
    }
}