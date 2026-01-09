using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Application.Banks.Read;

using Shift.Common;

namespace InSite.Common.Web.UI
{
    public class FindBankQuestion : BaseFindEntity<QBankQuestionFilter>
    {
        #region Properties

        public QBankQuestionFilter Filter => (QBankQuestionFilter)(ViewState[nameof(Filter)]
            ?? (ViewState[nameof(Filter)] = new QBankQuestionFilter { OrganizationIdentifier = CurrentSessionState.Identity.Organization.Identifier }));

        #endregion

        protected override string GetEntityName() => "Bank Question";

        protected override QBankQuestionFilter GetFilter(string keyword)
        {
            var filter = Filter.Clone();

            filter.QuestionTextWithAssetNumber = keyword;
            filter.HasParentQuestion = false;

            return filter;
        }

        protected override int Count(QBankQuestionFilter filter)
        {
            return ServiceLocator.BankSearch.CountQuestions(filter);
        }

        protected override DataItem[] Select(QBankQuestionFilter filter)
        {
            return ServiceLocator.BankSearch
                .GetQuestions(filter)
                .Select(GetDataItem)
                .ToArray();
        }

        private static DataItem GetDataItem(QBankQuestion x) => new DataItem
        {
            Value = x.QuestionIdentifier,
            Text = $"{StringHelper.Sanitize(StringHelper.Snip(x.QuestionText, 300), ' ', false, new char[] { })} [{x.QuestionAssetNumber}]"
        };

        protected override IEnumerable<DataItem> GetItems(Guid[] ids)
        {
            return ServiceLocator.BankSearch
                .GetQuestions(ids)
                .Select(GetDataItem);
        }
    }
}