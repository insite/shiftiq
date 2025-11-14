using System;
using System.Collections.Generic;
using System.Web.UI;

using InSite.Common.Web.UI;
using InSite.Persistence;

using ListItem = Shift.Common.ListItem;

namespace InSite.UI.Portal.Jobs.Candidates.MyPortfolio.Controls
{
    public partial class SelfEmploymentDetail : UserControl
    {
        public void BindDefaults()
        {
            BindYears(ExperienceDateFrom);
            BindYears(ExperienceDateTo, true);

            ExperienceDateFrom.Value = string.Empty;
            ExperienceDateTo.Value = string.Empty;
            CountrySelector.Value = ServiceLocator.CountrySearch.SelectByName("Canada")?.Identifier;
        }

        public void BindModelToControls(TCandidateExperience entity)
        {
            BindYears(ExperienceDateFrom);
            BindYears(ExperienceDateTo, true);

            EmployerName.Text = entity.EmployerName;
            ExperienceDateFrom.ValueAsInt = entity.ExperienceDateFrom.Year;
            ExperienceDateTo.ValueAsInt = entity.ExperienceDateTo?.Year;
            ExperienceCity.Text = entity.ExperienceCity;

            var countryName = (entity.ExperienceCountry != null ? entity.ExperienceCountry : (CurrentSessionState.Identity.Organization.PlatformCustomization.TenantLocation.Country ?? "Canada"));
            CountrySelector.Value = ServiceLocator.CountrySearch.SelectByName(countryName)?.Identifier;
        }

        public void BindControlsToModel(TCandidateExperience entity)
        {
            entity.EmployerName = EmployerName.Text;
            entity.ExperienceDateFrom = new DateTime(ExperienceDateFrom.ValueAsInt.Value, 1, 1);
            entity.ExperienceDateTo = ExperienceDateTo.ValueAsInt.HasValue ? new DateTime(ExperienceDateTo.ValueAsInt.Value, 1, 1) : (DateTime?)null;
            entity.ExperienceCity = ExperienceCity.Text;
            entity.ExperienceCountry = ServiceLocator.CountrySearch.SelectById(CountrySelector.Value)?.Name ?? string.Empty;
        }

        private void BindYears(ComboBox cmb, bool includePresent = false)
        {
            var data = new List<ListItem>();

            if (includePresent)
                data.Add(new ListItem { Text = "Present", Value = string.Empty });

            for (var i = DateTime.Now.Year; i >= 1930; i--)
                data.Add(new ListItem { Text = i.ToString(), Value = i.ToString() });


            cmb.LoadItems(data);
        }
    }
}