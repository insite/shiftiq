using System;
using System.Web.UI;

using InSite.Domain.Banks;

namespace InSite.Admin.Assessments.Questions.Controls
{
    public partial class WorkshopQuestionScript : UserControl
    {
        protected Guid BankID => Guid.Parse(Request.QueryString["bank"]);

        public void LoadData(BankState bank)
        {
            QuestionText.LoadData(bank);
        }
    }
}