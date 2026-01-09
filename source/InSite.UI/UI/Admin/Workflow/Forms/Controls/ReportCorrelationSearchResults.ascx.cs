using System;
using System.Collections.Generic;
using System.Text;

using InSite.Application.Surveys.Read;
using InSite.Common.Web.UI;
using InSite.Domain.Surveys.Forms;

using Shift.Common;

namespace InSite.Admin.Workflow.Forms.Controls
{
    public partial class ReportCorrelationSearchResults : BaseUserControl
    {
        #region Classes

        private class DataItem : ResponseAnalysisCorrelationItem
        {
            public Guid XQuestionIdentifier { get; set; }
            public string XTitle { get; set; }
            public Guid YQuestionIdentifier { get; set; }
            public string YTitle { get; set; }
        }

        #endregion

        #region Properties

        public QResponseCorrelationAnalysisFilter Filter
        {
            get => (QResponseCorrelationAnalysisFilter)ViewState[nameof(Filter)];
            private set => ViewState[nameof(Filter)] = value;
        }

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Variables.CellDataBound += Variables_CellDataBound;
        }

        #endregion

        #region Event handlers

        private void Variables_CellDataBound(object sender, ReportCorrelationMatrix.CellEventArgs e)
        {
            var row = (DataItem)e.Cell.DataItem;
            var matrix = (ReportCorrelationMatrix)e.Cell.FindControl("Matrix");

            matrix.RowField = "YSurveyOptionTitle";
            matrix.ColumnField = "XSurveyOptionTitle";
            matrix.ColumnHeader = row.XTitle;
            matrix.RowHeader = row.YTitle;

            var data = ServiceLocator.SurveySearch.GetCorrelationAnalysis(row.XQuestionIdentifier, row.YQuestionIdentifier);

            matrix.LoadData(data);

            var matrixLegend = e.Cell.FindControl("MatrixLegend");
            if (matrixLegend != null) matrixLegend.Visible = !Filter.ShowFrequencies && Filter.ShowRowPercentages != Filter.ShowColumnPercentages;
        }

        #endregion

        #region Search results

        public void Search(QResponseCorrelationAnalysisFilter filter)
        {
            var isFilterValid = filter.SurveyFormIdentifier.HasValue && filter.XAxis.Count > 0 && filter.YAxis.Count > 0;

            Variables.Visible = isFilterValid;

            if (!isFilterValid)
            {
                Variables.Clear();

                return;
            }

            Filter = filter;

            Variables.RowField = "YValue";
            Variables.ColumnField = "XValue";

            var t = new List<DataItem>();

            foreach (var x in filter.XAxis)
            {
                foreach (var y in filter.YAxis)
                {
                    t.Add(new DataItem
                    {
                        XQuestionIdentifier = x.QuestionIdentifier,
                        XTitle = x.Title,
                        YQuestionIdentifier = y.QuestionIdentifier,
                        YTitle = y.Title
                    });
                }
            }

            Variables.LoadData(t);
        }

        public void Clear(QResponseCorrelationAnalysisFilter filter)
        {
            Variables.Visible = false;
            Variables.Clear();

            Filter = filter;
        }

        #endregion

        #region Helpers

        protected string GetCellHtml(object rawData)
        {
            var data = (ResponseAnalysisCorrelationItem)rawData;
            var analysisFilter = Filter;

            var hasXOption = data.XSurveyOptionIdentifier.HasValue;
            var hasYOption = data.YSurveyOptionIdentifier.HasValue;

            var sb = new StringBuilder();

            if (!analysisFilter.ShowFrequencies && analysisFilter.ShowRowPercentages != analysisFilter.ShowColumnPercentages)
            {
                // Calculate "Pearson Correlation Coefficient" (as "Phi coefficient") and display colored square

                var p_x = (double)data.XValue / data.XTotalValue;
                var p_y = (double)data.YValue / data.YTotalValue;
                var p_x_y = (double)data.CrossValue / Math.Max(data.XTotalValue, data.YTotalValue);

                var correlation = Calculator.CalculateCorrelation(p_x, p_y, p_x_y);

                string colorCode = null;

                if (-1 <= correlation && correlation < -0.6) colorCode = "#CB4D4D";
                else if (-0.6 <= correlation && correlation < -0.2) colorCode = "#FAAF41";
                else if (-0.2 <= correlation && correlation < 0.2) colorCode = "#AAAAAA";
                else if (0.2 <= correlation && correlation < 0.6) colorCode = "#628BA8";
                else if (0.6 <= correlation && correlation <= 1) colorCode = "#8CBC3F";

                if (colorCode != null)
                {
                    sb.AppendFormat("<div style='width: 100%; height: 100%; border: solid 1px gray; background: {0};'>", colorCode);

                    if (hasXOption && !hasYOption)
                    {
                        if (data.XValue != 0)
                        {
                            if (analysisFilter.ShowRowPercentages)
                                sb.AppendFormat("<br/><span class='x center'>{0:p0}</span>", p_x);
                        }
                    }
                    else if (!hasXOption && hasYOption)
                    {
                        if (data.YValue != 0)
                        {
                            if (analysisFilter.ShowColumnPercentages)
                                sb.AppendFormat("<br/><span class='y center'>{0:p0}</span>", p_y);
                        }
                    }
                    else
                    {
                        if (data.CrossValue != 0)
                        {
                            if (analysisFilter.ShowRowPercentages)
                                sb.AppendFormat("<br/><span class='y center'>{0:p0}</span>", (double)data.CrossValue / data.YValue);

                            if (analysisFilter.ShowColumnPercentages)
                                sb.AppendFormat("<br/><span class='x center'>{0:p0}</span>", (double)data.CrossValue / data.XValue);
                        }
                    }

                    sb.Append("</div>");
                }
            }
            else
            {
                if (!hasXOption && !hasYOption)
                {
                    if (analysisFilter.ShowFrequencies)
                    {
                        sb.AppendFormat("<span class='y'>{0:n0}</span>", data.YTotalValue);
                        sb.AppendFormat("<br/><span class='x'>{0:n0}</span>", data.XTotalValue);
                    }
                }
                else
                {
                    if (hasXOption && !hasYOption)
                    {
                        if (data.XValue != 0)
                        {
                            if (analysisFilter.ShowFrequencies)
                                sb.AppendFormat("{0}", data.XValue);

                            if (analysisFilter.ShowRowPercentages)
                                sb.AppendFormat("<br/><span class='x'>{0:p0}</span>", (double)data.XValue / data.XTotalValue);
                        }
                    }
                    else if (!hasXOption && hasYOption)
                    {
                        if (data.YValue != 0)
                        {
                            if (analysisFilter.ShowFrequencies)
                                sb.AppendFormat("{0}", data.YValue);

                            if (analysisFilter.ShowColumnPercentages)
                                sb.AppendFormat("<br/><span class='y'>{0:p0}</span>", (double)data.YValue / data.YTotalValue);
                        }
                    }
                    else
                    {
                        if (data.CrossValue != 0)
                        {
                            if (analysisFilter.ShowFrequencies)
                                sb.AppendFormat("{0}", data.CrossValue);

                            if (analysisFilter.ShowRowPercentages)
                                sb.AppendFormat("<br/><span class='y'>{0:p0}</span>", (double)data.CrossValue / data.YValue);

                            if (analysisFilter.ShowColumnPercentages)
                                sb.AppendFormat("<br/><span class='x'>{0:p0}</span>", (double)data.CrossValue / data.XValue);
                        }
                    }
                }
            }

            return sb.ToString();
        }

        #endregion
    }
}