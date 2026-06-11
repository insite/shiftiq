using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

using Shift.Common.Timeline.Changes;

using Newtonsoft.Json;

using Shift.Common;

namespace InSite.Domain.Records
{
    public class RubricState : AggregateState
    {
        [JsonProperty]
        public Guid OrganizationId { get; private set; }

        [JsonProperty]
        public ContentContainer Content { get; private set; }

        public bool IsDeleted { get; set; }

        [JsonIgnore]
        public decimal Points => Criteria.Count == 0 ? 0 : Criteria.Sum(x => x.Points);

        public List<RubricCriterion> Criteria { get; set; } = new List<RubricCriterion>();

        [NonSerialized]
        private Dictionary<Guid, RubricRating> _ratings = new Dictionary<Guid, RubricRating>();

        #region Methods (when)

        public void When(RubricCreated c)
        {
            OrganizationId = c.OriginOrganization;

            Content = new ContentContainer();
            Content.Title.Text.Default = c.RubricTitle;
            Content.Title.CreateSnip();
        }

        public void When(RubricDeleted c)
        {
            IsDeleted = true;

            Criteria.Clear();
            _ratings.Clear();

            Content = null;
        }

        public void When(RubricRenamed c)
        {
            Content.Title.Text.Default = c.RubricTitle;
            Content.Title.CreateSnip();
        }

        public void When(RubricDescribed c)
        {
            Content.Description.Text.Default = c.RubricDescription;
            Content.Description.CreateSnip();
        }

        public void When(RubricTranslated c)
        {
            Content.Set(c.Content, ContentContainer.SetNullAction.Remove);
            Content.CreateSnips();
        }

        public void When(RubricTimestampModified c)
        {

        }

        public void When(RubricCriterionAdded c)
        {
            var index = c.CriterionSequence.HasValue ? c.CriterionSequence.Value - 1 : -1;
            var criterion = new RubricCriterion
            {
                Id = c.RubricCriterionId,
                IsRange = c.IsRange
            };

            criterion.Content.Title.Text.Default = c.CriterionTitle;
            criterion.Content.Title.CreateSnip();

            if (index >= 0 && index < Criteria.Count)
                Criteria.Insert(index, criterion);
            else
                Criteria.Add(criterion);
        }

        public void When(RubricCriterionRemoved c)
        {
            Criteria.RemoveAll(x => x.Id == c.RubricCriterionId);
        }

        public void When(RubricCriterionRenamed c)
        {
            var criterion = FindCriterion(c.RubricCriterionId);
            criterion.Content.Title.Text.Default = c.CriterionTitle;
            criterion.Content.Title.CreateSnip();
        }

        public void When(RubricCriterionDescribed c)
        {
            var criterion = FindCriterion(c.RubricCriterionId);
            criterion.Content.Description.Text.Default = c.CriterionDescription;
            criterion.Content.Description.CreateSnip();
        }

        public void When(RubricCriterionIsRangeModified c)
        {
            var criterion = FindCriterion(c.RubricCriterionId);
            criterion.IsRange = c.IsRange;
        }

        public void When(RubricCriterionTranslated c)
        {
            var criterion = FindCriterion(c.RubricCriterionId);
            criterion.Content.Set(c.Content, ContentContainer.SetNullAction.Remove);
            criterion.Content.CreateSnips();
        }

        public void When(RubricCriterionRatingAdded c)
        {
            var criterion = FindCriterion(c.RubricCriterionId);
            var rating = new RubricRating
            {
                Id = c.RubricRatingId,
                Points = c.RatingPoints
            };

            rating.Content.Title.Text.Default = c.RatingTitle;
            rating.Content.Title.CreateSnip();

            _ratings.Add(rating.Id, rating);
            criterion.Ratings.Add(rating, c.RatingSequence);
        }

        public void When(RubricCriterionRatingRemoved c)
        {
            var rating = FindRating(c.RubricRatingId);
            rating.Criterion.Ratings.Remove(rating);

            _ratings.Remove(c.RubricRatingId);

        }

        public void When(RubricCriterionRatingRenamed c)
        {
            var rating = FindRating(c.RubricRatingId);
            rating.Content.Title.Text.Default = c.RatingTitle;
            rating.Content.Title.CreateSnip();
        }

        public void When(RubricCriterionRatingDescribed c)
        {
            var rating = FindRating(c.RubricRatingId);
            rating.Content.Description.Text.Default = c.RatingDescription;
            rating.Content.Description.CreateSnip();
        }

        public void When(RubricCriterionRatingPointsModified c)
        {
            var rating = FindRating(c.RubricRatingId);
            rating.Points = c.RatingPoints;
        }

        public void When(RubricCriterionRatingTranslated c)
        {
            var rating = FindRating(c.RubricRatingId);
            rating.Content.Set(c.Content, ContentContainer.SetNullAction.Remove);
            rating.Content.CreateSnips();
        }

        #endregion

        #region Methods (serialization)

        [OnDeserialized]
        internal void OnDeserialized(StreamingContext context)
        {
            if (_ratings == null)
                _ratings = new Dictionary<Guid, RubricRating>();

            foreach (var rating in Criteria.SelectMany(x => x.Ratings))
                _ratings.Add(rating.Id, rating);
        }

        #endregion

        #region Methods (helpers)

        public RubricCriterion FindCriterion(Guid id) => Criteria.FirstOrDefault(x => x.Id == id);

        public RubricRating FindRating(Guid ratingId) => _ratings.GetOrDefault(ratingId);

        #endregion
    }
}
