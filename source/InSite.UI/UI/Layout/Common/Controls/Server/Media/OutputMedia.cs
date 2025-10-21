using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Web.UI;

using Newtonsoft.Json;

using Shift.Common;

namespace InSite.Common.Web.UI
{
    public abstract class OutputMedia : BaseControl, IPostBackDataHandler, IPostBackEventHandler
    {
        #region Constants

        private const float DefaultVolume = 0.5f;

        #endregion

        #region Events

        public event EventHandler DeleteClick;

        protected void OnDeleteClick()
        {
            DeleteClick?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Properties

        public bool AutoLoad
        {
            get => Enabled && (bool)(ViewState[nameof(AutoLoad)] ?? false);
            set => ViewState[nameof(AutoLoad)] = value;
        }

        public bool Enabled
        {
            get => (bool)(ViewState[nameof(Enabled)] ?? true);
            set => ViewState[nameof(Enabled)] = value;
        }

        public float Volume
        {
            get => (float)(ViewState[nameof(Volume)] ?? DefaultVolume);
            set => ViewState[nameof(Volume)] = Number.CheckRange(value, 0f, 1f);
        }

        public bool Muted
        {
            get => (bool)(ViewState[nameof(Muted)] ?? false);
            set => ViewState[nameof(Muted)] = value;
        }

        public bool AllowDelete
        {
            get => (bool)(ViewState[nameof(AllowDelete)] ?? false);
            set => ViewState[nameof(AllowDelete)] = value;
        }

        public virtual bool CausesValidation
        {
            get => (bool)(ViewState[nameof(CausesValidation)] ?? false);
            set => ViewState[nameof(CausesValidation)] = value;
        }

        public string ValidationGroup
        {
            get => (string)ViewState[nameof(ValidationGroup)];
            set => ViewState[nameof(ValidationGroup)] = value;
        }

        public bool Hidden
        {
            get => (bool)(ViewState[nameof(Hidden)] ?? false);
            set => ViewState[nameof(Hidden)] = value;
        }

        protected string DeleteScript => _deleteScript;

        #endregion

        #region Classes

        [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
        protected class BaseClientData
        {
            [DefaultValue(true)]
            [JsonProperty(PropertyName = "enabled", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
            public bool Enabled { get; set; }

            [JsonProperty(PropertyName = "position", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
            public float Position { get; set; }

            [DefaultValue(DefaultVolume)]
            [JsonProperty(PropertyName = "volume", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
            public float Volume { get; set; }

            [JsonProperty(PropertyName = "muted", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
            public bool Muted { get; set; }
        }

        #endregion

        #region Fields

        private BaseClientData _clientData;
        private string _deleteScript;

        #endregion

        #region Loading

        protected override void OnPreRender(EventArgs e)
        {
            if (AllowDelete)
            {
                var postBackOptions = new PostBackOptions(this, "delete");

                if (CausesValidation && Page.GetValidators(ValidationGroup).Count > 0)
                {
                    postBackOptions.PerformValidation = true;
                    postBackOptions.ValidationGroup = ValidationGroup;
                }

                _deleteScript = Page.ClientScript.GetPostBackEventReference(postBackOptions, true);
            }

            base.OnPreRender(e);
        }

        #endregion

        #region IPostBackDataHandler & IPostBackEventHandler

        protected virtual BaseClientData DeserializeClientData(string data)
        {
            return JsonConvert.DeserializeObject<BaseClientData>(data);
        }

        bool IPostBackDataHandler.LoadPostData(string postDataKey, NameValueCollection postCollection)
        {
            if (!Visible)
                return false;

            _clientData = DeserializeClientData(postCollection[postDataKey]);

            Enabled = _clientData.Enabled;
            Volume = _clientData.Volume;
            Muted = _clientData.Muted;

            return LoadPostData(_clientData);
        }

        protected virtual bool LoadPostData(BaseClientData clientData)
        {
            return false;
        }

        void IPostBackDataHandler.RaisePostDataChangedEvent()
        {
            RaisePostDataChangedEvent(_clientData);

            _clientData = null;
        }

        protected virtual void RaisePostDataChangedEvent(BaseClientData clientData)
        {

        }

        void IPostBackEventHandler.RaisePostBackEvent(string eventArgument)
        {
            if (!Visible)
                return;

            Page.ClientScript.ValidateEvent(UniqueID, eventArgument);

            if (CausesValidation)
                Page.Validate(ValidationGroup);

            if (AllowDelete && eventArgument == "delete")
                OnDeleteClick();

            RaisePostBackEvent(eventArgument);
        }

        protected virtual void RaisePostBackEvent(string eventArgument)
        {

        }

        #endregion
    }
}