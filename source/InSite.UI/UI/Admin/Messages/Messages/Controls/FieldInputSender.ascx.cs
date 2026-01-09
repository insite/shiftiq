using System;

using InSite.Common.Web.UI;
using InSite.Persistence;

namespace InSite.Admin.Messages.Messages.Controls
{
    public partial class FieldInputSender : BaseUserControl
    {
        public event EventHandler Changed;

        private void OnChanged()
        {
            Changed?.Invoke(this, new EventArgs());
        }

        public string SenderEmail
        {
            get => ViewState[nameof(SenderEmail)] as string;
            private set => ViewState[nameof(SenderEmail)] = value;
        }

        public string SenderName
        {
            get => ViewState[nameof(SenderName)] as string;
            private set => ViewState[nameof(SenderName)] = value;
        }

        public string SenderStatus
        {
            get => ViewState[nameof(SenderStatus)] as string;
            set
            {
                ViewState[nameof(SenderStatus)] = value;
                SenderCombo.SenderEnabled = value == "Enabled";
            }
        }

        public string SenderType
        {
            get { return (string)ViewState[nameof(SenderType)]; }
            set
            {
                ViewState[nameof(SenderType)] = value;
                SenderCombo.SenderType = value;
                SenderCombo.RefreshData();

                RefreshSenderDescription(Value);
            }
        }

        public Guid? Value
        {
            get
            {
                return SenderCombo.ValueAsGuid;
            }
            set
            {
                SenderCombo.ValueAsGuid = value;
                RefreshSenderDescription(value);
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SenderCombo.AutoPostBack = true;
            SenderCombo.ValueChanged += SenderSelector_ValueChanged;
        }

        protected override void SetupValidationGroup(string groupName)
        {
            SenderFormValidator.ValidationGroup = groupName;
        }

        public void BindModelToControls(Guid senderId)
        {
            SenderCombo.EnsureDataBound();
            SenderCombo.ValueAsGuid = senderId;
            RefreshSenderDescription(Value);

            var enterpriseEmail = ServiceLocator.Partition.Email;
            SenderInstruction.InnerHtml = $"Contact {enterpriseEmail} to configure the list of Senders available in your account.";
        }

        private void SenderSelector_ValueChanged(object sender, EventArgs e)
        {
            RefreshSenderDescription(Value);
            OnChanged();
        }

        private void RefreshSenderDescription(Guid? value)
        {
            SenderDescription.InnerText = string.Empty;

            if (value.HasValue)
            {
                var sender = TSenderSearch.Select(value.Value);
                if (sender == null)
                    return;

                SenderName = sender.SenderName;

                SenderEmail = sender.SenderEmail;

                SenderStatusOutput.InnerHtml = sender.SenderEnabled
                    ? "<span class='badge bg-success'>Enabled</span>"
                    : "<span class='badge bg-danger'>Disabled</span>";

                SenderDescription.InnerHtml = $"Send using {sender.SenderType}"
                    + $"<br />from {sender.SenderName} &lt;{sender.SenderEmail}&gt;"
                    + $"<br />with bouncebacks delivered to {sender.SystemMailbox}";
            }
        }
    }
}