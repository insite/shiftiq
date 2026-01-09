using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Admin.Assessments.Attempts.Utilities;
using InSite.Application.Attempts.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;

using OfficeOpenXml;
using OfficeOpenXml.Style;

using Shift.Common;

namespace InSite.Admin.Assessments.Attempts.Controls
{
    public partial class QuestionAnalysisRepeater : BaseUserControl
    {
        #region Properties

        private QAttemptFilter Filter
        {
            get => (QAttemptFilter)ViewState[nameof(Filter)];
            set => ViewState[nameof(Filter)] = value;
        }

        protected Guid? SelectedQuestionID { get; private set; }

        #endregion

        #region Fields

        private Dictionary<Guid, int> _questionSequenceMapping = null;

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Repeater.ItemDataBound += Repeater_ItemDataBound;

            DownloadButton.Click += DownloadButton_Click;
        }

        #endregion

        #region Event handlers

        private void DownloadButton_Click(object sender, CommandEventArgs e)
        {
            var analysis = AnalyzeAttempts();

            var questions = analysis.GetQuestionAnalysis();

            SendFileToClient(questions, e.CommandName);
        }

        private AttemptAnalysis AnalyzeAttempts()
        {
            var settings = new AttemptAnalysis.Settings(ServiceLocator.AttemptSearch, ServiceLocator.BankSearch)
            {
                Filter = Filter.Clone(),
                IncludeOnlyFirstAttempt = Filter is AdHocAttemptFilter adHocFilter && adHocFilter.IncludeOnlyFirstAttempt,
            };

            // Not every data set in the analysis contains the same set of columns, therefore we can't force the same
            // sort order on every data set. LoadAttemptQuestions throws an unhandled exception if the sort order is
            // not null, so we need to clear it.

            settings.Filter.OrderBy = null;

            var analysis = AttemptAnalysis.Create(settings);

            GetData(analysis);

            return analysis;
        }

        private void SendFileToClient(IAttemptAnalysisQuestion[] questions, string fileExtension)
        {
            if (questions.Length == 0)
            {
                return;
            }

            var fileName = string.Format("question-analysis-{0:yyyyMMdd}-{0:HHmmss}", DateTime.UtcNow);

            switch (fileExtension.ToLower())
            {
                case "csv":
                    SendCsv(fileName, questions);
                    break;

                case "xlsx":
                    SendXlsx(fileName, questions);
                    break;
            }
        }

        private void Repeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var dataItem = (IAttemptAnalysisQuestion)e.Item.DataItem;
            var outputTable = (QuestionAnalysisTable)e.Item.FindControl("AnalysisTable");

            outputTable.SetInputValues(dataItem);
        }

        #endregion

        #region Methods (data loading)

        public int LoadData(AttemptAnalysis analysis, QAttemptFilter filter, Guid? selectedQuestionId)
        {
            SelectedQuestionID = selectedQuestionId;

            Filter = filter;

            var data = GetData(analysis);

            Repeater.DataSource = data;
            Repeater.DataBind();

            return data.Count;
        }

        private IReadOnlyList<IAttemptAnalysisQuestion> GetData(AttemptAnalysis analysis)
        {
            _questionSequenceMapping = null;

            if (analysis.Forms.Count == 1)
            {
                _questionSequenceMapping = new Dictionary<Guid, int>();
                foreach (var field in analysis.Forms.Values.First().Sections.SelectMany(x => x.Fields))
                    _questionSequenceMapping.Add(field.QuestionIdentifier, _questionSequenceMapping.Count + 1);

                return analysis.GetQuestionAnalysis()
                    .OrderBy(x => _questionSequenceMapping.ContainsKey(x.Question.Identifier) ? _questionSequenceMapping[x.Question.Identifier] : int.MaxValue)
                    .ToArray();
            }

            return analysis.GetQuestionAnalysis();
        }

        #endregion

        #region Methods (export)

        private void SendCsv(string filename, IEnumerable<IAttemptAnalysisQuestion> data)
        {
            var csv = new StringBuilder();

            csv.AppendLine("Number,Question Text,Attempts,Success Rate");

            foreach (var question in data)
            {
                csv.Append(question.Question.Sequence);
                csv.Append(",");

                var text = question.Question.Content.Title?.Default;
                if (!string.IsNullOrEmpty(text))
                    csv.AppendFormat("\"{0}\"", text.Replace("\"", "\"\""));

                csv.Append(",");
                csv.AppendFormat("\"{0:n0}\"", question.AttemptCount);
                csv.Append(",");
                csv.AppendFormat("\"{0:p0}\"", question.SuccessRate);

                csv.AppendLine();
            }

            var dataBytes = Encoding.UTF8.GetBytes(csv.ToString());

            Page.Response.SendFile(filename, "csv", dataBytes);
        }

        private void SendXlsx(string filename, IEnumerable<IAttemptAnalysisQuestion> data)
        {
            var maxOptionsCount = data.Max(x => x.Question.Options.Count);

            using (var excel = new ExcelPackage())
            {
                #region Styles Definition

                var blueColor = ColorTranslator.FromHtml("#265f9f");
                var redColor = ColorTranslator.FromHtml("#c0392b");
                var greenColor = ColorTranslator.FromHtml("#27ae60");

                {
                    var defaultStyle = excel.Workbook.Styles.CellStyleXfs[0];
                    defaultStyle.Font.Name = "Calibri";
                    defaultStyle.Font.Size = 11;
                    defaultStyle.VerticalAlignment = ExcelVerticalAlignment.Top;
                }

                {
                    var newStyle = excel.Workbook.Styles.CreateNamedStyle("Main Header");
                    newStyle.Style.Font.Bold = true;
                    newStyle.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                }

                {
                    var newStyle = excel.Workbook.Styles.CreateNamedStyle("Question Sequence");
                    newStyle.Style.Font.Color.SetColor(blueColor);
                    newStyle.Style.Font.Bold = true;
                }

                {
                    var newStyle = excel.Workbook.Styles.CreateNamedStyle("Analysis Header");
                    newStyle.Style.Font.Bold = true;
                    newStyle.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }

                {
                    var newStyle = excel.Workbook.Styles.CreateNamedStyle("Analysis Header Correct");
                    newStyle.Style.Font.Color.SetColor(greenColor);
                    newStyle.Style.Font.Bold = true;
                    newStyle.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }

                {
                    var newStyle = excel.Workbook.Styles.CreateNamedStyle("Analysis Header Incorrect");
                    newStyle.Style.Font.Color.SetColor(redColor);
                    newStyle.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }

                {
                    var newStyle = excel.Workbook.Styles.CreateNamedStyle("Analysis Value");
                    newStyle.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }

                #endregion

                {
                    var sheet = excel.Workbook.Worksheets.Add("Question Analysis");

                    sheet.Column(1).Width = 5;
                    sheet.Column(2).Width = 30;
                    sheet.Column(3).Width = 80;
                    sheet.Column(4).Width = 30;

                    for (var colNum = 5; colNum <= 5 + maxOptionsCount; colNum++)
                        sheet.Column(colNum).Width = 10;

                    var rowNumber = 1;

                    {
                        var cell1 = sheet.Cells[rowNumber, 1];
                        cell1.Value = "#";
                        cell1.StyleName = "Main Header";

                        var cell2 = sheet.Cells[rowNumber, 2];
                        cell2.Value = "Code";
                        cell2.StyleName = "Main Header";

                        var cell3 = sheet.Cells[rowNumber, 3];
                        cell3.Value = "Question";
                        cell3.StyleName = "Main Header";

                        var cell4 = sheet.Cells[rowNumber, 4, rowNumber, 5 + maxOptionsCount];
                        cell4.Merge = true;
                        cell4.Value = "Answers";
                        cell4.StyleName = "Main Header";

                        rowNumber++;
                    }

                    foreach (var item in data)
                    {
                        var options = item.GetOptions();

                        {
                            var cell1 = sheet.Cells[rowNumber, 1, rowNumber + 5, 1];
                            cell1.Merge = true;
                            cell1.Value = GetQuestionSequence(item);
                            cell1.StyleName = "Question Sequence";

                            var cell2 = sheet.Cells[rowNumber, 2, rowNumber + 5, 2];
                            cell2.Merge = true;
                            cell2.Value = item.Question.Classification.Code;

                            var cell3 = sheet.Cells[rowNumber, 3, rowNumber + 5, 3];
                            cell3.Merge = true;
                            cell3.Style.WrapText = true;
                            cell3.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                            cell3.Value = item.Question.Content.Title.Default;

                            var cell4 = sheet.Cells[rowNumber, 4];
                            cell4.StyleName = "Analysis Header";
                            cell4.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            cell4.Value = $"{item.SuccessRate:p0} Success on {item.AttemptCount:n0} Attempts";
                            cell4.Style.Border.Bottom.Style = ExcelBorderStyle.Hair;

                            for (var i = 0; i < item.Question.Options.Count; i++)
                            {
                                var option = item.Question.Options[i];

                                var cellO = sheet.Cells[rowNumber, 5 + i];
                                cellO.StyleName = option.HasPoints ? "Analysis Header Correct" : "Analysis Header Incorrect";
                                cellO.Value = option.Letter;
                                cellO.Style.Border.Bottom.Style = ExcelBorderStyle.Hair;
                            }

                            var cellNa = sheet.Cells[rowNumber, 5 + item.Question.Options.Count];
                            cellNa.StyleName = "Analysis Header";
                            cellNa.Value = "NA";
                            cellNa.Style.Border.Bottom.Style = ExcelBorderStyle.Hair;
                        }

                        rowNumber++;

                        {
                            var cell4 = sheet.Cells[rowNumber, 4];
                            cell4.Value = "Answer Rate";

                            var index = 0;
                            foreach (var option in options)
                            {
                                var cellO = sheet.Cells[rowNumber, 5 + index++];
                                cellO.StyleName = "Analysis Value";
                                cellO.Style.Numberformat.Format = "#0%";
                                cellO.Value = option.AnswerRate;
                            }

                            var cellNa = sheet.Cells[rowNumber, 5 + item.Question.Options.Count];
                            cellNa.StyleName = "Analysis Value";
                            cellNa.Style.Numberformat.Format = "#0%";
                            cellNa.Value = item.NoAnswerRate;
                        }

                        rowNumber++;

                        {
                            var cell4 = sheet.Cells[rowNumber, 4];
                            cell4.Value = "Answer Count";

                            var index = 0;
                            foreach (var option in options)
                            {
                                var cellO = sheet.Cells[rowNumber, 5 + index++];
                                cellO.StyleName = "Analysis Value";
                                cellO.Style.Numberformat.Format = "0";
                                cellO.Value = option.AnswerCount;
                            }

                            var cellNa = sheet.Cells[rowNumber, 5 + item.Question.Options.Count];
                            cellNa.StyleName = "Analysis Value";
                            cellNa.Style.Numberformat.Format = "0";
                            cellNa.Value = item.NoAnswerCount;
                        }

                        rowNumber++;

                        {
                            var cell4 = sheet.Cells[rowNumber, 4];
                            cell4.Value = "Average Exam Score";

                            var index = 0;
                            foreach (var option in options)
                            {
                                var cellO = sheet.Cells[rowNumber, 5 + index++];
                                cellO.StyleName = "Analysis Value";
                                cellO.Style.Numberformat.Format = "#0%";
                                cellO.Value = (decimal)option.AverageAttemptScorePercent / 100;
                            }

                            var cellNa = sheet.Cells[rowNumber, 5 + item.Question.Options.Count];
                            cellNa.StyleName = "Analysis Value";
                            cellNa.Style.Numberformat.Format = "0";
                            cellNa.Value = item.NoAnswerAverageAttemptScorePercent;
                        }

                        rowNumber++;

                        {
                            var cell4 = sheet.Cells[rowNumber, 4];
                            cell4.Value = "Item Total Correlation";

                            var index = 0;
                            foreach (var option in options)
                            {
                                var oColor = ColorTranslator.FromHtml(QuestionAnalysisTable.GetDiscriminationColor(option.ItemTotalCorrelation));

                                var cellO = sheet.Cells[rowNumber, 5 + index++];
                                cellO.StyleName = "Analysis Value";
                                cellO.Style.Numberformat.Format = "0.00";
                                cellO.Style.Font.Color.SetColor(oColor);
                                cellO.Value = option.ItemTotalCorrelation;
                            }

                            var naColor = ColorTranslator.FromHtml(QuestionAnalysisTable.GetDiscriminationColor(item.NoAnswerItemTotalCorrelation));

                            var cellNa = sheet.Cells[rowNumber, 5 + item.Question.Options.Count];
                            cellNa.StyleName = "Analysis Value";
                            cellNa.Style.Numberformat.Format = "0.00";
                            cellNa.Style.Font.Color.SetColor(naColor);
                            cellNa.Value = item.NoAnswerItemTotalCorrelation;
                        }

                        rowNumber++;

                        {
                            var cell4 = sheet.Cells[rowNumber, 4];
                            cell4.Value = "Item Rest Coefficient";

                            var index = 0;
                            foreach (var option in options)
                            {
                                var cellO = sheet.Cells[rowNumber, 5 + index++];
                                cellO.StyleName = "Analysis Value";
                                cellO.Style.Numberformat.Format = "0.00";
                                cellO.Value = option.ItemRestCoefficient;
                            }

                            var cellNa = sheet.Cells[rowNumber, 5 + item.Question.Options.Count];
                            cellNa.StyleName = "Analysis Value";
                            cellNa.Style.Numberformat.Format = "0.00";
                            cellNa.Value = item.NoAnswerItemRestCoefficient;
                        }

                        {
                            var cell = sheet.Cells[rowNumber, 1, rowNumber, 4 + maxOptionsCount + 1];
                            cell.Style.Border.Bottom.Style = ExcelBorderStyle.Dotted;
                        }

                        rowNumber++;
                    }
                }

                excel.Workbook.Properties.Title = "Question Analysis";
                excel.Workbook.Properties.Company = CurrentSessionState.Identity.Organization.Name;
                excel.Workbook.Properties.Author = CurrentSessionState.Identity.User.FullName;
                excel.Workbook.Properties.Created = DateTimeOffset.Now.DateTime;

                foreach (var sheet in excel.Workbook.Worksheets)
                {
                    sheet.PrinterSettings.Orientation = eOrientation.Portrait;
                    sheet.PrinterSettings.PaperSize = ePaperSize.A4;
                    sheet.PrinterSettings.FitToPage = true;
                    sheet.PrinterSettings.FitToWidth = 1;
                    sheet.PrinterSettings.FitToHeight = 0;
                }

                var dataBytes = excel.GetAsByteArray();

                Page.Response.SendFile(filename, "xlsx", dataBytes);
            }
        }

        #endregion

        #region Methods (helpers)

        protected string GetContentTitle(object content)
        {
            return content is ContentExamQuestion c ? Markdown.ToHtml(c.Title.Default) : null;
        }

        protected string GetQuestionSequence(object obj)
        {
            var dataItem = (IAttemptAnalysisQuestion)obj;

            return _questionSequenceMapping == null
                ? dataItem.Question.Sequence.ToString()
                : _questionSequenceMapping.ContainsKey(dataItem.Question.Identifier)
                    ? _questionSequenceMapping[dataItem.Question.Identifier].ToString()
                    : "??";
        }

        #endregion
    }
}