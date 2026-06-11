using System;

namespace InSite.Domain.Foundations
{
    [Serializable]
    public class Claim
    {
        public Guid Identifier { get; set; }

        public string Name { get; set; }

        public AccessControl Access { get; set; }

        public Claim(Guid identifier, string name,
            AccessControl access)
        {
            Identifier = identifier;
            Name = name;
            Access = access;
        }

        public Claim(Guid identifier, string name,
            bool read, bool write,
            bool create, bool delete,
            bool administrate, bool configure,
            bool trial)
        {
            Identifier = identifier;
            Name = name;

            Access = new AccessControl
            {
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
