using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Application.Events.Read;
using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.UI.Portal.Events.Classes.Controls
{
    public partial class PersonList : BaseUserControl
    {
        private int? AvailableCount
        {
            get => (int?)ViewState[nameof(AvailableCount)];
            set => ViewState[nameof(AvailableCount)] = value;
        }

        protected bool AllowDelete { get; set; }

        private string LabelName => "[Learner Number Help].[Tooltip]";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            FinalValidator.ServerValidate += FinalValidator_ServerValidate;

            PersonRepeater.ItemCommand += PersonRepeater_ItemCommand;

            AddButton1.Click += AddButton_Click;
            AddButton2.Click += AddButton_Click;
        }

        private void FinalValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            var people = GetPeople();
            var emails = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var personItem in people)
            {
                if (!string.IsNullOrEmpty(personItem.Email) && !emails.Add(personItem.Email))
                {
                    args.IsValid = false;
                    FinalValidator.ErrorMessage = $"Email address <b>{personItem.Email}</b> appeared more than once in the list";
                    return;
                }
            }

            args.IsValid = true;
        }

        private void PersonRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                var itemIndex = int.Parse(e.CommandArgument.ToString());
                var people = GetPeople();

                people.RemoveAt(itemIndex);

                BindRepeater(people);
            }
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            var people = GetPeople();
            people.Add(new PersonItem());

            BindRepeater(people);
        }

        public void LoadData(QEvent @event)
        {
            AvailableCount = @event.CapacityMaximum.HasValue && @event.CapacityMaximum > 0
                ? @event.CapacityMaximum - @event.ConfirmedRegisteredCount
                : null;

            AllowDelete = false;

            var people = new List<PersonItem>
            {
                new PersonItem()
            };

            BindRepeater(people);
        }

        public List<PersonItem> GetPeople()
        {
            var people = new List<PersonItem>();

            foreach (RepeaterItem repeaterItem in PersonRepeater.Items)
            {
                var firstName = (ITextBox)repeaterItem.FindControl("FirstName");
                var lastName = (ITextBox)repeaterItem.FindControl("LastName");
                var email = (ITextBox)repeaterItem.FindControl("Email");
                var birthdate = (DateSelector)repeaterItem.FindControl("Birthdate");
                var personCode = (ITextBox)repeaterItem.FindControl("PersonCode");
                var phone = (ITextBox)repeaterItem.FindControl("Phone");

                var personItem = new PersonItem
                {
                    Email = email.Text,
                    FirstName = firstName.Text,
                    LastName = lastName.Text,
                    Birthdate = birthdate.Value,
                    PersonCode = personCode.Text,
                    Phone = phone.Text
                };

                people.Add(personItem);
            }

            return people;
        }

        protected string GetCustomHelp()
        {
            var currentOrg = CurrentSessionState.Identity.Organization.OrganizationIdentifier;
            var contents = ServiceLocator.ContentSearch.SelectContainers(
                x => x.ContainerIdentifier == LabelSearch.ContainerIdentifier && x.ContentLabel == LabelName && x.ContainerType == ContentContainerType.Application);

            if (contents != null && contents.Length > 0)
            {
                if (contents.FirstOrDefault(x => x.OrganizationIdentifier == currentOrg) != null)
                    return contents.FirstOrDefault(x => x.OrganizationIdentifier == currentOrg).ContentText;
                else if (contents.FirstOrDefault(x => x.OrganizationIdentifier == OrganizationIdentifiers.Global) != null)
                    return contents.FirstOrDefault(x => x.OrganizationIdentifier == OrganizationIdentifiers.Global).ContentText;
            }

            return string.Empty;
        }

        private void BindRepeater(List<PersonItem> people)
        {
            AllowDelete = people.Count > 1;

            PersonRepeater.DataSource = people;
            PersonRepeater.DataBind();

            if (AvailableCount.HasValue)
            {
                var maxRegistrations = AvailableCount - people.Count <= 0;

                MaxRegistrations.Visible = maxRegistrations;
                AddButton1.Enabled = !maxRegistrations;
                AddButton2.Enabled = !maxRegistrations;
            }
        }
    }
}