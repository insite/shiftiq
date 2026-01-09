using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Application.Attempts.Read;
using InSite.Common.Web.UI;

namespace InSite.UI.Admin.Assessments.Attempts.Controls
{
    public partial class PerformanceReportGrid : BaseUserControl
    {
        private Guid[] AttemptIds
        {
            get => (Guid[])ViewState[nameof(AttemptIds)];
            set => ViewState[nameof(AttemptIds)] = value;
        }

        public bool LoadData(Guid[] attemptIds)
        {
            var filter = new QAttemptFilter
            {
                FormOrganizationIdentifier = Organization.Key,
                AttemptIdentifiers = attemptIds
            };

            var data = ServiceLocator.AttemptSearch.GetAttempts(filter, x => x.Form);

            AttemptIds = data.Select(x => x.AttemptIdentifier).ToArray();

            AttemptRepeater.DataSource = data;
            AttemptRepeater.DataBind();

            return data.Count > 0;
        }

        public Guid[] GetSelectedAttempts()
        {
            var list = new List<Guid>();

            foreach (RepeaterItem row in AttemptRepeater.Items)
            {
                var assignCheckBox = (ICheckBoxControl)row.FindControl("AttemptCheckBox");
                if (assignCheckBox.Checked)
                    list.Add(AttemptIds[row.ItemIndex]);
            }

            return list.ToArray();
        }

        protected string FormatScore()
        {
            var attempt = (QAttempt)Page.GetDataItem();

            return AttemptReportHelper.FormatScore(
                attempt.AttemptGraded,
                attempt.AttemptSubmitted,
                attempt.AttemptIsPassing,
                attempt.AttemptScore,
                attempt.AttemptPoints,
                attempt.FormPoints
            );
        }

        protected string FormatTime()
        {
            var attempt = (QAttempt)Page.GetDataItem();

            return AttemptReportHelper.FormatTime(
                attempt.AttemptStarted,
                attempt.AttemptGraded,
                attempt.AttemptImported,
                attempt.AttemptDuration
            );
        }

        protected string GetFormAsset()
        {
            var attempt = (QAttempt)Page.GetDataItem();

            return AttemptReportHelper.GetFormAsset(
                attempt.Form?.FormAssetVersion,
                attempt.Form?.FormFirstPublished,
                attempt.Form?.FormAsset,
                attempt.AttemptStarted
            );
        }
    }
}