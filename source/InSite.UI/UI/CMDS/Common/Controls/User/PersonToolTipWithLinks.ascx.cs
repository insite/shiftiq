using System;
using System.Web.UI;

namespace InSite.Cmds.Controls.Contacts.Persons
{
    public partial class PersonToolTipWithLinks : UserControl
    {
        public Guid UserIdentifier
        {
            get { return ViewState[nameof(UserIdentifier)] == null ? Guid.Empty : (Guid)ViewState[nameof(UserIdentifier)]; }
            set { ViewState[nameof(UserIdentifier)] = value; }
        }

        public string FullName
        {
            get { return (string)ViewState[nameof(FullName)]; }
            set { ViewState[nameof(FullName)] = value; }
        }
    }
}