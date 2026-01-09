<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.UI.Admin.Sales.Taxes.Controls.SearchResults" %>

<asp:Literal id="Instructions" runat="server" />

<insite:Grid runat="server" ID="Grid">
    <Columns>

        <asp:TemplateField ItemStyle-Wrap="False" ItemStyle-Width="20px">
            <ItemTemplate>
                <insite:IconLink runat="server"
                    Name="pencil"
                    ToolTip="Edit"
                    NavigateUrl='<%# Eval("TaxIdentifier", "/ui/admin/sales/taxes/edit?id={0}") %>'
                />
                <insite:IconLink runat="server"
                    Name="trash-alt"
                    ToolTip="Delete"
                    NavigateUrl='<%# Eval("TaxIdentifier", "/ui/admin/sales/taxes/delete?id={0}") %>'
                />
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Name"> 
            <ItemTemplate>
                <a href='<%# Eval("TaxIdentifier", "/ui/admin/sales/taxes/edit?id={0}") %>'><%# Eval("TaxName") %></a>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:BoundField HeaderText="Country" DataField="CountryName" />

        <asp:BoundField HeaderText="Province / State" DataField="RegionName" />

        <asp:TemplateField HeaderText="Tax Percent" HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right"> 
            <ItemTemplate>
                <%# Eval("TaxPercent", "{0:n2}") %>%
            </ItemTemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>