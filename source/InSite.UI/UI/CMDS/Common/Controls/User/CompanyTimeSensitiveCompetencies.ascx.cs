using System;
using System.Web.UI;

using InSite.Domain.Organizations;
using InSite.Persistence.Plugin.CMDS;

using Shift.Common;
using Shift.Constant;

namespace InSite.Cmds.Controls.Contacts.Companies
{
    public partial class CompanyTimeSensitiveCompetencies : UserControl
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            TimeSensitiveCompetencyExpiry.AutoPostBack = true;
            TimeSensitiveCompetencyExpiry.SelectedIndexChanged += (s, a) => InitVisibility();

            CheckCompetenciesButton.Click += CheckCompetenciesButton_Click;
        }

        private void InitVisibility()
        {
            switch (TimeSensitiveCompetencyExpiry.SelectedIndex)
            {
                case 2:
                    SpecificDateContainer.Visible = true;
                    break;
                default:
                    SpecificDateContainer.Visible = false;
                    break;
            }
        }

        private void CheckCompetenciesButton_Click(object sender, EventArgs e)
        {
            var inserted = VCmdsCompetencyOrganizationRepository.UpdateCompanyCompetencies();

            ResultLiteral.Text = "Number of rows inserted: " + inserted;
        }

        public void SetInputValues(OrganizationState organization)
        {
            var auto = organization.PlatformCustomization.AutomaticCompetencyExpiration;

            switch (auto.Type)
            {
                case OrganizationExpirationType.Interval:
                    TimeSensitiveCompetencyExpiry.SelectedIndex = 1;
                    break;
                case OrganizationExpirationType.Date:
                    TimeSensitiveCompetencyExpiry.SelectedIndex = 2;
                    break;
                default:
                    TimeSensitiveCompetencyExpiry.SelectedIndex = 0;
                    break;
            }

            if (auto.Month.HasValue)
                SpecificMonth.Value = auto.Month.Value.ToString();

            if (auto.Day.HasValue)
                SpecificDay.Value = auto.Day.Value.ToString();

            InitVisibility();
        }

        public void GetInputValues(OrganizationState organization)
        {
            switch (TimeSensitiveCompetencyExpiry.SelectedIndex)
            {
                case 1:
                    organization.PlatformCustomization.AutomaticCompetencyExpiration.Type = OrganizationExpirationType.Interval;
                    break;
                case 2:
                    organization.PlatformCustomization.AutomaticCompetencyExpiration.Type = OrganizationExpirationType.Date;
                    break;
                default:
                    organization.PlatformCustomization.AutomaticCompetencyExpiration.Type = OrganizationExpirationType.None;
                    break;
            }

            if (SpecificMonth.Value.IsNotEmpty() && SpecificDay.Value.IsNotEmpty())
            {
                var month = int.Parse(SpecificMonth.Value);
                var day = int.Parse(SpecificDay.Value);

                if (month == 2 && day > 28)
                    day = 28;

                else if (day > DateTime.DaysInMonth(DateTime.Today.Year, month))
                    day = DateTime.DaysInMonth(DateTime.Today.Year, month);

                organization.PlatformCustomization.AutomaticCompetencyExpiration.Month = month;
                organization.PlatformCustomization.AutomaticCompetencyExpiration.Day = day;
            }
            else
            {
                organization.PlatformCustomization.AutomaticCompetencyExpiration.Month = null;
                organization.PlatformCustomization.AutomaticCompetencyExpiration.Day = null;
            }
        }
    }
}