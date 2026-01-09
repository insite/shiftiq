using Shift.Common.Timeline.Commands;

using InSite.Domain.Banks;

namespace InSite.Application.Banks.Write
{
    public class ImportBank : Command
    {
        public BankState Bank { get; set; }

        public ImportBank(BankState bank)
        {
            AggregateIdentifier = bank.Identifier;
            Bank = bank;
        }
    }
}