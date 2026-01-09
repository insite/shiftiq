using Shift.Sdk.UI;

namespace InSite.Common.Web.UI
{
    public class TemplateField : System.Web.UI.WebControls.TemplateField, IGridFieldHasName
    {
        public string FieldName
        {
            get => (string)ViewState[nameof(FieldName)];
            set => ViewState[nameof(FieldName)] = value;
        }
    }
}