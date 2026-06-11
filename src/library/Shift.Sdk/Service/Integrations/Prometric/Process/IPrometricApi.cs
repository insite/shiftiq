using InSite.Application.Registrations.Read;
using InSite.Domain.Integrations.Prometric;

namespace InSite.Application.Integrations.Prometric
{
    public interface IPrometricApi
    {
        string GetToken();

        bool EligibilityExists(GetEligibilityInput input);
        bool EligibilityExists(SaveEligibilityInput input);
        bool EligibilityExists(QRegistration registration);

        GetEligibilityOutput GetEligibility(GetEligibilityInput input);
        GetEligibilityOutput GetEligibility(QRegistration registration);

        SaveEligibilityOutput SaveEligibility(SaveEligibilityInput input);
        SaveEligibilityOutput DeleteEligibility(SaveEligibilityInput input);

        SaveEligibilityOutput SaveEligibility(QRegistration registration);
        SaveEligibilityOutput DeleteEligibility(QRegistration registration);
    }
}
