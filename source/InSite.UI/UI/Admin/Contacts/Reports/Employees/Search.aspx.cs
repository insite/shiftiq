using System;
using System.Collections.Generic;

using InSite.Common;
using InSite.Common.Web.UI;
using InSite.Domain.Reports;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

namespace InSite.UI.Admin.Contacts.Reports.Employees
{
    public partial class Search : SearchPage<EmployeeFilter>
    {
        public override string EntityName => "Employee";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PageHelper.AutoBindHeader(this);
        }

        protected override IEnumerable<DownloadColumn> GetExportColumns()
        {
            return new[]
            {
                new DownloadColumn("EmployeeFirstName", "Name"),
                new DownloadColumn("EmployeeLastName", "Name"),
                new DownloadColumn("EmployeeEmail", "Email"),
                new DownloadColumn("EmployeeJobTitle", "Job Title"),
                new DownloadColumn("EmployeeProcessStep", "Membership Status"),
                new DownloadColumn("EmployeeGender", "Gender"),
                new DownloadColumn("EmployeeMemberStartDate", "Join Date"),
                new DownloadColumn("EmployeeHonorific", "Honorific"),
                new DownloadColumn("EmployeePhone", "Phone"),
                new DownloadColumn("EmployeePhoneHome", "Home Phone"),
                new DownloadColumn("EmployeePhoneMobile", "Mobile Phone"),
                new DownloadColumn("EmployeeMemberEndDate", "Member End Date"),
                new DownloadColumn("EmployeeShippingAddressStreet1", "Shipping Address"),
                new DownloadColumn("EmployeeShippingAddressCity", "Shipping City"),
                new DownloadColumn("EmployeeShippingAddressProvince", "Shipping Province"),
                new DownloadColumn("EmployeeShippingAddressPostalCode", "Shipping Postal Code"),
                new DownloadColumn("EmployeeShippingAddressCountry", "Shipping Country"),
                new DownloadColumn("EmployeeShippingPreference", "Shipping Preference"),
                new DownloadColumn("EmployeeAccountNumber", LabelHelper.GetLabelContentText("Person Code")),
                new DownloadColumn("EmployeeHomeAddressStreet1", "Home Address"),
                new DownloadColumn("EmployeeHomeAddressCity", "Home City"),
                new DownloadColumn("EmployeeHomeAddressProvince", "Home Province"),
                new DownloadColumn("EmployeeHomeAddressPostalCode", "Home Postal Code"),
                new DownloadColumn("EmployeeHomeAddressCountry", "Home Country"),
                new DownloadColumn("EmployerGroupName", "Employer"),
                new DownloadColumn("EmployerGroupNumber", "Employer Number"),
                new DownloadColumn("EmployerContactLabel", "Employer Category"),
                new DownloadColumn("EmployerShippingAddressStreet1", "Employer Address"),
                new DownloadColumn("EmployerShippingAddressCity", "Employer City"),
                new DownloadColumn("EmployerShippingAddressProvince", "Employer Province"),
                new DownloadColumn("EmployerShippingAddressPostalCode", "Employer Postal Code"),
                new DownloadColumn("EmployerPhone", "Employer Phone"),
                new DownloadColumn("EmployerPhoneFax", "Employer Fax"),
                new DownloadColumn("EmployerParentFunctional", "Empoyer Parent (Functional)"),
                new DownloadColumn("EmployerDistrictName", "District"),
                new DownloadColumn("EmployerDistrictRegion", "District Region"),
                new DownloadColumn("RolesParticipationGroupName", "Roles - Participation")
            };
        }
    }
}