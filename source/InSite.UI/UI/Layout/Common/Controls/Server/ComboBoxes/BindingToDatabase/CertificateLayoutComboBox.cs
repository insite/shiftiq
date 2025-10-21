using InSite.Persistence;

using Shift.Common;
using Shift.Constant;

namespace InSite.Common.Web.UI
{
    public class CertificateLayoutComboBox : ComboBox
    {
        protected override ListItemArray CreateDataSource()
        {
            var organization = OrganizationSearch.Select(CurrentSessionState.Identity.Organization.Identifier);

            var items = ServiceLocator.Partition.IsE03()
                ? TCertificateLayoutSearch.Bind(
                    x => new ListItem
                    {
                        Value = x.CertificateLayoutCode,
                        Text = x.CertificateLayoutCode
                    },
                    x => x.OrganizationIdentifier == organization.OrganizationIdentifier 
                      || x.OrganizationIdentifier == OrganizationIdentifiers.CMDS
                      || x.OrganizationIdentifier == OrganizationIdentifiers.Keyera)
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