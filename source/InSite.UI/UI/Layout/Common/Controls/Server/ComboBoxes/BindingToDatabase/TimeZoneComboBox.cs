using Shift.Common;

namespace InSite.Common.Web.UI
{
    public class TimeZoneComboBox : ComboBox
    {
        protected override BindingType ControlBinding => BindingType.Code;

        protected override ListItemArray CreateDataSource()
        {
            var list = new ListItemArray();

            foreach (var timeZoneId in TimeZones.Supported)
                list.Add(new ListItem { Value = timeZoneId, Text = TimeZones.GetAbbreviation(timeZoneId).DisplayNameWithoutOffset });

            return list;
        }
    }
}
