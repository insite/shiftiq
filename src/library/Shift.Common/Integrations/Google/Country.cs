using System;

namespace Shift.Common
{
    public class Country
    {
        public Country()
        {
            Code = "--";
            Name = "None";
            Identifier = Guid.Empty;

        }

        public Country(string code, string name, Guid identifier)
        {
            Code = code;
            Name = name;
            Identifier = identifier;
        }

        public string Code { get; set; }
        public string Name { get; set; }
        public Guid Identifier { get; set; }
    }

    public class Province
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        public string Translations { get; set; }
    }
}
