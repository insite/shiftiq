using System;

namespace Shift.Common
{
    public class InvalidEmailAddressException : Exception
    {
        public InvalidEmailAddressException(string address)
            : base($"{address} is not a valid email address")
        {

        }
    }
}