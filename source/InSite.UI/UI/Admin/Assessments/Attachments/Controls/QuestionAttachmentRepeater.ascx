<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="QuestionAttachmentRepeater.ascx.cs" Inherits="InSite.Admin.Assessments.Attachments.Controls.QuestionAttachmentRepeater" %>

<asp:Repeater runat="server" ID="Repeater">
    <HeaderTemplate>
        <table class="table table-condensed">
            <thead>
                <tr>
                    <th>Type</th>
                    <th>Asset</th>
                    <th>Title</th>
                    <th>Publication Status</th>
                    <th></th>
                </tr>
            </thead>
            <tbody>
    </HeaderTemplate>
    <FooterTemplate>
            </tbody>
        </table>
    </FooterTemplate>
    <ItemTemplate>
        <tr>
            <td><%# Eval("TypeName") %></td>
            <td><%# Eval("AssetNumber") %>.<%# Eval("AssetVersion") %></td>
            <td><%# Eval("Title") %></td>
            <td><%# Eval("PublicationStatus") %></td>
            <td class="text-end">
                <insite:Button runat="server" NavigateTarget="_blank" NavigateUrl='<%# (string)Eval("Url") + "?download=1" %>' ButtonStyle="Primary" ToolTip="Download" Icon="fas fa-download" />
            </td>
        </tr>
    </ItemTemplate>
</asp:Repeater>
