using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Application.Contacts.Read;
using InSite.Common.Web.UI;
using InSite.Persistence;

namespace InSite.UI.Portal.Contacts.Groups.Controls
{
    public partial class EmployerEditor : UserControl
    {
        public void SetInputValues(Guid? entityId)
        {
            if (entityId.HasValue && entityId.Value != Guid.Empty)
            {
                var entity = VGroupEmployerSearch.SelectByGroup(entityId.Value);
                if (entity != null)
                    SetInputValues(entity);
            }
        }

        public void SetInputValues(VGroupEmployer entity)
        {
            IndustriesComboBox.EnsureDataBound();
            NumberOfEmployees.EnsureDataBound();
            SectorComboBox.OrganizationIdentifier = entity.OrganizationIdentifier;
            SectorComboBox.EnsureDataBound();

            CompanyName.Text = entity.GroupName;
            IndustriesComboBox.Value = entity.GroupIndustry;
            NumberOfEmployees.Value = entity.GroupSize;
            SectorComboBox.Value = entity.CompanySector;
            Phone.Text = entity.GroupPhone;
        }

        public void GetInputValues(QGroup entity, out string sector)
        {
            entity.GroupName = CompanyName.Text;
            entity.GroupIndustry = IndustriesComboBox.Value;
            entity.GroupSize = NumberOfEmployees.Value;
            entity.GroupPhone = Phone.Text;

            sector = SectorComboBox.Value;
        }

        public void EnabledPanelContents(AccordionPanel panel, bool enabled)
        {
            foreach (WebControl ctrl in this.Controls.OfType<WebControl>())
            {
                ctrl.Enabled = enabled;
            }
        }
    }
}