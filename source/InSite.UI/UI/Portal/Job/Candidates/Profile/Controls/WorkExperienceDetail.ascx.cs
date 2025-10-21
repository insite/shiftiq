using System.Web.UI;

using InSite.Persistence;

namespace InSite.UI.Portal.Jobs.Candidates.MyPortfolio.Controls
{
    public partial class WorkExperienceDetail : UserControl
    {
        public void BindDefaults()
        {
            CountrySelector.Value = ServiceLocator.CountrySearch.SelectByName("Canada")?.Identifier;
        }

        public void BindModelToControls(TCandidateExperience entity)
        {
            ExperienceJobTitle.Text = entity.ExperienceJobTitle;
            EmployerName.Text = entity.EmployerName;
            EmployerDescription.Text = entity.EmployerDescription;
            DateFrom.Value = entity.ExperienceDateFrom;
            DateTo.Value = entity.ExperienceDateTo;
            ExperienceCity.Text = entity.ExperienceCity;

            var countryName = (entity.ExperienceCountry != null ? entity.ExperienceCountry : (CurrentSessionState.Identity.Organization.PlatformCustomization.TenantLocation.Country ?? "Canada"));
            CountrySelector.Value = ServiceLocator.CountrySearch.SelectByName(countryName)?.Identifier;
        }

        public void BindControlsToModel(TCandidateExperience entity)
        {
            entity.ExperienceJobTitle = ExperienceJobTitle.Text;
            entity.EmployerName = EmployerName.Text;
            entity.EmployerDescription = EmployerDescription.Text;
            entity.ExperienceDateFrom = DateFrom.Value.Value;
            entity.ExperienceDateTo = DateTo.Value;
            entity.ExperienceCity = ExperienceCity.Text;
            entity.ExperienceCountry = ServiceLocator.CountrySearch.SelectById(CountrySelector.Value)?.Name ?? string.Empty;
        }

    }
}