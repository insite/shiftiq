<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AnalysisReportStandardsType2.ascx.cs" Inherits="InSite.UI.Portal.Standards.Controls.AnalysisReportStandardsType2" %>

<%@ Register TagPrefix="uc" TagName="AnalysisReportStandardsType1" Src="AnalysisReportStandardsType1.ascx" %>

<asp:Repeater runat="server" ID="ReportRepeater">
    <ItemTemplate>
        <h5 class="mt-2 mb-1">
            <%# Eval("Title") %>
        </h5>

        <uc:AnalysisReportStandardsType1 runat="server" ID="Report1" />
    </ItemTemplate>
</asp:Repeater>
