using System;
using System.Collections.Generic;

namespace Shift.Common
{
    public class Role
    {
        public string Name { get; set; }

        public Guid Identifier { get; set; }

        public Role(string name, Guid? id)
        {
            Name = name;
            Identifier = id
                ?? (
                    !string.IsNullOrEmpty(name)
                        ? SystemRole.GetSystemRoleId(name) ?? UuidFactory.CreateV5(name)
                        : Guid.Empty
                );
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

        public const string Any = Prefix + "Any";
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

        private static readonly Dictionary<string, Guid> _systemRoleIds;
        private static readonly Dictionary<Guid, string> _systemRoleNames;

        static SystemRole()
        {
            _systemRoleIds = new Dictionary<string, Guid>(StringComparer.OrdinalIgnoreCase)
            {
                { Any, Guid.Parse("e8c7eb3b-a8c5-4fcb-937c-2d2c60b07fce") },
                { Guest, Guid.Parse("e40d458c-5748-4b3f-b40c-f8a59558a5ac") },
                { Member, Guid.Parse("f341030a-f596-455c-a1f6-49ccc100a49f") },
                { Trainee, Guid.Parse("11125a5d-aa03-4b10-a544-51c4ecef66d3") },
                { Learner, Guid.Parse("152e1b78-a085-4b5b-b884-613e32c4e14f") },
                { Instructor, Guid.Parse("d8811638-546a-48c8-a899-8b0621fa68b0") },
                { Validator, Guid.Parse("0b8dce43-89fb-4e87-9104-c32d42da1b1a") },
                { Supervisor, Guid.Parse("60dc0ca6-80de-439b-b4a8-72b4e1b1a4bf") },
                { Manager, Guid.Parse("dbb24467-4732-4004-9d2f-6c8d07d389ff") },
                { Administrator, Guid.Parse("58fef6ae-7840-4964-9dd2-1ea095e0ed6e") },
                { Developer, Guid.Parse("f22376c3-c7af-41dd-ae87-921a376fd6f7") },
                { Operator, Guid.Parse("58f8b157-7b68-4fb0-9533-bb5d4ba86ee1") },
            };

            _systemRoleNames = new Dictionary<Guid, string>();
            foreach (var pair in _systemRoleIds)
                _systemRoleNames.Add(pair.Value, pair.Key);
        }

        public static AuthorityAccess ToAccess(string role) =>
            Enum.TryParse<AuthorityAccess>(role.Replace(Prefix, ""), ignoreCase: true, out var access)
                ? access
                : AuthorityAccess.Unspecified;

        public static string ToRole(AuthorityAccess access) => Prefix + access;

        public static Guid? GetSystemRoleId(string systemRoleName)
        {
            return _systemRoleIds.TryGetValue(systemRoleName, out var id)
                ? id
                : (Guid?)null;
        }

        public static string GetSystemRoleName(Guid roleId)
        {
            return _systemRoleNames.TryGetValue(roleId, out var name)
                ? name
                : null;
        }
    }
}