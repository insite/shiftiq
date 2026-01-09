using System;

using Shift.Common.Timeline.Commands;

using InSite.Application.Groups.Write;
using InSite.Common.Web;
using InSite.UI.Layout.Admin;
using InSite.Web.Data;

using Shift.Common;

namespace InSite.Admin.Accounts.Divisions.Forms
{
    public partial class Create : AdminBasePage
    {
        private const string EditUrl = "/ui/admin/accounts/divisions/edit";
        private const string SearchUrl = "/ui/admin/accounts/divisions/search";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            if (!CanCreate)
                HttpResponseHelper.Redirect(SearchUrl);

            PageHelper.AutoBindHeader(Page);

            DivisionDetails.SetDefaultInputValues();

            CancelButton.NavigateUrl = SearchUrl;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!IsValid)
                return;

            var division = DivisionFactory.Create(UniqueIdentifier.Create());

            DivisionDetails.GetInputValues(division);

            var commands = new ICommand[]
            {
                new CreateGroup(division.DivisionIdentifier, division.OrganizationIdentifier, "District", division.DivisionName),
                new DescribeGroup(division.DivisionIdentifier, null, division.DivisionCode, division.DivisionDescription, null)
            };

            ServiceLocator.SendCommands(commands);

            HttpResponseHelper.Redirect($"{EditUrl}?id={division.DivisionIdentifier}&status=saved");
        }
    }
}