using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

using InSite.Application.Events.Read;
using InSite.Common.Web.UI;
using InSite.Persistence.Integration.BCMail;
using InSite.UI.Desktops.Custom.SkilledTradesBC.Distributions.Models;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.UI.Desktops.Custom.SkilledTradesBC.Distributions.Controls
{
    public partial class SearchResults : SearchResultsGridViewController<QEventFilter>
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PlaceDistributionOrderButton.Click += PlaceDistributionOrderButton_Click;
        }

        private void PlaceDistributionOrderButton_Click(object sender, EventArgs e)
        {
            var factory = new DistributionRequestFactory();
            var requests = factory.CreateDistributionRequests(ServiceLocator.EventSearch, ServiceLocator.RegistrationSearch, ServiceLocator.BankSearch, Filter);
            var sent = new HashSet<Guid>();

            foreach (var request in requests)
            {
                if (sent.Contains(request.When.ActivityIdentifier))
                    continue;

                // if (IsRequested(request.When.ActivityIdentifier))
                //    continue;

                var order = new OrderDistribution(request.When.ActivityIdentifier, request, request.When.ExamType == EventExamType.Test.Value);
                ServiceLocator.SendCommand(order);
                sent.Add(request.When.ActivityIdentifier);
            }

            Search(Filter);
        }

        private bool IsRequested(Guid @event)
        {
            var e = ServiceLocator.EventSearch.GetEvent(@event);
            return e.DistributionCode != null;
        }

        public override void Search(QEventFilter filter, bool refreshLastSearched = false)
        {
            RequestRepeater.DataSource = null;
            RequestRepeater.DataBind();

            base.Search(filter, refreshLastSearched);
        }

        protected override int SelectCount(QEventFilter filter)
        {
            return ServiceLocator.EventSearch.CountEvents(filter);
        }

        protected override IListSource SelectData(QEventFilter filter)
        {
            var events = ServiceLocator.EventSearch
                .GetEvents(filter, x => x.Registrations, x => x.ExamForms, x => x.VenueLocation);

            if (events.Count == 1)
            {
                var settings = new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                    NullValueHandling = NullValueHandling.Include
                };

                var factory = new DistributionRequestFactory();

                var requests = factory.CreateDistributionRequests(ServiceLocator.EventSearch, ServiceLocator.RegistrationSearch, ServiceLocator.BankSearch, Filter);

                var warnings = new StringBuilder();
                var items = new List<DistributionRequestItem>();

                foreach (var request in requests)
                {
                    var item = new DistributionRequestItem($"{request.When.ActivityType} {request.When.ActivityNumber}-{request.When.ActivityBillingCode}", JsonConvert.SerializeObject(request, settings));
                    items.Add(item);

                    if (request.WhatCount == 0)
                        warnings.Append("Missing Forms. ");

                    if (request.WhoCount == 0)
                        warnings.Append("Missing Candidates. ");
                }

                RequestRepeater.DataSource = items;
                RequestRepeater.DataBind();

                PlaceDistributionOrderButton.Visible = items.Count > 0;
                PlaceDistributionOrderButton.OnClientClick = "return confirm('Are you sure you want to create a new BC Mail distribution package for the exam appointments listed in the search results?');";

                if (items.Count == 0)
                {
                    RequestMessage.Text = "None";
                }
                else if (warnings.Length > 0)
                {
                    RequestMessage.Text = $"<span class='text-danger'>Warning: {warnings}</span>";
                }
            }

            return events.ToSearchResult();
        }

        protected static string GetLocalTime(DateTimeOffset? value)
        {
            return value.Format(User.TimeZone, true);
        }
    }
}