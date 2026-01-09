using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Application.Attempts.Read;
using InSite.Application.Records.Read;
using InSite.Common.Web.UI;
using InSite.Domain.Banks;

using Newtonsoft.Json;

using Shift.Common;

namespace InSite.UI.Admin.Assessments.Attempts.Controls
{
    public partial class ViewComposed : BaseUserControl
    {
        public class DataItem
        {
            public int QuestionSequence { get; set; }
            public string QuestionText { get; set; }
            public Guid? AnswerFileIdentifier { get; set; }
            public int? AnswerAttemptLimit { get; set; }
            public int? AnswerRequestAttempt { get; set; }
            public string AnswerText { get; set; }
            public decimal? AnswerPoints { get; set; }
            public decimal? QuestionPoints { get; set; }
            public string CompetencyAreaCode { get; set; }
            public string CompetencyItemCode { get; set; }
            public bool IsGraded { get; set; }
            public string ExemplarHtml { get; set; }
            public Dictionary<Guid, decimal> Ratings { get; set; }
        }

        public class AnswerContainer : Control, INamingContainer
        {
            public DataItem DataItem { get; }

            internal AnswerContainer(DataItem dataItem)
            {
                DataItem = dataItem;
            }
        }

        public delegate void AnswerContainerEventHandler(object sender, AnswerContainerEventArgs e);

        public class AnswerContainerEventArgs : EventArgs
        {
            public AnswerContainer Container { get; }

            public AnswerContainerEventArgs(AnswerContainer container)
            {
                Container = container;
            }
        }

        [TemplateContainer(typeof(AnswerContainer))]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public ITemplate AnswerTemplate { get; set; }

        public event AnswerContainerEventHandler ContainerDataBound;

        private Dictionary<Guid, QRubricRating> _ratingCache;

        public void BindQuestions(BankState bank, IEnumerable<QAttemptQuestion> questions)
        {
            var dataItems = questions.Select(x =>
            {
                var bankQuestion = bank.FindQuestion(x.QuestionIdentifier);

                return new DataItem
                {
                    QuestionSequence = x.QuestionSequence,
                    QuestionText = x.QuestionText,
                    AnswerFileIdentifier = x.AnswerFileIdentifier,
                    AnswerAttemptLimit = x.AnswerAttemptLimit,
                    AnswerRequestAttempt = x.AnswerRequestAttempt,
                    AnswerText = x.AnswerText,
                    AnswerPoints = x.AnswerPoints,
                    QuestionPoints = x.QuestionPoints,
                    CompetencyAreaCode = x.CompetencyAreaCode,
                    CompetencyItemCode = x.CompetencyItemCode,
                    IsGraded = x.AnswerPoints.HasValue,
                    ExemplarHtml = Markdown.ToHtml(bankQuestion?.Content.Exemplar.Default).NullIfEmpty(),
                    Ratings = x.RubricRatingPoints.IsEmpty()
                        ? null
                        : JsonConvert.DeserializeObject<Dictionary<Guid, decimal>>(x.RubricRatingPoints)
                };
            }).ToArray();
            var ratingIds = dataItems
                .Where(x => x.IsGraded && x.Ratings.IsNotEmpty())
                .SelectMany(x => x.Ratings.Keys)
                .Distinct().ToArray();

            _ratingCache = ratingIds.IsNotEmpty()
                ? ServiceLocator.RubricSearch.GetRatings(ratingIds, x => x.RubricCriterion).ToDictionary(x => x.RubricRatingIdentifier)
                : new Dictionary<Guid, QRubricRating>();

            QuestionRepeater.ItemCreated += QuestionRepeater_ItemCreated;
            QuestionRepeater.ItemDataBound += QuestionRepeater_ItemDataBound;
            QuestionRepeater.DataSource = dataItems;
            QuestionRepeater.DataBind();
        }

        private void QuestionRepeater_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            if (AnswerTemplate != null)
            {
                var dataItem = (DataItem)e.Item.DataItem;
                AddTemplate("AnswerPlaceholder1", dataItem);
                AddTemplate("AnswerPlaceholder2", dataItem);
            }

            void AddTemplate(string placeholderId, DataItem dataItem)
            {
                var placeholder = (PlaceHolder)e.Item.FindControl(placeholderId);
                var container = new AnswerContainer(dataItem);

                AnswerTemplate.InstantiateIn(container);

                placeholder.Controls.Add(container);

                ContainerDataBound?.Invoke(this, new AnswerContainerEventArgs(container));
            }
        }

        private void QuestionRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var dataItem = (DataItem)e.Item.DataItem;

            var ratingRepeater = (Repeater)e.Item.FindControl("RatingRepeater");
            ratingRepeater.DataSource = dataItem.Ratings?.Keys.Select(x =>
            {
                var rating = _ratingCache.GetOrDefault(x);
                var criterionTitle = "<i>Not Found</i>";
                var ratingTitle = $"<i>{x}</i>";
                var ratingSequence = int.MaxValue;

                if (rating != null)
                {
                    ratingTitle = HttpUtility.HtmlEncode(rating.RatingTitle);
                    ratingSequence = rating.RatingSequence;

                    if (rating.RubricCriterion != null)
                        criterionTitle = HttpUtility.HtmlEncode(rating.RubricCriterion.CriterionTitle);
                }

                return new
                {
                    CriterionTitle = criterionTitle,
                    RatingTitle = ratingTitle,
                    Points = dataItem.Ratings[x],
                    Sequence = ratingSequence
                };
            }).OrderBy(x => x.Sequence).ThenBy(x => x.RatingTitle);
            ratingRepeater.DataBind();
        }

        protected string FormatScore()
        {
            var question = (DataItem)Page.GetDataItem();

            return question.AnswerPoints == null
                ? "<span class='badge bg-warning'>Not Graded</span>"
                : $"<span>{question.AnswerPoints} / {question.QuestionPoints}</span> <span class='badge bg-success ms-1'>Graded</span>";
        }

        protected string GetQuestionText()
        {
            var question = (DataItem)Page.GetDataItem();
            return Markdown.ToHtml(question.QuestionText);
        }

        protected string GetRatingTotalPoints()
        {
            var question = (DataItem)Page.GetDataItem();

            return question.Ratings.IsEmpty()
                ? "N/A"
                : question.Ratings.Values.Sum().ToString("n2");
        }
    }
}