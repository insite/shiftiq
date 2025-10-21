using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI;

using Newtonsoft.Json;

using Shift.Common;

namespace InSite.Common.Web.UI
{
    [PersistChildren(true), ParseChildren(typeof(MultiFieldView), ChildrenAsProperties = false)]
    public class MultiField : BaseControl
    {
        #region Classes

        [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
        protected class ClientSideSettings
        {
            [JsonProperty(PropertyName = "id")]
            public string ControlID { get; }

            [JsonProperty(PropertyName = "name")]
            public string UniqueID { get; }

            [DefaultValue(-1)]
            [JsonProperty(PropertyName = "index", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public int ActiveIndex { get; set; }

            [DefaultValue(true)]
            [JsonProperty(PropertyName = "validators", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public bool ManageValidators { get; set; }

            public ClientSideSettings(string clientId, string uniqueId)
            {
                ControlID = clientId;
                UniqueID = uniqueId;
            }
        }

        #endregion

        #region Properties

        public IReadOnlyList<MultiFieldView> Views => _views;

        private int ActiveIndex
        {
            get => (int)(ViewState[nameof(ActiveIndex)] ?? -1);
            set => ViewState[nameof(ActiveIndex)] = value;
        }

        public bool ManageValidators
        {
            get => (bool)(ViewState[nameof(ManageValidators)] ?? true);
            set => ViewState[nameof(ManageValidators)] = value;
        }

        #endregion

        #region Fields

        private MultiFieldView _activeView = null;
        private List<MultiFieldView> _views = new List<MultiFieldView>();

        #endregion

        #region Loading

        protected override void OnLoad(EventArgs e)
        {
            if (Page.IsPostBack)
            {
                if (int.TryParse(Page.Request.Form[$"{UniqueID}$index"], out var clientIndex))
                {
                    if (clientIndex >= 0 && clientIndex < _views.Count)
                        ActiveIndex = clientIndex;
                }

                if (ActiveIndex != -1)
                    _activeView = _views[ActiveIndex];
            }

            base.OnLoad(e);
        }

        #endregion

        #region Methods

        public bool IsActive(MultiFieldView view)
        {
            return _activeView == view;
        }

        public void SetView(MultiFieldView view)
        {
            var index = _views.IndexOf(view);
            if (index == -1)
                throw ApplicationError.Create("The view is not connected to the multi view.");

            ActiveIndex = index;
            _activeView = _views[index];
        }

        #endregion

        #region PreRender

        protected override void OnPreRender(EventArgs e)
        {
            var settings = new ClientSideSettings(ClientID, UniqueID)
            {
                ManageValidators = ManageValidators,
                ActiveIndex = ActiveIndex
            };

            ScriptManager.RegisterStartupScript(
                Page,
                typeof(MultiField),
                $"init {ClientID}",
                $"inSite.common.multiField.init({JsonHelper.SerializeJsObject(settings)});",
                true);

            base.OnPreRender(e);
        }

        #endregion

        #region Rendering

        protected override void Render(HtmlTextWriter writer)
        {
            AddAttributesToRender(writer);

            writer.AddAttribute(HtmlTextWriterAttribute.Id, ClientID);
            writer.AddAttribute(HtmlTextWriterAttribute.Class, ControlHelper.MergeCssClasses("multi-field", CssClass));

            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            base.Render(writer);

            writer.RenderEndTag();
        }

        protected override void RenderChildren(HtmlTextWriter writer)
        {
            foreach (var view in _views)
                view.RenderControl(writer);
        }

        #endregion

        #region Overriden methods

        protected override void AddedControl(Control control, int index)
        {
            base.AddedControl(control, index);

            if (control is MultiFieldView mfv)
                AddPanel(mfv);
            else if (control is UserControl uc)
                AddPanel(uc.Controls);
        }

        protected override void RemovedControl(Control control)
        {
            base.RemovedControl(control);

            if (control is MultiFieldView mfv)
                RemovePanel(mfv);
            else if (control is UserControl uc)
                RemovePanel(uc.Controls);
        }

        #endregion

        #region Helper methods

        private void AddPanel(MultiFieldView view)
        {
            _views.Add(view);

            view.SetupParent(this);
        }

        private void AddPanel(ControlCollection controls)
        {
            foreach (Control control in controls)
            {
                if (control is MultiFieldView mfv)
                    AddPanel(mfv);
                else if (control.HasControls())
                    AddPanel(control.Controls);
            }
        }

        private void RemovePanel(MultiFieldView panel)
        {
            _views.Remove(panel);
        }

        private void RemovePanel(ControlCollection controls)
        {
            foreach (Control control in controls)
            {
                if (control is MultiFieldView mfv)
                    RemovePanel(mfv);
                else if (control.HasControls())
                    RemovePanel(control.Controls);
            }
        }

        #endregion
    }
}