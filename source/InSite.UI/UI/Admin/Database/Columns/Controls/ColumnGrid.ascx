<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ColumnGrid.ascx.cs" Inherits="InSite.Admin.Metadata.Columns.Controls.ColumnGrid" %>

<insite:Grid runat="server" ID="Grid">
    <Columns>

        <asp:TemplateField HeaderText="Column" SortExpression="ColumnName">
            <ItemTemplate>
                <a href="/ui/admin/database/columns/outline?<%# string.Format("schemaName={0}&tableName={1}&columnName={2}", Eval("SchemaName"), Eval("TableName"), Eval("ColumnName")) %>"><%# Eval("ColumnName") %></a>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:BoundField HeaderText="Data Type" DataField="DataType" />
        <asp:BoundField HeaderText="Maximum Length" DataField="MaximumLength" HeaderStyle-CssClass="text-end" ItemStyle-CssClass="text-end" />
        <asp:TemplateField HeaderText="Required" SortExpression="IsRequired">
            <ItemTemplate>
                <%# (bool)Eval("IsRequired") ? "Required" : String.Empty %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField ItemStyle-Width="20px">
            <ItemTemplate>
                <insite:IconLink runat="server" Name="database" ToolTip="View"
                    NavigateUrl='<%# string.Format("/ui/admin/database/columns/outline?schemaName={0}&tableName={1}&columnName={2}", Eval("SchemaName"), Eval("TableName"), Eval("ColumnName")) %>' />
            </ItemTemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>
