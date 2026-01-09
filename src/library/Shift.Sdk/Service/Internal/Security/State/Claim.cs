using System;

using Shift.Constant;

namespace InSite.Domain.Foundations
{
    [Serializable]
    public class Claim
    {
        public Guid Identifier { get; set; }

        public ActionType Type { get; set; }

        public string Name { get; set; }

        public AccessControl Access { get; set; }

        public Claim(Guid identifier, ActionType type, string name,
            AccessControl access)
        {
            Identifier = identifier;
            Type = type;
            Name = name;
            Access = access;
        }

        public Claim(Guid identifier, ActionType type, string name,
            bool execute,
            bool read, bool write,
            bool create, bool delete,
            bool administrate, bool configure,
            bool trial)
        {
            Identifier = identifier;
            Type = type;
            Name = name;

            Access = new AccessControl
            {
                Execute = execute,

                Read = read,
                Write = write,

                Create = create,
                Delete = delete,

                Administrate = administrate,
                Configure = configure,

                Trial = trial
            };
        }
    }
}
