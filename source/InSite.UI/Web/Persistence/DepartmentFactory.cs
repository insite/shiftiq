using System;

using InSite.Persistence;

namespace InSite.Web.Data
{
    public class DepartmentFactory
    {
        public static Department Create(Guid id, Guid? creator = null)
        {
            var identity = CurrentSessionState.Identity;
            var organization = identity.Organization;

            var department = new Department();
            department.DepartmentIdentifier = id;
            department.OrganizationIdentifier = organization.OrganizationIdentifier;
            return department;
        }
    }
}