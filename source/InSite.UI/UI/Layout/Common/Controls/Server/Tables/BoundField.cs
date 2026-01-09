using Shift.Sdk.UI;

namespace InSite.Common.Web.UI
{
    public class BoundField : System.Web.UI.WebControls.BoundField, IGridFieldHasName
    {
        public string FieldName
        {
            get => (string)ViewState[nameof(FieldName)];
            set => ViewState[nameof(FieldName)] = value;
        }
    }
}