using System;

namespace Shift.Common
{
    public class PandocConverterException : Exception
    {
        public PandocConverterException()
        {

        }

        public PandocConverterException(string message) : base(message)
        {

        }

        public PandocConverterException(string message, Exception inner) : base(message, inner)
        {

        }
    }
}
