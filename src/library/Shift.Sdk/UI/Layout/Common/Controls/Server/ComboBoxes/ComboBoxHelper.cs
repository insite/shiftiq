using System;

using Shift.Common;
using Shift.Constant;

namespace Shift.Sdk.UI
{
    public static class ComboBoxHelper
    {
        public static ListItemArray CreateDataSource<T>(params T[] items) where T : Enum
        {
            var list = new ListItemArray();

            foreach (var item in items)
                list.Add(item.GetName(), item.GetDescription());

            return list;
        }
    }
}