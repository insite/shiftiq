<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AnalysisReportStandardsType2.ascx.cs" Inherits="InSite.Admin.Standards.Documents.Controls.AnalysisReportStandardsType2" %>

<%@ Register TagPrefix="uc" TagName="AnalysisReportStandardsType1" Src="~/UI/Admin/Standards/Documents/Controls/AnalysisReportStandardsType1.ascx" %>

<asp:Repeater runat="server" ID="ReportRepeater">
    <ItemTemplate>
        <h4><%# Eval("Title") %></h4>

        <uc:AnalysisReportStandardsType1 runat="server" ID="Report1" />
    </ItemTemplate>
</asp:Repeater>
