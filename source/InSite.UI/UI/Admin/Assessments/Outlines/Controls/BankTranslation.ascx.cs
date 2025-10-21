using System;
using System.Linq;

using InSite.Common.Web.UI;
using InSite.Domain.Organizations;
using InSite.Persistence;

using Shift.Common;

namespace InSite.Admin.Assessments.Outlines.Controls
{
    public partial class BankTranslation : BaseUserControl
    {

        public bool LoadData(Guid organizationIdentifier)
        {
            var organization = OrganizationSearch.Select(organizationIdentifier);

            return LoadData(organization);
        }

        public bool LoadData(OrganizationState organization)
        {
            var formLang = "en";
            var formTranslations = organization.Languages
                .Where(x => x.TwoLetterISOLanguageName != "en")
                .Select(x => Language.GetName(x.TwoLetterISOLanguageName))
                .ToArray();

            SurveyLanguage.Text = Language.GetName(formLang);
            SurveyTranslationLanguages.Text = formTranslations.IsNotEmpty()
                ? string.Join(", ", formTranslations)
                : "None";

            return true;
        }
    }
}