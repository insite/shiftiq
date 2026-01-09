using System;

namespace Shift.Sdk.UI
{
    [Serializable]
    public class DropDownButtonLinkItem : DropDownButtonItem
    {
        public string NavigateUrl { get; set; }
        public string Target { get; set; }
    }
}