<%@ Page Language="C#" AutoEventWireup="false" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" CodeBehind="CreatePerformanceReport.aspx.cs" Inherits="InSite.UI.Admin.Assessments.Attempts.CreatePerformanceReport" %>

<%@ Register Src="./Controls/PerformanceReportDetail.ascx" TagName="PerformanceReportDetail" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <uc:PerformanceReportDetail runat="server" ID="Detail" />

    <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Report" />
    <insite:CancelButton runat="server" ID="CancelButton" />

</asp:Content>