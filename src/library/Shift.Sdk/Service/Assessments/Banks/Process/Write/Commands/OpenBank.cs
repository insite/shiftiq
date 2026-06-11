using Shift.Common.Timeline.Commands;

using InSite.Domain.Banks;

namespace InSite.Application.Banks.Write
{
    public class OpenBank : Command
    {
        public BankState Bank { get; set; }

        public OpenBank(BankState bank)
        {
            AggregateIdentifier = bank.Identifier;
            Bank = bank;
        }
    }
}