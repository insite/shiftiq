using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Common.Web;
using InSite.Persistence;
using InSite.Persistence.Plugin.NCSHA;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Constant;

using Field = InSite.Persistence.Plugin.NCSHA.Field;

namespace InSite.Custom.NCSHA.Analytics.Forms
{
    public partial class Refresh : AdminBasePage
    {
        private class YearModel
        {
            public int Year { get; set; }
            public bool IsEnabled { get; set; }
            public bool IsSelected { get; set; }
        }

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
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (Organization.Identifier != OrganizationIdentifiers.NCSHA || !CurrentSessionState.Identity.IsGranted("Custom/NCSHA/Analytics", PermissionOperation.Configure))
                HttpResponseHelper.Redirect("/");

            if (!IsPostBack)
                LoadData();
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            var surveys = GetSelectedSurveys();
            var years = GetSelectedYears();

            if (surveys.Length > 0 && years.Length > 0)
            {
                try
                {
                    var filter = LinqExtensions1.Expr((Field x) => false);
                    foreach (var surveyCode in surveys)
                        filter = filter.Or(x => x.Code.StartsWith(surveyCode));

                    var fields = FieldRepository.Bind(x => x, LinqExtensions1.Expr((Field x) => x.IsNumeric && filter.Invoke(x)).Expand());

                    if (CounterRepository.Refresh(years, fields) > 0)
                        ChartRefreshSettingsModel.Set(User.UserIdentifier, surveys, years);
                }
                catch (Exception ex)
                {
                    AppSentry.SentryError(ex);

                    AlertMessage.AddMessage(AlertType.Error, ex.Message);
                }
            }

            LoadData();
        }

        private void LoadData()
        {
            PageHelper.AutoBindHeader(this);

            var settings = ChartRefreshSettingsModel.Get();
            if (settings != null)
            {
                var user = UserSearch.Bind(settings.UserId, x => new { x.FullName });
                var userName = user != null ? user.FullName : $"(UserID:{settings.UserId})";
                var userTimeZone = CurrentSessionState.Identity.User.TimeZone;
                var userDate = TimeZones.Format(settings.UtcDate, userTimeZone);

                AlertMessage.AddMessage(AlertType.Information, $"Last Refresh was done by {userName} on {userDate}.");
            }

            BindSurveys(settings);
            BindYears(settings);
        }

        private void BindSurveys(ChartRefreshSettingsModel settings)
        {
            var surveys = new[]
            {
                new SurveyModel { Code = "AB", Title = "Administration and Budget" },
                new SurveyModel { Code = "HI", Title = "HOME Investment Partnerships" },
                new SurveyModel { Code = "HC", Title = "Low Income Housing Tax Credits" },
                new SurveyModel { Code = "MF", Title = "Multifamily Bonds" },
                new SurveyModel { Code = "MR", Title = "Mortgage Revenue Bonds" },
                new SurveyModel { Code = "PA", Title = "Private Activity Bonds" }
            };

            var selectedSurveys = settings != null
                ? new HashSet<string>(settings.Surveys)
                : new HashSet<string>(CounterRepository.Bind(x => DbFunctions.Left(x.Code, 2), x => true).Distinct(), StringComparer.OrdinalIgnoreCase);

            foreach (var survey in surveys)
                survey.IsSelected = selectedSurveys.Contains(survey.Code);

            SurveyRepeater.DataSource = surveys;
            SurveyRepeater.DataBind();
        }

        private void BindYears(ChartRefreshSettingsModel settings)
        {
            var years = AbProgramRepository.Select(x => true).Select(x => new { Year = x.SurveyYear, SurveyCode = "AB", IsSubmitted = x.DateTimeSubmitted.HasValue });
            years = years.Concat(HcProgramRepository.Select(x => true).Select(x => new { Year = x.SurveyYear, SurveyCode = "HC", IsSubmitted = x.DateTimeSubmitted.HasValue }));
            years = years.Concat(HiProgramRepository.Select(x => true).Select(x => new { Year = x.SurveyYear, SurveyCode = "HI", IsSubmitted = x.DateTimeSubmitted.HasValue }));
            years = years.Concat(MfProgramRepository.Select(x => true).Select(x => new { Year = x.SurveyYear, SurveyCode = "MF", IsSubmitted = x.DateTimeSubmitted.HasValue }));
            years = years.Concat(MrProgramRepository.Select(x => true).Select(x => new { Year = x.SurveyYear, SurveyCode = "MR", IsSubmitted = x.DateTimeSubmitted.HasValue }));
            years = years.Concat(PaProgramRepository.Select(x => true).Select(x => new { Year = x.SurveyYear, SurveyCode = "PA", IsSubmitted = x.DateTimeSubmitted.HasValue }));

            var modelYears = years
                .GroupBy(x => new { x.Year, x.SurveyCode })
                .Select(x => new { x.Key.Year, x.Key.SurveyCode, Submitted = (decimal)x.Sum(y => y.IsSubmitted ? 1 : 0) / x.Count() })
                .GroupBy(x => x.Year)
                .Select(x => new YearModel { Year = x.Key, IsEnabled = true /* x.All(y => y.Submitted >= 0.95m) */ })
                .OrderByDescending(x => x.Year)
                .ToArray();

            var selectedYears = settings != null
                ? new HashSet<int>(settings.Years)
                : new HashSet<int>(CounterRepository.Select(x => true).Select(x => x.Year).Distinct());

            foreach (var year in modelYears)
                year.IsSelected = selectedYears.Contains(year.Year);

            YearRepeater.DataSource = modelYears;
            YearRepeater.DataBind();
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

        private int[] GetSelectedYears()
        {
            var years = new List<int>();
            foreach (RepeaterItem repeaterItem in YearRepeater.Items)
            {
                var yearLiteral = (Literal)repeaterItem.FindControl("Year");
                var isSelected = (CheckBox)repeaterItem.FindControl("IsSelected");

                if (isSelected.Checked)
                    years.Add(int.Parse(yearLiteral.Text));
            }

            return years.ToArray();
        }
    }
}
