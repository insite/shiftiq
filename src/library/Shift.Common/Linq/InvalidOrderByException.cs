using System;

namespace Shift.Common.Linq
{
    public class InvalidOrderByException : Exception
    {
        public InvalidOrderByException(Exception innerException) : base("Invalid OrderBy clause", innerException) { }        
    }
}