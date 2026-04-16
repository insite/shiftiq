using System;

using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Common.Web.UI
{
    [Serializable]
    public class DropDownButtonItem : DropDownButtonBaseItem, IHasToolTip, IHasText
    {
        public string Text { get; set; }
        public string ToolTip { get; set; }
        public string IconName { get; set; }
        public IconType IconType { get; set; }
        public bool Enabled { get; set; }
        public bool? CausesValidation { get; internal set; }
        public string ValidationGroup { get; internal set; }

        public DropDownButtonItem()
        {
            IconType = IconType.Solid;
            Enabled = true;
        }

        protected override void SaveState(IStateWriter writer)
        {
            base.SaveState(writer);

            writer.Add(Text);
            writer.Add(ToolTip);
            writer.Add(IconName);
            writer.Add(IconType);
            writer.Add(Enabled);
            writer.Add(CausesValidation);
            writer.Add(ValidationGroup);
        }

        protected override void LoadState(IStateReader reader)
        {
            base.LoadState(reader);

            reader.Get<string>(x => Text = x);
            reader.Get<string>(x => ToolTip = x);
            reader.Get<string>(x => IconName = x);
            reader.Get<IconType>(x => IconType = x);
            reader.Get<bool>(x => Enabled = x);
            reader.Get<bool?>(x => CausesValidation = x);
            reader.Get<string>(x => ValidationGroup = x);
        }
    }
}