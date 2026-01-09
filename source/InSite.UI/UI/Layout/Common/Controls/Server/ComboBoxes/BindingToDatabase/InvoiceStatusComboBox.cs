using System;

using Shift.Common;
using Shift.Constant;

namespace InSite.Common.Web.UI
{
    public class InvoiceStatusComboBox : ComboBox
    {
        protected override ListItemArray CreateDataSource()
        {
            var list = new ListItemArray();

            foreach (InvoiceStatusType item in Enum.GetValues(typeof(InvoiceStatusType)))
                list.Add(item.GetName(), item.GetDescription());

            return list;
        }
    }
}