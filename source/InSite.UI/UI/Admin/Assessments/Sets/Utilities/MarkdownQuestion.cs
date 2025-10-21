using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Assessments.Sets.Utilities
{
    public class MarkdownQuestion
    {
        #region Enums

        private enum MarkdownReadState
        {
            None,
            Question,
            Options,
            CorrectAnswerFeedback,
            IncorrectAnswerFeedback,
            Area,
            Competency
        }

        #endregion

        #region Properties

        public string Sequence { get; private set; }
        public string Text { get; private set; }
        public string Feedback { get; private set; }

        public string Standard { get; private set; }
        public string StandardParent { get; private set; }

        public IReadOnlyList<QuestionOption> Options => _options;

        #endregion

        #region Fields

        private List<QuestionOption> _options = new List<QuestionOption>();

        #endregion

        #region Construction

        private MarkdownQuestion()
        {

        }

        #endregion

        #region Methods (parse file)

        public static MarkdownQuestion[] Read(Stream input)
        {
            var lineNumber = 0;
            var list = new List<MarkdownQuestion>();

            using (var reader = new StreamReader(input, Encoding.UTF8))
            {
                var line = string.Empty;
                var state = MarkdownReadState.None;

                MarkdownQuestion question = null;

                while ((line = reader.ReadLine()) != null)
                {
                    lineNumber++;
                    line = line.Trim();

                    if (line.StartsWith("# QUESTION ", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!int.TryParse(line.Substring(11), out var number))
                            throw GetException($"invalid question number.");

                        list.Add(question = new MarkdownQuestion { Sequence = number.ToString() });

                        state = MarkdownReadState.Question;
                    }
                    else if (string.Equals(line, "## Options", StringComparison.OrdinalIgnoreCase))
                    {
                        if (state == MarkdownReadState.Question || state == MarkdownReadState.Competency)
                            state = MarkdownReadState.Options;
                        else
                            throw GetException($"invalid option declaration position.");
                    }
                    else if (string.Equals(line, "## Correct Answer Feedback", StringComparison.OrdinalIgnoreCase))
                    {
                        if (state == MarkdownReadState.Options)
                            state = MarkdownReadState.CorrectAnswerFeedback;
                        else
                            throw GetException($"Unexpected Token: Comment on Correct Answer");
                    }
                    else if (string.Equals(line, "## Incorrect Answer Feedback", StringComparison.OrdinalIgnoreCase))
                    {
                        if (state == MarkdownReadState.Options || state == MarkdownReadState.CorrectAnswerFeedback)
                            state = MarkdownReadState.IncorrectAnswerFeedback;
                        else
                            throw GetException($"Unexpected Token: Comment on Incorrect Answer");
                    }
                    else if (line.StartsWith("## Competency Area - ", StringComparison.OrdinalIgnoreCase))
                    {
                        if (state == MarkdownReadState.Options || state == MarkdownReadState.CorrectAnswerFeedback || state == MarkdownReadState.IncorrectAnswerFeedback)
                        {
                            state = MarkdownReadState.Area;
                            question.StandardParent = line.Substring("## Competency Area - ".Length);
                        }
                        else
                            throw GetException($"Unexpected Token: Competency Area");
                    }
                    else if (line.StartsWith("### Competency - ", StringComparison.OrdinalIgnoreCase))
                    {
                        if (state == MarkdownReadState.Area)
                        {
                            state = MarkdownReadState.Competency;
                            question.Standard = line.Substring("### Competency - ".Length);
                        }
                        else
                            throw GetException($"Unexpected Token: Competency");
                    }
                    else if (state == MarkdownReadState.None)
                    {
                        if (!string.IsNullOrEmpty(line))
                            throw GetException($"missed question declaration.");
                    }
                    else if (state == MarkdownReadState.Question)
                    {
                        question.Text += Environment.NewLine;
                        if (!string.IsNullOrEmpty(line))
                            question.Text += line.Trim();
                    }
                    else if (state == MarkdownReadState.Options)
                    {
                        if (string.IsNullOrEmpty(line))
                            continue;

                        if (!line.StartsWith("- "))
                            throw GetException($"option's declaration must starts with '- '.");

                        var text = line.Substring(2).Trim();
                        var option = text.StartsWith("*") && text.EndsWith("*")
                            ? new QuestionOption(text.Substring(1, text.Length - 2).Trim(), true)
                            : new QuestionOption(text, false);

                        question._options.Add(option);
                    }
                    else if (state == MarkdownReadState.CorrectAnswerFeedback)
                    {
                        question.Feedback += Environment.NewLine;
                        if (!string.IsNullOrEmpty(line))
                            question.Feedback += line.Trim();
                    }
                    else if (state == MarkdownReadState.IncorrectAnswerFeedback)
                    {
                        question.Feedback += Environment.NewLine;
                        if (!string.IsNullOrEmpty(line))
                            question.Feedback += line.Trim();
                    }
                    else if (state == MarkdownReadState.Area || state == MarkdownReadState.Competency)
                    {

                    }
                    else
                    {
                        throw GetException($"unexpected read state ({state.GetName()})");
                    }
                }
            }

            return list.ToArray();

            Exception GetException(string text)
            {
                return new ApplicationError($"Line {lineNumber}: {text}");
            }
        }

        #endregion
    }
}