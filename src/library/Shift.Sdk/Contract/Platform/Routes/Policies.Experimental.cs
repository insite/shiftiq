namespace Shift.Contract
{
    public static partial class Policies
    {
        public static partial class Directory
        {
            public static partial class People
            {
                public static partial class Person
                {
                    public const string Write = "Directory.People.Person.Write";
                }
            }
        }

        public static partial class Progress
        {
            public static partial class Gradebooks
            {
                public static partial class Gradebook
                {
                    public const string Read = "Progress.Gradebooks.Gradebook.Read";
                }
            }
        }

        public static partial class Variant
        {
            public static partial class Cmds
            {
                public static partial class Reporting
                {
                    public const string Read = "Variant.Cmds.Reporting.Read";
                }
            }
        }
    }
}