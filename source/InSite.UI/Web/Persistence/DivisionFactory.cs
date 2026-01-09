using System;

using InSite.Persistence;

namespace InSite.Web.Data
{
    public class DivisionFactory
    {
        public static Division Create(Guid id, Guid? creator = null)
        {
            var identity = CurrentSessionState.Identity;
            var organization = identity.Organization;

            var division = new Division();
            division.DivisionIdentifier = id;
            division.OrganizationIdentifier = organization.OrganizationIdentifier;
            return division;
        }
    }
}