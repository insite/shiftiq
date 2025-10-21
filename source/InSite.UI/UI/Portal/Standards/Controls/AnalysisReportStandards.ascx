<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AnalysisReportStandards.ascx.cs" Inherits="InSite.UI.Portal.Standards.Controls.AnalysisReportStandards" %>

<%@ Register TagPrefix="uc" TagName="AnalysisReportStandardsType1" Src="AnalysisReportStandardsType1.ascx" %>
<%@ Register TagPrefix="uc" TagName="AnalysisReportStandardsType2" Src="AnalysisReportStandardsType2.ascx" %>
<%@ Register TagPrefix="uc" TagName="AnalysisReportStandardsType3" Src="AnalysisReportStandardsType3.ascx" %>

<uc:AnalysisReportStandardsType1 runat="server" ID="ReportType1" />
<uc:AnalysisReportStandardsType2 runat="server" ID="ReportType2" />
<uc:AnalysisReportStandardsType3 runat="server" ID="ReportType3" />

<div class="mb-3" style="text-align:right;">
    <insite:Button runat="server" ID="DownloadButton" ButtonStyle="Primary" Text="Download (Word *.docx)" Icon="far fa-download" CausesValidation="true" ValidationGroup="Analysis" />
</div>