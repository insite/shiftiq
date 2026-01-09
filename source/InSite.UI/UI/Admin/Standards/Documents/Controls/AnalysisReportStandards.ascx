<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AnalysisReportStandards.ascx.cs" Inherits="InSite.Admin.Standards.Documents.Controls.AnalysisReportStandards" %>

<%@ Register TagPrefix="uc" TagName="AnalysisReportStandardsType1" Src="~/UI/Admin/Standards/Documents/Controls/AnalysisReportStandardsType1.ascx" %>
<%@ Register TagPrefix="uc" TagName="AnalysisReportStandardsType2" Src="~/UI/Admin/Standards/Documents/Controls/AnalysisReportStandardsType2.ascx" %>
<%@ Register TagPrefix="uc" TagName="AnalysisReportStandardsType3" Src="~/UI/Admin/Standards/Documents/Controls/AnalysisReportStandardsType3.ascx" %>

<uc:AnalysisReportStandardsType1 runat="server" ID="ReportType1" />
<uc:AnalysisReportStandardsType2 runat="server" ID="ReportType2" />
<uc:AnalysisReportStandardsType3 runat="server" ID="ReportType3" />

<div style="text-align:right; margin-top:15px;">
    <insite:Button runat="server" ID="DownloadButton" ButtonStyle="Primary" Text="Download (Word *.docx)" Icon="far fa-download" CausesValidation="true" ValidationGroup="Analysis" />
</div>
