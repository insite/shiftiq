using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Shift.Sdk.UI
{
    [Serializable]
    public class DropDownButtonItemCollection : Collection<DropDownButtonBaseItem>
    {
        public DropDownButtonBaseItem this[string name] => Items.FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase));
    }
}