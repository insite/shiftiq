using System;

namespace InSite.Persistence.Integration.DirectAccess
{
    public interface IDirectAccessSearch
    {
        string GetUniqueEmail(string daIndividualEmail, Guid daIndividualId, string shiftUserEmail, Guid shiftUserId);
    }
}