using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;

using InSite.Application.Banks.Read;
using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Constant;

namespace InSite.Admin.Assessments.Questions.Controls
{
    public partial class SearchResults : SearchResultsGridViewController<QBankQuestionFilter>
    {
        private List<TCollectionItem> _difficulties = null;

        protected override int SelectCount(QBankQuestionFilter filter)
        {
            return ServiceLocator.BankSearch.CountQuestions(filter);
        }

        protected override IListSource SelectData(QBankQuestionFilter filter)
        {
            _difficulties = TCollectionItemCache.Select(new TCollectionItemFilter
            {
                OrganizationIdentifier = CurrentSessionState.Identity.Organization.Identifier,
                CollectionName = CollectionName.Assessments_Questions_Classification_Difficulty
            });

            return ServiceLocator.BankSearch
                .GetQuestionDetails(filter)
                .ToSearchResult();
        }

        protected string GetQuestionTypeDescription()
        {
            var dataItem = (QBankQuestionDetail)Page.GetDataItem();
            return dataItem.QuestionType.ToEnumNullable<QuestionItemType>()?.GetDescription();
        }

        protected string GetQuestionDifficulty()
        {
            var dataItem = (QBankQuestionDetail)Page.GetDataItem();
            if (!dataItem.QuestionDifficulty.HasValue)
                return string.Empty;

            return _difficulties.FirstOrDefault(x => x.ItemSequence == dataItem.QuestionDifficulty)?.ItemName
                ?? dataItem.QuestionDifficulty.ToString();
        }

        protected string GetQuestionFlag()
        {
            var dataItem = (QBankQuestionDetail)Page.GetDataItem();
            var flag = dataItem.QuestionFlag.ToEnumNullable<FlagType>();

            return flag.HasValue ? flag.Value.ToIconHtml() + " " + flag.GetDescription() : null;
        }

        protected string GetPublicationStatus()
        {
            var dataItem = (QBankQuestionDetail)Page.GetDataItem();
            return dataItem.QuestionPublicationStatus.ToEnumNullable<PublicationStatus>()?.GetDescription();
        }

        protected string GetReportingTags()
        {
            var dataItem = (QBankQuestionDetail)Page.GetDataItem();

            var tags = QBankQuestion.GetQuestionTags(dataItem.QuestionTags);
            if (tags.IsEmpty())
                return string.Empty;

            var html = new StringBuilder();
            html.Append("<ul class='text-nowrap'>");

            foreach (var group in tags)
            {
                html.Append("<li>");
                html.Append(HttpUtility.HtmlEncode(group.Item1));
                html.Append("<ul>");

                foreach (var item in group.Item2)
                    html.Append("<li>")
                        .Append(HttpUtility.HtmlEncode(item))
                        .Append("</li>");

                html.Append("</ul>");
                html.Append("</li>");
            }

            html.Append("</ul>");
            return html.ToString();
        }

        public override IListSource GetExportData(QBankQuestionFilter filter, bool empty)
        {
            if (empty)
                return (new QBankQuestionDetail[0]).ToSearchResult();

            var data = base.GetExportData(filter, empty);

            foreach (QBankQuestionDetail item in data.GetList())
            {
                item.QuestionPublicationStatus = item.QuestionPublicationStatus
                    .ToEnumNullable<PublicationStatus>()?.GetDescription();

                var tags = QBankQuestion.GetQuestionTags(item.QuestionTags);
                item.QuestionTags = tags.IsEmpty()
                    ? string.Empty
                    : string.Join("; ", tags.Select(g => g.Item1 + ": " + string.Join(", ", g.Item2)));
            }

            return data;
        }
    }
}