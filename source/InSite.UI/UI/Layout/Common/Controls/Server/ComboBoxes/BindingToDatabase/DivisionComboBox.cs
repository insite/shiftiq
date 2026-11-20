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

        // When set, list divisions by name (alphabetical) instead of the default code-first display.
        public bool DisplayNameOnly
        {
            get => (bool)(ViewState[nameof(DisplayNameOnly)] ?? false);
            set => ViewState[nameof(DisplayNameOnly)] = value;
        }

        // When set (name-only lists only), restrict the options to these divisions - e.g. the ones the
        // current user can reach through their department memberships. Null leaves the list unrestricted.
        public Guid[] AllowedDivisionIdentifiers
        {
            get => ViewState[nameof(AllowedDivisionIdentifiers)] as Guid[];
            set => ViewState[nameof(AllowedDivisionIdentifiers)] = value;
        }

        protected override ListItemArray CreateDataSource()
        {
            var data = DisplayNameOnly
                ? DivisionSearch.SelectForDivisionComboBoxByName(OrganizationIdentifier, AllowedDivisionIdentifiers)
                : DivisionSearch.SelectForDivisionComboBox(OrganizationIdentifier);

            return new ListItemArray(data);
        }
    }
}