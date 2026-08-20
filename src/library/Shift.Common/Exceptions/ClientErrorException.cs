using System;

namespace Shift.Common.Exceptions
{
    public class ClientErrorException : Exception
    {
        public ClientErrorException(string message) : base(message) { }        
    }
}