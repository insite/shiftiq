using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Sales.Invoices.Write.Commands
{
    public class ChangeEmployee : Command
    {
        public ChangeEmployee(Guid invoiceIdentifier, Guid employee)
        {
            AggregateIdentifier = invoiceIdentifier;
            Employee = employee;
        }

        public Guid Employee { get; set; }
    }
}
