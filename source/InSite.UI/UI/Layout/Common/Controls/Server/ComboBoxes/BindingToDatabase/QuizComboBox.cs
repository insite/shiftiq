using System.Collections.Generic;

using InSite.Application.Quizzes.Read;
using InSite.Persistence;

using Shift.Common;

namespace InSite.Common.Web.UI
{
    public class QuizComboBox : ComboBox
    {
        protected override ListItemArray CreateDataSource()
        {
            var quizzes = GetQuizzes();

            return ConvertToListItemArray(quizzes);
        }

        private ListItemArray ConvertToListItemArray(IEnumerable<TQuiz> quizzes)
        {
            var list = new ListItemArray();

            foreach (var quiz in quizzes)
            {
                var item = new ListItem
                {
                    Value = quiz.QuizIdentifier.ToString(),
                    Text = quiz.QuizName
                };

                list.Add(item);
            }

            return list;
        }

        private List<TQuiz> GetQuizzes()
        {
            var filter = new TQuizFilter
            {
                OrganizationIdentifier = CurrentSessionState.Identity.Organization.Identifier
            };

            TQuizSearch search = new TQuizSearch();

            return search.Select(filter);
        }
    }
}