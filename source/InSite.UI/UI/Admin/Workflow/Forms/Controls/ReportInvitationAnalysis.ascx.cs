using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.UI;

using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;

namespace InSite.Admin.Workflow.Forms.Controls
{
    public partial class ReportInvitationAnalysis : BaseUserControl
    {
        #region Properties

        private Guid SurveyID
        {
            get => (Guid)ViewState[nameof(SurveyID)];
            set => ViewState[nameof(SurveyID)] = value;
        }

        public bool HasData
        {
            get => (bool?)ViewState[nameof(HasData)] == true;
            set => ViewState[nameof(HasData)] = value;
        }

        #endregion

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Download.Click += DownloadClicked;
        }

        public void Bind(Guid surveyId)
        {
            SurveyID = surveyId;

            var statistic = MessageAdapter.GetInvitationStatistic(SurveyID);

            var deliveryScheduled = statistic.DeliveryScheduled.Count;
            var deliveryRecipientScheduled = statistic.DeliveryScheduled.Sum(x => x.Recipients.Count);
            var deliveryDelivered = statistic.DeliveryDelivered.Count;
            var deliveryRecipientDelivered = statistic.DeliveryDelivered.Sum(x => x.Recipients.Count);

            var delivered = statistic.InvitationDelivered.Count;
            var deliveredMoreThanOnce = statistic.InvitationDeliveredMoreThanOnce.Count;
            var responded = statistic.InvitatioResponded.Count;

            HasData = deliveryRecipientScheduled > 0
                      || deliveryRecipientDelivered > 0
                      || deliveredMoreThanOnce > 0
                      || responded > 0;
            Visible = HasData;

            Scheduled.Visible = deliveryRecipientScheduled > 0;
            Scheduled.InnerHtml = string.Format(@"
<strong><a href='#' onclick='invitationAnalytics.downloadCsv(""SurveyInvitationDeliveryScheduled""); return false;'>{0:n0}</a></strong>
form invitation deliver{2} scheduled for
<strong><a href='#' onclick='invitationAnalytics.downloadCsv(""SurveyInvitationRespondentScheduled""); return false;'>{1:n0}</a></strong>
potential respondent{3}",
                deliveryScheduled,
                deliveryRecipientScheduled,
                deliveryScheduled > 1 ? "ies" : "y",
                deliveryRecipientScheduled > 1 ? "s" : string.Empty
            );

            Delivered.Visible = deliveryRecipientDelivered > 0;
            Delivered.InnerHtml = string.Format(@"
<strong><a href='#' onclick='invitationAnalytics.downloadCsv(""SurveyInvitationDeliveryDelivered""); return false;'>{0:n0}</a></strong>
form invitation{2} delivered to
<strong><a href='#' onclick='invitationAnalytics.downloadCsv(""SurveyInvitationRespondentDelivered""); return false;'>{1:n0}</a></strong>
potential respondent{3}",
                deliveryDelivered,
                deliveryRecipientDelivered,
                deliveryDelivered > 1 ? "s" : string.Empty,
                deliveryRecipientDelivered > 1 ? "s" : string.Empty
            );

            DeliveredMoreThanOnce.Visible = deliveredMoreThanOnce > 0;
            DeliveredMoreThanOnce.InnerHtml = $@"
<strong><a href='#' onclick='invitationAnalytics.downloadCsv(""SurveyInvitationDeliveredMoreThanOnce""); return false;'>{deliveredMoreThanOnce:n0}</a></strong>
potential respondent{(deliveredMoreThanOnce > 1 ? "s" : string.Empty)} received an invitation more than once";

            Responded.Visible = responded > 0;
            Responded.InnerHtml = string.Format(@"
<strong><a href='#' onclick='invitationAnalytics.downloadCsv(""SurveyInvitationResponded""); return false;'>{0:n0}</a></strong>
recipient{3} responded to the form and
<strong><a href='#' onclick='invitationAnalytics.downloadCsv(""SurveyInvitationNotResponded""); return false;'>{1:n0}</a></strong>
recipient{4} did not respond, for a submission rate of <strong>{2:n0}%</strong>",
                responded,
                delivered - responded,
                delivered > 0 ? (decimal)responded / delivered * 100 : 0,
                responded > 1 ? "s" : string.Empty,
                delivered - responded > 1 ? "s" : string.Empty
            );
        }

        private void DownloadClicked(object sender, EventArgs e)
        {
            var csvType = Page.Request.Form["__CSVTYPE"];
            if (string.IsNullOrEmpty(csvType))
                return;

            DownloadCsv(csvType);
        }

        #region Methods (downloading)

        private void DownloadCsv(string csvType)
        {
            if (csvType == "SurveyInvitationDeliveryScheduled")
            {
                DownloadCsv(
                    $"scheduled_{SurveyID}_{DateTime.UtcNow:yyyy-MM-dd}",
                    MessageAdapter.GetInvitationStatistic(SurveyID).DeliveryScheduled);
            }
            else if (csvType == "SurveyInvitationRespondentScheduled")
            {
                DownloadCsv(
                    $"scheduled_{SurveyID}_{DateTime.UtcNow:yyyy-MM-dd}",
                    MessageAdapter.GetInvitationStatistic(SurveyID).DeliveryScheduled,
                    "Scheduled",
                    "Scheduled On");
            }
            else if (csvType == "SurveyInvitationDeliveryDelivered")
            {
                DownloadCsv(
                    $"delivered_{SurveyID}_{DateTime.UtcNow:yyyy-MM-dd}",
                    MessageAdapter.GetInvitationStatistic(SurveyID).DeliveryDelivered);
            }
            else if (csvType == "SurveyInvitationRespondentDelivered")
            {
                DownloadCsv(
                    $"delivered-to_{SurveyID}_{DateTime.UtcNow:yyyy-MM-dd}",
                    MessageAdapter.GetInvitationStatistic(SurveyID).DeliveryDelivered,
                    "Completed",
                    "Delivered On");
            }
            else if (csvType == "SurveyInvitationDeliveredMoreThanOnce")
            {
                DownloadCsv(
                    $"delivered-more-than-once_{SurveyID}_{DateTime.UtcNow:yyyy-MM-dd}",
                    MessageAdapter.GetInvitationStatistic(SurveyID).InvitationDeliveredMoreThanOnce,
                    "Delivered On");
            }
            else if (csvType == "SurveyInvitationResponded")
            {
                DownloadCsv(
                    $"responded_{SurveyID}_{DateTime.UtcNow:yyyy-MM-dd}",
                    MessageAdapter.GetInvitationStatistic(SurveyID).InvitatioResponded,
                    "Responded On");
            }
            else if (csvType == "SurveyInvitationNotResponded")
            {
                var statistic = MessageAdapter.GetInvitationStatistic(SurveyID);

                DownloadCsv(
                    $"not-responded_{SurveyID}_{DateTime.UtcNow:yyyy-MM-dd}",
                    statistic.InvitationDelivered.Where(x => statistic.InvitatioResponded.All(y => y.Email != x.Email)),
                    "Delivered On");
            }
        }

        private void DownloadCsv(string filename, IEnumerable<MessageAdapter.DeliveryInfo> data)
        {
            var t = new DataTable();

            t.Columns.Add("Account");
            t.Columns.Add("Scheduled", typeof(DateTimeOffset));
            t.Columns.Add("Completed", typeof(DateTimeOffset));
            t.Columns.Add("RecipientCount", typeof(int));

            foreach (var delivery in data)
            {
                var row = t.NewRow();

                row["Account"] = delivery.OrganizationIdentifier.ToString();
                row["Completed"] = (object)delivery.Completed ?? DBNull.Value;
                row["Scheduled"] = delivery.Scheduled;
                row["RecipientCount"] = delivery.Recipients.Count;

                t.Rows.Add(row);
            }

            var tableView = t.DefaultView;
            tableView.Sort = "Completed DESC, Scheduled DESC";
            t = tableView.ToTable();

            var helper = new CsvExportHelper(t);

            helper.AddMapping("Account", "Account");
            helper.AddMapping("Scheduled", "Scheduled ", "{0:MMM d, yyyy} {0:h:mm tt}");
            helper.AddMapping("Completed", "Completed ", "{0:MMM d, yyyy} {0:h:mm tt}");
            helper.AddMapping("RecipientCount", "Recipients");

            var bytes = helper.GetBytes(Encoding.Unicode);

            Page.Response.SendFile(filename, "csv", bytes);
        }

        private void DownloadCsv(string filename, IEnumerable<MessageAdapter.DeliveryInfo> data,
            string statusColumn, string statusName)
        {
            var t = new DataTable();

            t.Columns.Add("PersonName");
            t.Columns.Add("PersonEmail");
            t.Columns.Add("Scheduled", typeof(DateTimeOffset));
            t.Columns.Add("Completed", typeof(DateTimeOffset));

            foreach (var delivery in data)
                foreach (var recipientEmail in delivery.Recipients)
                {
                    var row = t.NewRow();

                    row["PersonName"] = UserSearch.BindFirst(x => x.FullName, new UserFilter { EmailExact = recipientEmail });
                    row["PersonEmail"] = recipientEmail;
                    row["Scheduled"] = delivery.Scheduled;
                    row["Completed"] = (object)delivery.Completed ?? DBNull.Value;

                    t.Rows.Add(row);
                }

            var tableView = t.DefaultView;
            tableView.Sort = $"PersonName,PersonEmail,{statusColumn} DESC";
            t = tableView.ToTable();

            var helper = new CsvExportHelper(t);

            helper.AddMapping("PersonName", "Name");
            helper.AddMapping("PersonEmail", "Email");
            helper.AddMapping(statusColumn, statusName, "{0:MMM d, yyyy} {0:h:mm tt}");

            var bytes = helper.GetBytes(Encoding.Unicode);

            Page.Response.SendFile(filename, "csv", bytes);
        }

        private void DownloadCsv(string filename,
            IEnumerable<MessageAdapter.SingleInvitationInfo> data, string dateColumnName)
        {
            var t = new DataTable();

            t.Columns.Add("PersonName");
            t.Columns.Add("PersonEmail");
            t.Columns.Add("StatusDate", typeof(DateTimeOffset));

            foreach (var item in data)
            {
                var row = t.NewRow();

                row["PersonName"] = UserSearch.BindFirst(x => x.FullName, new UserFilter { EmailExact = item.Email });
                row["PersonEmail"] = item.Email;
                row["StatusDate"] = item.StatusDate;

                t.Rows.Add(row);
            }

            var tableView = t.DefaultView;
            tableView.Sort = "PersonName,PersonEmail,StatusDate DESC";
            t = tableView.ToTable();

            var helper = new CsvExportHelper(t);

            helper.AddMapping("PersonName", "Name");
            helper.AddMapping("PersonEmail", "Email");
            helper.AddMapping("StatusDate", dateColumnName, "{0:MMM d, yyyy} {0:h:mm tt}");

            var bytes = helper.GetBytes(Encoding.Unicode);

            Page.Response.SendFile(filename, "csv", bytes);
        }

        private void DownloadCsv(string filename, IEnumerable<MessageAdapter.InvitationInfo> data,
            string dateColumnName)
        {
            var t = new DataTable();

            t.Columns.Add("PersonName");
            t.Columns.Add("PersonEmail");
            t.Columns.Add("StatusDate", typeof(DateTimeOffset));
            t.Columns.Add("DeliveryCount", typeof(int));

            foreach (var item in data)
            {
                var row = t.NewRow();

                row["PersonName"] = UserSearch.BindFirst(x => x.FullName, new UserFilter { EmailExact = item.Email });
                row["PersonEmail"] = item.Email;
                row["StatusDate"] = item.StatusDate;
                row["DeliveryCount"] = item.DeliveryCount;

                t.Rows.Add(row);
            }

            var tableView = t.DefaultView;
            tableView.Sort = "PersonName,StatusDate DESC";
            t = tableView.ToTable();

            var helper = new CsvExportHelper(t);

            helper.AddMapping("PersonName", "Name");
            helper.AddMapping("PersonEmail", "Email");
            helper.AddMapping("StatusDate", dateColumnName, "{0:MMM d, yyyy} {0:h:mm tt}");
            helper.AddMapping("DeliveryCount", "Count");

            var bytes = helper.GetBytes(Encoding.Unicode);

            Page.Response.SendFile(filename, "csv", bytes);
        }

        #endregion
    }
}