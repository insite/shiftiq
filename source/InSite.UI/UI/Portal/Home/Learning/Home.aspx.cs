﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Application.Courses.Read;
using InSite.Application.Sites.Read;
using InSite.Common.Web;
using InSite.Persistence;
using InSite.UI.Admin.Assessments.Attempts.Controls.SkillsCheckReport;
using InSite.UI.Layout.Admin;
using InSite.UI.Layout.Portal;

namespace InSite.UI.Portal.Home.Learning
{
    public partial class Home : PortalBasePage
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            AssessmentRepeater.ItemCommand += AssessmentRepeater_ItemCommand;
        }

        private void AssessmentRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName != "Download")
                return;

            var parts = e.CommandArgument.ToString().Split(';');
            var attemptId = Guid.Parse(parts[0]);
            var managerUserId = Guid.Parse(parts[1]);

            var pdf = SkillsCheckReportControl.GetPdf(this, attemptId, managerUserId, Organization.Identifier, User.TimeZone);

            Response.SendFile("skillscheck_report.pdf", pdf);
        }

        protected override void OnLoad(EventArgs e)
        {
            if (!IsPostBack)
                CheckRequiredUserFields();

            base.OnLoad(e);

            if (IsPostBack)
                return;

            PortalMaster.ShowAvatar(dashboardUrl: "/ui/portal/learning/dashboard/home");
            PortalMaster.RenderHelpContent(null);
            PortalMaster.HideBreadcrumbs();

            PageHelper.AutoBindHeader(this);

            var site = GetCurrentSite();
            Page.Title = site?.SiteTitle ?? "Shift iQ";

            UserFirstName.Text = User.FirstName;

            BindDistributions();
        }

        private void BindDistributions()
        {
            var distributions = ServiceLocator.CourseDistributionSearch
                .GetCourseDistributionsByLearner(Organization.Identifier, User.Identifier)
                .OrderBy(x => x.AttemptStarted.HasValue)
                .ThenByDescending(x => x.AttemptStarted)
                .ThenByDescending(x => x.Modified)
                .ToList();

            var hasRows = distributions.Count > 0;

            AssessmentRepeater.Visible = hasRows;
            AssessmentRepeater.DataSource = distributions;
            AssessmentRepeater.DataBind();

            AttemptCard.Visible = hasRows;
            AttemptCardStartButton.Visible = false;
            AttemptCardContinueButton.Visible = false;
            AttemptCardAddToCartButton.Visible = false;
            AttemptCardViewCatalogButton.Visible = false;

            if (hasRows)
                SetCardStatus(distributions);
        }

        private void SetCardStatus(List<CourseDistributionGridItem> distributions)
        {
            var first = distributions[0];
            var status = first.GetStatus();

            if (status == CourseDistributionGridItem.StatusType.NotStarted)
            {
                AttemptCardHeader.InnerHtml =
                    "<i class='fas fa-fw fa-circle text-info'></i>" +
                    "<span class='fst-italic ps-1'>SkillsCheck – Not Started</span>";
                AttemptCardBody.InnerText =
                    "[AdminFirstName] at [Company Name] assigned this SkillsCheck to you.";
                AttemptCardStartButton.Visible = true;
                AttemptCardStartButton.NavigateUrl = GetGridAttemptStartUrl(first.FormIdentifier);
            }
            else if (status == CourseDistributionGridItem.StatusType.InProgress)
            {
                AttemptCardHeader.InnerHtml =
                    "<i class='fas fa-fw fa-circle text-warning'></i>" +
                    "<span class='fst-italic ps-1'>SkillsCheck – In Progress</span>";
                AttemptCardBody.InnerText =
                    "Looks like you haven't completed your SkillsCheck yet. You're almost there!";
                AttemptCardContinueButton.Visible = true;
                AttemptCardContinueButton.NavigateUrl = GetGridAttemptStartUrl(first.FormIdentifier);
            }
            else
            {
                AttemptCardHeader.InnerHtml =
                    "<span class='fst-italic fw-bold text-success'>Level up your skills!</span>";
                AttemptCardBody.InnerHtml =
                    "We found another SkillsCheck you might be interested in. " +
                    "Add to Cart or View Catalog to see more.";
                AttemptCardAddToCartButton.Visible = true;
                AttemptCardViewCatalogButton.Visible = true;
                AttemptCardViewCatalogButton.NavigateUrl = "/ui/portal/learning/dashboard/catalog";
            }
        }

        protected string GetGridScoreHtml()
        {
            var item = (CourseDistributionGridItem)Page.GetDataItem();
            return !item.AttemptGraded.HasValue ? string.Empty : $"{item.AttemptScore:p0}";
        }

        protected string GetGridAttemptStartUrl()
        {
            var item = (CourseDistributionGridItem)Page.GetDataItem();
            return GetGridAttemptStartUrl(item.CourseIdentifier);
        }

        protected string GetGridAttemptStartUrl(Guid? courseIdentifier)
            => $"/ui/portal/learning/course/{courseIdentifier}";

        protected string GetGridAttemptReportUrl()
        {
            var item = (CourseDistributionGridItem)Page.GetDataItem();
            return $"/ui/admin/assessments/attempts/skillscheck-report?attempt={item.AttemptIdentifier}&manager={item.ManagerUserIdentifier}";
        }

        private void CheckRequiredUserFields()
        {
            var user = User != null ? UserSearch.Select(User.UserIdentifier) : null;
            if (user == null)
                return;

            var type = user.GetType();

            foreach (var field in Organization.Fields.User)
            {
                if (!field.IsRequired)
                    continue;

                var prop = type.GetProperty(field.FieldName);
                if (prop != null && prop.GetValue(user) == null)
                    Response.Redirect("/ui/portal/profile", true);
            }
        }

        public static QSite GetCurrentSite()
        {
            var organization = CurrentSessionState.Identity.Organization;

            var portalName = $"{organization.Code}.{ServiceLocator.AppSettings.Security.Domain}";

            return ServiceLocator.SiteSearch.BindFirst(x => x, x => x.SiteDomain == portalName);
        }
    }
}