<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.Admin.Metadata.Columns.Controls.SearchResults" %>

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
        <asp:TemplateField HeaderText="Column">
            <ItemTemplate>
                <a href="/ui/admin/database/columns/outline?<%# string.Format("schemaName={0}&tableName={1}&columnName={2}", Eval("SchemaName"), Eval("TableName"), Eval("ColumnName")) %>"><%# Eval("ColumnName") %></a>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:BoundField HeaderText="Data Type" DataField="DataType" />
        <asp:BoundField HeaderText="Maximum Length" DataField="MaximumLength" ItemStyle-CssClass="text-end" HeaderStyle-CssClass="text-end" />
        <asp:TemplateField HeaderText="Required">
            <ItemTemplate>
                <%# (Boolean)Eval("IsRequired") ? "Required" : String.Empty %>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</insite:Grid>