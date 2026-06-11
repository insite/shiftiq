using InSite.Domain.Organizations;

namespace InSite.Application.Organizations.Write
{
    internal interface IHasAggregate
    {
        OrganizationAggregate Aggregate { get; }
    }
}
