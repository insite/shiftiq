using System.Web.UI;

namespace InSite.Common.Web.UI
{
    internal interface IRadioButton : ICheckBoxControl
    {
        bool Enabled { get; set; }
        string Text { get; set; }
        string Value { get; set; }
        string GroupName { get; set; }
    }
}
