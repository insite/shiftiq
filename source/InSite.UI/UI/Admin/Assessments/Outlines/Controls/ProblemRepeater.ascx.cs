using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Admin.Assessments.Options.Controls;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Banks;

using Shift.Common;

namespace InSite.Admin.Assessments.Outlines.Controls
{
    public partial class ProblemRepeater : BaseUserControl
    {
        public Guid BankIdentifier
        {
            get => (Guid)ViewState[nameof(BankIdentifier)];
            set => ViewState[nameof(BankIdentifier)] = value;
        }

        public Guid ContainerIdentifier
        {
            get => (Guid)ViewState[nameof(ContainerIdentifier)];
            set => ViewState[nameof(ContainerIdentifier)] = value;
        }

        public string ContainerType
        {
            get => (string)ViewState[nameof(ContainerType)];
            set => ViewState[nameof(ContainerType)] = value;
        }

        public int ItemCount
        {
            get => (int?)ViewState[nameof(ItemCount)] ?? 0;
            set => ViewState[nameof(ItemCount)] = value;
        }

        protected ReturnUrl ProblemsReturnUrl { get; set; }

        public void BindModelToControls(BankState bank)
        {
            BankIdentifier = bank.Identifier;
            ContainerIdentifier = bank.Identifier;
            ContainerType = "Bank";
            BindModelToControls(bank.Sets.SelectMany(x => x.Questions), "Bank");
        }

        public void BindModelToControls(Specification spec)
        {
            BankIdentifier = spec.Bank.Identifier;
            ContainerIdentifier = spec.Identifier;
            ContainerType = "Specification";
            BindModelToControls(spec.Bank.Sets.SelectMany(x => x.Questions), "Specification");
        }

        public void BindModelToControls(Form form)
        {
            BankIdentifier = form.Specification.Bank.Identifier;
            ContainerIdentifier = form.Identifier;
            ContainerType = "Form";
            BindModelToControls(form.GetQuestions(), "Form");
        }

        private void BindModelToControls(IEnumerable<Question> questions, string containerType)
        {
            var data = new List<QuestionProblemInfo>();

            foreach (var q in questions)
            {
                if (q.Type == QuestionItemType.SingleCorrect)
                {
                    if (q.Options.Count == 0)
                    {
                        data.Add(new QuestionProblemInfo
                        {
                            Question = q,
                            Description = "The question contains no options."
                        });
                    }
                    else if (q.Options.Count == 1)
                    {
                        data.Add(new QuestionProblemInfo
                        {
                            Question = q,
                            Description = "The question contains only one option."
                        });
                    }
                    else
                    {
                        var correctCount = q.Options.Where(x => x.HasPoints).Count();

                        if (correctCount == 0)
                        {
                            data.Add(new QuestionProblemInfo
                            {
                                Question = q,
                                Description = "The question contains no correct option."
                            });
                        }
                        else if (correctCount > 1)
                        {
                            data.Add(new QuestionProblemInfo
                            {
                                Question = q,
                                Description = "The question contains more than one correct option."
                            });
                        }
                    }
                }
            }

            ItemCount = data.Count;

            ProblemsReturnUrl = new ReturnUrl("bank&panel=bank.problems");

            ItemRepeater.DataSource = data;
            ItemRepeater.DataBind();
            ItemRepeater.Visible = ItemCount > 0;

            ZeroItemPanel.Visible = ItemCount == 0;
            if (ItemCount == 0)
                ZeroItemText.Text = $"Congrats! We couldn't find any problems with the questions in this {containerType}.";
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ItemRepeater.ItemDataBound += ItemRepeater_ItemDataBound;
            ItemRepeater.ItemCommand += ItemRepeater_ItemCommand;
        }

        private void ItemRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "JumpToQuestion")
            {
                var questionId = Guid.Parse(e.CommandArgument.ToString());
                var question = ServiceLocator.BankSearch.GetQuestion(questionId);

                if (ContainerType == "Bank")
                    HttpResponseHelper.Redirect($"/ui/admin/assessments/banks/outline?bank={BankIdentifier}&question={questionId}");
                else if (ContainerType == "Specification")
                    HttpResponseHelper.Redirect($"/ui/admin/assessments/specifications/workshop?bank={BankIdentifier}&spec={ContainerIdentifier}&set={question.SetIdentifier}&question={questionId}&panel=questions");
                else if (ContainerType == "Form")
                    HttpResponseHelper.Redirect($"/ui/admin/assessments/forms/workshop?bank={BankIdentifier}&form={ContainerIdentifier}&set={question.SetIdentifier}&question={questionId}&panel=questions");
            }
        }

        private void ItemRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e.Item))
                return;

            var data = (QuestionProblemInfo)e.Item.DataItem;
            var optionRepeater = (OptionReadRepeater)e.Item.FindControl("OptionReadRepeater");
            optionRepeater.LoadData(data.Question);
        }
    }
}