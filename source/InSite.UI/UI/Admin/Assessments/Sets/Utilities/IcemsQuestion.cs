using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using Shift.Common;

namespace InSite.Admin.Assessments.Sets.Utilities
{
    public class IcemsQuestion
    {
        #region Properties

        public string Sequence { get; private set; }
        public string Text { get; private set; }

        public IReadOnlyList<QuestionOption> Options { get; private set; }

        #endregion

        #region Construction

        private IcemsQuestion()
        {

        }

        #endregion

        #region Methods (reading)

        public static IcemsQuestion[] ReadV1(IEnumerable<XElement> items)
        {
            return items.Select(x =>
            {
                var sequence = x.Element("EXIM_SEQ_NO").Value;
                var answer = x.Element("tblCorrectResponse").Element("ITRP_LABEL").Value;

                return new IcemsQuestion
                {
                    Sequence = sequence,
                    Text = $"Question {sequence}",
                    Options = new[]
                    {
                        new QuestionOption("Option A", string.Equals(answer, "A", StringComparison.OrdinalIgnoreCase)),
                        new QuestionOption("Option B", string.Equals(answer, "B", StringComparison.OrdinalIgnoreCase)),
                        new QuestionOption("Option C", string.Equals(answer, "C", StringComparison.OrdinalIgnoreCase)),
                        new QuestionOption("Option D", string.Equals(answer, "D", StringComparison.OrdinalIgnoreCase))
                    }
                };
            }).ToArray();
        }

        public static IcemsQuestion[] ReadV2(IEnumerable<XElement> questionNodes)
        {
            var result = new List<IcemsQuestion>();

            foreach (var questionNode in questionNodes)
            {
                var textNode = questionNode.Element("Text")
                    ?? throw new ApplicationError("File has unsupported format. Node Occupation/Exam/Section/Question/Text is not found.");

                var question = new IcemsQuestion
                {
                    Text = textNode.Value
                };

                var listNode = questionNode.Element("AnswerList");
                var isList = listNode != null;

                if (!isList)
                {
                    listNode = questionNode.Element("AnswerTable")
                        ?? throw new ApplicationError("File has unsupported format. Node Occupation/Exam/Section/Question/AnswerList is not found.");

                    var heading1 = listNode.Element("Heading1")?.Value;
                    var heading2 = listNode.Element("Heading2")?.Value;

                    if (!string.IsNullOrEmpty(heading1) && !string.IsNullOrEmpty(heading2))
                        question.Text += "\n" + heading1 + " / " + heading2;
                }

                question.Options = listNode.Elements("Answer").Select((answerNode, index) =>
                {
                    var text = answerNode.Element("Value1")?.Value ?? $"Option {'A' + index}";
                    var isCorrect = bool.TryParse(answerNode.Attribute("Correct")?.Value, out var isCorrectParsed) && isCorrectParsed;

                    if (!isList)
                    {
                        var value2 = answerNode.Element("Value2")?.Value;

                        if (!string.IsNullOrEmpty(value2))
                            text += " / " + value2;
                    }

                    return new QuestionOption(text, isCorrect);
                }).ToArray();

                result.Add(question);
            }

            return result.ToArray();
        }

        #endregion
    }
}