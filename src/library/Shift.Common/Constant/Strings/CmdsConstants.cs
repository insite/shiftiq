namespace Shift.Constant.CMDS
{
    public static class Comments
    {
        public static string[] Validator = {
            "As a validator, I have personally tested the candidate with respect to the knowledge and the candidate demonstrated the skills components of this competency.",
            "As a validator, I have personally tested the candidate with respect to the knowledge and the candidate simulated the skills components of this competency.",
            "This candidate has adequate experience in this area and has been successfully completing this task.",
            "The candidate has previously been validated and is continuously using the competency.",
            "I agree that this competency is not applicable for this person.",
            "As a validator I have tested this person on their knowledge of this competency and another person (validated in this competency) has seen them perform this competency -- _______ (name)",
            "The candidate's certificate was obtained on _______ (date).",
            "I disagree because the candidate needs training with the knowledge components of this competency.",
            "I disagree because the candidate needs training with the skills of this competency."
        };
    }

    public static class EmployeeAchievementStatuses
    {
        public const string Completed = "Completed";
        public const string NotCompleted = "Not Completed";
        public const string Expired = "Expired";
    }

    public static class AccountScopes
    {
        public const string Partition = "Partition";       // all organization accounts
        public const string Enterprise = "Enterprise";     // root parent organization account
        public const string Organization = "Organization"; // one specific organization account
    }

    public static class SelfAssessedStatuses
    {
        public const string Expired = "Expired";
        public const string NotCompleted = "Not Completed";
        public const string NotApplicable = "Not Applicable";
        public const string NeedsTraining = "Needs Training";
        public const string SelfAssessed = "Self Assessed (Yes)";
    }

    public static class ValidationStatuses
    {
        public const string Expired = "Expired";
        public const string SubmittedForValidation = "Submitted for Validation";
        public const string NotCompleted = "Not Completed";
        public const string NotApplicable = "Not Applicable";
        public const string NeedsTraining = "Needs Training";
        public const string SelfAssessed = "Self-Assessed";
        public const string Validated = "Validated";
    }

    public static class ValidForUnits
    {
        public const string Months = "Months";
        public const string Years = "Years";
    }
}
