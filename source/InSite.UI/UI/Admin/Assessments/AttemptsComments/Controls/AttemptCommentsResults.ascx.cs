using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using InSite.Application.Attempts.Read;
using InSite.Common.Web.UI;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Admin.Assessments.Reports.Controls
{
    public partial class SubmissionCommentaryResults : SearchResultsGridViewController<QAttemptCommentaryFilter>
    {
        [Serializable]
        public class GridFilter : Filter
        {
            public Guid OrganizationIdentifier { get; set; }
        }

        private List<QAttemptCommentaryItem> _data;
        private List<QAttemptCommentaryItem> GetData(QAttemptCommentaryFilter filter) =>
            _data ?? (_data = ServiceLocator.AttemptSearch.SelectExaminationFeedback(filter));

        protected override int SelectCount(QAttemptCommentaryFilter filter)
        {
            return GetData(filter).Count;
        }

        protected override IListSource SelectData(QAttemptCommentaryFilter filter)
        {
            var list = GetData(filter)
                .ApplyPaging(filter)
                .ToList();

            foreach (var item in list)
                item.CommentText = Markdown.ToHtml(item.CommentText);

            return list.ToSearchResult();
        }
    }
}