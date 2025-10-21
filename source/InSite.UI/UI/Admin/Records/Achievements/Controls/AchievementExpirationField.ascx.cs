using System;

using InSite.Application.Records.Read;
using InSite.Common.Web.UI;
using InSite.Domain.Records;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Admin.Records.Achievements.Controls
{
    public partial class AchievementExpirationField : BaseUserControl
    {
        public string LabelText
        {
            get => (string)ViewState[nameof(LabelText)] ?? "Expiry";
            set => ViewState[nameof(LabelText)] = value;
        }

        public string HelpText
        {
            get => (string)ViewState[nameof(HelpText)] ?? "If achievement expires, select whether this is a fixed date or relative to when issued.";
            set => ViewState[nameof(HelpText)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ExpirationTypeNoExpiry.AutoPostBack = true;
            ExpirationTypeNoExpiry.CheckedChanged += ExpirationType_CheckedChanged;

            ExpirationTypeFixed.AutoPostBack = true;
            ExpirationTypeFixed.CheckedChanged += ExpirationType_CheckedChanged;

            ExpirationTypeRelative.AutoPostBack = true;
            ExpirationTypeRelative.CheckedChanged += ExpirationType_CheckedChanged;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                SetControlsVisibility();
        }

        protected override void OnPreRender(EventArgs e)
        {
            LabelTextOutput.Text = LabelText;

            HelpTextOutput.InnerText = HelpText;
            HelpTextOutput.Visible = HelpText.IsNotEmpty();

            base.OnPreRender(e);
        }

        public void SetExpiration(QAchievement achievement)
        {
            SetExpiration(new Expiration
            {
                Type = achievement.ExpirationType.ToEnum(ExpirationType.None),
                Date = achievement.ExpirationFixedDate,
                Lifetime = new Lifetime
                {
                    Quantity = achievement.ExpirationLifetimeQuantity ?? 0,
                    Unit = achievement.ExpirationLifetimeUnit
                }
            });
        }

        public void SetExpiration(VCredential achievement)
        {
            SetExpiration(new Expiration
            {
                Type = achievement.CredentialExpirationType.ToEnum(ExpirationType.None),
                Date = achievement.CredentialExpirationFixedDate,
                Lifetime = new Lifetime
                {
                    Quantity = achievement.CredentialExpirationLifetimeQuantity ?? 0,
                    Unit = achievement.CredentialExpirationLifetimeUnit
                }
            });
        }

        public void SetExpiration(Expiration expiration)
        {
            SetExpiration(expiration.Type);

            if (expiration.Type == ExpirationType.Fixed)
            {
                ExpirationDate.Value = expiration.Date;
            }
            else if (expiration.Type == ExpirationType.Relative)
            {
                LifetimeQuantity.ValueAsInt = expiration.Lifetime.Quantity;
                LifetimeUnit.Value = expiration.Lifetime.Unit;
            }

            SetControlsVisibility();
        }

        public void SetExpiration(ExpirationType type = ExpirationType.None)
        {
            var isFixed = type == ExpirationType.Fixed;
            var isRelative = type == ExpirationType.Relative;

            ExpirationTypeNoExpiry.Checked = !isFixed && !isRelative;
            ExpirationTypeFixed.Checked = isFixed;
            ExpirationTypeRelative.Checked = isRelative;

            ExpirationDate.Value = null;
            LifetimeQuantity.ValueAsInt = 3;
            LifetimeUnit.Value = "Year";

            SetControlsVisibility();
        }

        public Expiration GetExpiration()
        {
            var result = new Expiration
            {
                Type = GetSelectedExpirationType()
            };

            if (result.Type == ExpirationType.Fixed)
                result.Date = ExpirationDate.Value;
            else if (result.Type == ExpirationType.Relative)
                result.Lifetime = new Lifetime
                {
                    Quantity = LifetimeQuantity.ValueAsInt.Value,
                    Unit = LifetimeUnit.Value
                };

            return result;
        }

        private void ExpirationType_CheckedChanged(object sender, EventArgs e)
        {
            SetControlsVisibility();
        }

        private void SetControlsVisibility()
        {
            var expirationType = GetSelectedExpirationType();

            var isFixed = expirationType == ExpirationType.Fixed;
            var isRelative = expirationType == ExpirationType.Relative;

            ExpirationDateArea.Visible = isFixed;
            ExpirationDateRequiredValidator.Visible = isFixed;

            LifetimeArea.Visible = isRelative;
            LifetimeQuantityRequiredValidator.Visible = isRelative;
        }

        protected override void SetupValidationGroup(string groupName)
        {
            ExpirationDateRequiredValidator.ValidationGroup = groupName;
            LifetimeQuantityRequiredValidator.ValidationGroup = groupName;
        }

        private ExpirationType GetSelectedExpirationType()
        {
            if (ExpirationTypeFixed.Checked) return ExpirationType.Fixed;
            else if (ExpirationTypeRelative.Checked) return ExpirationType.Relative;

            return ExpirationType.None;
        }
    }
}