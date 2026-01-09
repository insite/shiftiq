using System.Web.UI;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Sdk.UI;

namespace InSite.Common.Web.UI
{
    public class ProgressControlTrigger : ProgressPanelTrigger
    {
        #region Classes

        private class ClientSideData : ProgressPanelTriggerClientData
        {
            public override string Type => "ctrl";

            [JsonProperty(PropertyName = "target")]
            public string UniqueID { get; set; }
        }

        #endregion

        #region Properties

        public string ControlID
        {
            get => _controlId;
            set
            {
                if (_controlId != value)
                    _uniqueId = null;

                _controlId = value;
            }
        }

        #endregion

        #region Fields

        private string _controlId;
        private string _uniqueId;
        private Control _control;

        #endregion

        #region Construction

        public ProgressControlTrigger()
        {

        }

        public ProgressControlTrigger(Control control)
        {
            _control = control;
        }

        #endregion

        #region Methods

        public override ProgressPanelTriggerClientData GetClientData(Control container)
        {
            if (_uniqueId.IsEmpty())
            {
                if (_control == null)
                {
                    if (_controlId.IsEmpty())
                        throw ApplicationError.Create("ProgressControlTrigger.ControlID is null");

                    _control = ControlHelper.GetControlSafe(container, _controlId);
                }

                if (_control == null)
                    throw ApplicationError.Create("Control not found: " + _controlId);

                if (!(_control is IPostBackDataHandler) && !(_control is IPostBackEventHandler))
                    throw ApplicationError.Create("Founded control can't be used as a trigger because it can't raise postback: " + _controlId);

                _uniqueId = _control.UniqueID;
            }

            return new ClientSideData
            {
                UniqueID = _uniqueId
            };
        }

        #endregion

        #region IStateManager

        protected override void SaveState(IStateWriter writer)
        {
            writer.Add(_controlId);
            writer.Add(_uniqueId);
        }

        protected override void LoadState(IStateReader reader)
        {
            reader.Get<string>(x => _controlId = x);
            reader.Get<string>(x => _uniqueId = x);
        }

        #endregion
    }
}