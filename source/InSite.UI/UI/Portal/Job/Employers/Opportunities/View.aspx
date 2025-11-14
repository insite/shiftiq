<%@ Page Title="Jobs | Candidates | Opportunities | View" Language="C#" CodeBehind="View.aspx.cs" Inherits="InSite.UI.Portal.Jobs.Employers.Opportunities.View" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<%@ Register Src="Controls/ViewDetails.ascx" TagName="Details" TagPrefix="uc" %>
<%@ Register Src="Controls/CandidatesGrid.ascx" TagName="CandidatesGrid" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <insite:Alert runat="server" ID="StatusAlert" />

    <asp:Label runat="server" ID="Instructions" />

    <div runat="server" id="Content" class="row"></div>

    <div runat="server" id="EditLinkPanel" class="row text-end mb-3 px-3">
        <div class="col-md-12">
            <insite:Button runat="server" ID="EditLink" Text="Edit" Visible="true" ButtonStyle="Default" Icon="far fa-fw fa-magnifying-glass" />
        </div>         
    </div>
    
    <uc:CandidatesGrid runat="server" ID="CandidatesGrid" visible="False" />

    <uc:Details ID="Details" runat="server" />

    <div class="row">
        <div class="col-10">
            <insite:Button runat="server" ID="BackLink" Text="Back" Visible="true" ButtonStyle="Default" Icon="far fa-fw fa-arrow-left" />
        </div>
    </div>

</asp:Content>