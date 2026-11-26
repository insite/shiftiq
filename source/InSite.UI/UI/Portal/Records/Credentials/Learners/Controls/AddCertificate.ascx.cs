using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Humanizer;

using InSite.Application.Credentials.Write;
using InSite.Application.Files.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Records;
using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;

using Shift.Common;
using Shift.Common.Events;
using Shift.Constant;

namespace InSite.UI.Portal.Records.Credentials.Learners.Controls
{
    public partial class AddCertificate : BaseUserControl
    {
        private class FileModel
        {
            public string DownloadUrl { get; set; }
            public string FileName { get; set; }
            public string FileSize { get; set; }
            public string DocumentType { get; set; }
            public DateTimeOffset Uploaded { get; set; }
        }

        private class AddCertificateModel
        {
            public Guid AchievementId { get; set; }
            public DateTimeOffset Issued { get; set; }
            public int Lifetime { get; set; }
            public FileModel File { get; set; }
            public List<string> Errors { get; set; } = new List<string>();
        }

        // An upload is temporary for eight hours (TempFileExpirationInMinutes). A page left open
        // longer, or posted again after Save, refers to files it can no longer attach.
        private const string UploadUnavailableMessage =
            "Your file upload is no longer available. Please start again and select your files.";

        public event AlertHandler Alert;

        private void OnAlert(AlertType type, string message) =>
            Alert?.Invoke(this, new AlertArgs(type, message));

        public Guid NewCredentialId
        {
            get => (Guid?)ViewState[nameof(NewCredentialId)] ?? Guid.Empty;
            set => ViewState[nameof(NewCredentialId)] = value;
        }

        public string HelpContent
        {
            get => (string)ViewState[nameof(HelpContent)];
            set => ViewState[nameof(HelpContent)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            AchievementSelector.AutoPostBack = true;
            AchievementSelector.ValueChanged += AchievementSelector_ValueChanged;

            AchievementSelector.Filter.OrganizationIdentifier = Organization.Identifier;
            AchievementSelector.Filter.AllowSelfDeclared = true;
            AchievementSelector.Filter.IssuesCertificate = true;
            AchievementSelector.Filter.ExcludePaidProgramAchievements = true;

            if (VCmdsAchievementSearch.CountForSelector(AchievementSelector.Filter, false) == 0)
                Visible = false;

            NextButton.Click += NextButton_Click;

            SaveButton.Click += SaveButton_Click;
            CancelButton.NavigateUrl = Page.Request.RawUrl;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                try
                {
                    HelpContent = GetEmbededHelpContent("#add-new-certificate").NullIfEmpty();
                }
                catch (InvalidOperationException ioex)
                {
                    HelpContent = null;

                    OnAlert(AlertType.Error, "Error loading embedded help content. " + ioex.Message);
                }
            }

            if (HelpContent != null)
            {
                EmbeddedHelp.Clear();
                EmbeddedHelp.AddMessage(AlertType.Information, Icons.EmbeddedHelp, HelpContent);
            }
        }

        private void AchievementSelector_ValueChanged(object sender, Shift.Sdk.UI.FindEntityValueChangedEventArgs e)
        {
            AchievementLifetime.Enabled = true;

            if (e.NewValue == null)
                return;

            var achievementId = e.NewValue.Value;

            var achievement = ServiceLocator.AchievementSearch.GetAchievement(achievementId);

            var lifetimeUnit = achievement.ExpirationLifetimeUnit;

            var lifetimeQuantity = achievement.ExpirationLifetimeQuantity ?? 0;

            var lifetimeInMonths = 0;

            if (StringHelper.Equals(lifetimeUnit, "Year"))
                lifetimeInMonths = 12 * lifetimeQuantity;

            else if (StringHelper.Equals(lifetimeUnit, "Month"))
                lifetimeInMonths = lifetimeQuantity;

            if (lifetimeInMonths == 0)
                return;

            AchievementLifetime.ValueAsInt = lifetimeInMonths;

            AchievementLifetime.Enabled = false;
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            var model = ValidateInputs();

            if (model.Errors.Any())
            {
                ValidationStatus.Visible = true;
                ValidationStatus.Text = string.Join("<br />", model.Errors);
                return;
            }

            // The uploads stay temporary until Save attaches them. Attaching them here left them on
            // the credential when the learner cancelled, and the next attempt added more files.
            var uploads = GetUploads();

            if (!CredentialFiles.CanAttach(uploads))
            {
                AchievementFile.ClearFiles();
                ValidationStatus.Visible = true;
                ValidationStatus.Text = UploadUnavailableMessage;
                return;
            }

            var achievement = ServiceLocator.AchievementSearch.GetAchievement(model.AchievementId);

            var achievementTitle = achievement.AchievementTitle;

            var issuedText = TimeZones.FormatDateOnly(model.Issued, TimeZones.Ids.Mountain, null, "{0:dddd MMMM d, yyyy}");

            var expiry = model.Issued.AddMonths(model.Lifetime);

            var expiryText = TimeZones.FormatDateOnly(expiry, TimeZones.Ids.Mountain, null, "{0:dddd MMMM d, yyyy}")
                + " (valid for " + Shift.Common.Humanizer.ToQuantity(model.Lifetime, "month") + ")";

            ConfirmAchievement.Text = achievementTitle;

            ConfirmIssued.Text = issuedText;

            ConfirmExpiry.Text = expiryText;

            var credential = ServiceLocator.AchievementSearch
                .GetCredential(model.AchievementId, User.Identifier);

            NewCredentialId = ServiceLocator.AchievementSearch
                .GetCredentialIdentifier(credential?.CredentialIdentifier, model.AchievementId, User.Identifier);

            ConfirmFile.Text = string.Join("<br />", uploads
                .Select(x => GetAttachmentHtml(ServiceLocator.StorageService.GetFile(x))));

            Wizard.ActiveViewIndex = 1;
        }

        private Guid[] GetUploads() =>
            AchievementFile.UploadedFiles.Select(x => x.FileIdentifier).ToArray();

        private static string GetAttachmentHtml(FileStorageModel file)
        {
            var attachment = new FileModel
            {
                DownloadUrl = ServiceLocator.StorageService
                    .GetFileUrl(file.FileIdentifier, file.FileName, true),
                FileName = HttpUtility.HtmlEncode(file.FileName),
                FileSize = file.FileSize.Bytes().Humanize("#")
            };

            return $"<a href='{attachment.DownloadUrl}'>{attachment.FileName}</a>"
                + $" <span class='text-muted fs-sm'>({attachment.FileSize})</span>";
        }

        private AddCertificateModel ValidateInputs()
        {
            var model = new AddCertificateModel();

            if (!AchievementSelector.Value.HasValue)
            {
                model.Errors.Add("Please select an achievement.");
                return model;
            }

            // The selector hides paid-program achievements; this catches a stale page or a forged post.
            var isPaidAchievement = PaidProgramAchievementSearch.IsPaidAchievement(AchievementSelector.Value.Value);
            if (isPaidAchievement)
            {
                model.Errors.Add(PaidProgramAchievementSearch.AssignmentRefusedMessage);
                return model;
            }

            if (!AchievementIssued.Value.HasValue)
            {
                model.Errors.Add("Please select the date when the certificate was issued for this achievement.");
                return model;
            }

            if (AchievementIssued.Value.Value > DateTimeOffset.UtcNow)
            {
                model.Errors.Add("Please select a date before today. You can add future achievements after they are completed.");
                return model;
            }

            if (!AchievementLifetime.ValueAsInt.HasValue
                || AchievementLifetime.ValueAsInt < AchievementLifetime.MinValue
                || AchievementLifetime.ValueAsInt > AchievementLifetime.MaxValue)
            {
                model.Errors.Add("Please input the number of months before the achievement expires.");
                return model;
            }

            var expiry = AchievementIssued.Value.Value.AddMonths(AchievementLifetime.ValueAsInt.Value);

            if (expiry < DateTimeOffset.UtcNow)
            {
                model.Errors.Add($"The certificate for this achievement expired {expiry:MMMM d, yyyy}. Please add current certificates only.");
                return model;
            }

            model.AchievementId = AchievementSelector.Value.Value;
            model.Issued = AchievementIssued.Value.Value;
            model.Lifetime = AchievementLifetime.ValueAsInt.Value;

            if (!AchievementFile.HasFile)
            {
                model.Errors.Add($"Please attach a copy of your certificate.");
            }

            return model;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            var org = Organization.Identifier;

            var model = ValidateInputs();

            // The Next step already validated, but a paid-program achievement is refused here too, so
            // a replayed save cannot self-declare one.
            if (model.Errors.Contains(PaidProgramAchievementSearch.AssignmentRefusedMessage))
            {
                OnAlert(AlertType.Error, PaidProgramAchievementSearch.AssignmentRefusedMessage);
                return;
            }

            var uploads = GetUploads();

            if (!CredentialFiles.CanAttach(uploads))
            {
                OnAlert(AlertType.Error, UploadUnavailableMessage);
                return;
            }

            // Read before the commands below, which leave the credential Submitted. Whether an
            // administrator accepted the previous submission decides if its files are replaced.
            var previousStatus = ServiceLocator.AchievementSearch
                .GetCredential(NewCredentialId)?.CredentialStatus;

            var utc = TimeZones.ConvertTime(model.Issued.Date, User.TimeZone, TimeZones.Ids.Utc);

            var description = $"Self-declared by {User.FirstName} on {DateTime.Today:MMM d, yyyy}.";

            var userId = User.UserIdentifier;

            var grant = new CreateAndGrantCredential(NewCredentialId, org,
                model.AchievementId, userId,
                utc, description, null, null, null);

            ServiceLocator.SendCommand(grant);

            var change = new ChangeCredentialAuthority(NewCredentialId, userId, null, "Self", "CMDS", null, null);

            ServiceLocator.SendCommand(change);

            var expiration = new Expiration(ExpirationType.Relative, null, model.Lifetime, "Month");

            var expiry = new ChangeCredentialExpiration(NewCredentialId, expiration);

            ServiceLocator.SendCommand(expiry);

            // Send this last. A self-uploaded certificate is not valid until an administrator reviews it, and both
            // the authority change and the expiration change would otherwise leave the credential marked Valid.
            var submit = new SubmitCredential(NewCredentialId, DateTimeOffset.UtcNow, description);

            ServiceLocator.SendCommand(submit);

            // Attach after the commands, so a failure above keeps the previous certificates. This
            // also deletes the files of a previous submission nobody accepted in the same cycle;
            // an accepted submission and an earlier cycle are the credential's record and stay.
            CredentialFiles.Attach(org, NewCredentialId, uploads, previousStatus);

            HttpResponseHelper.Redirect(Page.Request.RawUrl);
        }
    }
}
