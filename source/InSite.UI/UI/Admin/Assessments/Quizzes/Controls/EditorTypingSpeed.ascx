<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EditorTypingSpeed.ascx.cs" Inherits="InSite.UI.Admin.Assessments.Quizzes.Controls.EditorTypingSpeed" %>

<asp:Repeater runat="server" ID="ItemRepeater">
    <HeaderTemplate>
        <table class="table table-striped"><tbody>
    </HeaderTemplate>
    <FooterTemplate>
        </tbody></table>
    </FooterTemplate>
    <ItemTemplate>
        <tr>
            <td class="text-end" style="width:65px;">
                <strong><%# Eval("Letter") %></strong>
                <insite:RequiredValidator runat="server" ID="TextRequiredValidator" FieldName='<%# Eval("Letter", "Item Text ({0})") %>' ControlToValidate="Text" />
            </td>
            <td>
                <insite:TextBox runat="server" ID="Text" TextMode="MultiLine" Rows="5" Text='<%# Eval("Text") %>' />
            </td>
            <td class="text-nowrap" style="width:30px;">
                <insite:IconButton runat="server" CommandName="Delete" Name="trash-alt" ToolTip="Delete" CausesValidation="false" />
            </td>
        </tr>
    </ItemTemplate>
</asp:Repeater>

<div class="mt-3">
    <insite:Button runat="server" ID="AddItemButton" Icon="fas fa-plus-circle" Text="Add New Item" ButtonStyle="Default" CausesValidation="false" />
</div>
