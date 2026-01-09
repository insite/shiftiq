<%@ Page Language="C#" CodeBehind="Reports.aspx.cs" Inherits="InSite.UI.Portal.Home.Reports" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<%@ Register Src="./Controls/DashboardNavigation.ascx" TagName="DashboardNavigation" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="SideContent">

    <uc:DashboardNavigation runat="server" ID="DashboardNavigation" />

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <asp:Literal runat="server" ID="TBD">
        TBD
    </asp:Literal>

    <div runat="server" id="TempReportForDemo" visible="false">
        <div class="d-flex justify-content-between mb-4">
            <div>
                <h2>AHRI Capability Analysis</h2>
            </div>
            <div>
                <insite:DownloadButton runat="server" />
                <insite:Button runat="server"
                    Icon="fas fa-chevron-down"
                    ButtonStyle="Default"
                    Text="<span>Expand All</span>"
                    ToolTip="Expand All"
                />
                <insite:Button runat="server"
                    Icon="fas fa-chevron-up"
                    ButtonStyle="Default"
                    Text="<span>Collapse All</span>"
                    ToolTip="Collapse All"
                />
            </div>
        </div>
        <div>
            <img src="Report.jpg" alt="Reprot" />
        </div>
    </div>

</asp:Content>