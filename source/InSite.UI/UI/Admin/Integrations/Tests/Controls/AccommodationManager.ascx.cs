using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;

using Shift.Common;
using Shift.Sdk.UI;

using InSiteTextBox = InSite.Common.Web.UI.TextBox;

namespace InSite.UI.Admin.Integrations.Tests.Controls
{
    public partial class AccommodationManager : BaseUserControl
    {
        private string[] AccommodationAdditionalTypes
        {
            get => (string[])ViewState[nameof(AccommodationAdditionalTypes)];
            set => ViewState[nameof(AccommodationAdditionalTypes)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Repeater.ItemDataBound += Repeater_ItemDataBound;

            AddButton.Click += AddButton_Click;
        }

        public void InitControl()
        {
            AccommodationAdditionalTypes = ServiceLocator.RegistrationSearch
                .GetAccommodationTypes(Organization.OrganizationIdentifier)
                .ToArray();

            Repeater.DataSource = null;
            Repeater.DataBind();
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            var accommodations = GetItems();

            accommodations.Add(new GrantRegistrationAccommodation());

            Repeater.DataSource = accommodations;
            Repeater.DataBind();
        }

        private void Repeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var dataItem = (GrantRegistrationAccommodation)e.Item.DataItem;

            var typeSelector = (AccommodationTypeComboBox)e.Item.FindControl("TypeSelector");
            typeSelector.AdditionalOptions = AccommodationAdditionalTypes;
            typeSelector.RefreshData();
            typeSelector.Value = dataItem.Type;

            var name = (InSiteTextBox)e.Item.FindControl("Name");
            name.Text = dataItem.Name;
        }

        private static readonly Regex AccommodationTypeTimePattern =
            new Regex("\\(\\+(?:(?:(?<Hours>\\d+(?:\\.\\d+)?)hr)|(?:(?<Minutes>\\d+)min))\\)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public List<GrantRegistrationAccommodation> GetItems()
        {
            var result = new List<GrantRegistrationAccommodation>();

            foreach (RepeaterItem item in Repeater.Items)
            {
                var typeSelector = (AccommodationTypeComboBox)item.FindControl("TypeSelector");
                var name = (InSiteTextBox)item.FindControl("Name");
                var accommodation = new GrantRegistrationAccommodation
                {
                    Type = typeSelector.Value,
                    Name = name.Text,
                };

                var timeMatch = AccommodationTypeTimePattern.Match(accommodation.Type.EmptyIfNull());
                if (timeMatch.Success)
                {
                    if (decimal.TryParse(timeMatch.Groups["Hours"]?.Value, out var hours))
                        accommodation.TimeExtension = (int)(hours * 60);
                    else if (int.TryParse(timeMatch.Groups["Minutes"]?.Value, out var minutes))
                        accommodation.TimeExtension = minutes;
                }

                result.Add(accommodation);
            }

            return result;
        }
    }
}