using System;
using System.Linq;

using InSite.Application.Records.Read;
using InSite.Common.Web.UI;

namespace InSite.Admin.Achievements.Achievements.Controls
{
    public partial class SearchCriteria : SearchCriteriaController<QAchievementFilter>
    {
        public override QAchievementFilter Filter
        {
            get
            {
                var filter = new QAchievementFilter(Organization.OrganizationIdentifier, AchievementLabel.Text)
                {
                    AchievementTitle = AchievementTitle.Text,
                    AchievementDescription = AchievementDescription.Text,
                    AchievementIsEnabled = AchievementLockStatus.ValueAsBoolean,
                    ExpirationType = ExpirationType.Value,
                };

                if (filter.ExpirationType == "Fixed")
                {
                    filter.ExpirationFixedDateSince = ExpirationFixedDateSince.Value;
                    filter.ExpirationFixedDateBefore = ExpirationFixedDateBefore.Value;
                }
                else if (filter.ExpirationType == "Relative")
                {
                    filter.ExpirationLifetimeQuantity = ExpirationLifetimeQuantity.ValueAsInt;
                    filter.ExpirationLifetimeUnit = ExpirationLifetimeUnit.Value;
                }

                GetCheckedShowColumns(filter);

                return filter;
            }
            set
            {
                AchievementTitle.Text = value.AchievementTitle;
                AchievementLabel.Text = value.AchievementLabels.FirstOrDefault();
                AchievementDescription.Text = value.AchievementDescription;
                AchievementLockStatus.ValueAsBoolean = value.AchievementIsEnabled;
                ExpirationType.Value = value.ExpirationType;
                ExpirationLifetimeQuantity.ValueAsInt = value.ExpirationLifetimeQuantity;
                ExpirationLifetimeUnit.Value = value.ExpirationLifetimeUnit;

                ExpirationFixedDateSince.Value = value.ExpirationFixedDateSince;
                ExpirationFixedDateBefore.Value = value.ExpirationFixedDateBefore;

                SetVisibility();
            }
        }

        public override void Clear()
        {
            AchievementTitle.Text = null;
            AchievementLabel.Text = null;
            AchievementDescription.Text = null;

            AchievementLockStatus.ClearSelection();
            ExpirationType.ClearSelection();

            ExpirationLifetimeQuantity.ValueAsInt = null;
            ExpirationLifetimeUnit.ClearSelection();

            ExpirationFixedDateSince.Value = null;
            ExpirationFixedDateBefore.Value = null;

            SetVisibility();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ExpirationType.AutoPostBack = true;
            ExpirationType.ValueChanged += ExpirationType_ValueChanged;
        }

        private void ExpirationType_ValueChanged(object sender, EventArgs e)
        {
            SetVisibility();
        }

        private void SetVisibility()
        {
            var isFixed = ExpirationType.Value == "Fixed";
            ExpirationFixedDateSinceField.Visible = isFixed;
            ExpirationFixedDateBeforeField.Visible = isFixed;

            var isRelative = ExpirationType.Value == "Relative";
            ExpirationLifetimeQuantityField.Visible = isRelative;
            ExpirationLifetimeUnitField.Visible = isRelative;
        }
    }
}