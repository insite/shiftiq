using System;
using System.Linq;
using System.Web.UI;

using InSite.Application.Contacts.Read;
using InSite.Common.Web.UI;
using InSite.Persistence;

namespace InSite.UI.Portal.Jobs.Candidates.MyPortfolio.Controls
{
    public partial class DocumentSection : UserControl
    {
        private (TextBox Url, FileUploadV2 Upload)[] _referencesLetterInputs;
        private (TextBox Url, FileUploadV2 Upload)[] _certificateInputs;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            InitInputs();
        }

        public void SaveUploads(QPerson person)
        {
            var userId = person.UserIdentifier;

            UploadHelper.SaveContactUpload(userId, "Resume", "resume", ResumeUpload);
            UploadHelper.SaveContactUpload(userId, "CoverLetter", "cover-letter", CoverLetterUpload);
            UploadHelper.SaveContactUploads(userId, "ReferenceLetter", "reference-letter-{0}", _referencesLetterInputs.Select(x => x.Upload).ToList());
            UploadHelper.SaveContactUploads(userId, "Certificate", "certificate-{0}", _certificateInputs.Select(x => x.Upload).ToList());
        }

        public void BindModelToControls(Person person)
        {
            var userId = person.UserIdentifier;

            var resume = TCandidateUploadSearch.SelectFirst(x => x.CandidateUserIdentifier == userId && x.UploadType == "Resume");
            var coverLetter = TCandidateUploadSearch.SelectFirst(x => x.CandidateUserIdentifier == userId && x.UploadType == "CoverLetter");

            ResumeUrl.Text = resume != null && resume.FileIdentifier != Guid.Empty ? UploadHelper.GetFileRelativePath(resume.FileIdentifier) : null;
            CoverLetterUrl.Text = coverLetter != null && coverLetter.FileIdentifier != Guid.Empty ? UploadHelper.GetFileRelativePath(coverLetter.FileIdentifier) : null;

            LoadReferenceLetters(userId);
            LoadCertificates(userId);
        }

        private void LoadReferenceLetters(Guid userId)
        {
            var files = TCandidateUploadSearch.Select(x => x.CandidateUserIdentifier == userId && x.UploadType == "ReferenceLetter", "UploadName,UploadIdentifier");

            for (var i = 0; i < files.Count && i < _referencesLetterInputs.Length; i++)
            {
                var file = files[i];
                _referencesLetterInputs[i].Url.Text = file.FileIdentifier != Guid.Empty ?UploadHelper.GetFileRelativePath(file.FileIdentifier) : null;
            }
        }

        private void LoadCertificates(Guid userId)
        {
            var files = TCandidateUploadSearch.Select(x => x.CandidateUserIdentifier == userId && x.UploadType == "Certificate", "UploadName,UploadIdentifier");

            for (var i = 0; i < files.Count && i < _certificateInputs.Length; i++)
            {
                var file = files[i];
                _certificateInputs[i].Url.Text = file.FileIdentifier != Guid.Empty ? UploadHelper.GetFileRelativePath(file.FileIdentifier) : null;
            }
        }

        private void InitInputs()
        {
            if (_referencesLetterInputs != null)
                return;

            _referencesLetterInputs = new[]
            {
                (ReferenceLetterUrl1, ReferenceLetterUpload1),
                (ReferenceLetterUrl2, ReferenceLetterUpload2),
                (ReferenceLetterUrl3, ReferenceLetterUpload3),
                (ReferenceLetterUrl4, ReferenceLetterUpload4),
                (ReferenceLetterUrl5, ReferenceLetterUpload5)
            };

            _certificateInputs = new[]
            {
                (CertificateUrl1, CertificateUpload1),
                (CertificateUrl2, CertificateUpload2),
                (CertificateUrl3, CertificateUpload3),
                (CertificateUrl4, CertificateUpload4),
                (CertificateUrl5, CertificateUpload5)
            };
        }
    }
}