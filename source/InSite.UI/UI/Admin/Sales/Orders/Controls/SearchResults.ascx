<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.UI.Admin.Sales.Orders.Controls.SearchResults" %>

<asp:Literal ID="Instructions" runat="server" />

<insite:Grid runat="server" ID="Grid" DataKeyNames="OrderIdentifier">
    <Columns>

        <asp:BoundField HeaderText="Customer" DataField="CustomerFullName" />
        <asp:BoundField HeaderText="Order" DataField="ProductName" />
        <asp:BoundField HeaderText="URL" DataField="ProductUrl" />
        <asp:BoundField HeaderText="Quantity" DataField="TotalQuantity" DataFormatString="{0:n0}" />
        <asp:BoundField HeaderText="Tax Rate" DataField="TaxRate" DataFormatString="{0:p2}" />
        <asp:BoundField HeaderText="Tax Amount" DataField="TaxAmount" DataFormatString="{0:n2}" />
        <asp:BoundField HeaderText="Total" DataField="TotalAmount" DataFormatString="{0:n2}" />

        <asp:TemplateField HeaderText="Last Modified">
            <ItemTemplate>
                <%# GetTimestampHtml("Modified", "ModifiedBy") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Created">
          <ItemTemplate>
            <%# GetTimestampHtml("Created", "CreatedBy") %>
          </ItemTemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>
