using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;
using InSite.Domain.Reports;
using InSite.Persistence;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Layout.Common.Controls
{
    public partial class SearchDownloadManager : BaseReportManager
    {
        private static readonly Type _baseReportType = typeof(Download);
        private Type _reportType = _baseReportType;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Script.ContentKey = UniqueID;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            var translator = (IHasTranslator)Page;

            NewEntityText.EmptyMessage = translator.Translator.Translate("Enter the name to create a new download template");
        }

        public void SetReportType(Type type)
        {
            if (!type.IsSubclassOf(_baseReportType))
                throw ApplicationError.Create("The report type must be derived from " + _baseReportType.Name);

            _reportType = type;
        }

        protected override ControlsInfo GetControls() => new ControlsInfo
        {
            ReportSelector = SettingsSelector,
            SaveButton = SaveButton,
            RemoveButton = RemoveButton,
            CreateButton = CreateButton,
            ReportName = NewEntityText,
        };

        protected override List<ISearchReport> LoadReports()
        {
            var reports = TReportSearch.Select(
                x => x.OrganizationIdentifier == Organization.OrganizationIdentifier
                  && x.UserIdentifier == User.UserIdentifier
                  && x.ReportType == ReportType.Download
                  && x.ReportDescription == Route.Name);

            return reports
                .Select(x => x.ReportData.IsEmpty() ? null : JsonConvert.DeserializeObject(x.ReportData, _reportType))
                .Cast<ISearchReport>()
                .ToList();
        }

        protected override void SaveReports(List<ISearchReport> reports)
        {
            var entities = TReportSearch.Select(
                x => x.OrganizationIdentifier == Organization.OrganizationIdentifier
                  && x.UserIdentifier == User.UserIdentifier
                  && x.ReportType == ReportType.Download
                  && x.ReportDescription == Route.Name);

            foreach (Download report in reports)
            {
                if (!report.Identifier.HasValue)
                    report.Identifier = UniqueIdentifier.Create();

                var entity = entities.FirstOrDefault(x => x.ReportIdentifier == report.Identifier.Value);
                var isNew = entity == null;

                if (isNew)
                {
                    entity = new TReport
                    {
                        ReportIdentifier = report.Identifier.Value,
                        OrganizationIdentifier = Organization.OrganizationIdentifier,
                        UserIdentifier = User.UserIdentifier,
                        ReportType = ReportType.Download,
                        ReportTitle = report.Name,
                        ReportDescription = Route.Name,
                        Created = DateTimeOffset.Now,
                        CreatedBy = User.UserIdentifier,
                    };
                }

                entity.ReportData = JsonConvert.SerializeObject(report);
                entity.Modified = DateTimeOffset.Now;
                entity.ModifiedBy = User.UserIdentifier;

                if (isNew)
                    TReportStore.Insert(entity);
                else
                    TReportStore.Update(entity);
            }

            foreach (var report in entities)
            {
                if (!reports.Any(x => x.Identifier == report.ReportIdentifier))
                    TReportStore.Delete(report.ReportIdentifier);
            }
        }

        protected override bool BindSavedReports()
        {
            var hasReports = base.BindSavedReports();

            SavedField.Visible = hasReports;

            return hasReports;
        }
    }
}