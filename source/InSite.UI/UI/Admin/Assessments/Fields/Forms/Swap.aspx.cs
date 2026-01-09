using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Admin.Assessments.Banks;
using InSite.Application.Banks.Read;
using InSite.Application.Banks.Write;
using InSite.Common.Web;
using InSite.Domain.Banks;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

using Field = InSite.Domain.Banks.Field;

namespace InSite.Admin.Assessments.Fields.Forms
{
    public partial class Swap : AdminBasePage
    {
        private class FieldInfo
        {
            public string FormName { get; set; }
            public string SectionName { get; set; }
            public int BankSequence { get; set; }
            public int QuestionSequence { get; set; }
            public int Asset { get; set; }
            public string Code { get; set; }
            public string Status { get; set; }
            public string Title { get; set; }
            public int FormIndex { get; set; }
            public Question Question { get; set; }
            public List<Option> Options { get; set; }
        }

        private class SwapDetails
        {
            public QBank Bank { get; set; }
            public List<Field> Fields { get; set; }
            public Field Field1 { get; set; }
            public Field Field2 { get; set; }
        }

        private Guid BankID => Guid.TryParse(Request.QueryString["bank"], out var value) ? value : Guid.Empty;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            FieldRepeater.ItemDataBound += FieldRepeater_ItemDataBound;

            SwapButton.Click += SwapButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Identity.IsGranted(Route.ToolkitName, PermissionOperation.Write))
                HttpResponseHelper.Redirect("/ui/admin/assessments/banks/search");

            if (!IsPostBack)
            {
                var swapDetails = GetSwapDetails();

                if (swapDetails == null)
                    HttpResponseHelper.Redirect($"/ui/admin/assessments/banks/outline?bank={BankID}");

                PageHelper.AutoBindHeader(this);

                var list = new List<FieldInfo>();

                AddField(swapDetails.Field1, swapDetails.Fields, list);
                AddField(swapDetails.Field2, swapDetails.Fields, list);

                FieldRepeater.DataSource = list;
                FieldRepeater.DataBind();
            }
        }

        private void FieldRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var options = (IEnumerable<Option>)DataBinder.Eval(e.Item.DataItem, "Options");

            var optionRepeater = (Repeater)e.Item.FindControl("OptionRepeater");
            optionRepeater.DataSource = options.Select(x => new
            {
                x.Points,
                x.Letter,
                Title = x.Content.Title.Default
            });
            optionRepeater.DataBind();
        }

        private void SwapButton_Click(object sender, EventArgs e)
        {
            var swapDetails = GetSwapDetails();
            var cmd = new SwapFields(BankID, swapDetails.Field1.Identifier, swapDetails.Field2.Identifier);

            ServiceLocator.SendCommand(cmd);

            PinModel.RemoveModel(BankID);

            var url = $"/ui/admin/assessments/banks/outline?bank={BankID}&status=swapped";

            HttpResponseHelper.Redirect(url);
        }

        protected string GetReviewLabel(object status)
        {
            var label = string.Empty;

            if (status != null)
            {
                switch ((string)status)
                {
                    case "Requires Change":
                        label = "<div class='badge bg-danger'><i class='fas fa-flag'></i> Requires Change</div>";
                        break;
                    case "Modified":
                        label = "<div class='badge bg-info'>Modified</div>";
                        break;
                    case "Removed":
                        label = "<div class='badge bg-custom-default'>Removed</div>";
                        break;
                    default:
                        break;
                }
            }

            return label;
        }

        protected static string DisplayStandard(object item)
        {
            Question q = (Question)item;

            return q.Standard != Guid.Empty ? $"<div class='question-standard'>Standard: {SnippetBuilder.GetHtml(q.Standard)}</div>" : null;
        }

        protected string GetOptionIcon(decimal value)
            => value > 0
                ? $"<i class='far fa-check text-success'></i>"
                : $"<i class='far fa-times text-danger'></i>";

        private SwapDetails GetSwapDetails()
        {
            var pinModel = PinModel.GetModel(BankID);

            if (pinModel == null || pinModel.FieldAssetNumbers.Count != 2)
                return null;

            var fieldAssetNumbers = pinModel.FieldAssetNumbers.ToList();

            var swapDetails = new SwapDetails();

            swapDetails.Bank = ServiceLocator.BankSearch.GetBank(BankID);

            var bank = ServiceLocator.BankSearch.GetBankState(BankID);
            var forms = bank.Specifications.SelectMany(x => x.EnumerateAllForms()).ToList();
            var sections = forms.SelectMany(x => x.Sections);

            swapDetails.Fields = sections.SelectMany(x => x.Fields).ToList();
            swapDetails.Field1 = swapDetails.Fields.FirstOrDefault(y => y.Question.Asset == fieldAssetNumbers[0]);
            swapDetails.Field2 = swapDetails.Fields.FirstOrDefault(y => y.Question.Asset == fieldAssetNumbers[1]);

            return swapDetails;
        }

        private void AddField(Field field, List<Field> fields, List<FieldInfo> list)
        {
            var sectionTitle = field.Section.Criterion.ToString();

            if (string.IsNullOrEmpty(sectionTitle))
                sectionTitle = field.Section.Letter;

            if (string.IsNullOrEmpty(sectionTitle))
                sectionTitle = "(Untitled)";

            list.Add(new FieldInfo
            {
                FormName = field.Section.Form.Name,
                SectionName = sectionTitle,
                BankSequence = field.Question.BankIndex + 1,
                QuestionSequence = field.Question.Sequence,
                Asset = field.Question.Asset,
                Code = field.Question.Classification.Code,
                Title = Markdown.ToHtml(field.Question.Content.Title.Default),
                FormIndex = 1 + fields.IndexOf(field),
                Question = field.Question,
                Options = field.Question.Options
            });
        }
    }
}