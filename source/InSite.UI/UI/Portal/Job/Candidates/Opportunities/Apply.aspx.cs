using System;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Common.Web;
using InSite.Common.Web.Infrastructure;
using InSite.Domain.Messages;
using InSite.Domain.Organizations;
using InSite.Persistence;
using InSite.UI.Layout.Portal;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Portal.Jobs.Candidates
{
    public partial class Apply : PortalBasePage
    {

        #region Properties

        protected Person CurrentUser
        {
            get
            {
                if (!_isCurrentUserLoaded)
                {
                    _currentUser = PersonSearch.Select(
                        CurrentSessionState.Identity.Organization.OrganizationIdentifier,
                        CurrentSessionState.Identity.User.UserIdentifier);

                    if (_currentUser != null && !_currentUser.JobsApproved.HasValue &&
                        !(MembershipSearch.IsUserAssignedToDepartment(_currentUser.OrganizationIdentifier, _currentUser.UserIdentifier) ||
                          MembershipSearch.IsUserAssignedToRole(_currentUser.OrganizationIdentifier, _currentUser.UserIdentifier)))

                        _currentUser = null;

                    _isCurrentUserLoaded = true;
                }

                return _currentUser;
            }
        }

        protected bool IsUserApproved =>
            CurrentUser != null && CurrentUser.JobsApproved.HasValue && CurrentUser.UserAccessGranted.HasValue;

        #endregion

        #region Fields

        protected string SearchUrl => "/ui/portal/job/candidates/opportunities/search";
        protected Guid JobOpportunity => Guid.TryParse(Request.QueryString["id"], out var result) ? result : Guid.Empty;
        private Person _currentUser;
        private bool _isCurrentUserLoaded;

        #endregion

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ResumeUploadValidator.ServerValidate += ResumeUploadValidator_ServerValidate;
            CoverLetterUploadValidator.ServerValidate += CoverLetterUploadValidator_ServerValidate;

            SubmitButton.Click += SubmitButton_Click;
        }

        private void ItemsValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = false;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsUserApproved)
                HttpResponseHelper.Redirect(SearchUrl, true);

            if (IsPostBack)
                return;

            Layout.Admin.PageHelper.AutoBindHeader(this);

            if (!IsPostBack)
            {
                LoadData();
            }
        }

        private void LoadData()
        {
            SubmitButton.Visible = NotApproved.Visible = false;

            var jobOpportunity = TOpportunitySearch.Select(JobOpportunity);

            if (jobOpportunity == null)
            {
                HttpResponseHelper.Redirect(SearchUrl, true);
                return;
            }

            ResumeRequired.Visible = (jobOpportunity.ApplicationRequiresResume.HasValue ? jobOpportunity.ApplicationRequiresResume.Value : false);
            CoverLetterRequired.Visible = (jobOpportunity.ApplicationRequiresLetter.HasValue ? jobOpportunity.ApplicationRequiresLetter.Value : false);

            var opportunityApplication = TApplicationSearch.SelectJobApplication(CurrentUser.UserIdentifier, JobOpportunity);
            var title = $"Applying for {jobOpportunity.JobTitle} {jobOpportunity.JobLevel}";

            JobTitle.Text = title;

            CandidatePhoneNumber.Text = CurrentUser?.Phone;
            CandidateName.Text = CurrentSessionState.Identity.User.FullName;
            CandidateEmail.Text = CurrentSessionState.Identity.User.Email;

            if (opportunityApplication != null)
                StatusAlert.AddMessage(AlertType.Success, "You applied for this opportunity on "
                    + (TimeZones.FormatDateOnly(opportunityApplication.WhenCreated, User.TimeZone, CultureInfo.GetCultureInfo(Identity.Language))));

            SubmitButton.Visible = (IsUserApproved && opportunityApplication == null);
            NotApproved.Visible = !IsUserApproved;
        }

        #region Evevnt handlers

        private void SubmitButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var jobOpportunity = TOpportunitySearch.Select(JobOpportunity);
            bool missingUpload = false;

            missingUpload = EnsureUpload(jobOpportunity, missingUpload);

            if (missingUpload)
                return;

            var organization = CurrentSessionState.Identity.Organization;
            var phoneNumber = CandidatePhoneNumber.Text;
            var emailAddress = CandidateEmail.Text;

            var candidateIdentifier = CurrentUser.UserIdentifier;

            FileModel candidateCoverLetter, candidateResume;
            var attachment = GetAttachments(organization, candidateIdentifier, out candidateCoverLetter, out candidateResume);

            var entity = new TApplication
            {
                ApplicationIdentifier = UniqueIdentifier.Create(),
                OpportunityIdentifier = JobOpportunity,
                CandidateUserIdentifier = candidateIdentifier,
                CandidateLetter = candidateCoverLetter != null ? FileModel.GetName(candidateCoverLetter.Path) : "",
                CandidateResume = candidateResume != null ? FileModel.GetName(candidateResume.Path) : ""
            };

            entity.WhenCreated = DateTime.UtcNow;
            entity.WhenModified = DateTime.UtcNow;

            TApplicationStore.Insert(entity);

            var employerIdentifier = jobOpportunity.EmployerUserIdentifier;

            // Mar 27, 2025 - Daniel: 
            // JobOpportunity.ApplicationEmail is NOT used for this notification. Every email
            // message must be sent to a contact person who exists in the User table with a unique
            // UserIdentifier value. If there is some future requirement to send notification to
            // JobOpportunity.ApplicationEmail, and if this email address does not correspond to any
            // existing user in the database, then a new user must be created first. In the last 6
            // months this notification has been sent only 3 times, therefore this does not need a 
            // lot of our time and attention!

            var alertEmployer = new AlertJobsCandidateAppliedForOpportunity
            {
                OpportunityIdentifier = JobOpportunity,
                CandidateIdentifier = candidateIdentifier,
                CandidatePhoneNumber = phoneNumber,
                CandidateEmailAddress = emailAddress,
                CandidateUrl = $"{HttpRequestHelper.CurrentRootUrl}/ui/portal/job/employers/candidates/view?id={candidateIdentifier}",
                CandidateFirstName = User.FirstName,
                CandidateLastName = User.LastName,
                JobPosition = jobOpportunity.JobTitle,
                EmployerIdentifier = jobOpportunity.EmployerUserIdentifier
            };

            ServiceLocator.AlertMailer.Send(organization.Identifier, candidateIdentifier, alertEmployer, attachment);

            StatusAlert.AddMessage(AlertType.Success, "Your application for this opportunity has been successfully submitted. Thank you for applying!");

            SubmitButton.Visible = false;
        }

        private void ResumeUploadValidator_ServerValidate(object source, ServerValidateEventArgs e)
        {
            e.IsValid = e.Value == null
                || e.Value.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase)
                || e.Value.EndsWith(".doc", StringComparison.OrdinalIgnoreCase)
                || e.Value.EndsWith(".docx", StringComparison.OrdinalIgnoreCase)
                || e.Value.EndsWith(".txt", StringComparison.OrdinalIgnoreCase);
        }

        private void CoverLetterUploadValidator_ServerValidate(object source, ServerValidateEventArgs e)
        {
            e.IsValid = e.Value == null
                || e.Value.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase)
                || e.Value.EndsWith(".doc", StringComparison.OrdinalIgnoreCase)
                || e.Value.EndsWith(".docx", StringComparison.OrdinalIgnoreCase)
                || e.Value.EndsWith(".txt", StringComparison.OrdinalIgnoreCase);
        }

        #endregion

        #region Helper Methods

        private Guid? FindEmployerIdentifier(TOpportunity jobOpportunity)
        {
            var employerRepresentative = UserSearch.Select(jobOpportunity.EmployerUserIdentifier);
            if (employerRepresentative != null)
                return employerRepresentative.UserIdentifier;
            return null;
        }

        private bool EnsureUpload(TOpportunity jobOpportunity, bool missingUpload)
        {
            if (jobOpportunity.ApplicationRequiresResume.HasValue && jobOpportunity.ApplicationRequiresResume.Value)
            {
                if (!ResumeUpload.HasFile)
                {
                    StatusAlert.AddMessage(AlertType.Error, "Resume file is empty");
                    missingUpload = true;
                }
            }

            if (jobOpportunity.ApplicationRequiresLetter.HasValue && jobOpportunity.ApplicationRequiresLetter.Value)
            {
                if (!CoverLetterUpload.HasFile)
                {
                    StatusAlert.AddMessage(AlertType.Error, "Cover letter file is empty");
                    missingUpload = true;
                }
            }

            return missingUpload;
        }

        private string[] GetAttachments(OrganizationState organization, Guid candidateIdentifier, out FileModel candidateCoverLetter, out FileModel candidateResume)
        {
            StringCollection attachments = new StringCollection();

            candidateCoverLetter = null;
            candidateResume = null;
            if (CoverLetterUpload.HasFile) candidateCoverLetter = UploadCoverLetterFile(candidateIdentifier, JobOpportunity, organization.Identifier);
            if (ResumeUpload.HasFile) candidateResume = UploadResumeFile(candidateIdentifier, JobOpportunity, organization.Identifier);

            AttachFile(organization.Identifier, attachments, candidateCoverLetter);
            AttachFile(organization.Identifier, attachments, candidateResume);

            return attachments.Cast<String>().ToArray();
        }

        private void AttachFile(Guid organization, StringCollection attachments, FileModel file)
        {
            // If there is no file then abort.
            if (file == null)
                return;

            // Read the file content into a stream.
            using (var stream = FileHelper.Provider.Read(organization, file.Path))
            {
                // If the stream is empty then abort.
                if (stream == Stream.Null)
                    return;

                // Create a temporary directory in which to store the file so it can be attached to the job application.
                var name = FileModel.GetName(file.Path);
                var temp = $@"{ServiceLocator.FilePaths.FileStoragePath}\Temp\{file.Guid}";
                if (!Directory.Exists(temp))
                    Directory.CreateDirectory(temp);

                // Copy the file into the temporary directory.
                var physicalPath = Path.Combine(temp, name);
                using (var output = File.Create(physicalPath))
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    stream.CopyTo(output);
                    output.Close();
                }

                // Use the physical path to the temporary file for the attachment.
                attachments.Add(physicalPath);
            }
        }

        private FileModel UploadCoverLetterFile(Guid candidateIdentifier, Guid opportunityIdentifier, Guid organizationIdentifier)
        {
            var ext = FileModel.GetExtension(CoverLetterUpload.PostedFile.FileName);
            var fileName = opportunityIdentifier + "-cover-letter" + ext;
            var filePath = string.Format(OrganizationRelativePath.IecbcContactPathTemplate, candidateIdentifier) + fileName;

            return FileHelper.Provider.Save(organizationIdentifier, filePath, CoverLetterUpload.FileContent, isCheckFileSizeLimits: false);
        }

        private FileModel UploadResumeFile(Guid candidateIdentifier, Guid opportunityIdentifier, Guid organizationIdentifier)
        {
            var ext = FileModel.GetExtension(ResumeUpload.PostedFile.FileName);
            var fileName = opportunityIdentifier + "-resume" + ext;
            var filePath = string.Format(OrganizationRelativePath.IecbcContactPathTemplate, candidateIdentifier) + fileName;

            return FileHelper.Provider.Save(organizationIdentifier, filePath, ResumeUpload.FileContent, isCheckFileSizeLimits: false);
        }

        #endregion
    }
}