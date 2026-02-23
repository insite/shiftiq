using System;

namespace Shift.Common
{
    public class Role
    {
        public string Name { get; set; }

        public Guid Identifier { get; set; }

        public Role(string name, Guid? id)
        {
            Name = name;
            Identifier = id ?? (!string.IsNullOrEmpty(name) ? UuidFactory.CreateV5(name) : Guid.Empty);
        }

        public Role(string name)
            : this(name, null)
        {

        }

        public Role(Guid id)
            : this(id.ToString(), id)
        {

        }
    }

    /// <summary>
    /// System roles represent the platform-wide AuthorityAccess enumeration values
    /// </summary>
    public static class SystemRole
    {
        private const string Prefix = "(System) ";

        public const string Guest = Prefix + "Guest";
        public const string Member = Prefix + "Member";
        public const string Trainee = Prefix + "Trainee";
        public const string Learner = Prefix + "Learner";
        public const string Instructor = Prefix + "Instructor";
        public const string Validator = Prefix + "Validator";
        public const string Supervisor = Prefix + "Supervisor";
        public const string Manager = Prefix + "Manager";
        public const string Administrator = Prefix + "Administrator";
        public const string Developer = Prefix + "Developer";
        public const string Operator = Prefix + "Operator";

        public static AuthorityAccess ToAccess(string role) =>
            Enum.TryParse<AuthorityAccess>(role.Replace(Prefix, ""), ignoreCase: true, out var access)
                ? access
                : AuthorityAccess.Unspecified;

        public static string ToRole(AuthorityAccess access) => Prefix + access;
    }
}