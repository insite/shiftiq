using InSite.Common.Web.UI;

namespace InSite.Admin.Messages.Messages.Controls
{
    public partial class FieldOutputContent : BaseUserControl
    {
        public string Value
        {
            get => MessageContentOutput.InnerText;
            set => MessageContentOutput.InnerText = value;
        }
    }
}