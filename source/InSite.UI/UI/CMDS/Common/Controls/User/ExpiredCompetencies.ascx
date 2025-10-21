<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ExpiredCompetencies.ascx.cs" Inherits="InSite.Cmds.Controls.User.ExpiredCompetencies" %>

<asp:Repeater ID="rpCompetencies" runat="server">
    <HeaderTemplate>
        <table>
    </HeaderTemplate>
    <ItemTemplate>
        <tr>
            <td>
                <cmds:Flag runat="server" Type='<%# GetCompetencyFlag(Container.DataItem) %>' />
                &nbsp;
                <%# GetCompetencyText(Container.DataItem) %>
            </td>
        </tr>
    </ItemTemplate>
    <FooterTemplate>
        </table>
    </FooterTemplate>
</asp:Repeater>