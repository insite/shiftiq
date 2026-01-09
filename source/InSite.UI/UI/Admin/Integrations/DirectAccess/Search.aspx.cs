using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Humanizer;

using InSite.Common.Web.UI;
using InSite.Persistence.Integration.DirectAccess;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;
using Shift.Toolbox.Integration.DirectAccess;

namespace InSite.UI.Admin.Integrations.DirectAccess
{
    public partial class Search : SearchPage<IndividualFilter>
    {
        public override string EntityName => "Individuals";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SearchCriteria.DisableSearchOnEnterKey = true;

            RequestButton.Click += RequestButton_Click;
            ClearButton.Click += ClearButton_Click;
            RequestProgress.RequestCancelled += RequestProgress_RequestCancelled;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            PageHelper.AutoBindHeader(this);
        }

        private void ValidationFailed(List<string> errors)
        {
            var sb = new StringBuilder();

            foreach (var error in errors)
            {
                sb.Append($"<li>{error}</li>");
            }

            ScreenStatus.Indicator = AlertType.Error;
            ScreenStatus.Text = $"These search results could not be saved. <ul>{sb.ToString()}</ul>";
        }

        private void RequestButton_Click(object sender, EventArgs e)
        {
            var requests = new List<IndividualRequestInput>();

            if (!string.IsNullOrEmpty(IndividualId.Text))
            {
                var ids = StringHelper.Split(IndividualId.Text);

                foreach (var id in ids)
                {
                    requests.Add(new IndividualRequestInput
                    {
                        IndividualId = id,
                        FirstName = FirstName.Text,
                        LastName = LastName.Text,
                        Email = Email.Text,
                        Program = Program.Text
                    });
                }
            }
            else
            {
                requests.Add(new IndividualRequestInput
                {
                    IndividualId = IndividualId.Text,
                    FirstName = FirstName.Text,
                    LastName = LastName.Text,
                    Email = Email.Text,
                    Program = Program.Text
                });
            }

            var da = ServiceLocator.DirectAccessServer;
            var daCache = ServiceLocator.DirectAccessStore;

            int contactCount = 0;
            int newContactCount = 0;

            for (var i = 0; i < requests.Count; i++)
            {
                var request = requests[i];

                ProgressCallback("Searching Direct Access", i, requests.Count);

                var id = CurrentSessionState.Identity.User.UserIdentifier;
                var response = da.IndividualRequest(id, request);
                var errors = new List<string>();

                if (response?.Individuals != null)
                {
                    foreach (var individual in response.Individuals)
                        if (ValidateData(individual, errors))
                            daCache.Save(individual);

                    if (errors.Count > 0)
                        ValidationFailed(errors);

                    contactCount += response.Individuals.Length;
                    newContactCount += response.Individuals.Where(x => x.IsNew).Count();
                }
            }

            ScreenStatus.Indicator = AlertType.Success;
            ScreenStatus.Text = $"{("individual".ToQuantity(newContactCount))} added to the Shift iQ cache. {("individual".ToQuantity(contactCount - newContactCount))} updated.";

            SearchCriteria.BindSearchCriteria(IndividualId.Text, FirstName.Text, LastName.Text, Email.Text, Program.Text);
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            IndividualId.Text = null;
            FirstName.Text = null;
            LastName.Text = null;
            Email.Text = null;
            Program.Text = null;
        }

        private void RequestProgress_RequestCancelled(object sender, EventArgs e)
        {
            SearchResults.Search(SearchResults.Filter);
        }

        private void ProgressCallback(string status, int currentPosition, int positionMax)
        {
            RequestProgress.UpdateContext(context =>
            {
                var progressBar = (ProgressIndicator.ContextData)context.Items["Progress"];
                progressBar.Total = positionMax;
                progressBar.Value = currentPosition;

                context.Variables["status"] = status;
            });
        }

        private bool ValidateData(Shift.Toolbox.Integration.DirectAccess.Individual individual, List<string> errors)
        {
            if (string.IsNullOrEmpty(individual.Email))
                individual.Email = $"{individual.IndividualKey}@itaportal.ca";

            if (string.IsNullOrEmpty(individual.FirstName))
                errors.Add("First Name is a required and cannot be empty");

            if (string.IsNullOrEmpty(individual.LastName))
                errors.Add("Last Name is a required and cannot be empty");

            if (errors.Count == 0)
                return true;

            return false;
        }
    }
}
