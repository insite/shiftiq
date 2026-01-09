<%@ Page Language="C#" CodeBehind="Logbooks.aspx.cs" Inherits="InSite.UI.Portal.Home.Logbooks" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<%@ Register Src="./Controls/DashboardNavigation.ascx" TagName="DashboardNavigation" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="SideContent">

    <uc:DashboardNavigation runat="server" ID="DashboardNavigation" />

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

<!-- Content-->
<div class="row">
    <div class="col-lg-12">
        
        <insite:Alert runat="server" ID="StatusAlert" />
        
        <asp:Repeater runat="server" ID="JournalRepeater">
            <ItemTemplate>
                
                <div class="card mb-4">
                  <div class="card-body">
                    <h3 class="card-title">
                        <%# Eval("Title") %>
                    </h3>
                    <div class="card-text mb-3">
                        <div class="mb-1"><%= GetDisplayText("Number of Entries") %>: <%# Eval("ExperienceCount", "{0:n0}") %></div>
                    </div>
                    <div class="float-end">
                        <insite:Button runat="server" ID="AddEntryButton" Icon="fas fa-plus-circle" Text="Add Entry" ButtonStyle="Default"
                            NavigateUrl='<%# Eval("JournalSetupIdentifier", "/ui/portal/records/logbooks/learners/add-experience?journalsetup={0}") %>' 
                            Visible='<%# !((DateTimeOffset?)Eval("JournalSetupLocked")).HasValue %>'
                        />
                    </div>
                    <insite:Button runat="server" ID="ViewButton" Icon="far fa-search" Text="View" ButtonStyle="Info"
                        NavigateUrl='<%# Eval("JournalSetupIdentifier", "/ui/portal/records/logbooks/outline?journalsetup={0}") %>'
                    />
                  </div>
                </div>
                
            </ItemTemplate>
        </asp:Repeater>

    </div>
</div>

</asp:Content>