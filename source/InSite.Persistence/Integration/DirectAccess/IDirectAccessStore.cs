using Shift.Toolbox.Integration.DirectAccess;

namespace InSite.Persistence.Integration.DirectAccess
{
    public interface IDirectAccessStore
    {
        void Save(Individual daIndividual);
    }
}