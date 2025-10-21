<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.Admin.Utilities.Tables.Controls.SearchResults" %>

<asp:Literal runat="server" ID="Instructions" />

<insite:Grid runat="server" ID="Grid">
    <Columns>

        <asp:TemplateField HeaderText="Schema">
            <ItemTemplate>
                <%# Eval("SchemaName") %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Table">
            <ItemTemplate>
                <a href="/ui/admin/database/tables/outline?<%# string.Format("schemaName={0}&tableName={1}", Eval("SchemaName"), Eval("TableName")) %>"><%# Eval("TableName") %></a>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:BoundField HeaderText="Columns" DataField="ColumnCount" DataFormatString="{0:n0}" ItemStyle-CssClass="text-end" HeaderStyle-CssClass="text-end" />
        <asp:BoundField HeaderText="Rows" DataField="RowCount" DataFormatString="{0:n0}" ItemStyle-CssClass="text-end" HeaderStyle-CssClass="text-end" />

    </Columns>
</insite:Grid>