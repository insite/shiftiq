using System;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Persistence;

namespace InSite.UI.Portal.Jobs.Candidates.MyPortfolio.Controls
{
    public partial class ViewDocumentSection : UserControl
    {
        private (HyperLink Url, Panel Panel)[] _referencesLetterInputs;
        private (HyperLink Url, Panel Panel)[] _certificateInputs;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            InitInputs();
        }
        public void BindModelToControls(Person person)
        {
            var userId = person.UserIdentifier;

            var resume = TCandidateUploadSearch.SelectFirst(x => x.CandidateUserIdentifier == userId && x.UploadType == "Resume");
            var coverLetter = TCandidateUploadSearch.SelectFirst(x => x.CandidateUserIdentifier == userId && x.UploadType == "CoverLetter");

            if(resume != null && resume.FileIdentifier != Guid.Empty)
            {
                    ResumeUrl.Text = $"{GenerateFileIcon(GetFileExtension(resume.UploadName))} CV or Resume";
                    ResumeUrl.NavigateUrl = UploadHelper.GetFileRelativePath(resume.FileIdentifier);
            }

            if (coverLetter != null && coverLetter.FileIdentifier != Guid.Empty)
            {

                    CoverLetterUrl.Text = $"{GenerateFileIcon(GetFileExtension(coverLetter.UploadName))} Cover Letter";
                    CoverLetterUrl.NavigateUrl = UploadHelper.GetFileRelativePath(coverLetter.FileIdentifier);
            }

            LoadReferenceLetters(userId);
            LoadCertificates(userId);
        }

        private void LoadReferenceLetters(Guid userId)
        {
            var files = TCandidateUploadSearch.Select(x => x.CandidateUserIdentifier == userId && x.UploadType == "ReferenceLetter", "UploadName,UploadIdentifier");

            if (files != null && files.Count > 0)
                ReferenceLettersPanel.Visible = true;

            for (var i = 0; i < files.Count && i < _referencesLetterInputs.Length; i++)
            {
                var file = files[i];
                if (file != null && file.FileIdentifier != Guid.Empty)
                {
                    string filename = GetFileNameWithoutExtension(file.UploadName);

                    _referencesLetterInputs[i].Url.Text = $"{GenerateFileIcon(GetFileExtension(file.UploadName))} {filename}";
                    _referencesLetterInputs[i].Url.NavigateUrl = UploadHelper.GetFileRelativePath(file.FileIdentifier);
                    _referencesLetterInputs[i].Panel.Visible = true;
                }
            }
        }

        private void LoadCertificates(Guid userId)
        {
            var files = TCandidateUploadSearch.Select(x => x.CandidateUserIdentifier == userId && x.UploadType == "Certificate", "UploadName,UploadIdentifier");

            if (files != null && files.Count > 0)
                CertificatesPanel.Visible = true;

            for (var i = 0; i < files.Count && i < _certificateInputs.Length; i++)
            {
                var file = files[i];
                if(file != null && file.FileIdentifier != Guid.Empty) 
                {
                    string filename = GetFileNameWithoutExtension(file.UploadName);

                    _certificateInputs[i].Url.Text = $"{GenerateFileIcon(GetFileExtension(file.UploadName))} {filename}";
                    _certificateInputs[i].Url.NavigateUrl = UploadHelper.GetFileRelativePath(file.FileIdentifier);
                    _certificateInputs[i].Panel.Visible = true;
                }
                
            }
        }

        private void InitInputs()
        {
            if (_referencesLetterInputs != null)
                return;

            _referencesLetterInputs = new[]
            {
                (ReferenceLetterUrl1, ReferenceLetterPanel1),
                (ReferenceLetterUrl2, ReferenceLetterPanel2),
                (ReferenceLetterUrl3, ReferenceLetterPanel3),
                (ReferenceLetterUrl4, ReferenceLetterPanel4),
                (ReferenceLetterUrl5, ReferenceLetterPanel5)
            };

            _certificateInputs = new[]
            {
                (CertificateUrl1, CertificatePanel1),
                (CertificateUrl2, CertificatePanel2),
                (CertificateUrl3, CertificatePanel3),
                (CertificateUrl4, CertificatePanel4),
                (CertificateUrl5, CertificatePanel5)
            };
        }

        private string GenerateFileIcon(string extension)
        {
            switch (extension)
            {
                case ".jpg":
                    return "<i class=\"far fa-fw fa-file-image\" aria-hidden=\"true\"></i>";
                case ".png":
                    return "<i class=\"far fa-fw fa-file-image\" aria-hidden=\"true\"></i>";
                case ".pdf":
                    return "<i class=\"far fa-fw fa-file-pdf\" aria-hidden=\"true\"></i>";
                case ".docx":
                    return "<i class=\"far fa-fw fa-file-word\" aria-hidden=\"true\"></i>";
                default:
                    return "<i class=\"far fa-fw fa-file\" aria-hidden=\"true\"></i>";
            }
        }

        private String GetFileNameWithoutExtension(String hrefLink)
        {
            return Path.GetFileNameWithoutExtension(Uri.UnescapeDataString(hrefLink).Replace("/", "\\"));
        }

        private String GetFileExtension(String hrefLink)
        {
            return Path.GetExtension(Uri.UnescapeDataString(hrefLink).Replace("/", "\\"));
        }
    }
}