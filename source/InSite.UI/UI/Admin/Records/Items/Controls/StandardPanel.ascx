<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StandardPanel.ascx.cs" Inherits="InSite.Admin.Records.Items.Controls.StandardPanel" %>

<insite:UpdateProgress runat="server" AssociatedUpdatePanelID="UpdatePanel" />

<insite:UpdatePanel runat="server" ID="UpdatePanel">
    <ContentTemplate>
        <insite:FindStandard runat="server" ID="Standard" Width="499" />
        <insite:RequiredValidator runat="server" ControlToValidate="Standard" FieldName="Standard" ValidationGroup="AddStandard" Display="Dynamic" RenderMode="Dot" />

        <insite:Button runat="server" ID="AddButton" ButtonStyle="Default" ValidationGroup="AddStandard" Text="Add" Icon="fas fa-plus-circle" />

        <div style="margin-bottom:15px;">
            <asp:Repeater runat="server" ID="StandardRepeater">
                <HeaderTemplate>
                    <table class="table table-striped" style="margin-top:15px;">
                        <tbody>
                </HeaderTemplate>
                <FooterTemplate>
                    </tbody></table>
                </FooterTemplate>
                <ItemTemplate>
                    <tr>
                        <td>
                            <%# Eval("Title") %>
                        </td>
                        <td style="width:40px;">
                            <insite:IconButton runat="server" CommandName="Delete" Name="trash-alt" CommandArgument='<%# Eval("Identifier") %>' ToolTip="Remove Standard" OnClientClick="return confirm('Are you sure to remove this standard')" />
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </ContentTemplate>
</insite:UpdatePanel>