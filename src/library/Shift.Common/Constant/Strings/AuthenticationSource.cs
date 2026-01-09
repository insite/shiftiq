namespace Shift.Constant
{
    public static class AuthenticationSource
    {
        // when the user signs in from the normal sign-in form
        public const string ShiftIq = "Shift iQ";

        // when the user signs in from the exam event sign-in form
        public const string ShiftIqExamEvent = "Shift iQ Exam Event";

        // when the user signs in through the LTI launch form
        public const string LtiLaunch = "LTI Launch";

        // when the user authenticates with Microsoft
        public const string Microsoft = "Microsoft";

        // when the user authenticates with Google
        public const string Google = "Google";

        // when the user is not authenticated
        public const string None = "None";
    }

    public static class SecretType
    {
        public const string Authentication = "Authentication";
    }

    public static class SecretName
    {
        public const string ShiftClientSecret = "Shift iQ Client Secret";
    }
}
