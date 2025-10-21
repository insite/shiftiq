using System;
using System.Collections.Generic;

using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

namespace InSite.UI.Admin.Records.Reports.EngagementPrompt
{
    public partial class Search : SearchPage<VLearnerActivityFilter>
    {
        public override string EntityName => "Engagement Prompt";

        private List<DateTimeOffset> SnapshotRepeaterKeys
        {
            get => (List<DateTimeOffset>)ViewState[nameof(SnapshotRepeaterKeys)];
            set => ViewState[nameof(SnapshotRepeaterKeys)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            PageHelper.AutoBindHeader(this);
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (!IsPostBack)
                MessageTab.IsSelected = true;
        }
    }
}
