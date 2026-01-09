using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;

using InSite.Application.Attempts.Read;
using InSite.Application.Contents.Read;
using InSite.Domain.Attempts;
using InSite.Domain.Banks;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;

using BankOption = InSite.Domain.Banks.Option;
using BankQuestion = InSite.Domain.Banks.Question;

namespace InSite.Persistence
{
    public class InstructorAttemptStore : IInstructorAttemptStore
    {
        internal InternalDbContext CreateContext() => new InternalDbContext(true);

        #region Classes

        private class ImportAnswer
        {
            public int? AnswerKey { get; set; }
            public BankQuestion Question { get; set; }
            public BankOption[] Options { get; set; }
        }

        #endregion

        #region Methods (delete)

        public void DeleteAttempt(Guid attempt)
        {
            const string sql = @"
DELETE FROM assessments.QAttempt         WHERE AttemptIdentifier = @Aggregate;
DELETE FROM assets.QComment              WHERE AssessmentAttemptIdentifier = @Aggregate;
DELETE FROM assessments.QAttemptQuestion WHERE AttemptIdentifier = @Aggregate;
DELETE FROM assessments.QAttemptOption   WHERE AttemptIdentifier = @Aggregate;
DELETE FROM assessments.QAttemptMatch    WHERE AttemptIdentifier = @Aggregate;
DELETE FROM assessments.QAttemptPin      WHERE AttemptIdentifier = @Aggregate;
DELETE FROM assessments.QAttemptSolution WHERE AttemptIdentifier = @Aggregate;
DELETE FROM assessments.QAttemptSection  WHERE AttemptIdentifier = @Aggregate;

UPDATE registrations.QRegistration SET AttemptIdentifier = NULL WHERE AttemptIdentifier = @Aggregate;
";

            using (var db = CreateContext())
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    var entity = db.QAttempts.FirstOrDefault(x => x.AttemptIdentifier == attempt);

                    if (entity != null)
                    {
                        var summary = new AttemptSummary(db, entity.FormIdentifier, entity.LearnerUserIdentifier);
                        summary.OnRemove(entity);
                        db.SaveChanges();
                    }

                    db.Database.ExecuteSqlCommand(sql, new SqlParameter("@Aggregate", attempt));

                    transaction.Commit();
                }
            }
        }

        #endregion

        #region Methods (insert: imported)

        public void InsertAttempt(AttemptImported e, Form form)
        {
            var language = !string.IsNullOrEmpty(e.Language) ? e.Language : Language.Default;
            var answers = GetImportAnswers(e, form);

            using (var db = CreateContext())
            {
                var summary = new AttemptSummary(db, e.Form, e.Candidate);
                var attempt = CreateAttempt(e, summary);

                for (var i = 0; i < answers.Length; i++)
                    AddImportedQuestion(attempt, answers[i], i + 1, language);

                attempt.AttemptScore = CalcAttemptScore(attempt.AttemptPoints, attempt.FormPoints);
                attempt.AttemptIsPassing = AttemptIsPassing(attempt.AttemptScore, form.Specification.Calculation.PassingScore);

                db.QAttempts.Add(attempt);

                summary.OnInsert(attempt);

                db.SaveChanges();
            }
        }

        private static ImportAnswer[] GetImportAnswers(AttemptImported e, Form form)
        {
            var formQuestions = form.Sections
                .SelectMany(x => x.Fields.Select(y => y.Question))
                .ToDictionary(x => x.Identifier, x => x);
            var formOptions = formQuestions.Values
                .SelectMany(x => x.Options)
                .ToDictionary(x => x.Number, x => x);

            return e.Answers
                .Where(x => formQuestions.ContainsKey(x.Question))
                .Select(handle => new ImportAnswer
                {
                    Question = formQuestions[handle.Question],
                    AnswerKey = handle.Answer,
                    Options = handle.Options
                        .Where(x => formOptions.ContainsKey(x))
                        .Select(x => formOptions[x])
                        .Where(x => x.Question.Identifier == handle.Question)
                        .ToArray()
                })
                .ToArray();
        }

        private static QAttempt CreateAttempt(AttemptImported e, AttemptSummary summary)
        {
            return new QAttempt
            {
                AttemptIdentifier = e.AggregateIdentifier,
                AttemptImported = e.ChangeTime,
                AttemptGraded = e.Completed,
                AttemptSubmitted = e.Completed,
                AttemptNumber = summary.Entity.AttemptTotalCount + 1,
                AttemptStarted = e.Started,
                AttemptStatus = "Imported",
                AttemptTag = StringHelper.Snip(e.Tag, 100),
                AssessorUserIdentifier = e.Candidate,
                LearnerUserIdentifier = e.Candidate,
                FormIdentifier = e.Form,
                FormPoints = 0,
                AttemptPoints = 0,
                RegistrationIdentifier = e.Registration,
                OrganizationIdentifier = e.Tenant
            };
        }

        private static void AddImportedQuestion(QAttempt attempt, ImportAnswer answer, int sequence, string language)
        {
            var questionEntity = CreateImportedQuestion(answer.Question, language);
            questionEntity.QuestionSequence = sequence;
            questionEntity.AnswerOptionKey = answer.AnswerKey;

            for (var i = 0; i < answer.Options.Length; i++)
            {
                var option = CreateImportedOption(answer.Question, answer.Options[i], language);

                option.QuestionSequence = sequence;
                option.OptionSequence = i + 1;

                if (answer.AnswerKey == option.OptionKey)
                {
                    option.AnswerIsSelected = true;
                    questionEntity.AnswerPoints += option.OptionPoints;
                    attempt.AttemptPoints += option.OptionPoints;
                }

                if (questionEntity.QuestionPoints < option.OptionPoints)
                    questionEntity.QuestionPoints = option.OptionPoints;

                attempt.Options.Add(option);
            }

            attempt.FormPoints += questionEntity.QuestionPoints;

            attempt.Questions.Add(questionEntity);
        }

        private static QAttemptOption CreateImportedOption(BankQuestion question, BankOption option, string language)
        {
            return new QAttemptOption
            {
                QuestionIdentifier = question.Identifier,
                OptionKey = option.Number,
                OptionPoints = option.Points,
                OptionCutScore = option.CutScore.HasValue ? option.CutScore.Value / 100 : (decimal?)null,
                OptionText = option.Content?.Title?[language] ?? option.Content?.Title?.Default ?? "n/a",
                OptionIsTrue = option.IsTrue
            };
        }

        private static QAttemptQuestion CreateImportedQuestion(BankQuestion question, string language)
        {
            return new QAttemptQuestion
            {
                QuestionIdentifier = question.Identifier,
                QuestionText = question.Content.Title[language] ?? question.Content.Title.Default,
                QuestionType = question.Type.GetName(),
                QuestionCutScore = question.CutScore,
                QuestionCalculationMethod = question.CalculationMethod.GetName(),
                AnswerPoints = 0,
                QuestionPoints = 0
            };
        }

        #endregion

        #region Methods (insert: started)

        public void InsertAttempt(AttemptStarted2 e, Form form)
        {
            var attemptState = (AttemptState)e.AggregateState;
            var bank = form.Specification.Bank;
            var language = e.Language.IfNullOrEmpty(Language.Default);

            using (var db = CreateContext())
            {
                var summary = new AttemptSummary(db, e.FormIdentifier, e.LearnerUserIdentifier);
                var attempt = CreateAttempt(e, attemptState, summary);
                var sections = CreateSections(attempt, attemptState);

                for (var i = 0; i < e.Questions.Length; i++)
                {
                    var handle = e.Questions[i];
                    var bModel = bank.FindQuestion(handle.Question);
                    if (bModel == null)
                        continue;

                    var qModel = AttemptStarter.CreateQuestion(bModel, handle, language);
                    var qState = attemptState.Questions.FirstOrDefault(x => x.QuestionIdentifier == handle.Question);
                    var question = CreateQuestion(attempt, qModel, qState, i + 1);

                    attempt.Questions.Add(question);
                    attempt.FormPoints += question.QuestionPoints;
                }

                db.QAttempts.Add(attempt);

                summary.OnInsert(attempt);

                db.SaveChanges();
            }
        }

        private static QAttempt CreateAttempt(AttemptStarted2 e, AttemptState state, AttemptSummary summary)
        {
            var result = new QAttempt
            {
                AssessorUserIdentifier = e.AssessorUserIdentifier,
                LearnerUserIdentifier = e.LearnerUserIdentifier,
                AttemptIdentifier = e.AggregateIdentifier,
                FormIdentifier = e.FormIdentifier,
                RegistrationIdentifier = e.RegistrationIdentifier,
                OrganizationIdentifier = e.OrganizationIdentifier,
                AttemptNumber = summary.Entity.AttemptTotalCount + 1,
                AttemptStatus = "Started",
                AttemptTimeLimit = e.TimeLimit,
                AttemptLanguage = e.Language,
                FormPoints = 0,
                UserAgent = e.UserAgent,
                SectionsAsTabsEnabled = e.SectionsAsTabsEnabled,
                TabNavigationEnabled = e.TabNavigationEnabled,
                SingleQuestionPerTabEnabled = e.SingleQuestionPerTabEnabled,
                FormSectionsCount = e.FormSectionsCount,
                ActiveSectionIndex = e.ActiveSectionIndex,
                ActiveQuestionIndex = e.ActiveQuestionIndex
            };

            UpdateAttemptTime(result, state);

            return result;
        }

        public void InsertAttempt(AttemptStarted3 e)
        {
            var attemptState = (AttemptState)e.AggregateState;

            using (var db = CreateContext())
            {
                var summary = new AttemptSummary(db, e.FormIdentifier, e.LearnerUserIdentifier);
                var attempt = CreateAttempt(e, attemptState, summary);
                var sections = CreateSections(attempt, attemptState);

                var hasSections = sections.Length > 0;

                for (var i = 0; i < e.Questions.Length; i++)
                {
                    var question = e.Questions[i];
                    var questionState = attemptState.Questions.FirstOrDefault(x => x.QuestionIdentifier == question.Identifier);
                    var entity = CreateQuestion(attempt, question, questionState, i + 1);

                    attempt.Questions.Add(entity);
                    attempt.FormPoints += entity.QuestionPoints;
                }

                db.QAttempts.Add(attempt);

                summary.OnInsert(attempt);

                db.SaveChanges();
            }
        }

        private static QAttempt CreateAttempt(AttemptStarted3 e, AttemptState state, AttemptSummary summary)
        {
            var result = new QAttempt
            {
                AttemptIdentifier = e.AggregateIdentifier,
                AssessorUserIdentifier = e.AssessorUserIdentifier,
                LearnerUserIdentifier = e.LearnerUserIdentifier,

                FormIdentifier = e.FormIdentifier,
                RegistrationIdentifier = e.RegistrationIdentifier,
                OrganizationIdentifier = e.OrganizationIdentifier,
                AttemptNumber = summary.Entity.AttemptTotalCount + 1,
                AttemptStatus = "Started",
                AttemptPingInterval = e.Configuration.PingInterval,
                AttemptTimeLimit = e.Configuration.TimeLimit,
                AttemptLanguage = e.Configuration.Language,
                FormPoints = 0,
                UserAgent = e.UserAgent,
                SectionsAsTabsEnabled = e.Configuration.SectionsAsTabs,
                TabNavigationEnabled = e.Configuration.TabNavigation,
                SingleQuestionPerTabEnabled = e.Configuration.SingleQuestionPerTab,
                ActiveSectionIndex = state.ActiveSectionIndex,
                ActiveQuestionIndex = state.ActiveQuestionIndex
            };

            UpdateAttemptTime(result, state);

            return result;
        }

        private static QAttemptSection[] CreateSections(QAttempt attempt, AttemptState state)
        {
            if (!attempt.SectionsAsTabsEnabled || attempt.TabNavigationEnabled)
                return new QAttemptSection[0];

            var result = new List<QAttemptSection>();

            attempt.FormSectionsCount = state.Sections.Length;
            attempt.TabTimeLimit = state.Configuration.TabTimeLimit.GetName(SpecificationTabTimeLimit.Disabled);

            var isSomeTabLimit = state.Configuration.TabTimeLimit == SpecificationTabTimeLimit.SomeTabs;
            var isAllTabLimit = state.Configuration.TabTimeLimit == SpecificationTabTimeLimit.AllTabs;

            for (var i = 0; i < state.Sections.Length; i++)
            {
                var s = state.Sections[i];
                var entity = new QAttemptSection
                {
                    SectionIndex = s.Index,
                    SectionIdentifier = s.Identifier,
                    ShowWarningNextTab = s.ShowWarningNextTab,
                    IsBreakTimer = s.IsBreakTimer,
                };

                UpdateSectionTime(entity, s);

                if (isAllTabLimit || isSomeTabLimit && entity.IsBreakTimer)
                {
                    entity.TimerType = s.TimerType.GetName();

                    if (s.TimeLimit > 0)
                        entity.TimeLimit = s.TimeLimit;
                }

                attempt.Sections.Add(entity);
                result.Add(entity);
            }

            return result.ToArray();
        }

        private static QAttemptQuestion CreateQuestion(QAttempt attempt, AttemptQuestion model, AttemptQuestionState state, int number)
        {
            var question = new QAttemptQuestion
            {
                QuestionIdentifier = model.Identifier,
                QuestionText = model.Text,
                QuestionPoints = model.Points,
                QuestionSequence = state.QuestionIndex + 1,
                QuestionNumber = number,
                QuestionType = model.Type.GetName(),
                QuestionCutScore = model.CutScore,
                QuestionCalculationMethod = model.CalculationMethod.GetName(),
                AnswerPoints = 0,
                SectionIndex = state.SectionIndex
            };

            if (model.Type == QuestionItemType.ComposedVoice)
                BindEntityComposedVoice((AttemptQuestionComposedVoice)model, question);
            else if (model.Type.IsComposed())
                BindEntityComposed((AttemptQuestionComposed)model, question);
            else if (model.Type == QuestionItemType.Matching)
                BindEntityMatching(attempt, question, (AttemptQuestionMatch)model);
            else if (model.Type == QuestionItemType.Likert)
                BindEntityLikert(attempt, question, (AttemptQuestionLikert)model);
            else if (model.Type.IsHotspot())
                BindEntityHotspot(attempt, question, (AttemptQuestionHotspot)model);
            else if (model.Type == QuestionItemType.Ordering)
                BindEntityOrdering(attempt, question, (AttemptQuestionOrdering)model);
            else if (model.Type.IsCheckList())
            {
                var options = BindEntityDefault(attempt, question, (AttemptQuestionDefault)model);
                CalculateAnswerPoints(question, options);
            }
            else
                BindEntityDefault(attempt, question, (AttemptQuestionDefault)model);

            return question;
        }

        private static void BindEntityComposed(AttemptQuestionComposed model, QAttemptQuestion question)
        {
            // When QuestionPoints == -1 this means that the attempt was started without rubric assigned to the question
            question.QuestionPoints = model.Rubric?.Points ?? -1;
        }

        private static void BindEntityComposedVoice(AttemptQuestionComposedVoice model, QAttemptQuestion question)
        {
            BindEntityComposed(model, question);

            if (model.TimeLimit > 0)
                question.AnswerTimeLimit = model.TimeLimit;

            if (model.AttemptLimit > 0)
            {
                question.AnswerAttemptLimit = model.AttemptLimit;
                question.AnswerRequestAttempt = 0;
                question.AnswerSubmitAttempt = 0;
            }
        }

        private static void BindEntityMatching(QAttempt attempt, QAttemptQuestion question, AttemptQuestionMatch model)
        {
            for (var i = 0; i < model.Pairs.Length; i++)
            {
                var pair = model.Pairs[i];

                attempt.Matches.Add(new QAttemptMatch
                {
                    QuestionIdentifier = question.QuestionIdentifier,
                    QuestionSequence = question.QuestionSequence,
                    MatchSequence = i + 1,
                    MatchLeftText = pair.LeftText,
                    MatchRightText = pair.RightText,
                    MatchPoints = pair.Points,
                });
            }

            question.SetMatchDistractors(model.Distractors);
        }

        private static void BindEntityLikert(QAttempt attempt, QAttemptQuestion question, AttemptQuestionLikert model)
        {
            var number = 1;

            foreach (var q in model.Questions)
            {
                var subQuestion = new QAttemptQuestion
                {
                    QuestionIdentifier = q.Identifier,
                    ParentQuestionIdentifier = question.QuestionIdentifier,
                    QuestionSequence = question.QuestionSequence + number,
                    QuestionNumber = number,
                    QuestionText = q.Text,
                    QuestionPoints = q.Points,
                    QuestionType = QuestionItemType.SingleCorrect.GetName(),
                    QuestionCutScore = question.QuestionCutScore,
                    QuestionCalculationMethod = question.QuestionCalculationMethod,
                    AnswerPoints = 0,
                };

                for (var i = 0; i < q.Options.Length; i++)
                {
                    var o = q.Options[i];

                    attempt.Options.Add(new QAttemptOption
                    {
                        QuestionIdentifier = q.Identifier,
                        QuestionSequence = subQuestion.QuestionSequence,
                        OptionKey = o.Key,
                        OptionSequence = i + 1,
                        OptionPoints = o.Points,
                        OptionText = o.Text
                    });
                }

                attempt.Questions.Add(subQuestion);
                number++;
            }
        }

        private static void BindEntityHotspot(QAttempt attempt, QAttemptQuestion question, AttemptQuestionHotspot model)
        {
            question.PinLimit = model.PinLimit;
            question.ShowShapes = model.ShowShapes;
            question.HotspotImage = model.Image;

            for (var i = 0; i < model.Options.Length; i++)
            {
                var o = model.Options[i];

                attempt.Options.Add(new QAttemptOption
                {
                    QuestionIdentifier = question.QuestionIdentifier,
                    QuestionSequence = question.QuestionSequence,
                    OptionKey = o.Key,
                    OptionSequence = i + 1,
                    OptionPoints = o.Points,
                    OptionText = o.Text,
                    OptionShape = o.Shape.ToString()
                });
            }
        }

        private static void BindEntityOrdering(QAttempt attempt, QAttemptQuestion question, AttemptQuestionOrdering model)
        {
            question.QuestionTopLabel = model.TopLabel;
            question.QuestionBottomLabel = model.BottomLabel;

            var options = new List<QAttemptOption>();
            for (var i = 0; i < model.Options.Length; i++)
            {
                var option = model.Options[i];
                var entity = new QAttemptOption
                {
                    QuestionIdentifier = question.QuestionIdentifier,
                    QuestionSequence = question.QuestionSequence,
                    OptionKey = option.Key,
                    OptionSequence = i + 1,
                    OptionText = option.Text
                };

                attempt.Options.Add(entity);
                options.Add(entity);
            }

            var solutions = new List<QAttemptSolution>();
            for (var i = 0; i < model.Solutions.Length; i++)
            {
                var solution = model.Solutions[i];
                var entity = new QAttemptSolution
                {
                    QuestionIdentifier = question.QuestionIdentifier,
                    QuestionSequence = question.QuestionSequence,
                    SolutionIdentifier = solution.Identifier,
                    SolutionSequence = i + 1,
                    SolutionOptionsOrder = string.Join(",", solution.OptionsOrder),
                    SolutionPoints = solution.Points,
                    SolutionCutScore = solution.CutScore
                };

                attempt.Solutions.Add(entity);
                solutions.Add(entity);
            }

            var defaultOrder = options.Select(x => x.OptionKey).ToArray();
            if (AnswerSolutions(question, solutions, defaultOrder, out var defaultSolution))
                AnswerOrdering(question, options, defaultSolution, defaultOrder);
        }

        private static QAttemptOption[] BindEntityDefault(QAttempt attempt, QAttemptQuestion question, AttemptQuestionDefault model)
        {
            var result = new List<QAttemptOption>();

            for (var i = 0; i < model.Options.Length; i++)
            {
                var o = model.Options[i];

                var option = new QAttemptOption
                {
                    QuestionIdentifier = question.QuestionIdentifier,
                    QuestionSequence = question.QuestionSequence,
                    OptionKey = o.Key,
                    OptionSequence = i + 1,
                    OptionPoints = o.Points,
                    OptionCutScore = o.CutScore,
                    OptionText = o.Text,
                    OptionIsTrue = o.IsTrue
                };

                result.Add(option);
                attempt.Options.Add(option);
            }

            return result.ToArray();
        }

        #endregion

        #region Methods (update: exam)

        public void UpdateAttempt(AttemptPinged e)
        {
            var state = (AttemptState)e.AggregateState;

            using (var db = CreateContext())
            {
                UpdateAttemptTime(db, state);
                UpdateActiveSectionTime(db, state);

                db.SaveChanges();
            }
        }

        public void UpdateAttempt(AttemptResumed e)
        {
            var state = (AttemptState)e.AggregateState;

            using (var db = CreateContext())
            {
                UpdateAttemptTime(db, state);
                UpdateActiveSectionTime(db, state);

                db.SaveChanges();
            }
        }

        public void UpdateAttempt(AttemptSubmitted e)
        {
            var state = (AttemptState)e.AggregateState;

            using (var db = CreateContext())
            {
                var attempt = db.QAttempts.FirstOrDefault(x => x.AttemptIdentifier == e.AggregateIdentifier);
                if (attempt == null)
                    return;

                var summary = new AttemptSummary(db, attempt.FormIdentifier, attempt.LearnerUserIdentifier);
                summary.OnBeforeUpdate(attempt);

                UpdateAttemptScore(attempt);
                UpdateAttemptTime(attempt, state);
                UpdateAllSectionsTime(db, state);

                attempt.AttemptStatus = "Submitted";

                if (e.Grade)
                    attempt.AttemptGraded = e.ChangeTime;

                if (e.UserAgent.IsNotEmpty())
                    attempt.UserAgent = e.UserAgent;

                summary.OnAfterUpdate(attempt);

                db.SaveChanges();
            }
        }

        public void UpdateAttempt(AttemptCommentPosted e)
        {
            var state = (AttemptState)e.AggregateState;

            using (var db = CreateContext())
            {
                var comment = db.QComments
                    .SingleOrDefault(x => x.AssessmentAttemptIdentifier == e.AggregateIdentifier
                                       && x.AssessmentQuestionIdentifier == e.Question
                                       && x.AuthorUserIdentifier == e.OriginUser);

                if (!string.IsNullOrWhiteSpace(e.Comment))
                {
                    if (comment == null)
                    {
                        var attempt = db.QAttempts.FirstOrDefault(x => x.AttemptIdentifier == e.AggregateIdentifier)
                            ?? throw new ArgumentException($"Attempt or the related form does not exist: {e.AggregateIdentifier}");

                        var bankId = state.BankIdentifier;
                        if (!bankId.HasValue)
                        {
                            bankId = db.BankForms
                                .Where(x => x.FormIdentifier == attempt.FormIdentifier)
                                .Select(x => (Guid?)x.BankIdentifier)
                                .FirstOrDefault();
                        }

                        comment = new QComment
                        {
                            ContainerIdentifier = e.AggregateIdentifier,
                            ContainerType = "Assessment Attempt",
                            ContainerSubtype = "Question",
                            AuthorUserIdentifier = e.OriginUser,
                            AuthorUserRole = "Candidate",
                            CommentIdentifier = UniqueIdentifier.Create(),
                            AssessmentAttemptIdentifier = e.AggregateIdentifier,
                            AssessmentBankIdentifier = bankId,
                            AssessmentQuestionIdentifier = e.Question,
                            OrganizationIdentifier = attempt.OrganizationIdentifier,
                            TimestampCreated = e.ChangeTime,
                            TimestampCreatedBy = e.OriginUser
                        };

                        db.QComments.Add(comment);
                    }

                    comment.CommentPosted = e.ChangeTime;
                    comment.CommentText = e.Comment;
                    comment.OriginText = e.Comment;
                }
                else
                {
                    if (comment != null)
                        db.QComments.Remove(comment);
                }

                db.SaveChanges();
            }
        }

        public void UpdateAttempt(ComposedQuestionAnswered e)
        {
            using (var db = CreateContext())
            {
                if (!TryGetQuestion(db, e.AggregateIdentifier, e.Question, out var question))
                    return;

                var qType = question.QuestionType.ToEnum<QuestionItemType>();

                question.ClearAnswer();

                if (qType == QuestionItemType.ComposedVoice)
                {
                    var isValid = true;

                    if (question.AnswerAttemptLimit.HasValue)
                    {
                        if (!question.AnswerRequestAttempt.HasValue || !question.AnswerSubmitAttempt.HasValue)
                            isValid = false;
                        else if (question.AnswerSubmitAttempt.Value >= question.AnswerRequestAttempt.Value)
                            isValid = false;
                        else
                            question.AnswerSubmitAttempt = question.AnswerRequestAttempt;
                    }

                    if (isValid)
                        question.AnswerFileIdentifier = Guid.TryParse(e.Answer, out var value) ? value : (Guid?)null;
                }
                else if (qType == QuestionItemType.ComposedEssay)
                    question.AnswerText = e.Answer;
                else
                    return;

                db.SaveChanges();
            }
        }

        public void UpdateAttempt(ComposedQuestionAttemptStarted e)
        {
            using (var db = CreateContext())
            {
                if (!TryGetQuestion(db, e.AggregateIdentifier, e.Question, QuestionItemType.ComposedVoice, out var question))
                    return;

                if (!question.AnswerAttemptLimit.HasValue || !question.AnswerRequestAttempt.HasValue || !question.AnswerSubmitAttempt.HasValue)
                    return;

                if (question.AnswerRequestAttempt.Value < question.AnswerAttemptLimit.Value)
                {
                    if (question.AnswerSubmitAttempt < question.AnswerRequestAttempt)
                        question.AnswerSubmitAttempt = question.AnswerRequestAttempt;

                    question.AnswerRequestAttempt += 1;
                }
                else if (question.AnswerSubmitAttempt < question.AnswerRequestAttempt)
                {
                    question.AnswerSubmitAttempt = question.AnswerRequestAttempt;
                }
                else
                {
                    return;
                }

                db.SaveChanges();
            }
        }

        public void UpdateAttempt(MatchingQuestionAnswered e)
        {
            using (var db = CreateContext())
            {
                if (!TryGetQuestion(db, e.AggregateIdentifier, e.Question, QuestionItemType.Matching, out var question))
                    return;

                question.ClearAnswer();

                var matches = db.QAttemptMatches.Where(x => x.AttemptIdentifier == e.AggregateIdentifier && x.QuestionIdentifier == e.Question).ToArray();
                foreach (var m in matches)
                    m.AnswerText = e.Matches.ContainsKey(m.MatchSequence) ? e.Matches[m.MatchSequence] : null;

                CalculateAnswerPoints(question, matches);

                db.SaveChanges();
            }
        }

        public void UpdateAttempt(MultipleChoiceQuestionAnswered e)
        {
            using (var db = CreateContext())
            {
                if (!TryGetQuestion(db, e.AggregateIdentifier, e.Question, QuestionItemType.SingleCorrect, out var question))
                    return;

                AnswerOptions(db, question, true, o => e.Option == o.OptionKey);

                if (question.ParentQuestionIdentifier.HasValue)
                    UpdateAttemptQuestionParent(db, question);

                db.SaveChanges();
            }
        }

        private static void UpdateAttemptQuestionParent(InternalDbContext db, QAttemptQuestion child)
        {
            var parent = db.QAttemptQuestions.First(
                x => x.AttemptIdentifier == child.AttemptIdentifier
                  && x.QuestionIdentifier == child.ParentQuestionIdentifier.Value);

            var points = db.QAttemptQuestions
                .Where(
                    x => x.AttemptIdentifier == parent.AttemptIdentifier
                      && x.ParentQuestionIdentifier == parent.QuestionIdentifier
                      && x.QuestionIdentifier != child.QuestionIdentifier
                      && x.AnswerPoints.HasValue)
                .Sum(x => x.AnswerPoints);

            parent.AnswerPoints = (points ?? 0) + (child.AnswerPoints ?? 0);
        }

        public void UpdateAttempt(MultipleCorrectQuestionAnswered e)
        {
            using (var db = CreateContext())
            {
                if (!TryGetQuestion(db, e.AggregateIdentifier, e.Question, QuestionItemType.MultipleCorrect, out var question))
                    return;

                AnswerOptions(db, question, false, o => e.Options.Contains(o.OptionKey));

                db.SaveChanges();
            }
        }

        public void UpdateAttempt(BooleanTableQuestionAnswered e)
        {
            using (var db = CreateContext())
            {
                if (!TryGetQuestion(db, e.AggregateIdentifier, e.Question, QuestionItemType.BooleanTable, out var question))
                    return;

                AnswerOptions(db, question, false, o => e.Options.ContainsKey(o.OptionKey) ? e.Options[o.OptionKey] : (bool?)null);

                db.SaveChanges();
            }
        }

        public void UpdateAttempt(TrueOrFalseQuestionAnswered e)
        {
            using (var db = CreateContext())
            {
                if (!TryGetQuestion(db, e.AggregateIdentifier, e.Question, QuestionItemType.TrueOrFalse, out var question))
                    return;

                AnswerOptions(db, question, true, o => e.Option == o.OptionKey);

                db.SaveChanges();
            }
        }

        public void UpdateAttempt(HotspotQuestionAnswered e)
        {
            using (var db = CreateContext())
            {
                if (!TryGetQuestion(db, e.AggregateIdentifier, e.Question, out var question))
                    return;

                var qType = question.QuestionType.ToEnum<QuestionItemType>();
                if (!qType.IsHotspot())
                    return;

                if (!AnswerPins(db, question, e.Pins, out var allPins, out var newPins))
                    return;

                AnswerOptions(db, question, question.PinLimit == 1, o =>
                {
                    var shape = HotspotShape.FromString(o.OptionShape);

                    foreach (var newPin in newPins)
                    {
                        if (newPin.HasCoordinates && shape.IsPointInside(newPin.PinX, newPin.PinY))
                        {
                            newPin.OptionKey = o.OptionKey;
                            newPin.OptionPoints = o.OptionPoints;
                            newPin.OptionSequence = o.OptionSequence;
                            newPin.OptionText = o.OptionText;
                        }
                    }

                    return allPins.Any(x => x.OptionKey == o.OptionKey);
                });

                db.SaveChanges();
            }
        }

        public void UpdateAttempt(OrderingQuestionAnswered e)
        {
            using (var db = CreateContext())
            {
                if (!TryGetQuestion(db, e.AggregateIdentifier, e.Question, QuestionItemType.Ordering, out var question))
                    return;

                var options = db.QAttemptOptions
                    .Where(x => x.AttemptIdentifier == e.AggregateIdentifier && x.QuestionIdentifier == e.Question)
                    .ToArray();

                if (e.OptionsOrder.Length != options.Length || options.Any(x => !e.OptionsOrder.Contains(x.OptionKey)))
                    return;

                if (!AnswerSolutions(db, question, e.OptionsOrder, out var solution))
                    return;

                AnswerOrdering(question, options, solution, e.OptionsOrder);

                db.SaveChanges();
            }
        }

        public void UpdateAttempt(AttemptSectionSwitched e)
        {
            var state = (AttemptState)e.AggregateState;

            using (var db = CreateContext())
            {
                var attempt = db.QAttempts.First(x => x.AttemptIdentifier == e.AggregateIdentifier);

                attempt.ActiveSectionIndex = state.ActiveSectionIndex;

                UpdateAttemptTime(attempt, state);
                UpdateAllSectionsTime(db, state);

                db.SaveChanges();
            }
        }

        public void UpdateAttempt(AttemptQuestionSwitched e)
        {
            var state = (AttemptState)e.AggregateState;

            using (var db = CreateContext())
            {
                var attempt = db.QAttempts.First(x => x.AttemptIdentifier == e.AggregateIdentifier);

                attempt.ActiveSectionIndex = state.ActiveSectionIndex;
                attempt.ActiveQuestionIndex = state.ActiveQuestionIndex;

                UpdateAttemptTime(attempt, state);
                UpdateAllSectionsTime(db, state);

                db.SaveChanges();
            }
        }

        private static void UpdateAttemptTime(InternalDbContext db, AttemptState state)
        {
            var attempt = db.QAttempts.First(x => x.AttemptIdentifier == state.Identifier);

            var summary = new AttemptSummary(db, attempt.FormIdentifier, attempt.LearnerUserIdentifier);
            summary.OnBeforeUpdate(attempt);

            UpdateAttemptTime(attempt, state);

            summary.OnAfterUpdate(attempt);
        }

        private static void UpdateAttemptTime(QAttempt attempt, AttemptState state)
        {
            attempt.AttemptStarted = state.Started;
            attempt.AttemptPinged = state.Pinged;
            attempt.AttemptSubmitted = state.Submitted;
            attempt.AttemptDuration = state.Duration;
        }

        private static void UpdateActiveSectionTime(InternalDbContext db, AttemptState state)
        {
            if (state.Configuration.SectionsAsTabs && !state.Configuration.TabNavigation)
                UpdateSectionTime(db, state, state.ActiveSectionIndex.Value);
        }

        private static void UpdateAllSectionsTime(InternalDbContext db, AttemptState state)
        {
            if (!state.Configuration.SectionsAsTabs || state.Configuration.TabNavigation)
                return;

            var entities = db.QAttemptSections.Where(x => x.AttemptIdentifier == state.Identifier).ToArray();
            foreach (var sectionEntity in entities)
            {
                var sectionState = state.Sections[sectionEntity.SectionIndex];

                UpdateSectionTime(sectionEntity, sectionState);
            }
        }

        private static void UpdateSectionTime(InternalDbContext db, AttemptState state, int index)
        {
            var sectionEntity = db.QAttemptSections.First(x => x.AttemptIdentifier == state.Identifier
                                                            && x.SectionIndex == index);
            var sectionState = state.Sections[index];

            UpdateSectionTime(sectionEntity, sectionState);
        }

        private static void UpdateSectionTime(QAttemptSection section, AttemptSectionState state)
        {
            section.SectionStarted = state.Started;
            section.SectionCompleted = state.Completed;
            section.SectionDuration = state.Duration;
        }

        private static bool TryGetQuestion(InternalDbContext db, Guid attemptId, Guid questionId, out QAttemptQuestion question)
        {
            question = db.QAttemptQuestions
                .First(x => x.AttemptIdentifier == attemptId && x.QuestionIdentifier == questionId);

            return question != null;
        }

        private static bool TryGetQuestion(InternalDbContext db, Guid attemptId, Guid questionId, QuestionItemType validType, out QAttemptQuestion question)
        {
            return TryGetQuestion(db, attemptId, questionId, out question) && question.QuestionType.ToEnum<QuestionItemType>() == validType;
        }

        private static void AnswerOptions(InternalDbContext db, QAttemptQuestion question, bool isSingleAnswer, Func<QAttemptOption, bool?> isAnswered)
        {
            question.ClearAnswer();

            var options = db.QAttemptOptions
                .Where(x => x.AttemptIdentifier == question.AttemptIdentifier && x.QuestionIdentifier == question.QuestionIdentifier)
                .OrderBy(x => x.OptionSequence)
                .ToArray();

            foreach (var o in options)
            {
                o.AnswerIsSelected = isAnswered(o);

                if (isSingleAnswer && o.AnswerIsSelected == true)
                {
                    question.AnswerOptionKey = o.OptionKey;
                    question.AnswerOptionSequence = o.OptionSequence;
                }
            }

            CalculateAnswerPoints(question, options);
        }

        private static bool AnswerPins(
            InternalDbContext db,
            QAttemptQuestion question,
            AttemptHotspotPinAnswer[] inputPins,
            out List<QAttemptPin> allPins,
            out List<QAttemptPin> newPins)
        {
            allPins = null;
            newPins = null;

            var image = HotspotImage.FromString(question.HotspotImage);
            if (image.IsEmpty)
                return false;

            allPins = db.QAttemptPins
                .Where(p => p.AttemptIdentifier == question.AttemptIdentifier && p.QuestionIdentifier == question.QuestionIdentifier)
                .OrderBy(p => p.PinSequence)
                .ToList();
            newPins = inputPins
                .Skip(inputPins.Length > question.PinLimit.Value ? inputPins.Length - question.PinLimit.Value : 0)
                .Select(p => new QAttemptPin(question)
                {
                    PinX = (int)Math.Round(p.X * image.Width, MidpointRounding.AwayFromZero),
                    PinY = (int)Math.Round(p.Y * image.Height, MidpointRounding.AwayFromZero)
                })
                .ToList();

            var matchLen = allPins.Count < newPins.Count ? allPins.Count : newPins.Count;

            while (matchLen > 0)
            {
                var isMatch = true;
                var startIndex = allPins.Count - matchLen;

                for (var i = 0; i < matchLen; i++)
                {
                    var pin1 = allPins[startIndex + i];
                    var pin2 = newPins[i];

                    if (pin1.PinX != pin2.PinX || pin1.PinY != pin2.PinY)
                    {
                        isMatch = false;
                        break;
                    }
                }

                if (isMatch)
                    break;

                matchLen--;
            }

            newPins = newPins.Skip(matchLen).ToList();

            var nextSequence = allPins.Count == 0 ? 1 : allPins.Max(x => x.PinSequence) + 1;
            foreach (var pin in newPins)
            {
                pin.PinSequence = nextSequence++;
                db.QAttemptPins.Add(pin);
                allPins.Add(pin);
            }

            if (allPins.Count > question.PinLimit.Value)
            {
                db.QAttemptPins.RemoveRange(allPins.Take(allPins.Count - question.PinLimit.Value));
                allPins.RemoveRange(0, allPins.Count - question.PinLimit.Value);
            }

            return true;
        }

        private static bool AnswerSolutions(
            InternalDbContext db,
            QAttemptQuestion question,
            int[] inputOrder,
            out QAttemptSolution solution)
        {
            var solutions = db.QAttemptSolutions
                .Where(p => p.AttemptIdentifier == question.AttemptIdentifier && p.QuestionIdentifier == question.QuestionIdentifier)
                .OrderBy(p => p.SolutionSequence)
                .ToArray();

            return AnswerSolutions(question, solutions, inputOrder, out solution);
        }

        private static bool AnswerSolutions(
            QAttemptQuestion question,
            IEnumerable<QAttemptSolution> solutions,
            int[] inputOrder,
            out QAttemptSolution solution)
        {
            solution = null;

            foreach (var s in solutions)
            {
                var isMatch = solution == null
                    && s.SolutionOptionsOrder.Split(',').Select(x => int.Parse(x)).SequenceEqual(inputOrder);

                if (isMatch)
                {
                    if (s.AnswerIsMatched)
                        return false;

                    solution = s;
                }

                s.AnswerIsMatched = isMatch;
            }

            return true;
        }

        private static void AnswerOrdering(QAttemptQuestion question, IEnumerable<QAttemptOption> options, QAttemptSolution solution, int[] optionsOrder)
        {
            question.ClearAnswer();

            for (var i = 0; i < optionsOrder.Length; i++)
            {
                var number = optionsOrder[i];
                var option = options.First(x => x.OptionKey == number);

                option.OptionAnswerSequence = i + 1;
            }

            if (solution != null)
            {
                question.AnswerSolutionIdentifier = solution.SolutionIdentifier;
                question.AnswerPoints = solution.SolutionPoints;
            }
        }

        #endregion

        #region Attempts

        public void DeleteAttempt(AttemptVoided e)
        {
            DeleteAttempt(e.AggregateIdentifier);
        }

        public void UpdateAttempt(AttemptGraded e)
        {
            var state = (AttemptState)e.AggregateState;

            using (var db = CreateContext())
            {
                var attempt = db.QAttempts.FirstOrDefault(x => x.AttemptIdentifier == e.AggregateIdentifier);
                if (attempt == null)
                    return;

                var summary = new AttemptSummary(db, attempt.FormIdentifier, attempt.LearnerUserIdentifier);
                summary.OnBeforeUpdate(attempt);

                UpdateAttemptScore(attempt);
                UpdateAttemptTime(attempt, state);

                attempt.AttemptStatus = "Graded";
                attempt.AttemptGraded = e.ChangeTime;

                if (e.UserAgent.IsNotEmpty())
                    attempt.UserAgent = e.UserAgent;

                summary.OnAfterUpdate(attempt);

                UpdateAllSectionsTime(db, state);

                db.SaveChanges();
            }
        }

        public void UpdateAttempt(ScoreCalculated e)
        {
            using (var db = CreateContext())
            {
                var attempt = db.QAttempts.First(x => x.AttemptIdentifier == e.AggregateIdentifier);

                var summary = new AttemptSummary(db, attempt.FormIdentifier, attempt.LearnerUserIdentifier);
                summary.OnBeforeUpdate(attempt);

                attempt.AttemptPoints = e.Points;
                attempt.AttemptScore = Math.Round(e.Score, 8, MidpointRounding.AwayFromZero);
                attempt.AttemptIsPassing = e.IsPassing;

                summary.OnAfterUpdate(attempt);

                db.SaveChanges();
            }
        }

        public void UpdateAttempt(AttemptAnalyzed e)
        {
            using (var db = CreateContext())
            {
                // Update denormalized attributes for the attempt.
                var denormalize = "exec assessments.DenormalizeAttempt @Aggregate";
                db.Database.ExecuteSqlCommand(denormalize, new SqlParameter("Aggregate", e.AggregateIdentifier));
                db.SaveChanges();
            }
        }

        public void UpdateAttempt(AttemptFixed e)
        {
            using (var db = CreateContext())
            {
                var attempt = db.QAttempts.First(x => x.AttemptIdentifier == e.AggregateIdentifier);

                if (e.Registration.HasValue)
                    attempt.RegistrationIdentifier = e.Registration;

                db.SaveChanges();
            }
        }

        public void UpdateAttempt(AttemptTagged e)
        {
            using (var db = CreateContext())
            {
                var attempt = db.QAttempts.First(x => x.AttemptIdentifier == e.AggregateIdentifier);

                attempt.AttemptTag = e.Tag;

                db.SaveChanges();
            }
        }

        public void UpdateAttempt(QuestionVoided e)
        {
            // Remove the question from the attempt.

            using (var db = CreateContext())
            {
                var question = db.BankQuestions
                    .FirstOrDefault(x => x.QuestionIdentifier == e.Question);

                var answer = db.QAttemptQuestions
                    .FirstOrDefault(x => x.AttemptIdentifier == e.AggregateIdentifier && x.QuestionIdentifier == e.Question);

                if (question == null || answer == null)
                    return;

                db.QAttemptQuestions.RemoveRange(
                    db.QAttemptQuestions
                    .Where(x => x.AttemptIdentifier == e.AggregateIdentifier && x.QuestionIdentifier == e.Question));

                db.QAttemptOptions.RemoveRange(
                    db.QAttemptOptions
                    .Where(x => x.AttemptIdentifier == e.AggregateIdentifier && x.QuestionIdentifier == e.Question));

                var attempt = db.QAttempts.FirstOrDefault(x => x.AttemptIdentifier == e.AggregateIdentifier);
                if (attempt != null && question != null)
                {
                    var summary = new AttemptSummary(db, attempt.FormIdentifier, attempt.LearnerUserIdentifier);
                    summary.OnBeforeUpdate(attempt);

                    AdjustScore(attempt, answer.QuestionPoints ?? 0, answer.AnswerPoints);

                    summary.OnAfterUpdate(attempt);
                }

                db.QAttemptQuestions.Remove(answer);

                db.SaveChanges();
            }
        }

        public void UpdateAttempt(ComposedQuestionScored e)
        {
            using (var db = CreateContext())
            {
                var question = db.QAttemptQuestions.First(x => x.AttemptIdentifier == e.AggregateIdentifier && x.QuestionIdentifier == e.Question);

                if (!IsComposed(question.QuestionType))
                    return;

                var summary = new AttemptSummary(db, question.Attempt.FormIdentifier, question.Attempt.LearnerUserIdentifier);
                summary.OnBeforeUpdate(question.Attempt);

                question.AnswerPoints = e.RubricRatingPoints.Values.Sum(x => x);

                question.RubricRatingPoints = JsonConvert.SerializeObject(e.RubricRatingPoints);
                if (question.RubricRatingPoints.Length > 512)
                    question.RubricRatingPoints = null;

                summary.OnAfterUpdate(question.Attempt);

                db.SaveChanges();
            }
        }

        public void UpdateAttempt(AttemptGradedDateChanged e)
        {
            using (var db = CreateContext())
            {
                var attempt = db.QAttempts.First(x => x.AttemptIdentifier == e.AggregateIdentifier);

                var summary = new AttemptSummary(db, attempt.FormIdentifier, attempt.LearnerUserIdentifier);
                summary.OnBeforeUpdate(attempt);

                if (attempt.AttemptImported.HasValue)
                    attempt.AttemptGraded = e.Completed;

                if (attempt.AttemptGraded.HasValue && attempt.AttemptGraded.Value < e.Completed)
                    attempt.AttemptGraded = e.Completed;

                summary.OnAfterUpdate(attempt);

                db.SaveChanges();
            }
        }

        public void UpdateAttempt(QuestionRegraded e, Form form)
        {
            var questionData = form.Specification.Bank.FindQuestion(e.Question);
            if (questionData.Type == QuestionItemType.Matching || questionData.Type.IsComposed())
                return;

            using (var db = CreateContext())
            {
                var attempt = db.QAttempts.First(x => x.AttemptIdentifier == e.AggregateIdentifier);

                var summary = new AttemptSummary(db, attempt.FormIdentifier, attempt.LearnerUserIdentifier);
                summary.OnBeforeUpdate(attempt);

                var options = db.QAttemptOptions
                    .AsNoTracking()
                    .Where(x => x.AttemptIdentifier == e.AggregateIdentifier && x.QuestionIdentifier == e.Question)
                    .ToList();

                var question = db.QAttemptQuestions.FirstOrDefault(x => x.AttemptIdentifier == e.AggregateIdentifier && x.QuestionIdentifier == e.Question);
                if (question == null)
                    return;

                if (questionData.Type == QuestionItemType.SingleCorrect)
                    RegradeSingleCorrectOptions(e, questionData, question, options);
                else
                    RegradeMultiCorrectOptions(e, questionData, question, options);

                UpdateAttemptScore(attempt);

                db.Entry(question).State = EntityState.Modified;
                db.Entry(attempt).State = EntityState.Modified;

                summary.OnAfterUpdate(attempt);

                db.SaveChanges();
            }
        }

        public void UpdateAttempt(AttemptGradingAssessorAssigned e)
        {
            using (var db = CreateContext())
            {
                var attempt = db.QAttempts.First(x => x.AttemptIdentifier == e.AggregateIdentifier);

                attempt.GradingAssessorUserIdentifier = e.GradingAssessor;

                db.SaveChanges();
            }
        }

        public void UpdateAttempt(AttemptRubricPointsUpdated e)
        {
            UpdateAttemptRubricPoints((AttemptState)e.AggregateState, e.QuestionIds, e.RubricPoints);
        }

        public void UpdateAttempt(AttemptRubricChanged e)
        {
            UpdateAttemptRubricPoints((AttemptState)e.AggregateState, e.QuestionIds, e.NewRubricPoints);
        }

        public void UpdateAttempt(AttemptQuestionRubricChanged e)
        {
            UpdateAttemptRubricPoints((AttemptState)e.AggregateState, new[] { e.QuestionId }, e.Rubric.Points);
        }

        private void UpdateAttemptRubricPoints(AttemptState state, IEnumerable<Guid> questionIds, decimal rubricPoints)
        {
            var attemptId = state.Identifier;

            using (var db = CreateContext())
            {
                var attempt = db.QAttempts.FirstOrDefault(x => x.AttemptIdentifier == attemptId);
                if (attempt == null)
                    return;

                var questionPoints = 0m;
                var questions = db.QAttemptQuestions.Where(x => x.AttemptIdentifier == attemptId && questionIds.Contains(x.QuestionIdentifier));

                foreach (var question in questions)
                {
                    questionPoints += (question.QuestionPoints ?? 0) - rubricPoints;
                    question.QuestionPoints = rubricPoints;
                }

                var summary = new AttemptSummary(db, attempt.FormIdentifier, attempt.LearnerUserIdentifier);
                summary.OnBeforeUpdate(attempt);

                AdjustScore(attempt, questionPoints, null);

                summary.OnAfterUpdate(attempt);

                db.SaveChanges();
            }
        }

        #endregion

        #region Methods (helpers)

        private static void RegradeSingleCorrectOptions(QuestionRegraded e, BankQuestion questionData, QAttemptQuestion question, List<QAttemptOption> options)
        {
            question.AnswerPoints = 0;

            if (e.RegradeOption == RegradeOption.FullCreditForEveryone)
            {
                var selected = questionData.Options.Find(x => x.Points > 0);

                foreach (var o in options)
                {
                    if (o.AnswerIsSelected == true)
                    {
                        o.OptionPoints = selected.Points;
                        question.AnswerPoints = o.OptionPoints;
                    }
                    else
                    {
                        var formOption = questionData.Options.Find(x => x.Number == o.OptionKey);
                        o.OptionPoints = formOption.Points;
                    }
                }
            }
            else
            {
                foreach (var o in options)
                {
                    var formOption = questionData.Options.Find(x => x.Number == o.OptionKey);
                    if (formOption != null)
                    {
                        if (formOption.Points > 0 || o.OptionPoints == 0 || o.AnswerIsSelected != true || e.RegradeOption == RegradeOption.AwardPointsForCorrectedOnly)
                        {
                            o.OptionPoints = formOption.Points;
                        }

                        if (o.AnswerIsSelected == true)
                            question.AnswerPoints = o.OptionPoints;
                    }
                }
            }
        }

        private static void RegradeMultiCorrectOptions(QuestionRegraded e, BankQuestion questionData, QAttemptQuestion question, List<QAttemptOption> options)
        {
            foreach (var o in options)
            {
                var formOption = questionData.Options.Find(x => x.Number == o.OptionKey);
                if (formOption != null)
                {
                    o.OptionPoints = formOption.Points;

                    switch (e.RegradeOption)
                    {
                        case RegradeOption.AwardPointsForCorrectedAndPrevious:
                            if (formOption.IsTrue.HasValue && o.AnswerIsSelected == true && o.OptionIsTrue != formOption.IsTrue)
                            {
                                var oldOption = e.OldOptions.Find(x => x.Key == o.OptionKey);
                                if (oldOption.IsTrue == o.OptionIsTrue)
                                {
                                    o.OptionIsTrue = formOption.IsTrue;
                                    o.OptionPoints = oldOption.Points;
                                }
                            }
                            break;
                        case RegradeOption.AwardPointsForCorrectedOnly:
                        case RegradeOption.FullCreditForEveryone:
                            o.OptionIsTrue = formOption.IsTrue;
                            break;
                    }
                }
            }

            CalculateAnswerPoints(question, options, e.RegradeOption == RegradeOption.FullCreditForEveryone);
        }

        private static decimal CalcAttemptScore(decimal? attemptPoints, decimal? formPoints)
        {
            if ((formPoints ?? 0) == 0)
                return 0;

            var result = Calculator.GetPercentDecimal(attemptPoints ?? 0M, formPoints.Value, 8);

            if (result > 1)
                result = 1;

            return result;
        }

        private void AdjustScore(QAttempt attempt, decimal questionPoints, decimal? answerPoints)
        {
            // The total number of points available on the form after the question is removed.
            attempt.FormPoints = attempt.FormPoints - questionPoints;

            // If the candidate answered this question correctly then the total number of points awarded on the 
            // sumission will change. 
            if (answerPoints > 0)
                attempt.AttemptPoints = (attempt.AttemptPoints ?? 0M) - answerPoints;

            // If the points were calculated earlier then recalculate the points
            if (attempt.AttemptScore.HasValue)
            {
                attempt.AttemptScore = CalcAttemptScore(attempt.AttemptPoints, attempt.FormPoints);
                attempt.AttemptIsPassing = AttemptIsPassing(attempt.AttemptScore, attempt.Form?.FormPassingScore);
            }
        }

        private void UpdateAttemptScore(QAttempt attempt)
        {
            attempt.AttemptPoints = attempt.Questions.Where(x => !x.ParentQuestionIdentifier.HasValue).Sum(x => x.AnswerPoints) ?? 0;
            attempt.AttemptScore = CalcAttemptScore(attempt.AttemptPoints, attempt.FormPoints.Value);
            attempt.AttemptIsPassing = AttemptIsPassing(attempt.AttemptScore, attempt.Form?.FormPassingScore);
        }

        public static bool AttemptIsPassing(decimal? attemptScore, decimal? passingScore)
        {
            return passingScore.HasValue
                && Math.Round(attemptScore ?? 0M, 2, MidpointRounding.AwayFromZero) >= Math.Round(passingScore.Value, 2, MidpointRounding.AwayFromZero);
        }

        private bool IsComposed(string questionType)
        {
            return questionType.ToEnum<QuestionItemType>().IsComposed();
        }

        private static void CalculateAnswerPoints(QAttemptQuestion question, IEnumerable<IAttemptAnswer> answers, bool fullCredit = false)
        {
            question.AnswerPoints = 0;

            var calcMethod = question.QuestionCalculationMethod.ToEnum(QuestionCalculationMethod.Default);

            switch (calcMethod)
            {
                case QuestionCalculationMethod.AllOrNothing:
                    CalculateAnswerPoints_AllOrNothing(question, answers, fullCredit);
                    break;

                case QuestionCalculationMethod.CorrectMinusIncorrect:
                    CalculateAnswerPoints_CorrectMinusIncorrect(question, answers, fullCredit);
                    break;

                case QuestionCalculationMethod.EquallyWeighted:
                    CalculateAnswerPoints_EquallyWeighted(question, answers, fullCredit);
                    break;

                case QuestionCalculationMethod.LimitedCorrect:
                    CalculateAnswerPoints_LimitedCorrect(question, answers, fullCredit);
                    break;

                default:
                    CalculateAnswerPoints_Default(question, answers, fullCredit);
                    break;
            }
        }

        private static void CalculateAnswerPoints_AllOrNothing(QAttemptQuestion question, IEnumerable<IAttemptAnswer> answers, bool fullCredit)
        {
            foreach (var a in answers)
            {
                if (a.IsTrue.HasValue && (fullCredit || a.IsSelected == a.IsTrue.Value))
                    question.AnswerPoints += a.Points;
            }

            if (question.AnswerPoints < question.QuestionPoints)
                question.AnswerPoints = 0;
        }

        private static void CalculateAnswerPoints_CorrectMinusIncorrect(QAttemptQuestion question, IEnumerable<IAttemptAnswer> answers, bool fullCredit)
        {
            foreach (var a in answers)
            {
                if (!a.IsTrue.HasValue)
                    continue;

                if (fullCredit || a.IsSelected == a.IsTrue)
                    question.AnswerPoints += a.Points;
                else
                    question.AnswerPoints -= a.Points;
            }

            if (question.AnswerPoints < 0)
                question.AnswerPoints = 0;
        }

        private static void CalculateAnswerPoints_EquallyWeighted(QAttemptQuestion question, IEnumerable<IAttemptAnswer> answers, bool fullCredit)
        {
            foreach (var a in answers)
            {
                if (a.IsTrue.HasValue && (fullCredit || a.IsSelected == a.IsTrue))
                    question.AnswerPoints += a.Points;
            }
        }

        private static void CalculateAnswerPoints_LimitedCorrect(QAttemptQuestion question, IEnumerable<IAttemptAnswer> answers, bool fullCredit)
        {
            var correctOptionCount = 0;
            var correctAnswerCount = 0;
            var answerCount = 0;

            foreach (var a in answers)
            {
                if (a.IsTrue == true)
                {
                    correctOptionCount++;

                    if (fullCredit || a.IsSelected == true)
                        correctAnswerCount++;
                }

                if (a.IsSelected == true)
                    answerCount++;
            }

            var questionType = question.QuestionType.ToEnum<QuestionItemType>();

            if (questionType.IsCheckList() && answerCount > correctOptionCount)
                question.AnswerPoints = 0;
            else if (correctAnswerCount == correctOptionCount)
                question.AnswerPoints = question.QuestionPoints;
            else
                question.AnswerPoints = correctOptionCount != 0 ? correctAnswerCount * question.QuestionPoints / correctOptionCount : 0;
        }

        private static void CalculateAnswerPoints_Default(QAttemptQuestion question, IEnumerable<IAttemptAnswer> answers, bool fullCredit)
        {
            foreach (var a in answers)
            {
                if (fullCredit || a.IsSelected == true)
                    question.AnswerPoints += a.Points;
            }
        }

        #endregion
    }
}