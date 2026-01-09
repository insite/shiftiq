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

        public event AlertHandler Alert;

        private void OnAlert(AlertType type, string message) =>
            Alert?.Invoke(this, new AlertArgs(type, message));

        private const string AchievementType = "Time-Sensitive Safety Certificate";

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

            AchievementSelector.Filter.AchievementType = AchievementType;
            AchievementSelector.Filter.OrganizationIdentifier = Organization.Identifier;
            AchievementSelector.Filter.AllowSelfDeclared = true;

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

            var achievement = ServiceLocator.AchievementSearch.GetAchievement(model.AchievementId);

            var achievementTitle = achievement.AchievementTitle;

            var issuedText = TimeZones.FormatDateOnly(model.Issued, TimeZones.Mountain, null, "{0:dddd MMMM d, yyyy}");

            var expiry = model.Issued.AddMonths(model.Lifetime);

            var expiryText = TimeZones.FormatDateOnly(expiry, TimeZones.Mountain, null, "{0:dddd MMMM d, yyyy}")
                + " (valid for " + Shift.Common.Humanizer.ToQuantity(model.Lifetime, "month") + ")";

            ConfirmAchievement.Text = achievementTitle;

            ConfirmIssued.Text = issuedText;

            ConfirmExpiry.Text = expiryText;

            var credential = ServiceLocator.AchievementSearch
                .GetCredential(model.AchievementId, User.Identifier);

            NewCredentialId = ServiceLocator.AchievementSearch
                .GetCredentialIdentifier(credential?.CredentialIdentifier, model.AchievementId, User.Identifier);

            if (AchievementFile.HasFile)
            {
                var file = AchievementFile.SaveFile(NewCredentialId, FileObjectType.Credential);

                var attachment = new FileModel
                {
                    DownloadUrl = ServiceLocator.StorageService.GetFileUrl(file.FileIdentifier, file.FileName, true),
                    FileName = HttpUtility.HtmlEncode(file.FileName),
                    FileSize = file.FileSize.Bytes().Humanize("#")
                };

                ConfirmFile.Text = $"<a href='{attachment.DownloadUrl}'>{attachment.FileName}</a> <span class='text-muted fs-sm'>({attachment.FileSize})</span>";
            }

            Wizard.ActiveViewIndex = 1;
        }

        private AddCertificateModel ValidateInputs()
        {
            var model = new AddCertificateModel();

            if (!AchievementSelector.Value.HasValue)
            {
                model.Errors.Add("Please select an achievement.");
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

            var utc = TimeZoneInfo.ConvertTime(model.Issued.Date, User.TimeZone, TimeZoneInfo.Utc);

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

            HttpResponseHelper.Redirect(Page.Request.RawUrl);
        }
    }
}