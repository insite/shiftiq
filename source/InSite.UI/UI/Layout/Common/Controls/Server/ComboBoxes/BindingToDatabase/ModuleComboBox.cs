using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Persistence;

using Shift.Common;

namespace InSite.Common.Web.UI
{
    public class ModuleComboBox : ComboBox
    {
        public Guid UnitID
        {
            get => (Guid)(ViewState[nameof(UnitID)] ?? Guid.Empty);
            set => ViewState[nameof(UnitID)] = value;
        }

        protected override ListItemArray CreateDataSource()
        {
            if (UnitID != null && UnitID != Guid.Empty)
            {
                var data = CourseSearch.SelectModulesForUnit(UnitID).ToList();
                if (data.Count > 0)
                {
                    var items = new List<ListItem>();
                    foreach (var module in data)
                    {
                        items.Add(
                            new ListItem()
                            {
                                Text = module.ModuleName,
                                Value = module.ModuleIdentifier.ToString()
                            });
                    }
                    return new ListItemArray(items);
                }

            }
            return new ListItemArray();
        }
    }
}