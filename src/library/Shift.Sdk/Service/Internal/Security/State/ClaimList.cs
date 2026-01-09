using System;
using System.Collections.Generic;
using System.Linq;

using Shift.Common;
using Shift.Constant;

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

        public void Add(Guid identifier, string type, string name, bool execute, bool read, bool write, bool create, bool delete, bool administrate, bool configure, bool trial)
        {
            var claimType = type.ToEnum(ActionType.Action);

            if (!_claims.ContainsKey(identifier))
            {
                var item = new Claim(identifier, claimType, name, execute, read, write, create, delete, administrate, configure, trial);
                _claims.Add(identifier, item);
            }
            else
            {
                var access = _claims[identifier].Access;
                access.Execute = access.Execute || execute;
                access.Read = access.Read || read;
                access.Write = access.Write || write;
                access.Create = access.Create || create;
                access.Delete = access.Delete || delete;
                access.Administrate = access.Administrate || administrate;
                access.Configure = access.Configure || configure;
                access.Trial = access.Trial || trial;
                _claims[identifier] = new Claim(identifier, claimType, name, access);
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

        public bool IsGranted(Guid identifier, PermissionOperation operation)
        {
            if (!Contains(identifier))
                return false;

            var claim = _claims[identifier];
            return Allow(claim, operation);
        }

        public bool IsTrial(Guid identifier)
        {
            return Contains(identifier) && _claims[identifier].Access.Trial;
        }

        public bool IsGranted(string name, PermissionOperation operation)
        {
            if (!Contains(name))
                return false;

            var claim = _claims.Values.FirstOrDefault(x => StringHelper.Equals(x.Name, name));
            return Allow(claim, operation);
        }

        private bool Allow(Claim claim, PermissionOperation operation)
        {
            if (claim == null)
                return false;

            switch (operation)
            {
                case PermissionOperation.Execute: return claim.Access.Execute;
                case PermissionOperation.Read: return claim.Access.Read;
                case PermissionOperation.Write: return claim.Access.Write;
                case PermissionOperation.Create: return claim.Access.Create;
                case PermissionOperation.Delete: return claim.Access.Delete;
                case PermissionOperation.Administrate: return claim.Access.Administrate;
                case PermissionOperation.Configure: return claim.Access.Configure;
                default: return false;
            }
        }

        public SortedList<string, string> ToSortedList()
        {
            var result = new SortedList<string, string>();

            foreach (var claim in _claims)
                result.Add($"{claim.Value.Type}:{claim.Value.Name}", claim.ToString());

            return result;
        }
    }
}
