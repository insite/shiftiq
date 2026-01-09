<%@ Page Language="C#" CodeBehind="Forms.aspx.cs" Inherits="InSite.UI.Portal.Home.Forms" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<%@ Register Src="./Controls/DashboardNavigation.ascx" TagName="DashboardNavigation" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="SideContent">
    
    <uc:DashboardNavigation runat="server" ID="DashboardNavigation" />

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="StatusAlert" />

    <div class="row">
        <div class="col-lg-12">
        
            <asp:Repeater runat="server" ID="ResponseRepeater">
                <ItemTemplate>
                
                    <div class="card mb-4">
                      <div class="card-body">
                        <h3 class="card-title">
                            <%# Eval("SurveyFormName") %>
                        </h3>
                        <div class="card-text mb-3">
                            <div class="mb-1"><%= GetDisplayText("Started") %> <%# LocalizeTime(Eval("ResponseSessionStarted")) %></div>
                            <div class="mb-1"><%= GetDisplayText("Completed") %> <%# LocalizeTime(Eval("ResponseSessionCompleted")) %></div>
                            <div class="mb-1"><%= GetDisplayText("Current Status") %>: <%# GetDisplayText((string)Eval("ResponseSessionStatus")) %></div>
                            <div class="text-body-secondary"><small><%# Eval("FirstAnswerText") %></small></div>
                        </div>
                        <div class="float-end">
                            <insite:Button runat="server" ID="RestartButton" Icon="fas fa-undo-alt" ToolTip="Start Again" ButtonStyle="Default" />
                            <insite:Button runat="server" ID="DeleteButton" Icon="fas fa-trash-alt" ToolTip="Delete Submission" ButtonStyle="Default" />
                        </div>
                        <insite:Button runat="server" ID="StartButton" ButtonStyle="Default" />
                      </div>
                    </div>
                
                </ItemTemplate>
            </asp:Repeater>

        </div>
    </div>

</asp:Content>