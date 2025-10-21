using System.ComponentModel;

using Humanizer;

using InSite.Application.Banks.Read;
using InSite.Common.Web.UI;

using Shift.Common.Linq;

namespace InSite.Admin.Assessments.Forms.Controls
{
    public partial class SearchResults : SearchResultsGridViewController<QBankFormFilter>
    {
        protected override int SelectCount(QBankFormFilter filter)
        {
            return ServiceLocator.BankSearch.CountForms(filter);
        }

        protected override IListSource SelectData(QBankFormFilter filter)
        {
            return ServiceLocator.BankSearch
                .GetForms(filter)
                .ToSearchResult();
        }

        #region Methods (render)

        protected string GetSuccessRate(object item)
        {
            var form = (QBankForm)item;

            if (form.AttemptSubmittedCount == 0)
                return "-";

            var rate = 0D;
            if (form.AttemptPassedCount > 0 && form.AttemptSubmittedCount > 0)
                rate = form.AttemptPassedCount / (double)form.AttemptSubmittedCount;

            var pass = "pass".ToQuantity((int)Eval("AttemptPassedCount"));
            return $"{rate:p2} <div class='form-text'>{pass}</div>";
        }

        #endregion
    }
}