using System;

namespace InSite.Application.Records
{
    [Serializable]
    internal class MissingGradeItemException : Exception
    {
        public MissingGradeItemException(Guid item)
            : this($"Grade item {item} is not in this gradebook.")
        {

        }

        protected MissingGradeItemException(string message)
            : base(message)
        {

        }
    }
}