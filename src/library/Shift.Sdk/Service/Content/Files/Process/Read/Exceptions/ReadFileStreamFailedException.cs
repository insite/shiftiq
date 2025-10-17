using System;
using System.Runtime.Serialization;

namespace InSite.Application.Files.Read
{
    [Serializable]
    public class ReadFileStreamFailedException : Exception
    {
        public ReadFileStreamFailedException(string filePath, Exception innerException)
            : base($"ReadFileStream failed: '{filePath}'", innerException)
        {
        }

        protected ReadFileStreamFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
