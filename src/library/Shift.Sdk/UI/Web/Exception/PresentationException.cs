using System;

namespace Shift.Sdk.UI
{
    public class PresentationException : Exception
    {
        public PresentationException(string message, Exception inner)
            : base(message, inner)
        {
            
        }
    }
}