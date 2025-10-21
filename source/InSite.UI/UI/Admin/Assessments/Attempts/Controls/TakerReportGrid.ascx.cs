using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Application.Attempts.Read;
using InSite.Common.Web.UI;

using SystemLiteral = System.Web.UI.WebControls.Literal;

namespace InSite.UI.Admin.Assessments.Attempts.Controls
{
    public partial class TakerReportGrid : BaseUserControl
    {
        public bool LoadData(QAttemptFilter filter)
        {
            var data = ServiceLocator.TakerReportSearch.GetTakerReport(filter);

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
                if (!assignCheckBox.Checked)
                    continue;

                var attemptIdentifierLiteral = (SystemLiteral)row.FindControl("AttemptIdentifier");
                var attemptIdentifier = Guid.Parse(attemptIdentifierLiteral.Text);

                list.Add(attemptIdentifier);
            }

            return list.ToArray();
        }

        protected string FormatScore()
        {
            var item = Page.GetDataItem();
            var attemptGraded = DataBinder.Eval(item, "AttemptGraded") as DateTimeOffset?;
            var attemptSubmitted = DataBinder.Eval(item, "AttemptSubmitted") as DateTimeOffset?;
            var attemptIsPassing = (bool)DataBinder.Eval(item, "AttemptIsPassing");
            var attemptScore = DataBinder.Eval(item, "AttemptScore") as decimal?;
            var attemptPoints = DataBinder.Eval(item, "AttemptPoints") as decimal?;
            var formPoints = DataBinder.Eval(item, "FormPoints") as decimal?;

            return AttemptReportHelper.FormatScore(
                attemptGraded,
                attemptSubmitted,
                attemptIsPassing,
                attemptScore,
                attemptPoints,
                formPoints
            );
        }

        protected string FormatTime()
        {
            var item = Page.GetDataItem();
            var attemptStarted = DataBinder.Eval(item, "AttemptStarted") as DateTimeOffset?;
            var attemptGraded = DataBinder.Eval(item, "AttemptGraded") as DateTimeOffset?;
            var attemptImported = DataBinder.Eval(item, "AttemptImported") as DateTimeOffset?;
            var attemptDuration = DataBinder.Eval(item, "AttemptDuration") as decimal?;

            return AttemptReportHelper.FormatTime(
                attemptStarted,
                attemptGraded,
                attemptImported,
                attemptDuration
            );
        }

        protected string GetFormAsset()
        {
            var item = Page.GetDataItem();
            var formAssetVersion = DataBinder.Eval(item, "FormAssetVersion") as int?;
            var formFirstPublished = DataBinder.Eval(item, "FormFirstPublished") as DateTimeOffset?;
            var formAsset = DataBinder.Eval(item, "FormAsset") as int?;
            var attemptStarted = DataBinder.Eval(item, "AttemptStarted") as DateTimeOffset?;

            return AttemptReportHelper.GetFormAsset(
                formAssetVersion,
                formFirstPublished,
                formAsset,
                attemptStarted
            );
        }
    }
}