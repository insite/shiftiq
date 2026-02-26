namespace Shift.Constant
{
    public static class GroupNames
    {
        // The CMDS group decided to use the term "Orientation" instead of "Skills Passport" for the name of the system
        // role used to identify trainees (i.e. those required to complete onboarding orientation training). The term
        // "Orientation" is self-documenting. For example, the group name "Orientation Users" is more clear than the
        // name "Skills Passport Users". Also, it is much more flexible for open-source use.

        public const string Trainee = "Skills Passport"; // <-- Change this to "Orientation" in the next hotfix
    }
}
