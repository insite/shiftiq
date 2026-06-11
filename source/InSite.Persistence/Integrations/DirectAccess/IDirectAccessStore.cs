using Shift.Toolbox.Integrations.DirectAccess;

namespace InSite.Persistence.Integration.DirectAccess
{
    public interface IDirectAccessStore
    {
        void Save(Individual daIndividual);
    }
}