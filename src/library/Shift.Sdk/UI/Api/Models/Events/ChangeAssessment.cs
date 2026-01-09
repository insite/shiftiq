namespace Shift.Sdk.UI
{
    /// <summary>
    /// This is the data transfer object for changing the assessment form assigned to an existing event registration.
    /// </summary>
    public class ChangeAssessment
    {
        /// <summary>
        /// An alphanumeric code that should (in theory) uniquely identify an assessment form.
        /// </summary>
        public string Assessment { get; set; }
    }
}