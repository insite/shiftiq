<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="QuestionCompetencyRatioRepeater.ascx.cs" Inherits="InSite.Admin.Assessments.Attempts.Controls.QuestionCompetencyRatioRepeater" %>

<div class="mb-3">
    <insite:DropDownButton runat="server" ID="ExportButton" DefaultAction="PostBack" IconName="download" Text="Download">
        <Items>
            <insite:DropDownButtonItem Name="XLSX" IconName="file-excel" Text="Excel (*.xlsx)" />
            <insite:DropDownButtonItem Name="CSV" IconName="file-csv" Text="Text (*.csv)" />
        </Items>
    </insite:DropDownButton>
</div>

<asp:Repeater runat="server" ID="Repeater">
    <HeaderTemplate>
        <table class="table table-competency-ratio">
            <thead>
                <tr>
                    <th>Name</th>
                    <th>Type</th>
                    <th class="text-end">Writes</th>
                    <th class="text-end">W&nbsp;Ratio</th>
                    <th class="text-end">Fails</th>
                    <th class="text-end">Pass&nbsp;Rate</th>
                    <th class="text-end">P&nbsp;Ratio</th>
                    <th class="text-end">Average</th>
                    <th class="text-end">A&nbsp;Ratio</th>
                </tr>
            </thead>
            <tbody>
    </HeaderTemplate>
    <FooterTemplate>
            </tbody>
            <tfoot>
                <tr>
                    <th></th>
                    <th></th>
                    <th class="text-end"><asp:Literal runat="server" ID="Writes" /></th>
                    <th class="text-end"><asp:Literal runat="server" ID="WritesRatio" /></th>
                    <th class="text-end"><asp:Literal runat="server" ID="Fails" /></th>
                    <th class="text-end"><asp:Literal runat="server" ID="PassRate" /></th>
                    <th class="text-end"><asp:Literal runat="server" ID="PassRateRatio" /></th>
                    <th class="text-end"><asp:Literal runat="server" ID="Score" /></th>
                    <th class="text-end"><asp:Literal runat="server" ID="ScoreRatio" /></th>
                </tr>
            </tfoot>
        </table>
    </FooterTemplate>
    <ItemTemplate>
        <tr>
            <td style='<%# string.Format("padding-left:{0}px", ((int)Eval("Depth") - 1) * 20 + 8) %>'><%# Eval("TitleHtml") %></td>
            <td><%# Eval("Type") %></td>
            <td class="text-end"><%# Eval("Writes", "{0:n0}") %></td>
            <td class="text-end"><%# Eval("WritesRatio", "{0:n4}") %></td>
            <td class="text-end"><%# Eval("Fails", "{0:n0}") %></td>
            <td class="text-end"><%# Eval("PassRate", "{0:n4}") %></td>
            <td class="text-end"><%# Eval("PassRateRatio", "{0:n4}") %></td>
            <td class="text-end"><%# Eval("Score", "{0:n2}") %></td>
            <td class="text-end"><%# Eval("ScoreRatio", "{0:n4}") %></td>
        </tr>
    </ItemTemplate>
</asp:Repeater>
