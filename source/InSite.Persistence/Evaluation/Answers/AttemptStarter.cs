using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Domain.Attempts;

using Shift.Common;
using Shift.Constant;

using BankForm = InSite.Domain.Banks.Form;
using BankQuestion = InSite.Domain.Banks.Question;

namespace InSite.Persistence
{
    public static class AttemptStarter
    {
        #region Question

        private static void BindQuestion(AttemptQuestion aQuestion, BankQuestion bQuestion, string language)
        {
            aQuestion.Identifier = bQuestion.Identifier;
            aQuestion.Text = GetText(bQuestion.Content.Title, language);
            aQuestion.Points = bQuestion.Points ?? bQuestion.Options.Sum(x => x.Points);
            aQuestion.Type = bQuestion.Type;
            aQuestion.CutScore = bQuestion.CutScore;
            aQuestion.CalculationMethod = bQuestion.CalculationMethod;
        }

        private static void BindQuestionComposed(AttemptQuestionComposed aQuestion, BankQuestion bQuestion, string language)
        {
            BindQuestion(aQuestion, bQuestion, language);

            var rubric = ServiceLocator.RubricSearch.GetQuestionRubric(aQuestion.Identifier);
            if (rubric != null)
                aQuestion.Rubric = new AttemptQuestionRubric
                {
                    Identifier = rubric.RubricIdentifier,
                    Points = rubric.RubricPoints,
                };
        }

        public static AttemptQuestion CreateQuestion(BankQuestion bQuestion, bool allowRandomization, string language)
        {
            if (bQuestion.Type == QuestionItemType.ComposedVoice)
                return CreateQuestionComposedVoice(bQuestion, language);

            if (bQuestion.Type.IsComposed())
                return CreateQuestionComposed(bQuestion, language);

            if (bQuestion.Type == QuestionItemType.Matching)
                return CreateQuestionMatching(bQuestion, language);

            if (bQuestion.Type == QuestionItemType.Likert)
            {
                var rows = bQuestion.Likert.Rows.Select(x => x.Identifier).ToArray();
                var columns = bQuestion.Likert.Columns.Select(x => x.Identifier).ToArray();

                ApplyRandomization(rows);

                return CreateQuestionLikert(bQuestion, rows, columns, language);
            }

            if (bQuestion.Type.IsHotspot())
                return CreateQuestionHotspot(bQuestion, language);

            if (bQuestion.Type == QuestionItemType.Ordering)
            {
                var options = bQuestion.Ordering.Options.Select(x => x.Number).ToArray();

                ApplyRandomization(options);

                return CreateQuestionOrdering(bQuestion, options, language);
            }

            {
                var options = bQuestion.Options.Select(x => x.Number).ToArray();

                ApplyRandomization(options);

                return CreateQuestionDefault(bQuestion, options, language);
            }

            void ApplyRandomization<T>(IList<T> list)
            {
                if (!allowRandomization || !bQuestion.Randomization.Enabled)
                    return;

                if (bQuestion.Randomization.Count >= 2 && bQuestion.Randomization.Count < list.Count)
                    list.Shuffle(0, bQuestion.Randomization.Count);
                else
                    list.Shuffle();
            }
        }

        public static AttemptQuestion CreateQuestion(BankQuestion bQuestion, AttemptStarted2.QuestionHandle handle, string language)
        {
            AttemptQuestion result;

            if (bQuestion.Type == QuestionItemType.ComposedVoice)
                result = CreateQuestionComposedVoice(bQuestion, language);

            else if (bQuestion.Type.IsComposed())
                result = CreateQuestionComposed(bQuestion, language);

            else if (bQuestion.Type == QuestionItemType.Matching)
                result = CreateQuestionMatching(bQuestion, language);

            else if (bQuestion.Type == QuestionItemType.Likert)
                result = CreateQuestionLikert(bQuestion, handle.LikertRows, handle.LikertColumns, language);

            else if (bQuestion.Type.IsHotspot())
                result = CreateQuestionHotspot(bQuestion, language);

            else if (bQuestion.Type == QuestionItemType.Ordering)
                result = CreateQuestionOrdering(bQuestion, handle.Options, language);

            else
                result = CreateQuestionDefault(bQuestion, handle.Options, language);

            if (handle.Section.HasValue)
                result.Section = handle.Section.Value;

            return result;
        }

        public static AttemptQuestionMatch CreateQuestionMatching(BankQuestion bQuestion, string language)
        {
            var pairs = new List<AttemptQuestionMatchPair>();
            var data = bQuestion.Matches.Pairs;

            for (var i = 0; i < data.Count; i++)
            {
                var item = data[i];

                pairs.Add(new AttemptQuestionMatchPair
                {
                    LeftText = GetText(item.Left.Title, language),
                    RightText = GetText(item.Right.Title, language),
                    Points = item.Points,
                });
            }

            var distractors = GetDistractors(bQuestion.Matches.Distractors, language);

            var result = new AttemptQuestionMatch
            {
                Pairs = pairs.ToArray(),
                Distractors = distractors,
            };

            BindQuestion(result, bQuestion, language);

            return result;
        }

        private static string[] GetDistractors(List<ContentTitle> distractors, string language)
        {
            var languageDistractors = distractors
                .Select(x => x.Title[language])
                .Where(x => !string.IsNullOrEmpty(x))
                .ToArray();

            if (languageDistractors.Length > 0 || string.Equals(language, MultilingualString.DefaultLanguage, StringComparison.OrdinalIgnoreCase))
                return languageDistractors;

            return distractors
                .Select(x => x.Title.Default)
                .Where(x => !string.IsNullOrEmpty(x))
                .ToArray();
        }

        public static AttemptQuestionLikert CreateQuestionLikert(BankQuestion bQuestion, IEnumerable<Guid> rows, IEnumerable<Guid> columns, string language)
        {
            var questions = new List<AttemptQuestionDefault>();

            foreach (var row in rows.Select(x => bQuestion.Likert.GetRow(x)))
            {
                var options = new List<AttemptOption>();

                foreach (var o in columns.Select(x => row.GetOption(x)))
                {
                    if (o == null)
                        continue;

                    options.Add(new AttemptOption
                    {
                        Key = o.Number,
                        Points = o.Points,
                        Text = GetText(o.Column.Content.Title, language) ?? "n/a",
                    });
                }

                questions.Add(new AttemptQuestionDefault
                {
                    Identifier = row.Identifier,
                    Text = GetText(row.Content.Title, language),
                    Points = row.Points,
                    Options = options.ToArray()
                });
            }

            var result = new AttemptQuestionLikert
            {
                Questions = questions.ToArray(),
            };

            BindQuestion(result, bQuestion, language);

            return result;
        }

        public static AttemptQuestionHotspot CreateQuestionHotspot(BankQuestion bQuestion, string language)
        {
            var options = new List<AttemptOptionHotspot>();

            foreach (var o in bQuestion.Hotspot.Options)
            {
                options.Add(new AttemptOptionHotspot
                {
                    Key = o.Number,
                    Points = o.Points,
                    Text = o.Content?.Title?[language] ?? o.Content?.Title?.Default ?? "n/a",
                    Shape = o.Shape.ToString()
                });
            }

            var result = new AttemptQuestionHotspot
            {
                PinLimit = bQuestion.Hotspot.PinLimit,
                ShowShapes = bQuestion.Hotspot.ShowShapes,
                Image = bQuestion.Hotspot.Image.ToString(),
                Options = options.ToArray()
            };

            BindQuestion(result, bQuestion, language);

            return result;
        }

        public static AttemptQuestionComposed CreateQuestionComposed(BankQuestion bQuestion, string language)
        {
            var result = new AttemptQuestionComposed();

            BindQuestionComposed(result, bQuestion, language);

            return result;
        }

        public static AttemptQuestionComposedVoice CreateQuestionComposedVoice(BankQuestion bQuestion, string language)
        {
            var result = new AttemptQuestionComposedVoice
            {
                TimeLimit = bQuestion.ComposedVoice.TimeLimit,
                AttemptLimit = bQuestion.ComposedVoice.AttemptLimit
            };

            BindQuestionComposed(result, bQuestion, language);

            return result;
        }

        public static AttemptQuestionOrdering CreateQuestionOrdering(BankQuestion bQuestion, IEnumerable<int> options, string language)
        {
            var optionList = new List<AttemptOption>();
            foreach (var key in options)
            {
                var o = bQuestion.Ordering.Options.SingleOrDefault(x => x.Number == key);
                if (o == null)
                    continue;

                optionList.Add(new AttemptOption
                {
                    Key = o.Number,
                    Text = GetText(o.Content.Title, language) ?? "n/a"
                });
            }

            var solutions = new List<AttemptQuestionOrderingSolution>();
            foreach (var s in bQuestion.Ordering.Solutions)
            {
                solutions.Add(new AttemptQuestionOrderingSolution
                {
                    Identifier = s.Identifier,
                    OptionsOrder = s.Options.Select(x => bQuestion.Ordering.GetOption(x).Number).ToArray(),
                    Points = s.Points,
                    CutScore = s.CutScore
                });
            }

            var result = new AttemptQuestionOrdering
            {
                Options = optionList.ToArray(),
                Solutions = solutions.ToArray()
            };

            var label = bQuestion.Ordering.Label;
            if (label.Show)
            {
                result.TopLabel = GetText(label.TopContent.Title, language);
                result.BottomLabel = GetText(label.BottomContent.Title, language);
            }

            BindQuestion(result, bQuestion, language);

            return result;
        }

        public static AttemptQuestionDefault CreateQuestionDefault(BankQuestion bQuestion, IEnumerable<int> options, string language)
        {
            var optionList = new List<AttemptOption>();

            foreach (var key in options)
            {
                var o = bQuestion.Options.SingleOrDefault(x => x.Number == key);
                if (o == null)
                    continue;

                optionList.Add(new AttemptOption
                {
                    Key = o.Number,
                    Points = o.Points,
                    CutScore = o.CutScore.HasValue ? o.CutScore.Value / 100 : (decimal?)null,
                    Text = GetText(o.Content.Title, language) ?? "n/a",
                    IsTrue = o.IsTrue
                });
            }

            var result = new AttemptQuestionDefault
            {
                Options = optionList.ToArray(),
            };

            BindQuestion(result, bQuestion, language);

            return result;
        }

        #endregion

        #region Section

        public static AttemptSection[] CreateSections(BankForm bankForm)
        {
            if (bankForm.Specification.Type != SpecificationType.Static)
                return new AttemptSection[0];

            var result = new List<AttemptSection>();

            for (var i = 0; i < bankForm.Sections.Count; i++)
            {
                var section = bankForm.Sections[i];

                result.Add(new AttemptSection
                {
                    Identifier = section.Identifier,
                    ShowWarningNextTab = section.WarningOnNextTabEnabled,
                    IsBreakTimer = section.BreakTimerEnabled,
                    TimeLimit = section.TimeLimit,
                    TimerType = section.TimerType
                });
            }

            return result.ToArray();
        }

        #endregion

        private static string GetText(MultilingualString data, string lang)
        {
            return data == null ? null : (data[lang] ?? data.Default).NullIfEmpty();
        }
    }
}
