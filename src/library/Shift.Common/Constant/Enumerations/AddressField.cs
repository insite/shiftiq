using System;

namespace Shift.Constant
{
    [Flags]
    public enum AddressField
    {
        None = 0,
        All = 2047,

        Description = 1,
        Street1 = 2,
        Street2 = 4,
        City = 8,
        Country = 16,
        Province = 32,
        PostalCode = 64,
        Phone = 128,
        Mobile = 256,
        Fax = 512,
        Email = 1024,
    }
}
