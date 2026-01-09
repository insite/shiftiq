using System;

using InSite.Domain.Organizations;

namespace InSite.Application.Organizations.Read
{
    public interface IOrganizationSearch
    {
        QOrganization Get(Guid organization);

        OrganizationState GetModel(Guid organization);
    }
}