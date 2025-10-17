using System;

namespace InSite.Application.Records
{
    [Serializable]
    internal class MissingGradebookException : Exception
    {
        public MissingGradebookException(Guid gradebook)
            : this($"Gradebook {gradebook} is not found.")
        {

        }

        protected MissingGradebookException(string message)
            : base(message)
        {

        }
    }
}
