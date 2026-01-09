using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

using InSite.Application.Banks.Read;
using InSite.Application.Records.Read;
using InSite.Common.Web;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Admin.Records.Rurbics
{
    public partial class Outline : AdminBasePage
    {
        class RatingItem
        {
            public string Title { get; set; }
            public string Description { get; set; }
            public string Points { get; set; }
        }

        class CriterionItem
        {
            public string Title { get; set; }
            public string Description { get; set; }
            public string Range { get; set; }
            public string Points { get; set; }

            public List<RatingItem> Ratings { get; set; }
        }

        public const string NavigateUrl = "/ui/admin/records/rubrics/outline";

        public static string GetNavigateUrl(Guid rubricId, string tab = null)
        {
            return NavigateUrl + "?" + GetNavigateParams(rubricId, tab);
        }

        public static string GetNavigateParams(Guid rubricId, string tab = null)
        {
            var url = "rubric=" + rubricId;

            if (tab.IsNotEmpty())
                url += "&tab=" + HttpUtility.UrlEncode(tab);

            return url;
        }

        public static void Redirect(Guid rubricId, string tab = null) =>
            HttpResponseHelper.Redirect(GetNavigateUrl(rubricId, tab));

        private Guid RubricIdentifier => Guid.TryParse(Request.QueryString["rubric"], out var id) ? id : Guid.Empty;

        private string Tab => Request.QueryString["tab"];

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            CriteriaRepeater.ItemDataBound += CriteriaRepeater_ItemDataBound;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            var rubric = ServiceLocator.RubricSearch.GetRubric(RubricIdentifier);
            if (rubric == null || rubric.OrganizationIdentifier != Organization.Identifier)
                Search.Redirect();

            PageHelper.AutoBindHeader(this, null, rubric.RubricTitle);

            if (string.Equals(Tab, "details", StringComparison.OrdinalIgnoreCase))
                DetailsTab.IsSelected = true;
            else if (string.Equals(Tab, "criteria", StringComparison.OrdinalIgnoreCase))
                CriteriaTab.IsSelected = true;

            LoadData(rubric);

            CopyLink.NavigateUrl = Create.GetNavigateUrl(rubricId: RubricIdentifier, action: "duplicate");
            TranslateLink1.NavigateUrl = Rurbics.Translate.GetNavigateUrl(RubricIdentifier, "details");
            TranslateLink2.NavigateUrl = Rurbics.Translate.GetNavigateUrl(RubricIdentifier, "criteria");
            ChangeTitleLink.NavigateUrl = Edit.GetNavigateUrl(RubricIdentifier, "details");
            ChangeDescriptionLink.NavigateUrl = Edit.GetNavigateUrl(RubricIdentifier, "details");
            ChangePointsLink.NavigateUrl = Edit.GetNavigateUrl(RubricIdentifier, "details");
            EditButton.NavigateUrl = Edit.GetNavigateUrl(RubricIdentifier, "criteria");
            AddButton.NavigateUrl = Edit.GetNavigateUrl(RubricIdentifier, "criteria");
        }

        private void CriteriaRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var criterion = (CriterionItem)e.Item.DataItem;
            var ratingRepeater = (Repeater)e.Item.FindControl("RatingRepeater");

            ratingRepeater.DataSource = criterion.Ratings;
            ratingRepeater.DataBind();
        }

        private void LoadData(QRubric rubric)
        {
            RubricTitle.Text = rubric.RubricTitle;
            RubricDescription.Text = rubric.RubricDescription;
            RubricPoints.Text = $"{rubric.RubricPoints:n2}";
            CriteriaRubricPoints.Text = $"{rubric.RubricPoints:n2}";

            var criteria = GetCriteria();

            if (criteria.Count == 0)
            {
                NoCriteria.AddMessage(AlertType.Warning, "No Criteria");
            }
            else
            {
                CriteriaRepeater.DataSource = criteria;
                CriteriaRepeater.DataBind();
            }

            var isLocked = ServiceLocator.RubricSearch.RubricHasAttempts(rubric.RubricIdentifier);
            if (isLocked)
                OutlineAlert.AddMessage(AlertType.Warning, "The rubric cannot be modified because is used in one or more attempts");

            TranslateLink1.Visible = !isLocked;
            TranslateLink2.Visible = !isLocked;
            ChangeTitleLink.Visible = !isLocked;
            ChangeDescriptionLink.Visible = !isLocked;
            ChangePointsLink.Visible = !isLocked;
            EditButton.Visible = !isLocked && criteria.Count > 0;
            AddButton.Visible = !isLocked && criteria.Count == 0;

            var questions = ServiceLocator.BankSearch.GetQuestions(new QBankQuestionFilter
            {
                OrganizationIdentifier = Organization.Identifier,
                RubricIdentifier = rubric.RubricIdentifier
            });
            QuestionsTab.Visible = questions.Count > 0;
            QuestionRepeater.DataSource = questions;
            QuestionRepeater.DataBind();
        }

        private List<CriterionItem> GetCriteria()
        {
            var criteria = ServiceLocator.RubricSearch.GetCriteria(RubricIdentifier, x => x.RubricRatings);

            return criteria.OrderBy(x => x.CriterionSequence).Select(x => new CriterionItem
            {
                Title = x.CriterionTitle,
                Description = x.CriterionDescription,
                Range = x.IsRange ? "<label class='badge bg-custom-default'>Range</label>" : null,
                Points = x.IsRange
                        ? $"{x.CriterionPoints:n0} pts"
                        : $"{x.CriterionPoints:n2} pts",
                Ratings = GetRatings(x)
            }).ToList();
        }

        private List<RatingItem> GetRatings(QRubricCriterion criterion)
        {
            var result = new List<RatingItem>();

            var ratings = criterion.RubricRatings
                .OrderBy(x => x.RatingSequence)
                .ToList();

            var prevPoint = 0m;

            for (int i = ratings.Count - 1; i >= 0; i--)
            {
                var rating = ratings[i];

                var ratingItem = new RatingItem
                {
                    Title = rating.RatingTitle,
                    Description = rating.RatingDescription,
                    Points = !criterion.IsRange
                        ? $"{rating.RatingPoints:n2} pts"
                        : rating.RatingPoints == 0
                            ? $"{rating.RatingPoints:n0} pts"
                            : $"{rating.RatingPoints:n0} to >{prevPoint:n0} pts"
                };

                prevPoint = rating.RatingPoints;

                result.Insert(0, ratingItem);
            }

            return result;
        }
    }
}