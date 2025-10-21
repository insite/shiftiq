using System;
using System.Linq;
using System.Web.UI;

using InSite.Common.Web.UI;
using InSite.Persistence;

namespace InSite.UI.Portal.Jobs.Candidates.MyPortfolio.Controls
{
    public partial class CredentialSection : UserControl
    {
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
        }

        public void BindControlsToModel(Person person)
        {
            
        }

        public void SaveCertifications(Person person)
        {
            SaveValues(person.UserIdentifier, "Professional Designation", ProfessionalCertifications);
            SaveValues(person.UserIdentifier, "Professional Membership", MembershipProfessional);
            SaveValues(person.UserIdentifier, "Volunteer Experience", VolunteerExperience);
            SaveValues(person.UserIdentifier, "Other Training", OtherTraining);
        }

        public void BindModelToControls(Person person)
        {
            LoadValues(person.UserIdentifier, "Professional Designation", ProfessionalCertifications);
            LoadValues(person.UserIdentifier, "Professional Membership", MembershipProfessional);
            LoadValues(person.UserIdentifier, "Volunteer Experience", VolunteerExperience);
            LoadValues(person.UserIdentifier, "Other Training", OtherTraining);
        }

        private void LoadValues(Guid userId, string name, TextBox input)
        {
            var organization = CurrentSessionState.Identity.Organization.Identifier;

            var values = TPersonFieldSearch.Bind(x => x.FieldValue,
                x => x.FieldName == name && x.UserIdentifier == userId && x.OrganizationIdentifier == organization,
                null,
                nameof(TPersonField.FieldSequence)
            );

            input.Text = string.Join("\r\n", values);
        }

        private void SaveValues(Guid userId, string name, TextBox input)
        {
            var organization = CurrentSessionState.Identity.Organization.Identifier;
            var list = input.Text.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            var values = list.Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x)).ToArray();

            TPersonFieldStore.Update(organization, userId, name, values);
        }
    }
}