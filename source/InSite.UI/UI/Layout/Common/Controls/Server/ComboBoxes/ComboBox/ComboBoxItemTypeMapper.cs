using Shift.Common;

namespace InSite.Common.Web.UI
{
    public class ComboBoxItemTypeMapper : TypeMapper<ComboBoxItem>
    {
        public static ComboBoxItemTypeMapper Instance { get; }

        static ComboBoxItemTypeMapper()
        {
            Instance = new ComboBoxItemTypeMapper();
        }
    }
}