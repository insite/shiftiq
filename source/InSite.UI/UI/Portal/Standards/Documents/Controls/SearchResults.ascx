<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.UI.Portal.Standards.Documents.Controls.SearchResults" %>

<insite:Literal runat="server" ID="Instructions" />

<div class="table-responsive">
    <insite:Grid runat="server" ID="Grid" Translation="Header" OnRowCreated="Grid_RowDataBound">
        <Columns>
            <asp:TemplateField HeaderText="Document Title">
                <ItemTemplate>
                    <a href='<%# GetOutlineUrl() %>'>
                        <%# Eval("TranslatedTitle") %>
                    </a>
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Posted">
                <ItemTemplate>
                    <%# GetDateString(Eval("DatePosted") as DateTimeOffset?) %>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Document">
                <ItemTemplate>
                    <%# Translate((string)Eval("DocumentType")) %>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Template">
                <ItemTemplate>
                    <%# (bool)Eval("IsTemplate") ? Translate("Yes") : "" %>
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Action" HeaderStyle-Width="40px" ItemStyle-Wrap="false" ItemStyle-CssClass="text-end" HeaderStyle-CssClass="text-end">
                <ItemTemplate>
                    <insite:Button Icon="far fa-trash-alt" runat="server" CommandName="DeleteDocument" CommandArgument='<%# Eval("StandardIdentifier") %>' Style="padding: 8px;"
                        ToolTip="Delete Document" ConfirmText="Are you sure to delete this document?" ButtonStyle="Default" />
                </ItemTemplate>
            </asp:TemplateField>

        </Columns>
    </insite:Grid>
</div>