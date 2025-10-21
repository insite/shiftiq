using Shift.Common;

namespace InSite.Common.Web.UI
{
    public class AssetTypeComboBox : ComboBox
    {
        protected override BindingType ControlBinding => BindingType.Code;

        protected override ListItemArray CreateDataSource()
        {
            var resultList = new ListItemArray();
            resultList.Add("Achievement", "Achievement");
            resultList.Add("Standard", "Standard");
            return resultList;
        }
    }
}