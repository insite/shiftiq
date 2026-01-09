using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

using Shift.Common.Timeline.Commands;

using InSite.Application.Attempts.Read;
using InSite.Application.Attempts.Write;
using InSite.Application.Rubrics.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI.Admin.Records.Rurbics.Controls;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Admin.Records.Rurbics
{
    public partial class Edit : AdminBasePage, IHasParentLinkParameters
    {
        public const string NavigateUrl = "/ui/admin/records/rubrics/edit";

        public static string GetNavigateUrl(Guid rubriId, string tab = null)
        {
            var url = NavigateUrl + "?rubric=" + rubriId;

            if (tab.IsNotEmpty())
                url += "&tab=" + HttpUtility.UrlEncode(tab);

            return url;
        }

        public static void Redirect(Guid rubriId, string tab = null) =>
            HttpResponseHelper.Redirect(GetNavigateUrl(rubriId, tab));

        private Guid RubricIdentifier => Guid.TryParse(Request.QueryString["rubric"], out var id) ? id : Guid.Empty;

        private string Tab => Request.QueryString["tab"];

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            PageHelper.AutoBindHeader(this);

            CancelButton.NavigateUrl = GetOutlineUrl();

            if (string.Equals(Tab, "details", StringComparison.OrdinalIgnoreCase))
                DetailsTab.IsSelected = true;
            else if (string.Equals(Tab, "criteria", StringComparison.OrdinalIgnoreCase))
                CriteriaTab.IsSelected = true;

            Open();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var criteria = CriteriaList.GetCriteria();
            if (criteria.Count == 0)
            {
                AlertStatus.AddMessage(AlertType.Error, "At least one criterion entry must be added.");
                return;
            }

            var rubric = ServiceLocator.RubricSearch.GetRubric(RubricIdentifier);
            rubric.RubricPoints = criteria.Sum(x => x.Points);

            if (rubric.RubricPoints <= 0)
            {
                AlertStatus.AddMessage(AlertType.Error, "The rubric has no points.");
                return;
            }

            Detail.GetInputValues(rubric);

            var commands = new List<ICommand>
            {
                new RenameRubric(rubric.RubricIdentifier, rubric.RubricTitle),
                new DescribeRubric(rubric.RubricIdentifier, rubric.RubricDescription),
            };

            RubricCriteriaList.Save(rubric.RubricIdentifier, criteria, commands);

            ServiceLocator.SendCommand(new RunCommands(rubric.RubricIdentifier, commands.ToArray()));

            var attemptIds = ServiceLocator.AttemptSearch.BindAttempts(x => x.AttemptIdentifier, new QAttemptFilter
            {
                RubricIdentifier = rubric.RubricIdentifier
            });

            foreach (var attemptId in attemptIds)
                ServiceLocator.SendCommand(new UpdateAttemptRubricPoints(attemptId, rubric.RubricIdentifier, rubric.RubricPoints));

            var url = GetOutlineUrl();

            HttpResponseHelper.Redirect(url);
        }

        private void Open()
        {
            var rubric = ServiceLocator.RubricSearch.GetRubric(RubricIdentifier);
            if (rubric == null || rubric.OrganizationIdentifier != Organization.Identifier)
                Search.Redirect();

            var isLocked = ServiceLocator.RubricSearch.RubricHasAttempts(rubric.RubricIdentifier);
            if (isLocked)
                Outline.Redirect(rubric.RubricIdentifier);

            Detail.SetInputValues(rubric);

            CriteriaList.LoadData(rubric);
        }

        private string GetOutlineUrl()
            => Outline.GetNavigateUrl(RubricIdentifier, Tab);

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? Outline.GetNavigateParams(RubricIdentifier, Tab)
                : null;
        }
    }
}