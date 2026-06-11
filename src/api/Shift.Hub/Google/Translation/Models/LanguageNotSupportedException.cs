namespace Shift.Hub.Google
{
    [Serializable]
    public class LanguageNotSupportedException : Exception
    {
        public LanguageNotSupportedException()
        {
        }

        public LanguageNotSupportedException(string? message) : base(message)
        {
        }

        public LanguageNotSupportedException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
