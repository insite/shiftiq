using System;

using InSite.Common.Web.UI;

namespace InSite.Custom.CMDS.Common.Controls.Server
{
    // Keeps a division combo and a department finder in step: the combo lists only the divisions the
    // user can reach a department in, so selecting one never lands on an empty department finder. Both
    // the shared TrainingReportCriteria control and the standalone User Training Details report bind
    // through here, so the scoping rule cannot drift between them. Call after the finder's Filter is
    // configured (organization, and the per-user restriction when the caller applies one).
    public static class DivisionScoping
    {
        public static void Bind(DivisionComboBox divisionCombo, FindDepartment finder, Guid organizationIdentifier)
        {
            divisionCombo.OrganizationIdentifier = organizationIdentifier;
            divisionCombo.AllowedDivisionIdentifiers = finder.GetAccessibleDivisionIdentifiers();
            divisionCombo.RefreshData();
        }
    }
}
