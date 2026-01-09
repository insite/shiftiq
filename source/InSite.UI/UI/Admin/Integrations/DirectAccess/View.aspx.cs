using System;

using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence.Integration.DirectAccess;
using InSite.UI.Layout.Admin;

namespace InSite.UI.Admin.Integrations.DirectAccess
{
    public partial class View : ViewerController
    {

        private int IndividualKey => int.TryParse(Request["individual"], out var value) ? value : -1;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                Open();
            }
        }

        private void Open()
        {
            var individual = DirectAccessSearch.SelectIndividual(IndividualKey);

            if (individual == null)
                HttpResponseHelper.Redirect("/ui/admin/integrations/directaccess/search");

            SetInputValues(individual);
        }

        private void SetInputValues(Shift.Toolbox.Integration.DirectAccess.Individual individual)
        {
            PageHelper.AutoBindHeader(this);

            Name.Text = GetString(individual.Name);
            FirstName.Text = GetString(individual.FirstName);
            MiddleName.Text = GetString(individual.MiddleName);
            LastName.Text = GetString(individual.LastName);

            CrmIdentifier.Text = GetString(individual.CrmIdentifier);
            Birthdate.Text = GetDateString(individual.Birthdate);

            Gender.Text = GetString(individual.Gender);
            Mobile.Text = GetString(individual.Mobile);
            Email.Text = GetString(individual.Email);
            PersonalEducationNumber.Text = GetString(individual.PersonalEducationNumber);
            HashCode.Text = GetString(individual.HashCode);
            AddressLine1.Text = GetString(individual.AddressLine1);
            AddressLine2.Text = GetString(individual.AddressLine2);
            AddressCity.Text = GetString(individual.AddressCity);
            AddressProvince.Text = GetString(individual.AddressProvince);
            AddressPostalCode.Text = GetString(individual.AddressPostalCode);
            AboriginalIndicator.Text = GetString(individual.AboriginalIndicator);
            AboriginalIdentity.Text = GetString(individual.AboriginalIdentity);

            IsNew.Text = GetString(individual.IsNew);
            IsActive.Text = GetString(individual.IsActive);
            IsDeceased.Text = GetString(individual.IsDeceased);
            IsMerged.Text = GetString(individual.IsMerged);

            Refreshed.Text = individual.Refreshed.ToString();

            ContactGroupSection.Visible = individual.ContactIdentifier.HasValue;
            ExamWorkflowSection.Visible = individual.ContactIdentifier.HasValue;

            if (individual.ContactIdentifier.HasValue)
            {
                ContactGroups.LoadData(individual.ContactIdentifier.Value);
                ContactGroupSection.SetTitle("Contact Groups", ContactGroups.RowCount);

                ExamWorkflows.LoadData(individual.ContactIdentifier.Value);
                ExamWorkflowSection.SetTitle("Exams", ExamWorkflows.RowCount);
            }

            NotificationSection.Visible = !string.IsNullOrEmpty(individual.Email);

            if (!string.IsNullOrEmpty(individual.Email))
            {
                Mailouts.LoadData(individual.Email);
                NotificationSection.SetTitle("Notifications", Mailouts.RowCount);
            }
        }

    }
}
