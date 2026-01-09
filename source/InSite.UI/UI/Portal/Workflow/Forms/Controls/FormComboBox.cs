using System;

namespace InSite.UI.Portal.Workflow.Forms.Controls
{
    public class FormComboBox : Common.Web.UI.ComboBox
    {
        public Guid? ListIdentifier
        {
            get { return (Guid?)ViewState[nameof(ListIdentifier)]; }
            set { ViewState[nameof(ListIdentifier)] = value; }
        }
    }
}