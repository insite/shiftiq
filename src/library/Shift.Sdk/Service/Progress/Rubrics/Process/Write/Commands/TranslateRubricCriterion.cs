using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Records;

using Newtonsoft.Json;

using Shift.Common;

namespace InSite.Application.Rubrics.Write
{
    public class TranslateRubricCriterion : Command, IHasRun
    {
        public Guid RubricCriterionId { get; set; }
        public ContentContainer Content { get; private set; }

        [JsonConstructor]
        public TranslateRubricCriterion()
        {

        }

        public TranslateRubricCriterion(Guid rubricId, Guid rubricCriterionId)
        {
            AggregateIdentifier = rubricId;
            RubricCriterionId = rubricCriterionId;
            Content = new ContentContainer();
        }

        public TranslateRubricCriterion(Guid rubricId, Guid rubricCriterionId, ContentContainer content)
        {
            AggregateIdentifier = rubricId;
            RubricCriterionId = rubricCriterionId;
            Content = content;
        }

        bool IHasRun.Run(RubricAggregate aggregate)
        {
            if (Content?.HasItems != true)
                return true;

            var state = aggregate.Data;
            var criterion = state.FindCriterion(RubricCriterionId);
            if (criterion == null)
                return true;

            var changeContent = Content.Clone();

            foreach (var label in changeContent.GetLabels())
            {
                var stateItem = criterion.Content[label];
                var changeItem = changeContent[label];
                changeItem.Text.RemoveExist(stateItem.Text);
                changeItem.Html.RemoveExist(stateItem.Html);
                changeItem.Snip.RemoveExist(stateItem.Snip);
            }

            if (!changeContent.HasItems)
                return true;

            if (changeContent.Title.Text.Exists(MultilingualString.DefaultLanguage) && changeContent.Title.Text.Default.IsEmpty())
                return true;

            aggregate.Apply(new RubricCriterionTranslated(RubricCriterionId, changeContent));

            return true;
        }
    }
}
