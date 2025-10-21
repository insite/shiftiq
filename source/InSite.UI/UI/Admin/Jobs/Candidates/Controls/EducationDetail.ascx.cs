using System;
using System.Web.UI;

using InSite.Persistence;

using Shift.Constant;

namespace InSite.UI.Admin.Jobs.Candidates.Controls
{
    public partial class EducationDetail : UserControl
    {
        public void GetInputValues(TCandidateEducation entity)
        {
            entity.EducationDateFrom = DateFrom.Value.Value;
            entity.EducationDateTo = DateTo.Value;
            entity.EducationInstitution = InstitutionName.Text;
            entity.EducationName = CourseName.Text;
            entity.EducationCity = City.Text;
            entity.EducationCountry = ServiceLocator.CountrySearch.SelectById(CountrySelector.Value)?.Name ?? string.Empty;
            entity.EducationQualification = EducationQualification.Value;

            if (StatusComplete.Checked)
                entity.EducationStatus = "complete";
            else if (StatusIncomplete.Checked)
                entity.EducationStatus = "incomplete";
            else if (StatusInProgress.Checked)
                entity.EducationStatus = "inprogress";
        }

        public void SetInputValues(TCandidateEducation entity)
        {
            BindQualifications();

            EducationQualification.Value = entity.EducationQualification;

            var countryName = (entity.EducationCountry != null ? entity.EducationCountry : (CurrentSessionState.Identity.Organization.PlatformCustomization.TenantLocation.Country ?? "Canada"));
            CountrySelector.Value = ServiceLocator.CountrySearch.SelectByName(countryName)?.Identifier;

            DateFrom.Value = entity.EducationDateFrom;
            DateTo.Value = entity.EducationDateTo;
            InstitutionName.Text = entity.EducationInstitution;
            CourseName.Text = entity.EducationName;
            City.Text = entity.EducationCity;

            if (string.Equals(entity.EducationStatus, "complete", StringComparison.OrdinalIgnoreCase))
                StatusComplete.Checked = true;
            else if (string.Equals(entity.EducationStatus, "incomplete", StringComparison.OrdinalIgnoreCase))
                StatusIncomplete.Checked = true;
            else if (string.Equals(entity.EducationStatus, "inprogress", StringComparison.OrdinalIgnoreCase))
                StatusInProgress.Checked = true;

        }

        public void BindDefaults()
        {
            BindQualifications();
            CountrySelector.Value = ServiceLocator.CountrySearch.SelectByName("Canada")?.Identifier;
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