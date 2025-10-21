<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ContentSectionViewer.ascx.cs" Inherits="InSite.Admin.Sites.Pages.Controls.ContentSectionViewer" %>

<div class="form-group px-4">
    <div class="float-end" style="margin-left:20px">
        <div class="content-cmds" style="position:inherit;">
            <insite:Button runat="server" id="EditLink" ToolTip="Revise" ButtonStyle="Default" Text="Edit" Icon="far fa-pencil" />
        </div>
    </div>
    <div>
        <asp:Repeater runat="server" ID="ValueRepeater">
            <HeaderTemplate><table></HeaderTemplate>
            <FooterTemplate></table></FooterTemplate>
            <ItemTemplate>
                <tr>
                    <td class="align-top" style="width:30px;">
                        <strong><%# Eval("Language") %></strong>:
                    </td>
                    <td>
                        <%# Eval("Text") %>
                    </td>
                </tr>
            </ItemTemplate>
        </asp:Repeater>
    </div>
</div>
