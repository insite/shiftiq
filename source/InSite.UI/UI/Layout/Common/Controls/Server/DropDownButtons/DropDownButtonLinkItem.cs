using System;

using Shift.Sdk.UI;

namespace InSite.Common.Web.UI
{
    [Serializable]
    public class DropDownButtonLinkItem : DropDownButtonItem
    {
        public string NavigateUrl { get; set; }
        public string Target { get; set; }

        protected override void SaveState(IStateWriter writer)
        {
            base.SaveState(writer);

            writer.Add(NavigateUrl);
            writer.Add(Target);
        }

        protected override void LoadState(IStateReader reader)
        {
            base.LoadState(reader);

            reader.Get<string>(x => NavigateUrl = x);
            reader.Get<string>(x => Target = x);
        }
    }
}