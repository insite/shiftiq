using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;

using InSite.Admin.Assessments.Questions.Utilities;
using InSite.Common.Web;
using InSite.Domain.Attempts;
using InSite.Domain.Banks;
using InSite.Domain.Organizations;
using InSite.Web.Helpers;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Admin.Assessments.Questions.Controls
{
    public partial class QuestionPrintCompact : UserControl
    {
        #region Classes

        private class ControlData
        {
            public string AssetNumber { get; }
            public string Title { get; }
            public string Name { get; }
            public List<DataItem> Items { get; } = new List<DataItem>();

            public ControlData(BankState bank)
            {
                AssetNumber = bank.Asset.ToString();
                Title = (bank.Content?.Title?.Default).IfNullOrEmpty(bank.Name);
                Name = bank.Name;
            }

            public ControlData(Form form)
            {
                AssetNumber = $"{form.Asset}.{form.AssetVersion}";
                Title = (form.Content.Title?.Default).IfNullOrEmpty(form.Name);
                Name = form.Name;
            }
        }

        public class DataItem
        {
            public int Sequence { get; set; }
            public string Letter { get; set; }
        }

        public class BankOptions : Options
        {
            public Guid BankID { get; }

            public BankOptions(OrganizationState organization, Guid bankId)
                : base(organization)
            {
                BankID = bankId;
            }
        }

        public class FormOptions : Options
        {
            public Guid FormID { get; }

            public FormOptions(OrganizationState organization, Guid formId)
                : base(organization)
            {
                FormID = formId;
            }
        }

        public class Options
        {
            public Guid OrganizationID { get; }

            public string CurrentUrl { get; }
            public string HeaderUrl { get; }

            public Options(OrganizationState organization)
            {
                OrganizationID = organization.Identifier;

                var request = HttpContext.Current.Request;

                CurrentUrl = request.Url.Scheme + "://" + request.Url.Host + request.RawUrl;
                HeaderUrl = HttpRequestHelper.GetAbsoluteUrl("~/UI/Admin/Assessments/Questions/Html/PrintInternalHeader.html");
            }
        }

        #endregion

        private void LoadData(ControlData data)
        {
            PageTitle.InnerText = data.Title.IfNullOrEmpty("Untitled");

            var hasData = data.Items.Count > 0;

            NoDataMessage.Visible = !hasData;

            ItemRepeater.Visible = hasData;
            ItemRepeater.DataSource = data.Items;
            ItemRepeater.DataBind();
        }

        #region Methods (render PDF)

        public static PrintOutputFile RenderPdf(BankOptions options)
        {
            var bank = ServiceLocator.BankSearch.GetBankState(options.BankID);
            if (bank == null || bank.Tenant != options.OrganizationID)
                return null;

            var data = new ControlData(bank);

            var sequence = 1;

            foreach (var q in bank.Sets.SelectMany(x => x.Questions))
            {
                IEnumerable<Option> bankOptions;

                if (q.Type.IsRadioList())
                    bankOptions = q.Options.Where(x => x.HasPoints);
                else if (q.Type.IsCheckList())
                    bankOptions = q.Options.Where(x => x.IsTrue == true);
                else
                    continue;

                foreach (var o in bankOptions)
                {
                    data.Items.Add(new DataItem
                    {
                        Sequence = sequence,
                        Letter = o.Letter
                    });
                }

                sequence++;
            }

            return RenderPdf(data, options);
        }

        public static PrintOutputFile RenderPdf(FormOptions options)
        {
            var form = ServiceLocator.BankSearch.GetFormData(options.FormID);
            if (form == null || form.Specification.Bank.Tenant != options.OrganizationID)
                return null;

            var data = new ControlData(form);

            var sequence = 1;

            foreach (var q in QuestionPrintHelper.GetQuestions(form))
            {
                var question = q.AttemptQuestion as AttemptQuestionDefault;
                if (question == null)
                    continue;

                IEnumerable<string> optionLetters;

                if (q.AttemptQuestion.Type.IsRadioList())
                    optionLetters = question.Options
                        .Select((x, i) => (x.Points, Sequence: i + 1))
                        .Where(x => x.Points != 0)
                        .Select(x => Calculator.ToBase26(x.Sequence));
                else if (q.AttemptQuestion.Type.IsCheckList())
                    optionLetters = question.Options
                        .Select((x, i) => (x.IsTrue, Sequence: i + 1))
                        .Where(x => x.IsTrue == true)
                        .Select(x => Calculator.ToBase26(x.Sequence));
                else
                    continue;

                foreach (var o in optionLetters)
                {
                    data.Items.Add(new DataItem
                    {
                        Sequence = sequence,
                        Letter = o
                    });
                }

                sequence++;
            }

            return RenderPdf(data, options);
        }

        private static PrintOutputFile RenderPdf(ControlData data, Options options)
        {
            using (var page = new Page())
            {
                page.EnableEventValidation = false;
                page.EnableViewState = false;

                var report = (QuestionPrintCompact)page.LoadControl("~/UI/Admin/Assessments/Questions/Controls/QuestionPrintCompact.ascx");

                report.LoadData(data);

                var htmlBuilder = new StringBuilder();

                using (var writer = new StringWriter(htmlBuilder))
                {
                    using (var htmlWriter = new HtmlTextWriter(writer))
                        report.RenderControl(htmlWriter);
                }

                var htmlString = HtmlHelper.ResolveRelativePaths(options.CurrentUrl, htmlBuilder);

                var fileName = $"{StringHelper.Sanitize(data.Title, '_')}_compact_{DateTime.UtcNow:yyyyMMdd}-{DateTime.UtcNow:HHmmss}";

                var pdfData = HtmlConverter.HtmlToPdf(htmlString, new HtmlConverterSettings(ServiceLocator.AppSettings.Application.WebKitHtmlToPdfExePath)
                {
                    EnablePrintMediaType = true,
                    PageSize = PageSizeType.Letter,
                    MarginTop = 25f,
                    HeaderSpacing = 7,
                    HeaderUrl = options.HeaderUrl,
                    Variables = new HtmlConverterSettings.Variable[]
                    {
                        new HtmlConverterSettings.Variable("title", data.Title),
                        new HtmlConverterSettings.Variable("name", data.Name),
                        new HtmlConverterSettings.Variable("asset_number", data.AssetNumber),
                    },
                });

                return new PrintOutputFile(fileName, pdfData);
            }
        }

        #endregion
    }
}