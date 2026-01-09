<%@ Page Language="C#" CodeBehind="RequestAccess.aspx.cs" Inherits="InSite.UI.Lobby.RequestAccess" MasterPageFile="~/UI/Layout/Lobby/LobbyLogin.master" %>

<asp:Content ContentPlaceHolderID="BodyContent" runat="server">

<div class="row">
	<div class="col-lg-4 col-md-6 offset-lg-1">

<!-- Sign in view-->
<div class="view show" id="signin-view">

    <h1 class="h2"><insite:Literal runat="server" Text="Request Organization Access" /></h1>

    <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="UpdatePanel" />

    <insite:UpdatePanel runat="server" ID="UpdatePanel" ChildrenAsTriggers="false">
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="RequestButton" />
            <asp:AsyncPostBackTrigger ControlID="CloseButton" />
        </Triggers>
        <ContentTemplate>

            <insite:Alert runat="server" ID="ScreenStatus" />
            
            <insite:ValidationSummary runat="server" ValidationGroup="Access" />

            <asp:MultiView runat="server" ID="ScreenViews">

                <asp:View runat="server" ID="ViewRequest">

                    <div runat="server" id="RequestOrganizationField" class="form-group mb-3" visible="false">
                        <label class="form-label">
                            <insite:Literal runat="server" Text="Organization" />
                            <insite:RequiredValidator runat="server" ControlToValidate="RequestOrganizationSelector" FieldName="Organization" Display="None" ValidationGroup="Access" />
                        </label>
                        <insite:FindOrganization runat="server" ID="RequestOrganizationSelector" />
                    </div>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            <insite:Literal runat="server" Text="Email" />
                            <insite:RequiredValidator runat="server" ControlToValidate="RequestUserName" FieldName="Email" Display="None" ValidationGroup="Access" />
                        </label>
                        <insite:TextBox runat="server" ID="RequestUserName" MaxLength="128" autocomplete="off" TabIndex="1" Enabled="false" />
                    </div>

                    <div style="padding-top:20px;">
                        <insite:Button runat="server" ID="RequestButton" ButtonStyle="Success" Icon="fas fa-paper-plane" Text="Submit" ValidationGroup="Access" />
                        <insite:CloseButton runat="server" ID="CloseButton" />
                    </div>

                </asp:View>

                <asp:View runat="server" ID="ViewAccept">


                </asp:View>

            </asp:MultiView>
    
        </ContentTemplate>
    </insite:UpdatePanel>

</div>
        
    </div>
</div>

</asp:Content>