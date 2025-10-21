using System;

namespace InSite.Persistence.Integration.DirectAccess
{
    public class DirectAccessException : Exception
    {
        public DirectAccessException(string message) 
            : base(message)
        {

        }
    }
}
