using System.Collections.Generic;

using InSite.Application.Attempts.Read;
using InSite.Common.Web.UI;
using InSite.Domain.Banks;

namespace InSite.UI.Admin.Assessments.Attempts.Controls
{
    public partial class ViewComposedEssay : BaseUserControl
    {
        public void BindQuestions(BankState bank, IEnumerable<QAttemptQuestion> questions)
        {
            ComposedRepeater.BindQuestions(bank, questions);
        }
    }
}