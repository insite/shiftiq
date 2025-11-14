<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AuthorCommentGrid.ascx.cs" Inherits="InSite.UI.Admin.Jobs.Candidates.Controls.AuthorCommentGrid" %>

<div>

    <insite:Grid runat="server" ID="Grid" DataKeyNames="CommentIdentifier">
        <Columns>

            <asp:BoundField HeaderText="Author" DataField="AuthorName"></asp:BoundField>
            <asp:BoundField HeaderText="Subject" DataField="CandidateName"></asp:BoundField>
            <asp:BoundField HeaderText="Text" DataField="CommentText"></asp:BoundField>
            <asp:TemplateField HeaderText="Last Modified">
                <ItemTemplate>
                    <%# LocalizeTime(Eval("CommentModified")) %>
                </ItemTemplate>
            </asp:TemplateField>
            <insite:TemplateField ItemStyle-Width="40px" FieldName="Commands" ItemStyle-Wrap="False">
                <ItemTemplate>
                    <insite:IconButton runat="server" ID="EditCommand" Name="pencil" OnClientClick='<%# Eval("CommentIdentifier", "contactCommentGrid.showEditor(\"{0}\"); return false;") %>' ToolTip="Edit" />
                    <insite:IconButton runat="server" Name="trash-alt" ToolTip="Delete" CommandName="Delete" ConfirmText="Are you sure you want to delete this comment?" />
                </ItemTemplate>
            </insite:TemplateField>

        </Columns>
    </insite:Grid>

</div>