using System;

using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;

namespace InSite.UI.Admin.Reports.Controls
{
    public partial class SearchCriteria : SearchCriteriaController<VReportFilter>
    {
        public override VReportFilter Filter
        {
            get
            {
                var filter = new VReportFilter
                {
                    OrganizationIdentifier = Organization.Identifier,
                    UserIdentifier = User.UserIdentifier,
                    ReportTitle = ReportTitle.Text,
                    ReportDescription = ReportDescription.Text,
                    ReportTypes = ReportType.Value.IsNotEmpty() ? new[] { ReportType.Value } : null,
                    IsCreator = SharedReportMine.Checked,
                    CreatedBy = CreatedBy.ValueAsGuid,
                    ModifiedSince = ModifiedSince.Value.ToDateTimeOffset(TimeSpan.Zero),
                    ModifiedBefore = ModifiedBefore.Value.ToDateTimeOffset(TimeSpan.Zero),
                    IncludeShared = true
                };

                GetCheckedShowColumns(filter);

                return filter;
            }
            set
            {
                ReportTitle.Text = value.ReportTitle;
                ReportDescription.Text = value.ReportDescription;
                ReportType.Value = value.ReportTypes.IsNotEmpty() ? value.ReportTypes[0] : null;
                SharedReportMine.Checked = (bool)(value.IsCreator != null ? value.IsCreator : (bool?)null);
                CreatedBy.ValueAsGuid = value.CreatedBy;
                ModifiedSince.Value = value.ModifiedSince?.UtcDateTime;
                ModifiedBefore.Value = value.ModifiedBefore?.UtcDateTime;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                RenderSearchCriteria();
        }

        public override void Clear()
        {
            ReportTitle.Text = null;
            ReportDescription.Text = null;
            ReportType.ClearSelection();
            SharedReportMine.Checked = true;
            SharedReportYes.Checked = false;
            CreatedBy.ClearSelection();
            ModifiedSince.Value = null;
            ModifiedBefore.Value = null;
        }

        #region Methods (render)

        private void RenderSearchCriteria()
        {
            SharedReportMine.Text = Translate("Mine");
            SharedReportYes.Text = Translate("Shared with me");

            CreatedBy.LoadItems(VReportSearch.GetCreatedByListItems(Organization.Identifier, User.UserIdentifier));
        }

        #endregion
    }
}