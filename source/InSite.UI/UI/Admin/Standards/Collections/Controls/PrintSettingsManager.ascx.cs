using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Common.Web.UI;

using Shift.Common;

namespace InSite.Admin.Standards.Collections.Controls
{
    public partial class PrintSettingsManager : BaseReportManager
    {
        private static readonly string BasePersonalizationName = typeof(PrintSettingsManager).FullName + ":Base";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Script.ContentKey = UniqueID;
        }

        #region Methods (BaseReportManager)

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
            return PersonalizationRepository
                .GetValue<Filter[]>(Organization.OrganizationIdentifier, User.UserIdentifier, BasePersonalizationName, false)
                .EmptyIfNull()
                .Cast<ISearchReport>()
                .ToList();
        }

        protected override void SaveReports(List<ISearchReport> value)
        {
            PersonalizationRepository.SetValue(
                Organization.OrganizationIdentifier,
                User.UserIdentifier,
                BasePersonalizationName,
                value);
        }

        protected override bool BindSavedReports()
        {
            var hasReports = base.BindSavedReports();

            SavedFilterField.Visible = hasReports;

            return hasReports;
        }

        #endregion
    }
}