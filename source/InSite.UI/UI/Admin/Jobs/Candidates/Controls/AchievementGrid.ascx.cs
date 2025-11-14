using System;
using System.Globalization;
using System.Web.UI;

using Humanizer;
using Humanizer.Localisation;

using InSite.Application.Records.Read;
using InSite.Common.Web.UI;

using Shift.Common;

namespace InSite.UI.Admin.Jobs.Candidates.Controls
{
    public partial class AchievementGrid : BaseUserControl
    {
        private VCredentialFilter Filter
        {
            get => (VCredentialFilter)ViewState[nameof(Filter)];
            set => ViewState[nameof(Filter)] = value;
        }

        public int RowsCount => Grid.VirtualItemCount;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Grid.DataBinding += Grid_DataBinding;
        }

        private void Grid_DataBinding(object source, EventArgs e)
        {
            if (Filter == null)
                return;

            Filter.Paging = Paging.SetPage(Grid.PageIndex + 1, Grid.PageSize);

            Grid.DataSource = ServiceLocator.AchievementSearch.GetCredentials(Filter);
        }

        public void LoadData(Guid organizationId, Guid userId)
        {
            Filter = new VCredentialFilter
            {
                OrganizationIdentifier = organizationId,
                UserIdentifier = userId
            };

            var count = ServiceLocator.AchievementSearch.CountCredentials(Filter);
            var hasData = count > 0;

            NoAchievements.Visible = !hasData;
            Grid.Visible = hasData;

            Grid.PageIndex = 0;
            Grid.VirtualItemCount = count;
            Grid.DataBind();
        }

        protected string GetExpiration()
        {
            var dataItem = (VCredential)Page.GetDataItem();

            var credentialExpiration = new InSite.Domain.Records.Expiration(
                dataItem.CredentialExpirationType, dataItem.CredentialExpirationFixedDate,
                dataItem.CredentialExpirationLifetimeQuantity, dataItem.CredentialExpirationLifetimeUnit);

            var achievementExpiration = new InSite.Domain.Records.Expiration(
                dataItem.AchievementExpirationType, dataItem.AchievementExpirationFixedDate,
                dataItem.AchievementExpirationLifetimeQuantity, dataItem.AchievementExpirationLifetimeUnit);

            var credential = credentialExpiration.ToString(User.TimeZone);
            var achievement = achievementExpiration.ToString(User.TimeZone);

            var warning = string.Empty;
            if (!credential.Equals(achievement))
            {
                var icon = "<i class='fas fa-exclamation-triangle text-warning' style='padding-right:5px;'></i>";
                warning = $"<span class='pull-left' title='Default expiration is {achievement}'>{icon}</span>";
            }

            return warning + credential;
        }

        protected string GetLocalTime(string name)
        {
            var dataItem = (VCredential)Page.GetDataItem();
            var when = (DateTimeOffset?)DataBinder.Eval(dataItem, name);

            return when.Format(User.TimeZone, true);
        }

        protected string GetCredentialExpiry()
        {
            var dataItem = (VCredential)Page.GetDataItem();
            var expected = dataItem.CredentialExpirationExpected;
            var actual = dataItem.CredentialExpired;
            var now = DateTimeOffset.UtcNow;

            if (expected.HasValue && expected.Value > now)
            {
                var age = TimeZones.Format(expected.Value, User.TimeZone, true);
                var span = expected.Value - now;
                var days = span.TotalDays;

                if (days > 90)
                    return $"<div>{age}</div><div class='badge bg-success'>{span.Humanize(1, true, CultureInfo.CurrentCulture, TimeUnit.Month)} from now</div>";
                else
                    return $"<div>{age}</div><div class='badge bg-warning'>{expected.Value.Humanize()}</div>";
            }
            else if (actual.HasValue)
            {
                var age = TimeZones.Format(actual.Value, User.TimeZone, true);

                return $"<div>{age}</div><div class='badge bg-danger'>{actual.Value.Humanize()}</div>";
            }

            return null;
        }
    }
}