using System.Web.UI;

namespace InSite.Common.Web.UI
{
    internal interface ICheckBox : ICheckBoxControl
    {
        string Value { get; set; }
        bool Enabled { get; set; }
        string Text { get; set; }
    }
}
