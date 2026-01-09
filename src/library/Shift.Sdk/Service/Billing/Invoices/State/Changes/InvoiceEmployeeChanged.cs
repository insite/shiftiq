using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Sales.Invoices.Changes
{
    public class InvoiceEmployeeChanged : Change
    {
        public InvoiceEmployeeChanged(Guid employee)
        {
            Employee = employee;
        }

        public Guid Employee { get; set; }
    }
}
