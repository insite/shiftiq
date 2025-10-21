using Shift.Common;

using Shift.Constant;

namespace InSite.Common.Web.UI
{
    public class OrganizationRoleMultiComboBox : MultiComboBox
    {
        protected override BindingType ControlBinding => BindingType.Code;

        protected override ListItemArray CreateDataSource()
        {
            return new ListItemArray
            {
                OrganizationPersonTypes.Administrator,
                OrganizationPersonTypes.Learner
            };
        }
    }
}