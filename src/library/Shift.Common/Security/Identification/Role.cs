using System;
using System.Collections.Generic;

namespace Shift.Common
{
    public class Role
    {
        public Guid Identifier { get; set; }

        public string Name { get; set; }

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

    public class RolePermissions
    {
        public string Role { get; set; }
        public List<ResourceAccessBundle> Permissions { get; set; } = new List<ResourceAccessBundle>();
    }

    public class RoleAccessBundle
    {
        public List<string> Roles { get; set; } = new List<string>();
        public List<string> Access { get; set; } = new List<string>();
    }

    /// <summary>
    /// System roles represent the platform-wide AuthorityAccess enumeration values
    /// </summary>
    public static class SystemRole
    {
        private const string Prefix = "(System) ";

        public const string Visitor = Prefix + "Visitor";
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
                : AuthorityAccess.None;

        public static string ToRole(AuthorityAccess access) => Prefix + access;
    }
}