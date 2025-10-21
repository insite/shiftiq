using Shift.Common;

namespace InSite.Common.Web.UI
{
    public class PositionTypeComboBox : ComboBox
    {
        protected override BindingType ControlBinding => BindingType.Code;

        protected override ListItemArray CreateDataSource()
        {
            var resultList = new ListItemArray();
            resultList.Add("Not Remote", "Fully on site");
            resultList.Add("Remote", "Fully remote");
            resultList.Add("Potential Remote", "Potential for some remote");
            return resultList;
        }
    }
}