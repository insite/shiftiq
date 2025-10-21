using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using InSite.Admin.Attempts.Questions.Models;
using InSite.Application.Attempts.Read;
using InSite.Common.Web.UI;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Admin.Attempts.Questions.Controls
{
    public partial class SearchResults : SearchResultsGridViewController<QAttemptQuestionFilter>
    {
        #region Properties

        private QuestionAttemptDataItem[] DataItems
        {
            get => (QuestionAttemptDataItem[])ViewState[nameof(DataItems)];
            set => ViewState[nameof(DataItems)] = value;
        }

        #endregion

        #region Methods (data binding)

        public override void Search(QAttemptQuestionFilter filter, bool refreshLastSearched = false)
        {
            BindDataItems(filter);

            base.Search(filter, refreshLastSearched);
        }

        public override void SearchWithCurrentPageIndex(QAttemptQuestionFilter filter)
        {
            BindDataItems(filter);

            base.SearchWithCurrentPageIndex(filter);
        }

        private void BindDataItems(QAttemptQuestionFilter filter)
        {
            DataItems = null;

            if (filter == null)
                return;

            if (!filter.FormIdentifier.HasValue && !filter.QuestionIdentifier.HasValue && !filter.LearnerUserIdentifier.HasValue)
                return;

            var results = new List<QuestionAttemptDataItem>();
            var questions = ServiceLocator.AttemptSearch.GetAttemptQuestions(filter);

            foreach (var question in questions)
            {
                var dataItem = new QuestionAttemptDataItem(question);

                results.Add(dataItem);
            }

            DataItems = results.ToArray();
        }

        protected override int SelectCount(QAttemptQuestionFilter filter)
        {
            return DataItems?.Length ?? 0;
        }

        protected override IListSource SelectData(QAttemptQuestionFilter filter)
        {
            return (DataItems ?? new QuestionAttemptDataItem[0])
                .ApplyPaging(filter)
                .AsQueryable()
                .ToSearchResult();
        }

        #endregion

        #region Helper methods

        protected string GetMarkdownHtml(object obj)
        {
            if (obj == null)
                return string.Empty;

            return Markdown.ToHtml((string)obj);
        }

        protected string GetDecimalString(object obj)
        {
            if (obj != null)
            {
                var d = (decimal?)obj;
                if (d != null)
                {
                    return d.Value.ToString();
                }
            }

            return string.Empty;
        }

        #endregion
    }
}