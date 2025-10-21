using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Persistence.Plugin.NCSHA;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Constant;

using static InSite.Persistence.Plugin.NCSHA.ProgramRepository;

namespace InSite.UI.Desktops.Custom.Ncsha.Reports.Forms
{
    public partial class Refresh : AdminBasePage
    {
        private class SurveyModel
        {
            public string Code { get; set; }
            public string Title { get; set; }
            public bool IsSelected { get; set; }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            RefreshButton.Click += RefreshButton_Click;

            YearComboBox2.AutoPostBack = true;
            YearComboBox2.ValueChanged += (x, y) => AgencySelected();

            AgencyComboBox.ListFilter.OrganizationIdentifier = OrganizationIdentifiers.NCSHA;
            AgencyComboBox.ListFilter.GroupType = "Employer";
            AgencyComboBox.AutoPostBack = true;
            AgencyComboBox.ValueChanged += (x, y) => AgencySelected();

            MR05.CheckedChanged += (x, y) => MrProgramVisibilityChanged("MR05", MR05.Checked);
            MR06.CheckedChanged += (x, y) => MrProgramVisibilityChanged("MR06", MR06.Checked);
            MR07.CheckedChanged += (x, y) => MrProgramVisibilityChanged("MR07", MR07.Checked);
            MR08.CheckedChanged += (x, y) => MrProgramVisibilityChanged("MR08", MR08.Checked);
            MR09.CheckedChanged += (x, y) => MrProgramVisibilityChanged("MR09", MR09.Checked);

            MF09.CheckedChanged += (x, y) => MfProgramVisibilityChanged("MF09", MF09.Checked);
            MF10.CheckedChanged += (x, y) => MfProgramVisibilityChanged("MF10", MF10.Checked);
        }

        private void AgencySelected()
        {
            SetVisibility();

            if (!YearComboBox2.ValueAsInt.HasValue || !AgencyComboBox.ValueAsGuid.HasValue)
                return;

            var mr = MrProgramRepository.SelectFirst(x => x.SurveyYear == YearComboBox2.ValueAsInt && x.AgencyGroupIdentifier == AgencyComboBox.ValueAsGuid);
            if (mr != null)
            {
                MR05.Enabled = true;
                MR06.Enabled = true;
                MR07.Enabled = true;
                MR08.Enabled = true;
                MR09.Enabled = true;

                MR05.Checked = mr.IsVisibleOnTable05;
                MR06.Checked = mr.IsVisibleOnTable06;
                MR07.Checked = mr.IsVisibleOnTable07;
                MR08.Checked = mr.IsVisibleOnTable08;
                MR09.Checked = mr.IsVisibleOnTable09;
            }

            var mf = MfProgramRepository.SelectFirst(x => x.SurveyYear == YearComboBox2.ValueAsInt && x.AgencyGroupIdentifier == AgencyComboBox.ValueAsGuid);
            if (mf != null)
            {
                MF09.Enabled = true;
                MF10.Enabled = true;

                MF09.Checked = mf.IsVisibleOnTable09;
                MF10.Checked = mf.IsVisibleOnTable10;
            }
        }

        private void SetVisibility()
        {
            MR05.Enabled = false;
            MR06.Enabled = false;
            MR07.Enabled = false;
            MR08.Enabled = false;
            MR09.Enabled = false;
            MF09.Enabled = false;
            MF10.Enabled = false;

            MR05.Checked = false;
            MR06.Checked = false;
            MR07.Checked = false;
            MR08.Checked = false;
            MR09.Checked = false;
            MF09.Checked = false;
            MF10.Checked = false;
        }

        private void MrProgramVisibilityChanged(string field, bool isChecked)
        {
            if (!YearComboBox2.ValueAsInt.HasValue || !AgencyComboBox.ValueAsGuid.HasValue)
                return;

            var mr = MrProgramRepository.SelectFirst(x => x.SurveyYear == YearComboBox2.ValueAsInt && x.AgencyGroupIdentifier == AgencyComboBox.ValueAsGuid);
            if (mr != null)
            {
                MrProgramRepository.Update(mr.MrProgramId, x =>
                {
                    if (field == "MR05")
                        x.IsVisibleOnTable05 = isChecked;

                    if (field == "MR06")
                        x.IsVisibleOnTable06 = isChecked;

                    if (field == "MR07")
                        x.IsVisibleOnTable07 = isChecked;

                    if (field == "MR08")
                        x.IsVisibleOnTable08 = isChecked;

                    if (field == "MR09")
                        x.IsVisibleOnTable09 = isChecked;
                });
            }
        }

        private void MfProgramVisibilityChanged(string field, bool isChecked)
        {
            if (!YearComboBox2.ValueAsInt.HasValue || !AgencyComboBox.ValueAsGuid.HasValue)
                return;

            var mf = MfProgramRepository.SelectFirst(x => x.SurveyYear == YearComboBox2.ValueAsInt && x.AgencyGroupIdentifier == AgencyComboBox.ValueAsGuid);
            if (mf != null)
            {
                MfProgramRepository.Update(mf.MfProgramId, x =>
                {
                    if (field == "MF09")
                        x.IsVisibleOnTable09 = isChecked;

                    if (field == "MF10")
                        x.IsVisibleOnTable10 = isChecked;
                });
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                PageHelper.AutoBindHeader(this);
                BindReports();
                BindYearComboBox();
                BindSurveyPrograms();
                BindVisibilities();
            }
        }

        private void BindReports()
        {
            if (ReportSection.Visible)
            {
                var reports = CustomReportHelper.GetReports()
                    .OrderBy(x => x.Code)
                    .ToList();

                foreach (var report in reports)
                    report.NavigateUrl = report.GetPreviewUrl();

                Reports.DataSource = reports.ToSearchResult();
                Reports.DataBind();
            }
        }

        public void BindYearComboBox()
        {
            YearComboBox1.YearList = SubmissionReportYears().Take(2).ToList();
            YearComboBox2.YearList = YearComboBox1.YearList;

        }

        private void BindSurveyPrograms()
        {
            var surveys = new[]
            {
                new SurveyModel { Code = "AbProgram", Title = "Administration and Budget", IsSelected = false },
                new SurveyModel { Code = "HiProgram", Title = "HOME Investment Partnerships", IsSelected = false  },
                new SurveyModel { Code = "HcProgram", Title = "Low Income Housing Tax Credits", IsSelected = false  },
                new SurveyModel { Code = "MfProgram", Title = "Multifamily Bonds", IsSelected = false  },
                new SurveyModel { Code = "MrProgram", Title = "Mortgage Revenue Bonds", IsSelected = false  },
                new SurveyModel { Code = "PaProgram", Title = "Private Activity Bonds", IsSelected = false  }
            };

            SurveyRepeater.DataSource = surveys;
            SurveyRepeater.DataBind();
        }

        private void BindVisibilities()
        {
            AgencySelected();
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            Page.Server.ScriptTimeout = 5 * 60;

            try
            {
                var surveys = GetSelectedSurveys();
                var year = YearComboBox1.ValueAsInt;

                if (year.HasValue && year.Value > 0 && surveys != null && surveys.Length > 0)
                {
                    var surveyMigration = new SurveyMigration(year.Value);
                    if (surveyMigration.Answers.Count > 0)
                    {
                        var repo = new ProgramRepository();
                        repo.RefreshReportTables(surveys, surveyMigration, year.Value.ToString());
                        var user = CurrentSessionState.Identity.User;
                        var userTimeZone = CurrentSessionState.Identity.User.TimeZone;
                        var userDate = TimeZones.Format(DateTimeOffset.Now, userTimeZone);

                        AlertMessage.AddMessage(AlertType.Information, $"Last Refresh was done by {user.FullName} on {userDate}.");

                        foreach (var warning in repo.Warnings)
                            AlertMessage.AddMessage(AlertType.Warning, warning);
                    }
                }
            }
            catch (Exception ex)
            {
                AppSentry.SentryError(ex);

                AlertMessage.AddMessage(AlertType.Error, ex.Message);
            }
        }

        private string[] GetSelectedSurveys()
        {
            var surveys = new List<string>();
            foreach (RepeaterItem repeaterItem in SurveyRepeater.Items)
            {
                var codeLiteral = (Literal)repeaterItem.FindControl("Code");
                var isSelected = (CheckBox)repeaterItem.FindControl("IsSelected");

                if (isSelected.Checked)
                    surveys.Add(codeLiteral.Text);
            }

            return surveys.ToArray();
        }
    }
}