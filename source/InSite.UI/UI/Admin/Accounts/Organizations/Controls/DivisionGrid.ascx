<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DivisionGrid.ascx.cs" Inherits="InSite.Admin.Accounts.Organizations.Controls.DivisionGrid" %>

<p runat="server" ID="EmptyGrid" class="help">This organization has no divisions.</p>

<insite:Grid runat="server" ID="Grid">
    
    <Columns>

        <asp:TemplateField HeaderText="Name">
            <ItemTemplate>
                <a href='<%# Eval("DivisionIdentifier", "/ui/admin/accounts/divisions/edit?id={0}") %>'><%# Eval("DivisionName") %></a>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Code">
            <ItemTemplate>
                <%# Eval("DivisionCode") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Created">
            <ItemTemplate>
                <%# LocalDate(Eval("Created")) %>
            </ItemTemplate>
        </asp:TemplateField>

    </Columns>
    
</insite:Grid>