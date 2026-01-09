using System;

using InSite.Application.Standards.Read;
using InSite.Persistence;

using Shift.Constant;

namespace InSite.Web.Data
{
    public static class StandardFactory
    {
        public static QStandard Create(string standardType)
        {
            var identity = CurrentSessionState.Identity;
            var organization = identity.Organization;

            var standard = new QStandard
            {
                OrganizationIdentifier = organization.Key,
                StandardType = standardType,
                AssetNumber = Sequence.Increment(organization.OrganizationIdentifier, SequenceType.Asset)
            };
            standard.Created = standard.Modified = DateTimeOffset.UtcNow;
            standard.CreatedBy = standard.ModifiedBy = identity.User.UserIdentifier;
            standard.AuthorDate = DateTime.Today;
            standard.DatePosted = DateTime.Today;
            standard.Sequence = 1;
            return standard;
        }
    }
}