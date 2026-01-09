using System;

namespace InSite.Persistence
{
    public class IntegrationException : Exception
    {
        public IntegrationException(string s) : base(s) { }

        public IntegrationException(string s, Exception exception) : base(s, exception) { }
    }
}