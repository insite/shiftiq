using Shift.Common;

namespace InSite.Common.Web.UI
{
    public class DropDownButtonItemTypeMapper : TypeMapper<DropDownButtonBaseItem>
    {
        public static DropDownButtonItemTypeMapper Instance { get; }

        static DropDownButtonItemTypeMapper()
        {
            Instance = new DropDownButtonItemTypeMapper();
        }
    }
}
