using System;
using System.Web.UI;

using InSite.Common.Web.Infrastructure;
using InSite.Persistence;

using Shift.Constant;

namespace InSite.UI.Admin.Jobs.Applications.Controls
{
    public partial class Detail : UserControl
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            OpportunityID.AutoPostBack = true;
            OpportunityID.ValueChanged += (x, y) => SetControlsVisibility();

            CandidateUserID.AutoPostBack = true;
            CandidateUserID.ValueChanged += (x, y) => SetControlsVisibility();
            CandidateUserID.Filter.IsCandidate = true;

            UploadCoverLetter.Click += UploadCoverLetter_Click;
            UploadResume.Click += UploadResume_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                SetControlsVisibility();
        }

        private void UploadCoverLetter_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var organization = CurrentSessionState.Identity.Organization;
            var ext = FileModel.GetExtension(CoverLetterUpload.PostedFile.FileName);
            var filePath = string.Format(OrganizationRelativePath.IecbcContactPathTemplate, CandidateUserID.Value) + OpportunityID.Value + "-cover-letter" + ext;

            FileHelper.Provider.Save(organization.Identifier, filePath, CoverLetterUpload.FileContent);

            CoverLetterUrl.Text = FileHelper.GetUrl(filePath);
        }

        private void UploadResume_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var organization = CurrentSessionState.Identity.Organization;
            var ext = FileModel.GetExtension(ResumeUpload.PostedFile.FileName);
            var filePath = string.Format(OrganizationRelativePath.IecbcContactPathTemplate, CandidateUserID.Value) + OpportunityID.Value + "-resume" + ext;

            FileHelper.Provider.Save(organization.Identifier, filePath, ResumeUpload.FileContent);

            ResumeUrl.Text = FileHelper.GetUrl(filePath);
        }

        public void SetInputValues(TApplication entity)
        {
            OpportunityID.Value = entity.OpportunityIdentifier;
            CandidateUserID.Value = entity.CandidateUserIdentifier;

            var organization = CurrentSessionState.Identity.Organization.Identifier;

            var fileContactCoverLetterFolder = string.Format(OrganizationRelativePath.IecbcContactPathTemplate, CandidateUserID.Value);

            if (!string.IsNullOrEmpty(entity.CandidateLetter))
            {
                CoverLetterUrl.Text = !string.IsNullOrEmpty(entity.CandidateLetter) ? FileHelper.GetUrl(fileContactCoverLetterFolder + entity.CandidateLetter) : null;
                if (!FileHelper.IsFileExists(CoverLetterUrl.Text))
                {
                    CoverLetterUrl.Text = "";
                    var candidateData = PersonSearch.Select(organization, CandidateUserID.Value.Value);
                    if (candidateData != null && candidateData.CustomKey != null && candidateData.CustomKey > 0)
                    {
                        fileContactCoverLetterFolder = string.Format(OrganizationRelativePath.IecbcContactPathTemplate, candidateData.CustomKey);
                        CoverLetterUrl.Text = !string.IsNullOrEmpty(entity.CandidateLetter) ? FileHelper.GetUrl(fileContactCoverLetterFolder + entity.CandidateLetter) : null;
                        if (!FileHelper.IsFileExists(CoverLetterUrl.Text))
                            CoverLetterUrl.Text = "";
                    }
                }

                UploadCoverLetter.Visible = true;
                ViewCoverLetter.Visible = !string.IsNullOrEmpty(CoverLetterUrl.Text);
            }

            var fileContactResumeFolder = string.Format(OrganizationRelativePath.IecbcContactPathTemplate, CandidateUserID.Value);

            if (!string.IsNullOrEmpty(entity.CandidateResume))
            {
                ResumeUrl.Text = !string.IsNullOrEmpty(entity.CandidateResume) ? FileHelper.GetUrl(fileContactResumeFolder + entity.CandidateResume) : null;
                if (!FileHelper.IsFileExists(ResumeUrl.Text))
                {
                    ResumeUrl.Text = "";
                    var candidateData = PersonSearch.Select(organization, CandidateUserID.Value.Value);
                    if (candidateData != null && candidateData.CustomKey != null && candidateData.CustomKey > 0)
                    {
                        fileContactResumeFolder = string.Format(OrganizationRelativePath.IecbcContactPathTemplate, candidateData.CustomKey);
                        ResumeUrl.Text = !string.IsNullOrEmpty(entity.CandidateResume) ? FileHelper.GetUrl(fileContactResumeFolder + entity.CandidateResume) : null;
                        if (!FileHelper.IsFileExists(ResumeUrl.Text))
                            ResumeUrl.Text = "";
                    }
                }

                UploadResume.Visible = true;
                ViewResume.Visible = !string.IsNullOrEmpty(ResumeUrl.Text);
            }

            OpportunityID.Enabled = false;
            CandidateUserID.Enabled = false;

            SetControlsVisibility();
        }

        public void GetInputValues(TApplication entity)
        {
            entity.OpportunityIdentifier = OpportunityID.Value.Value;
            entity.CandidateUserIdentifier = CandidateUserID.Value.Value;

            entity.CandidateLetter = FileModel.GetName(CoverLetterUrl.Text);
            entity.CandidateResume = FileModel.GetName(ResumeUrl.Text);
        }

        private void SetControlsVisibility()
        {
            CandidateCoverLetterField.Visible = OpportunityID.Value != null && CandidateUserID.Value != null;
            CandidateResumeField.Visible = OpportunityID.Value != null && CandidateUserID.Value != null;
        }
    }
}