using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Application.Records.Read;
using InSite.Common.Web.UI;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.UI.Admin.Assessments.Attempts.Controls
{
    public partial class RubricCriteriaList : BaseUserControl
    {
        private GradeCriterionItem _criterion;

        private List<GradeCriterionItem> Criteria
        {
            get => (List<GradeCriterionItem>)ViewState[nameof(Criteria)];
            set => ViewState[nameof(Criteria)] = value;
        }

        protected string UpdateSumFunc
        {
            get => (string)ViewState[nameof(UpdateSumFunc)];
            set => ViewState[nameof(UpdateSumFunc)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            CriteriaRepeater.ItemDataBound += CriteriaRepeater_ItemDataBound;
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            foreach (RepeaterItem criterion in CriteriaRepeater.Items)
            {
                var ratingRepeater = (Repeater)criterion.FindControl("RatingRepeater");
                foreach (RepeaterItem rating in ratingRepeater.Items)
                {
                    var isSelected = (IRadioButton)rating.FindControl("IsSelected");
                    var answerPoints = (NumericBox)rating.FindControl("AnswerPoints");

                    answerPoints.Enabled = isSelected.Checked;
                }
            }
        }

        private void CriteriaRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var item = (GradeCriterionItem)e.Item.DataItem;

            _criterion = item;

            var ratingRepeater = (Repeater)e.Item.FindControl("RatingRepeater");
            ratingRepeater.ItemDataBound += RatingRepeater_ItemDataBound;
            ratingRepeater.DataSource = item.Ratings;
            ratingRepeater.DataBind();
        }

        private void RatingRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var item = (GradeRatingItem)e.Item.DataItem;

            var isSelected = (IRadioButton)e.Item.FindControl("IsSelected");
            isSelected.GroupName = _criterion.CriterionId.ToString();
            isSelected.Checked = item.SelectedPoints.HasValue;

            var answerPoints = (NumericBox)e.Item.FindControl("AnswerPoints");
            answerPoints.Visible = _criterion.IsRange;

            if (_criterion.IsRange)
            {
                answerPoints.MinValue = item.CurrentPoints == 0 ? 0 : item.PrevPoints + 0.01m;
                answerPoints.MaxValue = item.CurrentPoints;

                var hasValue = item.SelectedPoints.HasValue && item.SelectedPoints >= answerPoints.MinValue && item.SelectedPoints <= answerPoints.MaxValue;
                if (hasValue)
                    answerPoints.ValueAsDecimal = item.SelectedPoints;
            }
        }

        public bool LoadData(Guid rubricId, List<GradeCriterionItem> criteria, string updateSumFunc, Dictionary<Guid, decimal> initRatinsPoints)
        {
            UpdateSumFunc = updateSumFunc;

            if (criteria == null)
            {
                Criteria = GetCriteria(rubricId);

                if (initRatinsPoints.IsNotEmpty())
                {
                    foreach (var rating in Criteria.SelectMany(x => x.Ratings))
                    {
                        if (initRatinsPoints.TryGetValue(rating.RatingId, out var points))
                            rating.SelectedPoints = points;
                    }
                }
            }
            else
                Criteria = criteria;

            CriteriaRepeater.DataSource = Criteria;
            CriteriaRepeater.DataBind();

            return Criteria.Count > 0;
        }

        public List<GradeCriterionItem> SaveCriteria()
        {
            for (int criterionIndex = 0; criterionIndex < CriteriaRepeater.Items.Count; criterionIndex++)
            {
                var criterion = Criteria[criterionIndex];
                var criterionItem = CriteriaRepeater.Items[criterionIndex];
                var ratingRepeater = (Repeater)criterionItem.FindControl("RatingRepeater");

                for (int ratingIndex = 0; ratingIndex < ratingRepeater.Items.Count; ratingIndex++)
                {
                    var rating = criterion.Ratings[ratingIndex];
                    var ratingItem = ratingRepeater.Items[ratingIndex];

                    var answerPoints = (NumericBox)ratingItem.FindControl("AnswerPoints");
                    var isSelected = (IRadioButton)ratingItem.FindControl("IsSelected");

                    if (isSelected.Checked)
                        rating.SelectedPoints = criterion.IsRange ? answerPoints.ValueAsDecimal : rating.CurrentPoints;
                    else
                    {
                        rating.SelectedPoints = null;
                        answerPoints.ValueAsDecimal = null;
                    }
                }
            }

            return Criteria;
        }

        internal static List<GradeCriterionItem> GetCriteria(Guid rubricId)
        {
            var lang = Identity.Language;
            var criteria = ServiceLocator.RubricSearch.GetCriteria(rubricId, x => x.RubricRatings);

            IDictionary<Guid, ContentContainer> contents = null;
            if (lang != Language.Default)
            {
                var allIds = criteria.SelectMany(x => x.RubricRatings).Select(x => x.RubricRatingIdentifier)
                    .Concat(criteria.Select(x => x.RubricCriterionIdentifier))
                    .ToArray();
                contents = ServiceLocator.ContentSearch.GetBlocks(allIds, lang, new[] { ContentLabel.Title, ContentLabel.Description });
            }

            var criteriaItems = new List<GradeCriterionItem>();

            foreach (var criterion in criteria)
            {
                var content = contents?.GetOrDefault(criterion.RubricCriterionIdentifier);
                var criterionItem = new GradeCriterionItem
                {
                    CriterionId = criterion.RubricCriterionIdentifier,
                    Title = (content?.Title.GetText(lang)).IfNullOrEmpty(criterion.CriterionTitle),
                    Description = (content?.Description.GetText(lang)).IfNullOrEmpty(criterion.CriterionDescription),
                    IsRange = criterion.IsRange,
                    Points = criterion.IsRange
                        ? $"{criterion.CriterionPoints:n0} pts"
                        : $"{criterion.CriterionPoints:n2} pts",
                    Ratings = GetRatings(criterion, contents, lang)
                };

                criteriaItems.Add(criterionItem);
            }

            return criteriaItems;
        }

        private static List<GradeRatingItem> GetRatings(QRubricCriterion criterion, IDictionary<Guid, ContentContainer> contents, string lang)
        {
            var result = new List<GradeRatingItem>();

            var ratings = criterion.RubricRatings
                .OrderBy(x => x.RatingSequence)
                .ToList();

            var prevPoint = 0m;

            for (int i = ratings.Count - 1; i >= 0; i--)
            {
                var rating = ratings[i];
                var content = contents?.GetOrDefault(rating.RubricRatingIdentifier);

                var ratingItem = new GradeRatingItem
                {
                    RatingId = rating.RubricRatingIdentifier,
                    Title = (content?.Title.GetText(lang)).IfNullOrEmpty(rating.RatingTitle),
                    Description = (content?.Description.GetText(lang)).IfNullOrEmpty(rating.RatingDescription),
                    CurrentPoints = rating.RatingPoints,
                    PrevPoints = prevPoint
                };

                if (!criterion.IsRange)
                    ratingItem.Points = $"{rating.RatingPoints:n2} pts";
                else
                {
                    ratingItem.Points = rating.RatingPoints == 0
                        ? $"{rating.RatingPoints:n0} pts"
                        : $"{rating.RatingPoints:n0} to >{prevPoint:n0} pts";
                }

                prevPoint = rating.RatingPoints;

                result.Insert(0, ratingItem);
            }

            return result;
        }
    }
}