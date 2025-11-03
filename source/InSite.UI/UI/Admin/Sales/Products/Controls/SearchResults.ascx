<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.Admin.Invoices.Products.Controls.SearchResults" %>

<asp:Literal id="Instructions" runat="server" />

<insite:Grid runat="server" ID="Grid" DataKeyNames="ProductIdentifier">
    <Columns>

        <asp:TemplateField HeaderText="Product Name"> 
            <ItemTemplate>
                <a href='/ui/admin/sales/products/edit?<%# Eval("ProductIdentifier", "id={0}") %>'><%# Eval("ProductName") %></a>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Product Description" ItemStyle-Wrap="True"> 
            <ItemTemplate>
                 <%# Eval("ProductDescription") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Product Type">
            <ItemTemplate>
                <%# Eval("ProductType") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Publication">
            <ItemTemplate>
                <%# Eval("Published") != null ? "Published" : "Unpublished" %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField ItemStyle-Wrap="False" ItemStyle-Width="20px">
            <ItemTemplate>
                <insite:IconLink runat="server" Name="pencil" ToolTip="Edit"
                    NavigateUrl='<%# Eval("ProductIdentifier", "/ui/admin/sales/products/edit?id={0}") %>' />
                <insite:IconLink runat="server" Name="trash-alt" ToolTip="Delete"
                    NavigateUrl='<%# Eval("ProductIdentifier", "/admin/sales/products/delete?id={0}") %>' />
            </ItemTemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>