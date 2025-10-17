using System;

using Shift.Constant;

namespace Shift.Sdk.UI
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
    }
}