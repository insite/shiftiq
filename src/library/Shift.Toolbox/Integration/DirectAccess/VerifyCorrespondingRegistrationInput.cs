namespace Shift.Toolbox.Integration.DirectAccess
{
    public class VerifyCorrespondingRegistrationInput
    {
        public string IndividualId { get; set; }

        public string ExamId { get; set; }

        public VerifyCorrespondingRegistrationInput(string individual, string exam)
        {
            IndividualId = individual;

            ExamId = exam;
        }
    }
}