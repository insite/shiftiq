<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.Admin.Utilities.Constraints.Controls.SearchResults" %>

<asp:Literal runat="server" ID="Instructions" />

<insite:Grid runat="server" ID="Grid">
    <Columns>

        <asp:TemplateField HeaderText="Foreign Schema">
            <ItemTemplate>
                <%# Eval("ForeignSchemaName") %>
            </ItemTemplate>
        </asp:TemplateField>
            
        <asp:TemplateField HeaderText="Foreign Table">
            <ItemTemplate>
                <a href="/ui/admin/database/tables/outline?<%# string.Format("schemaName={0}&tableName={1}", Eval("ForeignSchemaName"), Eval("ForeignTableName")) %>"><%# Eval("ForeignTableName") %></a>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Foreign Column">
            <ItemTemplate>
                <a href="/ui/admin/database/columns/outline?<%# string.Format("schemaName={0}&tableName={1}&columnName={2}", Eval("ForeignSchemaName"), Eval("ForeignTableName"), Eval("ForeignColumnName")) %>"><%# Eval("ForeignColumnName") %></a>
            </ItemTemplate>
        </asp:TemplateField>
            
        <asp:TemplateField HeaderText="Primary Schema">
            <ItemTemplate>
                <%# Eval("PrimarySchemaName") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Primary Table">
            <ItemTemplate>
                <a href="/ui/admin/database/tables/outline?<%# string.Format("schemaName={0}&tableName={1}", Eval("PrimarySchemaName"), Eval("PrimaryTableName")) %>"><%# Eval("PrimaryTableName") %></a>
            </ItemTemplate>
        </asp:TemplateField>
            
        <asp:TemplateField HeaderText="Primary Column">
            <ItemTemplate>
                <a href="/ui/admin/database/columns/outline?<%# string.Format("schemaName={0}&tableName={1}&columnName={2}", Eval("PrimarySchemaName"), Eval("PrimaryTableName"), Eval("PrimaryColumnName")) %>"><%# Eval("PrimaryColumnName") %></a>
            </ItemTemplate>
        </asp:TemplateField>
            
        <asp:TemplateField HeaderText="Status">
            <ItemTemplate>
                <span class="badge bg-danger" runat="server" ID="NotEnforced"><i class="far fa-exclamation"></i> Not Enforced</span>
                <span class="badge bg-success" runat="server" ID="Enforced"><i class="far fa-check"></i> Enforced</span>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField ItemStyle-Width="20px">
            <ItemTemplate>
                <insite:IconLink runat="server" Name="database" ToolTip="View" NavigateUrl='<%# Eval("UniqueName", "/ui/admin/database/constraints/outline?constraintName={0}") %>' />
            </ItemTemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>