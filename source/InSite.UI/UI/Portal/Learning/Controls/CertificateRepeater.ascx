<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CertificateRepeater.ascx.cs" Inherits="InSite.UI.Portal.Learning.Controls.CertificateRepeater" %>

<asp:Literal runat="server" ID="NoResults" Text="There are no certificates here." />

<asp:Repeater runat="server" ID="Repeater">
    <HeaderTemplate>
        <div class="table-responsive">
            <table class="table table-hover">
                <thead>
                    <tr>
                        <th><%# Translate("Achievement") %></th>
                        <th><%# Translate("Issued") %></th>
                        <th><%# Translate("Expiry") %></th>
                        <th><%# Translate("Status") %></th>
                        <th></th>
                    </tr>
                </thead>
                <tbody>
    </HeaderTemplate>
    <ItemTemplate>
        <tr>
            <th>
                <%# Eval("AchievementTitle") %>
                <div class="form-text">
                    <%# Eval("AchievementLabel") %>
                </div>
            </th>
            <td class="text-nowrap">
                <%# GetDateString((DateTimeOffset?)Eval("CredentialGranted")) %>
            </td>
            <td class="text-nowrap">
                <%# GetDateString((DateTimeOffset?)Eval("CredentialExpirationExpected")) %>
            </td>
            <td class="text-nowrap">
                <%# GetStatusHtml((string)Eval("CredentialStatus")) %>
            </td>
            <td>
                <%# GetCertificateDownloadLink() %>
                <insite:Button runat="server" 
                    CommandName="DownloadBadge" 
                    ButtonStyle="Info" 
                    Visible='<%# IsBadgeConfigured() %>' 
                    Text="<i class='far fa-award me-2'></i> Download" />
            </td>
        </tr>
    </ItemTemplate>
    <FooterTemplate>
                </tbody>
            </table>
        </div>
    </FooterTemplate>
</asp:Repeater>
