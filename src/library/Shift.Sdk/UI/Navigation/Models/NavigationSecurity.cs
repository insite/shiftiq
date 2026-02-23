using System.Collections.Generic;
using System.Linq;

using InSite.Domain.Foundations;

using Shift.Common;
using Shift.Constant;

namespace Shift.Sdk.UI.Navigation
{
    public class NavigationSecurity
    {
        public NavigationSecurityOperator? Operator { get; set; }
        public List<NavigationSecurity> Conditions { get; set; }

        public NavigationSecurityClaim? Claim { get; set; }
        public string Value { get; set; }

        public bool IsLeaf => Claim.HasValue;

        public bool Evaluate(INavigationIdentity identity) =>
            IsLeaf ? EvaluateLeaf(identity) : EvaluateNode(identity);

        private bool EvaluateLeaf(INavigationIdentity identity)
        {
            switch (Claim.Value)
            {
                case NavigationSecurityClaim.IsAdministrator:
                    return identity.IsAdministrator == bool.Parse(Value);

                case NavigationSecurityClaim.IsOperator:
                    return identity.IsOperator == bool.Parse(Value);

                case NavigationSecurityClaim.GroupName:
                    return identity.IsInRole(Value);

                case NavigationSecurityClaim.PermissionName:
                    return identity.IsGranted(Value);

                default:
                    throw ApplicationError.Create("Unexpected security claim: {0}", Claim.Value.GetName());
            }
        }

        private bool EvaluateNode(INavigationIdentity identity)
        {
            switch (Operator.Value)
            {
                case NavigationSecurityOperator.AND:
                    return Conditions.All(x => x.Evaluate(identity));

                case NavigationSecurityOperator.OR:
                    return Conditions.Any(x => x.Evaluate(identity));

                default:
                    throw ApplicationError.Create("Unexpected security operator: {0}", Operator.Value.GetName());
            }
        }

        public NavigationSecurity Clone() => IsLeaf
            ? new NavigationSecurity
            {
                Claim = Claim.Value,
                Value = Value
            }
            : new NavigationSecurity
            {
                Operator = Operator.Value,
                Conditions = Conditions.Select(x => x.Clone()).ToList()
            };
    }
}