using System;

using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;

namespace InSite.UI.Admin.Records.Reports.EngagementPrompt
{
    public partial class SearchCriteria : SearchCriteriaController<VLearnerActivityFilter>
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                
            }
        }

        public override VLearnerActivityFilter Filter
        {
            get
            {
                var filter = new VLearnerActivityFilter();

                filter.OrganizationIdentifier = Organization.OrganizationIdentifier;

                filter.CertificateStatus = CertificateStatus.Value;
                filter.GradebookNames = !string.IsNullOrEmpty(GradebookName.Text) ? new string[] { GradebookName.Text } : null;
                filter.LearnerEmail = LearnerEmail.Text;
                filter.LearnerNameFirst = LearnerNameFirst.Text;
                filter.LearnerNameLast = LearnerNameLast.Text;
                filter.ProgramNames = !string.IsNullOrEmpty(ProgramName.Text) ? new string[] { ProgramName.Text } : null;
                filter.EngagementPrompt = EngagementPromptStatus.Value.ToEnum<EngagementPromptType>();

                GetCheckedShowColumns(filter);

                return filter;
            }
            set
            {
                CertificateStatus.Value = value.CertificateStatus;
                GradebookName.Text = value.GradebookNames != null ? value.GradebookNames[0] : null;
                LearnerEmail.Text = value.LearnerEmail;
                LearnerNameFirst.Text = value.LearnerNameFirst;
                LearnerNameLast.Text = value.LearnerNameLast;
                ProgramName.Text = value.ProgramNames != null ? value.ProgramNames[0] : null;

                EngagementPromptStatus.Value = value.EngagementPrompt.ToString();
            }
        }

        public override void Clear()
        {
            CertificateStatus.Value = null;
            GradebookName.Text = null;
            LearnerEmail.Text = null;
            LearnerNameFirst.Text = null;
            LearnerNameLast.Text = null;
            ProgramName.Text = null;

            EngagementPromptStatus.Value = EngagementPromptType.None.ToString();
        }
    }
}