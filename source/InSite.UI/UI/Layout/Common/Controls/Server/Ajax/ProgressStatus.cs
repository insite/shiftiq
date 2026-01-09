using System;
using System.Web.UI;

using Newtonsoft.Json;

using Shift.Sdk.UI;

namespace InSite.Common.Web.UI
{
    [ParseChildren(true, nameof(Text))]
    public class ProgressStatus : ProgressPanelItem, IHasText
    {
        #region Classes

        private class ClientSideData : ProgressPanelItemClientData
        {
            public override string Type => "status";

            [JsonProperty(PropertyName = "text", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public string Text { get; set; }

            public ClientSideData(string id) : base(id) { }
        }

        #endregion

        #region Properties

        [PersistenceMode(PersistenceMode.InnerDefaultProperty)]
        public string Text { get; set; }

        public override bool HasContextItem => false;

        #endregion

        #region Methods

        public override ProgressPanelItemClientData GetClientData()
        {
            return new ClientSideData(Name)
            {
                Text = Text
            };
        }

        public override ProgressPanelContextItem GetContextData()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IStateManager

        protected override void SaveState(IStateWriter writer)
        {
            base.SaveState(writer);

            writer.Add(Text);
        }

        protected override void LoadState(IStateReader reader)
        {
            base.LoadState(reader);

            reader.Get<string>(x => Text = x);
        }

        #endregion
    }
}