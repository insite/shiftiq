using InSite.Common.Web.UI;
using InSite.Persistence;

namespace InSite.Admin.Identities.Divisions.Controls
{
    public partial class Details : BaseUserControl
    {
        public void SetDefaultInputValues()
        {
            OrganizationIdentifier.Value = Organization.Key;
        }

        public void SetInputValues(Division entity)
        {
            OrganizationIdentifier.Value = entity.OrganizationIdentifier;
            DivisionName.Text = entity.DivisionName;
            DivisionCode.Text = entity.DivisionCode;
            DivisionDescription.Text = entity.DivisionDescription;
        }

        public void GetInputValues(Division entity)
        {
            entity.OrganizationIdentifier = OrganizationIdentifier.Value.Value;
            entity.DivisionName = DivisionName.Text;
            entity.DivisionCode = DivisionCode.Text;
            entity.DivisionDescription = DivisionDescription.Text;
        }
    }
}