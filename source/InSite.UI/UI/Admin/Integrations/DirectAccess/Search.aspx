<%@ Page Language="C#" CodeBehind="Search.aspx.cs" Inherits="InSite.UI.Admin.Integrations.DirectAccess.Search" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register TagPrefix="uc" TagName="SearchCriteria" Src="./Controls/SearchCriteria.ascx" %>
<%@ Register TagPrefix="uc" TagName="SearchResults" Src="./Controls/SearchResults.ascx" %>
<%@ Register TagPrefix="uc" TagName="SearchDownload" Src="~/UI/Layout/Common/Controls/SearchDownload.ascx" %>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="ScreenStatus" />

    <insite:Nav runat="server">
        <insite:NavItem runat="server" ID="SearchResultsTab" Icon="fas fa-database" Title="Results">
            <uc:SearchResults runat="server" ID="SearchResults" />
        </insite:NavItem>
        <insite:NavItem runat="server" ID="SearchCriteriaTab" Icon="fas fa-filter" Title="Criteria">
            <uc:SearchCriteria runat="server" ID="SearchCriteria" />
        </insite:NavItem>
        <insite:NavItem runat="server" ID="DownloadsTab" Icon="fas fa-download" Title="Downloads">
            <uc:SearchDownload runat="server" ID="SearchDownload" />
        </insite:NavItem>
        <insite:NavItem runat="server" Title="Direct Access">
                 
            

<div class="row">
    <div class="col-lg-12">
        <div id="toolbox" class="toolbox-section">
            <h4 class="mb-0">Criteria</h4>
            <div class="text-body-secondary fs-sm mb-2">
                Use the Direct Access Web API to request records for individuals
                that may or may not be cached in Shift iQ.
            </div>
            <div class="row">
                <div class="col-6">

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="IndividualId" EmptyMessage="Individual ID(s)" TextMode="MultiLine" Height="151" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="FirstName" EmptyMessage="First Name" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="LastName" EmptyMessage="Last Name" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="Email" EmptyMessage="Email" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="Program" EmptyMessage="Program" />
                    </div>

                    <insite:Button runat="server" ID="RequestButton" Text="Request" Icon="far fa-plug" 
                        OnClientClick="return confirm('Are you sure you want to search Direct Access? It may take a few minutes to return your search results.');" />
                    <insite:ClearButton runat="server" ID="ClearButton" />

                    

                    <insite:ProgressPanel runat="server" ID="RequestProgress" HeaderText="Waiting for Direct Access..." Cancel="PostBack">
                        <Triggers>
                            <insite:ProgressControlTrigger ControlID="RequestButton" />
                        </Triggers>
                        <Items>
                            <insite:ProgressIndicator Name="Progress" Caption="Completed: {percent}%" />
                            <insite:ProgressStatus Text="Status: {status}{running_ellipsis}" />
                            <insite:ProgressStatus Text="Elapsed time: {time_elapsed}s" />
                        </Items>
                    </insite:ProgressPanel>

                </div>
            </div>
        </div>
    </div>
</div>

        </insite:NavItem>

    </insite:Nav>

</asp:Content>
