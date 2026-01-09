using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Sales.Invoices.Write.Commands
{
    public class ChangeBusinessCustomer : Command
    {
        public ChangeBusinessCustomer(Guid invoiceIdentifier, Guid? businessCustomer)
        {
            AggregateIdentifier = invoiceIdentifier;
            BusinessCustomer = businessCustomer;
        }

        public Guid? BusinessCustomer { get; set; }
    }

   
}
