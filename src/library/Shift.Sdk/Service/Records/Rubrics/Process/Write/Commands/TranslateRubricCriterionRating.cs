using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Records;

using Newtonsoft.Json;

using Shift.Common;

namespace InSite.Application.Rubrics.Write
{
    public class TranslateRubricCriterionRating : Command, IHasRun
    {
        public Guid RubricRatingId { get; set; }
        public ContentContainer Content { get; private set; }

        [JsonConstructor]
        public TranslateRubricCriterionRating()
        {

        }

        public TranslateRubricCriterionRating(Guid rubricId, Guid rubricRatingId)
        {
            AggregateIdentifier = rubricId;
            RubricRatingId = rubricRatingId;
            Content = new ContentContainer();
        }

        public TranslateRubricCriterionRating(Guid rubricId, Guid rubricRatingId, ContentContainer content)
        {
            AggregateIdentifier = rubricId;
            RubricRatingId = rubricRatingId;
            Content = content;
        }

        bool IHasRun.Run(RubricAggregate aggregate)
        {
            if (Content?.HasItems != true)
                return true;

            var state = aggregate.Data;
            var rating = state.FindRating(RubricRatingId);
            if (rating == null)
                return true;

            var changeContent = Content.Clone();

            foreach (var label in changeContent.GetLabels())
            {
                var stateItem = rating.Content[label];
                var changeItem = changeContent[label];
                changeItem.Text.RemoveExist(stateItem.Text);
                changeItem.Html.RemoveExist(stateItem.Html);
                changeItem.Snip.RemoveExist(stateItem.Snip);
            }

            if (!changeContent.HasItems)
                return true;

            if (changeContent.Title.Text.Exists(MultilingualString.DefaultLanguage) && changeContent.Title.Text.Default.IsEmpty())
                return true;

            aggregate.Apply(new RubricCriterionRatingTranslated(RubricRatingId, changeContent));

            return true;
        }

        public bool Run(RubricAggregate course)
        {
            throw new NotImplementedException();
        }
    }
}
