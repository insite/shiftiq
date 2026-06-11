using InSite.Domain.Organizations;

namespace InSite.Application.Organizations.Write
{
    internal interface IHasRun
    {
        bool Run(OrganizationAggregate aggregate);
    }
}
