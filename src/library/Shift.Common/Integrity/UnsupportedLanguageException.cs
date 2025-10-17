using System;

namespace Shift.Common
{
    [Serializable]
    public class UnsupportedLanguageException : Exception
    {
        public UnsupportedLanguageException()
        {
        }

        public UnsupportedLanguageException(string message) : base(message)
        {
        }

        public UnsupportedLanguageException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}