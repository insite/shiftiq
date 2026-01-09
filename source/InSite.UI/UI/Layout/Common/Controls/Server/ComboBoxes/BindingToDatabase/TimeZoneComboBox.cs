using Shift.Common;

namespace InSite.Common.Web.UI
{
    public class TimeZoneComboBox : ComboBox
    {
        protected override BindingType ControlBinding => BindingType.Code;

        protected override ListItemArray CreateDataSource()
        {
            var list = new ListItemArray();

            foreach (var item in TimeZones.Supported)
                list.Add(item.Id);

            return list;
        }
    }
}