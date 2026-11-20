using InSite.Domain.Banks;

using Shift.Common;
using Shift.Constant;

namespace Shift.Test.Assessments
{
    public class BankQuestionContentTests
    {
        [Fact]
        public void BankAggregate_ChangeQuestionContent_ContentConvertedWithMultilingualTranslations()
        {
            var aggregate = new BankAggregate();
            var bankId = Guid.NewGuid();
            var orgId = Guid.NewGuid();
            var bankState = new BankState { Identifier = bankId, Tenant = orgId };
            aggregate.OpenBank(bankState);

            var setId = Guid.NewGuid();
            aggregate.AddSet(setId, "Set 1", Guid.Empty);

            var questionId = Guid.NewGuid();
            var initialContent = new ContentExamQuestion();
            initialContent.Title["en"] = "Initial English Title";

            aggregate.AddQuestion2(
                setId,
                questionId,
                QuestionItemType.SingleCorrect,
                "Condition",
                1,
                Guid.Empty,
                null,
                1,
                QuestionCalculationMethod.Default,
                initialContent);

            var updatedContent = new ContentExamQuestion();
            updatedContent.Title["en"] = "The quick brown fox";
            updatedContent.Title["fr"] = "Le renard brun rapide";
            updatedContent.Rationale["en"] = "English rationale";
            updatedContent.Rationale["fr"] = "Raisonnement en français";
            // Set summary in the command to verify it is NOT applied by When(QuestionContentChanged)
            updatedContent.Summary["en"] = "Summary that should not be applied";

            aggregate.ChangeQuestionContent(questionId, updatedContent);

            var question = aggregate.Data.FindQuestion(questionId);
            Assert.NotNull(question);

            // Verify the aggregate state has both en and fr titles
            Assert.Equal("The quick brown fox", question.Content.Title["en"]);
            Assert.Equal("Le renard brun rapide", question.Content.Title["fr"]);
            Assert.Equal("English rationale", question.Content.Rationale["en"]);
            Assert.Equal("Raisonnement en français", question.Content.Rationale["fr"]);

            // Quirk pin: BankState.When(QuestionContentChanged) does NOT apply Summary
            Assert.True(string.IsNullOrEmpty(question.Content.Summary["en"]));

            // Verify conversion to ContentContainer
            var container = ContentContainer.Create(question.Content);
            Assert.True(container.IsLoaded);
            Assert.Equal("The quick brown fox", container.GetText(ContentLabel.Title, "en"));
            Assert.Equal("Le renard brun rapide", container.GetText(ContentLabel.Title, "fr"));
            Assert.Equal("English rationale", container.GetText(ContentLabel.Rationale, "en"));
            Assert.Equal("Raisonnement en français", container.GetText(ContentLabel.Rationale, "fr"));
            Assert.False(container.Exists(ContentLabel.Summary));
        }

        [Fact]
        public void BankAggregate_UpgradeLikertQuestion_DeduplicatesLikertRowContainers()
        {
            var aggregate = new BankAggregate();
            var bankId = Guid.NewGuid();
            var orgId = Guid.NewGuid();
            var bankState = new BankState { Identifier = bankId, Tenant = orgId };
            aggregate.OpenBank(bankState);

            var setId = Guid.NewGuid();
            aggregate.AddSet(setId, "Set 1", Guid.Empty);

            var questionId = Guid.NewGuid();
            var initialContent = new ContentExamQuestion();
            initialContent.Title["en"] = "Likert Question Title";

            aggregate.AddQuestion2(
                setId,
                questionId,
                QuestionItemType.Likert,
                "Condition",
                1,
                Guid.Empty,
                null,
                1,
                QuestionCalculationMethod.Default,
                initialContent);

            var rowId = Guid.NewGuid();
            var rowContent = new ContentTitle();
            rowContent.Title["en"] = "Lorem ipsum dolor sit amet, consectetur adipiscing elit";

            aggregate.AddQuestionLikertRow(questionId, rowId, Guid.Empty, new Guid[0], rowContent);

            var upgradedQuestionId = Guid.NewGuid();
            aggregate.UpgradeQuestion(questionId, upgradedQuestionId);

            // Verify both question versions exist in the bank
            var questions = aggregate.Data.Sets.SelectMany(x => x.Questions).SelectMany(x => x.EnumerateAllVersions()).ToList();
            Assert.Equal(2, questions.Count);

            // Both versions reference the exact same Likert Row identifier
            Assert.Equal(rowId, questions[0].Likert.Rows.Single().Identifier);
            Assert.Equal(rowId, questions[1].Likert.Rows.Single().Identifier);

            // Verify dictionary deduplication collects exactly 3 containers: v1 question, v2 question, and rowId once
            var containers = new Dictionary<Guid, MultilingualDictionary>();
            foreach (var q in questions)
            {
                if (q.Content != null && !q.Content.IsEmpty)
                    containers[q.Identifier] = q.Content;

                foreach (var r in q.Likert.Rows)
                {
                    if (r.Content != null && !r.Content.IsEmpty)
                        containers[r.Identifier] = r.Content;
                }
            }

            Assert.Equal(3, containers.Count);
            Assert.True(containers.ContainsKey(questionId));
            Assert.True(containers.ContainsKey(upgradedQuestionId));
            Assert.True(containers.ContainsKey(rowId));

            // Verify no duplicate keys when collecting into (containerId, label, lang)
            var insertedKeys = new HashSet<(Guid, Guid, string, string)>();
            foreach (var pair in containers)
            {
                var container = ContentContainer.Create(pair.Value);
                foreach (var label in container.GetLabels())
                {
                    var item = container[label];
                    foreach (var lang in item.Text.Languages.Concat(item.Html.Languages).Distinct())
                    {
                        var added = insertedKeys.Add((orgId, pair.Key, label, lang));
                        Assert.True(added, $"Duplicate key found for container {pair.Key}, label {label}, lang {lang}");
                    }
                }
            }

            Assert.Equal(3, insertedKeys.Count);
        }
    }
}
