using System.Collections.Specialized;
using System.Web.UI;

using Shift.Common;

namespace InSite.Common.Web.UI
{
    public abstract class BaseCheck : BaseToggle, ICheckBox
    {
        public string Value
        {
            get => (string)ViewState[nameof(Value)];
            set => ViewState[nameof(Value)] = value;
        }

        public BaseCheck() : base("checkbox")
        {

        }

        protected override bool IsChecked(string postDataKey, NameValueCollection postCollection)
        {
            var value = postCollection[postDataKey];

            return value != null && (Value.IsEmpty() || value == Value);
        }

        protected void RenderInput(HtmlTextWriter writer, bool isStandalone)
        {
            base.RenderInput(writer, UniqueID, Value, isStandalone);
        }

        protected void RenderBlock(HtmlTextWriter writer, string cssClass)
        {
            base.RenderBlock(writer, UniqueID, Value, cssClass);
        }
    }
}