<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AccommodationManager.ascx.cs" Inherits="InSite.UI.Admin.Integrations.Tests.Controls.AccommodationManager" %>

<asp:Repeater runat="server" ID="Repeater">
    <HeaderTemplate>
        <table class="table table-sm table-stripped mb-3">
            <tbody>
    </HeaderTemplate>
    <FooterTemplate>
            </tbody>
        </table>
    </FooterTemplate>
    <ItemTemplate>
        <tr>
            <td>
                <insite:RequiredValidator runat="server" ControlToValidate="TypeSelector" FieldName="Accommodation Type" ValidationGroup="D365" RenderMode="Dot" Display="None" />
                <insite:AccommodationTypeComboBox runat="server" ID="TypeSelector" AllowBlank="true" EnableSearch="true" EmptyMessage="Type" ButtonSize="Small" />
            </td>
            <td>
                <insite:TextBox runat="server" ID="Name" MaxLength="100" EmptyMessage="Name" CssClass="form-control-sm" />
            </td>
            <td style="width:50px;">
                <insite:IconButton runat="server"
                    CommandName="Delete"
                    Name="trash-alt"
                    ToolTip="Remove Accommodation"
                    ConfirmText="Are you sure you to remove this accommodation?"
                />
            </td>
        </tr>
    </ItemTemplate>
</asp:Repeater>
<div>
    <insite:AddButton runat="server" ID="AddButton" />
</div>
