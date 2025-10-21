using System;

using InSite.Persistence;

using Shift.Common;
using Shift.Constant;

namespace InSite.Common.Web.UI
{
    public class DivisionComboBox : ComboBox
    {
        public Guid OrganizationIdentifier
        {
            get => (Guid)(ViewState[nameof(OrganizationIdentifier)] ?? (ViewState[nameof(OrganizationIdentifier)] = OrganizationIdentifiers.CMDS));
            set => ViewState[nameof(OrganizationIdentifier)] = value;
        }

        protected override ListItemArray CreateDataSource()
        {
            var data = DivisionSearch.SelectForDivisionComboBox(OrganizationIdentifier);

            return new ListItemArray(data);
        }
    }
}