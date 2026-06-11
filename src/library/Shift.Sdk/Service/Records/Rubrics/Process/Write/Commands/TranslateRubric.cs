using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Records;

using Newtonsoft.Json;

using Shift.Common;

namespace InSite.Application.Rubrics.Write
{
    public class TranslateRubric : Command, IHasRun
    {
        public ContentContainer Content { get; private set; }

        [JsonConstructor]
        public TranslateRubric()
        {

        }

        public TranslateRubric(Guid rubricId)
        {
            AggregateIdentifier = rubricId;
            Content = new ContentContainer();
        }

        public TranslateRubric(Guid rubricId, ContentContainer content)
        {
            AggregateIdentifier = rubricId;
            Content = content;
        }

        bool IHasRun.Run(RubricAggregate aggregate)
        {
            if (Content?.HasItems != true)
                return true;

            var changeContent = Content.Clone();

            foreach (var label in changeContent.GetLabels())
            {
                var stateItem = aggregate.Data.Content[label];
                var changeItem = changeContent[label];
                changeItem.Text.RemoveExist(stateItem.Text);
                changeItem.Html.RemoveExist(stateItem.Html);
                changeItem.Snip.RemoveExist(stateItem.Snip);
            }

            if (!changeContent.HasItems)
                return true;

            if (changeContent.Title.Text.Exists(MultilingualString.DefaultLanguage) && changeContent.Title.Text.Default.IsEmpty())
                return true;

            aggregate.Apply(new RubricTranslated(changeContent));

            return true;
        }
    }
}
