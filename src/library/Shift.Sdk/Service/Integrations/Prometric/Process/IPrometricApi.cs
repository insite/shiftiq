using InSite.Application.Registrations.Read;
using InSite.Domain.Integrations.Prometric;

namespace InSite.Application.Integrations.Prometric
{
    public interface IPrometricApi
    {
        string GetToken();

        bool EligibilityExists(GetEligibilityInput input);
        bool EligibilityExists(SaveEligibilityInput input);
        bool EligibilityExists(QRegistration registration, QRegistrationExamForm examForm);

        GetEligibilityOutput GetEligibility(GetEligibilityInput input);
        GetEligibilityOutput GetEligibility(QRegistration registration, QRegistrationExamForm examForm);

        SaveEligibilityOutput SaveEligibility(SaveEligibilityInput input);
        SaveEligibilityOutput DeleteEligibility(SaveEligibilityInput input);

        SaveEligibilityOutput SaveEligibility(QRegistration registration, QRegistrationExamForm examForm);
        SaveEligibilityOutput DeleteEligibility(QRegistration registration, QRegistrationExamForm examForm);
    }
}
