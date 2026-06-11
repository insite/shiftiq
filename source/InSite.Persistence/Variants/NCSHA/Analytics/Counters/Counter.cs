using System;

namespace InSite.Persistence.Plugin.NCSHA
{
    public class Counter
    {
        public Guid CounterIdentifier { get; set; }

        public string Category { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Scope { get; set; }
        public int Year { get; set; }
        public decimal Value { get; set; }
        public string Unit { get; set; }
    }
}