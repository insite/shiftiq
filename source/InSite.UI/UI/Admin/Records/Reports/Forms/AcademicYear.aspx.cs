using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Common.Web;
using InSite.UI.Admin.Records.Reports.Models;
using InSite.UI.Layout.Admin;

namespace InSite.UI.Admin.Records.Reports.Forms
{
    public partial class AcademicYear : AdminBasePage
    {
        [Serializable]
        private class PeriodItem
        {
            public Guid PeriodIdentifier { get; set; }
            public string PeriodName { get; set; }
        }

        private List<PeriodItem> Periods
        {
            get => (List<PeriodItem>)ViewState[nameof(Periods)]
                ?? (List<PeriodItem>)(ViewState[nameof(Periods)] = new List<PeriodItem>());
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            AddPeriodButton.Click += AddPeriodButton_Click;

            PeriodRepeater.ItemCommand += PeriodRepeater_ItemCommand;

            DownloadReportButton.Click += DownloadReportButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                PageHelper.AutoBindHeader(this);
        }

        private void AddPeriodButton_Click(object sender, EventArgs e)
        {
            if (Period.Value == null || Periods.Any(x => x.PeriodIdentifier == Period.Value))
                return;

            var period = ServiceLocator.PeriodSearch.GetPeriod(Period.Value.Value);
            if (period == null)
                return;

            Periods.Add(new PeriodItem
            {
                PeriodIdentifier = period.PeriodIdentifier,
                PeriodName = period.PeriodName
            });

            if (Period.Filter.ExcludeIdentifiers == null)
                Period.Filter.ExcludeIdentifiers = new HashSet<Guid>();

            Period.Filter.ExcludeIdentifiers.Add(period.PeriodIdentifier);

            BindPeriods();
        }

        private void PeriodRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName != "Delete")
                return;

            var periodIdentifier = Guid.Parse((string)e.CommandArgument);
            var period = Periods.Find(x => x.PeriodIdentifier == periodIdentifier);

            if (period == null)
                return;

            Periods.Remove(period);

            Period.Filter.ExcludeIdentifiers.Remove(period.PeriodIdentifier);

            BindPeriods();
        }

        private void DownloadReportButton_Click(object sender, EventArgs e)
        {
            var gradebookPeriods = Periods.Select(x => x.PeriodIdentifier).ToHashSet();
            var data = AcademicYearReport.GetXlsx(Organization.OrganizationIdentifier, gradebookPeriods);

            Response.SendFile("academic-year", "xlsx", data);
        }

        private void BindPeriods()
        {
            PeriodRepeater.DataSource = Periods;
            PeriodRepeater.DataBind();

            Period.Value = null;

            DownloadReportButton.Visible = Periods.Count > 0;
        }
    }
}
