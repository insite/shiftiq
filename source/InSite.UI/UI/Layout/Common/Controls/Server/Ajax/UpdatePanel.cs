using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Web.UI;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Common.Events;

namespace InSite.Common.Web.UI
{
    public class UpdatePanel : System.Web.UI.UpdatePanel, IPostBackDataHandler
    {
        #region Events

        public event StringValueHandler Request;
        private void OnRequest(string value) =>
            Request?.Invoke(this, new StringValueArgs(value));

        #endregion

        #region Classes

        [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
        protected class ClientSideSettings
        {
            [JsonProperty(PropertyName = "id")]
            public string ClientID { get; set; }

            [JsonProperty(PropertyName = "name")]
            public string UniqueID { get; set; }

            [JsonProperty(PropertyName = "postBack", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public string PostBackScript { get; set; }

            [JsonProperty(PropertyName = "onRequestStart", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public string OnRequestStart { get; set; }

            [JsonProperty(PropertyName = "onResponseEnd", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public string OnResponseEnd { get; set; }

            public ClientSideSettings(string clientId, string uniqueId)
            {
                ClientID = clientId;
                UniqueID = uniqueId;
            }
        }

        #endregion

        #region Properties

        public string CssClass
        {
            get => (string)ViewState[nameof(CssClass)];
            set => ViewState[nameof(CssClass)] = value;
        }

        public bool RequestAsTrigger
        {
            get => (bool)(ViewState[nameof(RequestAsTrigger)] ?? true);
            set => ViewState[nameof(RequestAsTrigger)] = value;
        }

        public System.Web.UI.CssStyleCollection Style => Attributes.CssStyle;

        [PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public UpdatePanelClientEvents ClientEvents => _clientEvents;

        #endregion

        #region Fields

        private string _requestValue;

        private readonly UpdatePanelClientEvents _clientEvents;

        #endregion

        #region Construction

        public UpdatePanel()
        {
            UpdateMode = System.Web.UI.UpdatePanelUpdateMode.Conditional;

            _clientEvents = new UpdatePanelClientEvents(nameof(ClientEvents), ViewState);
        }

        #endregion

        #region Initialization and Loading

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            var settings = new ClientSideSettings(ClientID, UniqueID)
            {
                PostBackScript = Page.ClientScript.GetPostBackEventReference(
                    new PostBackOptions(this) { AutoPostBack = true }, true),
                OnRequestStart = ClientEvents.OnRequestStart,
                OnResponseEnd = ClientEvents.OnResponseEnd,
            };

            ScriptManager.RegisterStartupScript(
                Page,
                typeof(UpdatePanel),
                "init_" + ClientID,
                $"inSite.common.updatePanel.init({JsonHelper.SerializeJsObject(settings)});", true);

        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (!IsInPartialRendering)
                Attributes["class"] = ControlHelper.MergeCssClasses("InSiteUpdatePanel", CssClass);

            base.Render(writer);
        }

        #endregion

        #region IPostBackEventHandler

        bool IPostBackDataHandler.LoadPostData(string postDataKey, NameValueCollection postCollection) => LoadPostData(postDataKey, postCollection);

        void IPostBackDataHandler.RaisePostDataChangedEvent() => RaisePostDataChangedEvent();

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

            OnRequest(_requestValue);

            if (RequestAsTrigger)
                Update();
        }

        #endregion
    }
}