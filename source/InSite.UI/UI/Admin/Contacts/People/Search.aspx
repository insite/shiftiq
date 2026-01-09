<%@ Page Title="" Language="C#" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" AutoEventWireup="true" CodeBehind="Search.aspx.cs" Inherits="InSite.UI.Admin.Contacts.People.Search" %>
<%@ Register Src="./Controls/SearchCriteria.ascx" TagName="SearchCriteria" TagPrefix="uc" %>
<%@ Register Src="./Controls/SearchResults.ascx" TagName="SearchResults" TagPrefix="uc" %>
<%@ Register Src="~/UI/Layout/Common/Controls/SearchDownload.ascx" TagName="SearchDownload" TagPrefix="uc" %>

<asp:Content ContentPlaceHolderID="BodyContent" runat="server">

    <insite:Alert runat="server" ID="SearchAlert" />
    <insite:Alert runat="server" ID="ScreenStatus" />

    <insite:Nav runat="server">
        <insite:NavItem runat="server" ID="SearchResultsTab" Icon="fas fa-database" Title="Results">
            <uc:SearchResults runat="server" ID="SearchResults" />
        </insite:NavItem>
        <insite:NavItem runat="server" ID="SearchCriteriaTab" Icon="fas fa-filter" Title="Criteria">
            <uc:SearchCriteria runat="server" ID="SearchCriteria" />
        </insite:NavItem>
        <insite:NavItem runat="server" ID="BuildMailingListTab" Icon="fas fa-mail-bulk" Title="Build Mailing List">
            <div class="row settings">
                <div class="col-md-6">
                    <h3>Append to Existing Group 
                        <insite:RequiredValidator runat="server" ControlToValidate="GroupID" FieldName="Group" ValidationGroup="Append" />
                    </h3>

                    <div class="form-group mb-3">
                        <div>
                            <insite:FindGroup runat="server" ID="GroupID" />
                        </div>
                        <div style="padding-top:5px;">
                            <insite:Button runat="server" ID="AppendToGroupButton" ButtonStyle="Success" Icon="fas fa-cloud-upload" Text="Append" ValidationGroup="Append" />
                        </div>
                    </div>
                </div>
            </div>        
            
            <div class="row settings">
                <div class="col-md-6">
                    <h3>Add to New Group
                        <insite:RequiredValidator runat="server" ControlToValidate="GroupName" FieldName="Group Name" ValidationGroup="AddToNew" />
                    </h3>

                    <div class="form-group mb-3">
                        <div>
                            <insite:TextBox runat="server" ID="GroupName" MaxLength="90" />
                        </div>
                        <div style="padding-top:5px;">
                            <insite:Button runat="server" ID="AddToNewGroupButton" ButtonStyle="Success" Icon="fas fa-plus-circle" Text="Add New" ValidationGroup="AddToNew" />
                        </div>
                    </div>
                </div>
            </div>
        </insite:NavItem>
        <insite:NavItem runat="server" ID="DownloadsTab" Icon="fas fa-download" Title="Downloads">
            <uc:SearchDownload runat="server" ID="SearchDownload" />
        </insite:NavItem>
    </insite:Nav>
        
</asp:Content>
