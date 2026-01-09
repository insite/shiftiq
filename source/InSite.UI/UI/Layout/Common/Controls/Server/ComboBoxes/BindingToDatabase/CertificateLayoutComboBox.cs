using InSite.Persistence;

using Shift.Common;

namespace InSite.Common.Web.UI
{
    public class CertificateLayoutComboBox : ComboBox
    {
        protected override ListItemArray CreateDataSource()
        {
            var organizationId = CurrentSessionState.Identity.Organization.Identifier;

            var organization = OrganizationSearch.Select(organizationId);

            // Certificate layouts in the E03 (CMDS) partition are partition-wide.

            var items = ServiceLocator.Partition.IsE03()
                ? TCertificateLayoutSearch.Bind(
                    x => new ListItem
                    {
                        Value = x.CertificateLayoutCode,
                        Text = x.CertificateLayoutCode
                    },
                    x => true)
                : TCertificateLayoutSearch.Bind(
                    x => new ListItem
                    {
                        Value = x.CertificateLayoutCode,
                        Text = x.CertificateLayoutCode
                    },
                    x => x.OrganizationIdentifier == organization.OrganizationIdentifier);

            return new ListItemArray(items);
        }
    }
}