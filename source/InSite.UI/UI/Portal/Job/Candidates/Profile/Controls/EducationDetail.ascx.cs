using System;
using System.Web.UI;

using InSite.Persistence;

using Shift.Constant;

namespace InSite.UI.Portal.Jobs.Candidates.MyPortfolio.Controls
{
    public partial class EducationDetail : UserControl
    {
        public void BindDefaults()
        {
            BindQualifications();

            CountrySelector.Value = ServiceLocator.CountrySearch.SelectByName("Canada")?.Identifier;
        }

        public void BindModelToControls(TCandidateEducation entity)
        {
            BindQualifications();

            EducationQualification.Value = entity.EducationQualification;
            EducationName.Text = entity.EducationName;
            EducationInstitution.Text = entity.EducationInstitution;
            EducationDateFrom.Value = entity.EducationDateFrom;
            EducationDateTo.Value = entity.EducationDateTo;

            if (string.Equals(entity.EducationStatus, "complete", StringComparison.OrdinalIgnoreCase))
                StatusComplete.Checked = true;
            else if (string.Equals(entity.EducationStatus, "incomplete", StringComparison.OrdinalIgnoreCase))
                StatusIncomplete.Checked = true;
            else if (string.Equals(entity.EducationStatus, "inprogress", StringComparison.OrdinalIgnoreCase))
                StatusInProgress.Checked = true;

            ExperienceCity.Text = entity.EducationCity;

            var countryName = (entity.EducationCountry != null ? entity.EducationCountry : (CurrentSessionState.Identity.Organization.PlatformCustomization.TenantLocation.Country ?? "Canada"));
            CountrySelector.Value = ServiceLocator.CountrySearch.SelectByName(countryName)?.Identifier;
        }

        public void BindControlsToModel(TCandidateEducation entity)
        {
            entity.EducationQualification = EducationQualification.Value;
            entity.EducationName = EducationName.Text;
            entity.EducationInstitution = EducationInstitution.Text;
            entity.EducationDateFrom = EducationDateFrom.Value.Value;
            entity.EducationDateTo = EducationDateTo.Value.HasValue ? EducationDateTo.Value : (DateTime?)null;

            if (StatusComplete.Checked)
                entity.EducationStatus = "complete";
            else if (StatusIncomplete.Checked)
                entity.EducationStatus = "incomplete";
            else if (StatusInProgress.Checked)
                entity.EducationStatus = "inprogress";

            entity.EducationCity = ExperienceCity.Text;
            entity.EducationCountry = ServiceLocator.CountrySearch.SelectById(CountrySelector.Value)?.Name;
        }

        private void BindQualifications()
        {
            var items = TCollectionItemCache.Select(new TCollectionItemFilter
            {
                OrganizationIdentifier = CurrentSessionState.Identity.Organization.Identifier,
                CollectionName = CollectionName.Jobs_Candidates_Education_Qualification
            });

            EducationQualification.LoadItems(items, nameof(TCollectionItem.ItemName), nameof(TCollectionItem.ItemName));
        }

    }
}