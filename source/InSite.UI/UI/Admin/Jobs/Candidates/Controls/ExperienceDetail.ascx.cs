using System;
using System.Web.UI;

using InSite.Persistence;

namespace InSite.UI.Admin.Jobs.Candidates.Controls
{
    public partial class ExperienceDetail : UserControl
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        public void GetInputValues(TCandidateExperience entity)
        {
            entity.ExperienceDateFrom = DateFrom.Value.Value;
            entity.ExperienceDateTo = DateTo.Value;
            entity.EmployerName = EmployerName.Text;
            entity.EmployerDescription = EmployerDescription.Text;
            entity.ExperienceJobTitle = JobTitle.Text;
            entity.ExperienceCountry = ServiceLocator.CountrySearch.SelectById(CountrySelector.Value)?.Name ?? string.Empty;
            entity.ExperienceCity = City.Text;
        }

        public void SetInputValues(TCandidateExperience entity)
        {
            DateFrom.Value = entity.ExperienceDateFrom;
            DateTo.Value = entity.ExperienceDateTo;
            EmployerName.Text = entity.EmployerName;
            EmployerDescription.Text = entity.EmployerDescription;
            JobTitle.Text = entity.ExperienceJobTitle;
            City.Text = entity.ExperienceCity;

            var countryName = (entity.ExperienceCountry != null ? entity.ExperienceCountry : (CurrentSessionState.Identity.Organization.PlatformCustomization.TenantLocation.Country ?? "Canada"));
            CountrySelector.Value = ServiceLocator.CountrySearch.SelectByName(countryName)?.Identifier;
        }
    }
}