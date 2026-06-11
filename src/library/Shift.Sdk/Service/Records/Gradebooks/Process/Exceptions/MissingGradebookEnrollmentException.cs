using System;

namespace InSite.Application.Records
{
    [Serializable]
    internal class MissingGradebookEnrollmentException : Exception
    {
        public MissingGradebookEnrollmentException(Guid learner)
            : this($"Learner {learner} is not enrolled in this gradebook.")
        {

        }

        protected MissingGradebookEnrollmentException(string message)
            : base(message)
        {

        }
    }
}