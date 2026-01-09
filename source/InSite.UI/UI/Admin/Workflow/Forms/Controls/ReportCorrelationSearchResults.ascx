<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ReportCorrelationSearchResults.ascx.cs" Inherits="InSite.Admin.Workflow.Forms.Controls.ReportCorrelationSearchResults" %>

<%@ Register TagPrefix="uc" Assembly="InSite.UI" Namespace="InSite.Admin.Workflow.Forms.Controls" %>

<insite:PageHeadContent runat="server">
<style type="text/css">

    .correlation-results table.variables-matrix {
        line-height: 1 !important;
    }

        .correlation-results table.variables-matrix tr.tr-row::before,
        .correlation-results table.variables-matrix tr.tr-row::after {
            content: none !important;
        }

        .correlation-results table.variables-matrix tr td table.matrix tr.total td {
            border-top: 1px solid #bfbfbf;
            border-bottom: 1px solid #bfbfbf;
        }

        .correlation-results table.variables-matrix tr td table.matrix tr td.value.total {
            border-left: 1px solid #bfbfbf;
            border-right: 1px solid #bfbfbf;
        }

        .correlation-results table.variables-matrix tr td table.matrix tr td {
            white-space: nowrap;
            text-align: right;
        }

            .correlation-results table.variables-matrix tr td table.matrix tr td span.x {
                color: #008000;
            }

            .correlation-results table.variables-matrix tr td table.matrix tr td span.y {
                color: #0000FF;
            }

            .correlation-results table.variables-matrix tr td table.matrix tr td span.center {
                padding: 10px;
            }

        .correlation-results table.variables-matrix tr td table.matrix tr th.matrix-x-axis {
            text-align: center;
            font-weight: 700;
            color: #008000;
        }

        .correlation-results table.variables-matrix tr td table.matrix tr th.column {
            text-align: center;
        }

        .correlation-results table.variables-matrix label.matrix-y-axis {
            float: left;
            writing-mode: vertical-rl;
            transform: rotate(180deg);

            font-weight: 700;
            margin-top: 90px;
            height: 100px;
            padding: 0 20px 0 5px;

            color: #0000FF;
        }

    .correlation-results .matrix-legend {
        width: 100%;
        text-align: center;
        padding: 15px;
    }

        .correlation-results .matrix-legend div.item {
            width: 12px;
            height: 12px;
            display: inline-block;
            vertical-align: middle;
        }

        .correlation-results .matrix-legend label.item-text {
            font-size: 12px;
            padding-left: 5px;
        }

</style>
</insite:PageHeadContent>

<div class="correlation-results">
    <uc:ReportCorrelationMatrix runat="server" ID="Variables" Width="100%" CssClass="variables-matrix" ShowColumnLabels="false" ShowRowLabels="false">
        <CellTemplate>
            <uc:ReportCorrelationMatrix runat="server" ID="Matrix" Width="100%" HasTotalCell="true">
                <CellTemplate>
                    <%# GetCellHtml(Container.DataItem) %>
                </CellTemplate>
            </uc:ReportCorrelationMatrix>
            <div runat="server" id="MatrixLegend" class="matrix-legend">
                <div class="item" style="background: #CB4D4D;"></div><label class="item-text"> - [-1, -0.6]</label>
                <div class="item" style="background: #FAAF41;"></div><label class="item-text"> - [-0.6, -0.2]</label>
                <div class="item" style="background: #AAAAAA;"></div><label class="item-text"> - [-0.2, 0.2]</label><br />
                <div class="item" style="background: #628BA8;"></div><label class="item-text"> - [0.2, 0.6]</label>
                <div class="item" style="background: #8CBC3F;"></div><label class="item-text"> - [-0.6, 1]</label>
            </div>
        </CellTemplate>
    </uc:ReportCorrelationMatrix>
</div>
