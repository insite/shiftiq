using System;
using System.Web.UI;

using InSite.Persistence;

namespace InSite.UI.Admin.Jobs.Candidates.Controls
{
    public partial class CommentDetail : UserControl
    {
        public void SetDefaultInputValues(Guid candidateId)
        {
            CandidateID.Enabled = false;
            CandidateID.Value = candidateId;
        }

        public void GetInputValues(VCandidateComment entity)
        {
            entity.CommentText = Text.Text;
        }

        public void SetInputValues(VCandidateComment entity)
        {
            CandidateID.Value = entity.CandidateUserIdentifier;
            CandidateID.Enabled = false;
            Text.Text = entity.CommentText;
        }
    }
}