using System;
using System.Globalization;
using System.Web.UI;

using InSite.Application.Records.Read;
using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Portal.Jobs.Candidates.MyPortfolio.Controls
{
    public partial class AchievementSection : BaseUserControl
    {
        private VCredentialFilter Filter
        {
            get => (VCredentialFilter)ViewState[nameof(Filter)];
            set => ViewState[nameof(Filter)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            AchievementsGrid.DataBinding += AchievementsGrid_DataBinding;
        }

        private void AchievementsGrid_DataBinding(object source, EventArgs e)
        {
            if (Filter == null)
                return;

            Filter.Paging = Paging.SetPage(AchievementsGrid.PageIndex + 1, AchievementsGrid.PageSize);
            Filter.OrderBy = $"{nameof(VCredential.AchievementTitle)}, {nameof(VCredential.CredentialGranted)} desc";

            AchievementsGrid.DataSource = ServiceLocator.AchievementSearch.GetCredentials(Filter);
        }

        public void BindModelToControls(Person person)
        {
            Filter = new VCredentialFilter
            {
                OrganizationIdentifier = person.OrganizationIdentifier,
                UserIdentifier = person.UserIdentifier
            };

            var count = ServiceLocator.AchievementSearch.CountCredentials(Filter);
            var hasData = count > 0;

            NoAchievements.Visible = !hasData;
            AchievementsGrid.Visible = hasData;

            AchievementsGrid.PageIndex = 0;
            AchievementsGrid.VirtualItemCount = count;
            AchievementsGrid.DataBind();
        }

        protected string GetDateString(string name)
        {
            var dataItem = Page.GetDataItem();
            var date = (DateTimeOffset?)DataBinder.Eval(dataItem, name);

            return date.FormatDateOnly(User.TimeZone, CultureInfo.GetCultureInfo(Identity.Language));
        }

        protected string GetStatusHtml()
        {
            var credential = (VCredential)Page.GetDataItem();
            var status = credential.CredentialStatus.ToEnum(CredentialStatus.Undefined);

            switch (status)
            {
                case CredentialStatus.Valid:
                    return $"<span class='text-success'><i class='fas fa-flag'></i></span> {Translate("Valid")}";
                case CredentialStatus.Pending:
                    return $"<span class='text-warning'><i class='fas fa-flag'></i></span> {Translate("Pending")}";
                case CredentialStatus.Expired:
                    return $"<span class='text-danger'><i class='fas fa-flag'></i></span> {Translate("Expired")}";
                default:
                    return string.Empty;
            }
        }

        protected string GetDownloadCertificateButton()
        {
            var credential = (VCredential)Page.GetDataItem();
            var status = credential.CredentialStatus.ToEnum(CredentialStatus.Undefined);

            return status != CredentialStatus.Valid || !ServiceLocator.Partition.IsE03() && credential.AchievementCertificateLayoutCode.IsEmpty()
                ? null
                : $"<a target='_blank' class='btn btn-primary btn-sm' href='/ui/portal/records/credentials/certificate?credential={credential.CredentialIdentifier}'><i class='far fa-award me-2'></i> Download</a>";
        }

    }
}