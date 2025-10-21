using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

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

        protected string GetQuestionTypeDescription(string questionType)
        {
            var qType = questionType.ToEnumNullable<QuestionItemType>();

            return !qType.HasValue ? string.Empty : qType.Value.GetDescription();
        }

        protected string GetQuestionDifficulty(int? questionDifficulty)
        {
            if (!questionDifficulty.HasValue)
                return string.Empty;

            return _difficulties.FirstOrDefault(x => x.ItemSequence == questionDifficulty)?.ItemName ?? questionDifficulty.ToString();
        }

        protected string GetQuestionFlag()
        {
            var dataItem = (QBankQuestionDetail)Page.GetDataItem();
            var flag = dataItem.QuestionFlag.ToEnumNullable<FlagType>();

            return flag.HasValue ? flag.Value.ToIconHtml() + " " + flag.GetDescription() : null;
        }
    }
}