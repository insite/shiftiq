<%@ Page Language="C#" CodeBehind="Environments.aspx.cs" Inherits="InSite.UI.Portal.Settings.Environments.Select" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <div class="row">
        <div class="col-lg-12">
            <div runat="server" id="EnvironmentPanel">
                
                <insite:Button runat="server" ID="GoToProduction" ButtonStyle="Success" Icon="fas fa-home" Text="Production" />
                <insite:Button runat="server" ID="GoToSandbox" ButtonStyle="Warning" Icon="fas fa-presentation" Text="Sandbox" />
                <insite:Button runat="server" ID="GoToDevelopment" ButtonStyle="Danger" Icon="fas fa-laptop-code" Text="Development" />

            </div>
        </div>
    </div>

</asp:Content>