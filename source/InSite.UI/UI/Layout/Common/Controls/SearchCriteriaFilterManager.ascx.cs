using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;
using InSite.Persistence;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Layout.Common.Controls
{
    public partial class SearchCriteriaFilterManager : BaseReportManager
    {
        #region Fields

        private static readonly JsonSerializerSettings jsonSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto
        };

        #endregion

        protected override void OnInit(System.EventArgs e)
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

            NewFilterText.EmptyMessage = translator.Translator.Translate("New Saved Filter");
        }

        protected override ControlsInfo GetControls() => new ControlsInfo
        {
            ReportSelector = SavedFilterSelector,
            SaveButton = SaveFilterButton,
            RemoveButton = RemoveFilterButton,
            CreateButton = NewFilterButton,
            ReportName = NewFilterText,
        };

        protected override List<ISearchReport> LoadReports()
        {
            var reports = TReportSearch.Select(
                x => x.OrganizationIdentifier == Organization.OrganizationIdentifier
                  && x.UserIdentifier == User.UserIdentifier
                  && x.ReportType == ReportType.Filter
                  && x.ReportDescription == Route.Name);

            return reports
                .Select(x => DeserializeFilter(x.ReportData))
                .Where(x => x != null)
                .Cast<ISearchReport>()
                .ToList();
        }

        private static Filter DeserializeFilter(string reportData)
        {
            try
            {
                return JsonConvert.DeserializeObject<Filter>(reportData, jsonSettings);
            }
            catch
            {
                return null;
            }
        }

        protected override void SaveReports(List<ISearchReport> reports)
        {
            var entities = TReportSearch.Select(
                x => x.OrganizationIdentifier == Organization.OrganizationIdentifier
                  && x.UserIdentifier == User.UserIdentifier
                  && x.ReportType == ReportType.Filter
                  && x.ReportDescription == Route.Name);

            foreach (Filter filter in reports)
            {
                var entity = entities.FirstOrDefault(x => x.ReportTitle == filter.FilterName);
                var isNew = entity == null;

                if (isNew)
                    entity = new TReport
                    {
                        ReportIdentifier = UniqueIdentifier.Create(),
                        OrganizationIdentifier = Organization.OrganizationIdentifier,
                        UserIdentifier = User.UserIdentifier,
                        ReportType = ReportType.Filter,
                        ReportTitle = filter.FilterName,
                        ReportDescription = Route.Name,
                        Created = DateTimeOffset.Now,
                        CreatedBy = User.UserIdentifier,
                    };

                entity.ReportData = JsonConvert.SerializeObject(filter, typeof(Filter), jsonSettings);
                entity.Modified = DateTimeOffset.Now;
                entity.ModifiedBy = User.UserIdentifier;

                if (isNew)
                    TReportStore.Insert(entity);
                else
                    TReportStore.Update(entity);
            }

            foreach (var report in entities)
            {
                if (!reports.Any(x => x.Name == report.ReportTitle))
                    TReportStore.Delete(report.ReportIdentifier);
            }
        }

        protected override bool BindSavedReports()
        {
            var hasFilters = base.BindSavedReports();

            SavedFilterField.Visible = hasFilters;

            return hasFilters;
        }
    }
}