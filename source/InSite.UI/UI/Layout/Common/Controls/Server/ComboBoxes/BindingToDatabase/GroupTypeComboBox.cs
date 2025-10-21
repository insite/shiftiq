using System;
using System.Collections.Generic;

using Shift.Common;
using Shift.Constant;

namespace InSite.Common.Web.UI
{
    public class GroupTypeComboBox : ComboBox
    {
        protected override BindingType ControlBinding => BindingType.Code; 

        public List<string> Exclude => (List<string>)(ViewState[nameof(Exclude)]
            ?? (ViewState[nameof(Exclude)] = new List<string>()));

        public List<string> Include => (List<string>)(ViewState[nameof(Include)]
            ?? (ViewState[nameof(Include)] = new List<string>()));

        public static string[] Values => GroupTypes.Values;

        protected override ListItemArray CreateDataSource()
        {
            var list = new ListItemArray();

            var exclude = Exclude.Count > 0 ? new HashSet<string>(Exclude, StringComparer.OrdinalIgnoreCase) : null;
            var include = Include.Count > 0 ? new HashSet<string>(Include, StringComparer.OrdinalIgnoreCase) : null;

            foreach (var value in Values)
            {
                if ((exclude == null || !exclude.Contains(value)) && (include == null || include.Contains(value)))
                    list.Add(value);
            }

            return list;
        }
    }
}