using System;
using System.ComponentModel;

using Humanizer;

using InSite.Application.Banks.Read;
using InSite.Common.Web.UI;

using Shift.Common.Linq;

namespace InSite.Admin.Assessments.Banks.Controls
{
    public partial class SearchResults : SearchResultsGridViewController<QBankFilter>
    {
        protected string GetAttachmentsLabel(object data)
        {
            var bank = (QBank)data;
            if (bank.AttachmentCount > 0)
                return $"<span class='badge bg-custom-default'>" + "attachment".ToQuantity(bank.AttachmentCount) + "</span>";
            return string.Empty;
        }

        protected string GetCommentsLabel(object data)
        {
            var bank = (QBank)data;
            if (bank.CommentCount > 0)
                return $"<span class='badge bg-primary'>" + "comment".ToQuantity(bank.CommentCount) + "</span>";
            return string.Empty;
        }

        protected string GetStandardLabel(object data)
        {
            var bank = (QBank)data;
            if (bank.FrameworkIdentifier == Guid.Empty)
                return $"<span class='badge bg-warning'>Missing Standard</span>";
            return string.Empty;
        }

        protected override int SelectCount(QBankFilter filter)
        {
            return ServiceLocator.BankSearch.CountBanks(filter);
        }

        protected override IListSource SelectData(QBankFilter filter)
        {
            return ServiceLocator.BankSearch
                .GetBanks(filter)
                .ToSearchResult();
        }
    }
}