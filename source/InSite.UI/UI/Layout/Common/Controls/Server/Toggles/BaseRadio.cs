using System.Collections.Specialized;
using System.Web.UI;

using Shift.Common;

namespace InSite.Common.Web.UI
{
    public class BaseRadio : BaseToggle, IRadioButton
    {
        public string GroupName
        {
            get => (string)ViewState[nameof(GroupName)];
            set => ViewState[nameof(GroupName)] = value;
        }

        public string Value
        {
            get => (string)ViewState[nameof(Value)];
            set => ViewState[nameof(Value)] = value;
        }

        public bool AllowCheckedPostBack
        {
            get => (bool)(ViewState[nameof(AllowCheckedPostBack)] ?? false);
            set => ViewState[nameof(AllowCheckedPostBack)] = value;
        }

        private string InputName => GroupName.IfNullOrEmpty(UniqueID);

        public BaseRadio() : base("radio")
        {

        }

        protected override bool IsChecked(string postDataKey, NameValueCollection postCollection)
        {
            var value = postCollection[InputName];

            return string.Compare(value, UniqueID) == 0;
        }

        protected void RenderInput(HtmlTextWriter writer, bool isStandalone)
        {
            base.RenderInput(writer, InputName, UniqueID, isStandalone);
        }

        protected void RenderBlock(HtmlTextWriter writer, string cssClass)
        {
            base.RenderBlock(writer, InputName, UniqueID, cssClass);
        }

        protected override string GetPostBackScript()
        {
            return !AllowCheckedPostBack && Checked ? null : base.GetPostBackScript();
        }
    }
}