using System;
using System.Collections.Generic;
using System.Linq;

using Shift.Common;

namespace InSite.Domain.Foundations
{
    [Serializable]
    public class ClaimList
    {
        public bool IsEmpty => _claims.Count == 0;

        public int Count => _claims.Count;

        private readonly Dictionary<Guid, Claim> _claims;

        public ClaimList()
        {
            _claims = new Dictionary<Guid, Claim>();
        }

        public void Add(Guid identifier, string name, bool read, bool write, bool create, bool delete, bool administrate, bool configure, bool trial)
        {
            if (!_claims.ContainsKey(identifier))
            {
                var item = new Claim(identifier, name, read, write, create, delete, administrate, configure, trial);
                _claims.Add(identifier, item);
            }
            else
            {
                var access = _claims[identifier].Access;
                access.Read = access.Read || read;
                access.Write = access.Write || write;
                access.Create = access.Create || create;
                access.Delete = access.Delete || delete;
                access.Administrate = access.Administrate || administrate;
                access.Configure = access.Configure || configure;
                access.Trial = access.Trial || trial;
                _claims[identifier] = new Claim(identifier, name, access);
            }
        }

        public bool Contains(string name)
        {
            return _claims.Any(x => StringHelper.Equals(x.Value.Name, name));
        }

        public bool Contains(Guid identifier)
        {
            return _claims.ContainsKey(identifier);
        }

        public bool IsGranted(Guid identifier, DataAccess operation)
        {
            if (!Contains(identifier))
                return false;

            var claim = _claims[identifier];
            return Allow(claim, operation);
        }

        public bool IsGranted(string name, DataAccess operation)
        {
            if (!Contains(name))
                return false;

            var claim = _claims.Values.FirstOrDefault(x => StringHelper.Equals(x.Name, name));
            return Allow(claim, operation);
        }

        public bool IsGranted(string name, FeatureAccess operation)
        {
            if (!Contains(name))
                return false;

            var claim = _claims.Values.FirstOrDefault(x => StringHelper.Equals(x.Name, name));
            return Allow(claim, operation);
        }

        private bool Allow(Claim claim, DataAccess operation)
        {
            if (claim == null)
                return false;

            switch (operation)
            {
                case DataAccess.Read: return claim.Access.Read;
                case DataAccess.Update: return claim.Access.Write;
                case DataAccess.Create: return claim.Access.Create;
                case DataAccess.Delete: return claim.Access.Delete;
                case DataAccess.Administrate: return claim.Access.Administrate;
                case DataAccess.Configure: return claim.Access.Configure;
                default: return false;
            }
        }

        private bool Allow(Claim claim, FeatureAccess operation)
        {
            if (claim == null)
                return false;

            switch (operation)
            {
                case FeatureAccess.Trial: return claim.Access.Trial;
                case FeatureAccess.Use: return claim.Access.Trial;
                default: return false;
            }
        }

        public SortedList<string, string> ToSortedList()
        {
            var result = new SortedList<string, string>();

            foreach (var claim in _claims)
                result.Add(claim.Value.Name, claim.ToString());

            return result;
        }
    }
}
