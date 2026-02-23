using System;

using InSite.Domain.Foundations;

using Shift.Sdk.UI.Navigation;

namespace InSite.UI.Layout.Admin
{
    internal class NavigationIdentity : INavigationIdentity
    {
        private readonly ISecurityFramework _identity;
        private readonly string _partitionSlug;

        public string PartitionSlug => _partitionSlug;

        public bool IsAdministrator => _identity.IsAdministrator;

        public bool IsOperator => _identity.IsOperator;

        public NavigationIdentity(ISecurityFramework identity, string partitionSlug)
        {
            _identity = identity;
            _partitionSlug = partitionSlug;
        }

        public bool IsGranted(string resource)
        {
            return _identity.IsGranted(resource);
        }

        public bool IsInRole(string role)
        {
            return _identity.IsInRole(role);
        }
    }
}