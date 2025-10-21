<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ConstraintGrid.ascx.cs" Inherits="InSite.Admin.Utilities.Constraints.Controls.ConstraintGrid" %>

<insite:Grid runat="server" ID="Grid">
    <Columns>
        
        <asp:TemplateField HeaderText="Foreign Schema">
            <itemtemplate>
                <%# Eval("ForeignSchemaName") %>
            </itemtemplate>
        </asp:TemplateField>
        
        <asp:TemplateField HeaderText="Foreign Table">
            <itemtemplate>
                <a href="/ui/admin/database/tables/outline?<%# string.Format("schemaName={0}&tableName={1}", Eval("ForeignSchemaName"), Eval("ForeignTableName")) %>"><%# Eval("ForeignTableName") %></a>
            </itemtemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Column">
            <itemtemplate>
                <a href="/ui/admin/database/columns/outline?<%# string.Format("schemaName={0}&tableName={1}&columnName={2}", Eval("ForeignSchemaName"), Eval("ForeignTableName"), Eval("ForeignColumnName")) %>"><%# Eval("ForeignColumnName") %></a>
            </itemtemplate>
        </asp:TemplateField>
        
        <asp:TemplateField HeaderText="Primary Schema">
            <itemtemplate>
                <%# Eval("PrimarySchemaName") %>
            </itemtemplate>
        </asp:TemplateField>
        
        <asp:TemplateField HeaderText="Primary Table">
            <itemtemplate>
                <a href="/ui/admin/database/tables/outline?<%# string.Format("schemaName={0}&tableName={1}", Eval("PrimarySchemaName"), Eval("PrimaryTableName")) %>"><%# Eval("PrimaryTableName") %></a>
            </itemtemplate>
        </asp:TemplateField>

        <asp:TemplateField ItemStyle-Width="20px">
            <itemtemplate>
                <insite:IconLink runat="server" Name="key" ToolTip="View" 
                    NavigateUrl='<%# Eval("UniqueName", "/ui/admin/database/constraints/outline?constraintName={0}") %>' />
            </itemtemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>
