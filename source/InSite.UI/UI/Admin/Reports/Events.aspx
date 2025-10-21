<%@ Page Language="C#" CodeBehind="Events.aspx.cs" Inherits="InSite.Admin.Events.Reports.Forms.Dashboard" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>
<%@ Register Src="./Controls/EventSummary.ascx" TagName="EventSummary" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    
    <insite:Alert runat="server" ID="HomeStatus" />

    <section runat="server" ID="SummaryPanel" class="mb-3">
        <uc:EventSummary runat="server" ID="EventSummary" />
    </section>

</asp:Content>
