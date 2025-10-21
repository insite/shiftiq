using System.IO;

using Newtonsoft.Json;

using Shift.Sdk.UI;

namespace InSite.Common.Web.UI
{
    public class ProgressIndicator : ProgressPanelItem
    {
        #region Classes

        private class ClientSideData : ProgressPanelItemClientData
        {
            public override string Type => "pbar";

            [JsonProperty(PropertyName = "caption", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public string Caption { get; set; }

            [JsonProperty(PropertyName = "label", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public string Label { get; set; }

            public ClientSideData(string id) : base(id) { }
        }

        public class ContextData : ProgressPanelContextItem
        {
            public int Total { get; set; }

            public int Value { get; set; }

            public ContextData(string id)
                : base(id)
            {

            }

            protected override void WriteJson(TextWriter output)
            {
                output.Write($"{{\"total\":{Total},\"value\":{Value}}}");
            }
        }

        #endregion

        #region Properties

        public string Caption { get; set; }

        public string Label { get; set; }

        #endregion

        #region Methods

        public override ProgressPanelItemClientData GetClientData()
        {
            return new ClientSideData(Name)
            {
                Caption = Caption,
                Label = Label
            };
        }

        public override ProgressPanelContextItem GetContextData()
        {
            return new ContextData(Name);
        }

        #endregion

        #region IStateManager

        protected override void SaveState(IStateWriter writer)
        {
            base.SaveState(writer);

            writer.Add(Caption);
            writer.Add(Label);
        }

        protected override void LoadState(IStateReader reader)
        {
            base.LoadState(reader);

            reader.Get<string>(x => Caption = x);
            reader.Get<string>(x => Label = x);
        }

        #endregion
    }
}