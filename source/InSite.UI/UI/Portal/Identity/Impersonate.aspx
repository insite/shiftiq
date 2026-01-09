<%@ Page Language="C#" CodeBehind="Impersonate.aspx.cs" Inherits="InSite.UI.Portal.Accounts.Users.Impersonate" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="SideContent">

    <h3 class="rounded-top d-block bg-secondary fs-sm fw-semibold text-body-secondary mb-0 px-4 py-3 rounded-top">About Impersonation</h3>
    <div class="p-4">
        <p>The following rules help to ensure impersonation is a secure operation:</p>
        <ul>
            <li><insite:Literal runat="server" Text="While impersonating another user, an administrator is not permitted to impersonate yet another user. In other words, an impersonator cannot impersonate, and instead must stop impersonating one user before proceeding to impersonate a different user." /></li>
            <li><insite:Literal runat="server" Text="While impersonating another user, an administrator cannot select a different organization account for the current session. Impersonation can occur only within the context of the active organization for the administrator's session." /></li>
            <li><insite:Literal runat="server" Text="Impersonation is allowed only if the administrator (performing the impersonation) and the user (being impersonated) are both assigned to the active organization." /></li>
        </ul>
    </div>

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <div class="row pb-4">
        <div class="col-lg-6">
            <span class="fs-ms text-body-secondary"><insite:Literal runat="server" Text="User" /></span>
            <span class="d-block fs-sm">
                <asp:Literal runat="server" ID="DetailUserToImpersonate" />
            </span>
        </div>
        <div class="col-lg-6">
            <span class="fs-ms text-body-secondary"><insite:Literal runat="server" Text="Impersonator" /></span>
            <span class="d-block fs-sm">
                <asp:Literal runat="server" ID="DetailAdministratorName" />
            </span>
            <ul class="d-block fs-sm mt-2">
                <asp:Repeater runat="server" ID="DetailAdministratorGroups">
                    <ItemTemplate>
                        <li>
                            <%# Eval("Name") %>
                        </li>
                    </ItemTemplate>
                </asp:Repeater>
            </ul>
        </div>
    </div>
        
    <insite:Alert runat="server" ID="ImpersonationStatus" />

    <div class="row">
        <div class="col-lg-12">
            <insite:Button runat="server" ID="ContinueButton" Text="Continue" ButtonStyle="Success" Icon="fas fa-arrow-alt-right" IconPosition="AfterText" Visible="false" />
            <insite:Button runat="server" ID="StopButton" Text="Stop Impersonating" ButtonStyle="Primary" Icon="fas fa-power-off" Visible="false" NavigateUrl="/ui/portal/identity/impersonate" />
        </div>
    </div>

</asp:Content>
