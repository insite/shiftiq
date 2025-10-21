﻿using System;
using System.Linq;

using InSite.Application.Surveys.Read;
using InSite.Domain.Surveys.Forms;

using Shift.Common;
using Shift.Constant;

namespace InSite.Portal.Surveys.Responses
{
    public static class ResponseSessionHelper
    {
        [Serializable]
        public class GoToInfo
        {
            public int Page { get; }
            public Guid? Question { get; }

            public GoToInfo(int page)
            {
                Page = page;
            }

            public GoToInfo(int page, Guid question)
                : this(page)
            {
                Question = question;
            }
        }

        internal static GoToInfo GoToNextPage(ResponseSessionState state) =>
            GoToNextPage(state.Survey, state.Session, state.PageNumber);

        public static GoToInfo GoToNextPage(SurveyForm survey, QResponseSession session, int page)
        {
            var lastPage = survey.CountPages();
            var currentPage = Number.CheckRange(page, 1, lastPage);

            var surveyPage = survey.GetPage(currentPage);
            if (surveyPage == null || surveyPage.Questions.Any(q => !q.IsHidden && q.IsRequired && q.HasInput && !IsQuestionHasAnswer(q, session)))
                return new GoToInfo(currentPage);

            // Get all the option items in the survey.

            var options = surveyPage.FlattenOptionItems();
            if (options != null)
            {
                // Get all the selected option items in the response.

                var selections = session.QResponseOptions
                    .Where(x => x.ResponseOptionIsSelected && options.Any(y => y.Identifier == x.SurveyOptionIdentifier))
                    .OrderBy(x => x.OptionSequence)
                    .ToArray();

                if (selections.Length > 0)
                {
                    // If the respondent selected an option with a branch then determine its page number.

                    foreach (var selection in selections)
                    {
                        var option = options.Single(x => x.Identifier == selection.SurveyOptionIdentifier);

                        if (option.BranchToQuestionIdentifier.HasValue)
                        {
                            var branchToPageNumber = survey
                                .GetPageNumber(option.BranchToQuestionIdentifier.Value).Value;

                            if (branchToPageNumber > currentPage)
                            {
                                // If the current page contains multiple branches, then follow the first branch only.

                                return new GoToInfo(branchToPageNumber, option.BranchToQuestionIdentifier.Value);
                            }
                        }
                    }
                }
            }

            return surveyPage.Questions.Any(x => x.Type == SurveyQuestionType.Terminate)
                ? new GoToInfo(lastPage + 1)
                : new GoToInfo(currentPage + 1);
        }

        internal static GoToInfo GoToPreviousPage(ResponseSessionState state) =>
            GoToPreviousPage(state.Survey, state.Session, state.PageNumber);

        public static GoToInfo GoToPreviousPage(SurveyForm survey, QResponseSession session, int page)
        {
            page = Number.CheckRange(page, 1, survey.CountPages());

            var pageQuestions = survey.GetPage(page).Questions.Select(x => x.Identifier).ToHashSet();
            var branchOptions = survey.FlattenOptionItems()
                .Where(x => x.BranchToQuestionIdentifier.HasValue && pageQuestions.Contains(x.BranchToQuestionIdentifier.Value))
                .ToDictionary(x => x.Identifier);

            if (branchOptions.Count > 0)
            {
                var redirectTo = session.QResponseOptions
                    .Where(x => x.ResponseOptionIsSelected && branchOptions.ContainsKey(x.SurveyOptionIdentifier))
                    .OrderBy(x => x.OptionSequence)
                    .Select(x =>
                    {
                        var question = branchOptions[x.SurveyOptionIdentifier].List.Question.Identifier;
                        return new GoToInfo(survey.GetPageNumber(question).Value, question);
                    })
                    .OrderBy(x => x.Page)
                    .FirstOrDefault();

                if (redirectTo != null)
                    return redirectTo;
            }

            return new GoToInfo(page - 1);
        }

        internal static GoToInfo GoToLastPage(ResponseSessionState state) =>
            GoToLastPage(state.Survey, state.Session);

        public static GoToInfo GoToLastPage(SurveyForm survey, QResponseSession response)
        {
            var result = new GoToInfo(1);

            var pageCount = survey.CountPages();
            if (pageCount <= 1)
                return result;

            var prevGoTo = result;

            while (true)
            {
                var goTo = GoToNextPage(survey, response, prevGoTo.Page);
                if (goTo.Page <= prevGoTo.Page || goTo.Page > pageCount)
                    break;

                var goToPage = survey.GetPage(goTo.Page);
                if (goToPage == null)
                    break;

                var questions = goToPage.Questions.Where(x => !x.IsHidden && x.HasInput).ToArray();
                var hasQuestions = questions.Length > 0;

                if (hasQuestions && !questions.Any(x => IsQuestionHasAnswer(x, response)))
                    break;

                prevGoTo = goTo;

                if (hasQuestions)
                    result = goTo;

                if (goToPage.Questions.Any(x => x.Type == SurveyQuestionType.Terminate))
                    break;
            }

            return result;
        }

        private static bool IsQuestionHasAnswer(SurveyQuestion question, QResponseSession response)
        {
            return question.IsList || question.Type == SurveyQuestionType.Likert
                ? response.QResponseOptions
                    .Any(x => x.SurveyQuestionIdentifier == question.Identifier && x.ResponseOptionIsSelected)
                : response.QResponseAnswers
                    .Any(x => x.SurveyQuestionIdentifier == question.Identifier && x.ResponseAnswerText.HasValue());
        }
    }
}