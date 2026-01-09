using System.Web.UI;

using Shift.Sdk.UI;

namespace InSite.Common.Web.UI
{
    internal interface ITextBox : IEditableTextControl, IHasEmptyMessage
    {
        bool Visible { get; set; }
    }
}
