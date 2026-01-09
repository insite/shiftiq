<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.UI.Admin.Sales.Packages.Controls.SearchResults" %>

<asp:Literal id="Instructions" runat="server" />

<insite:Grid runat="server" ID="Grid" DataKeyNames="ProductIdentifier">
    <Columns>

        <asp:TemplateField HeaderText="Package Title"> 
            <ItemTemplate>
                <a href='<%# GetEditUrl() %>'><%# Eval("ProductName") %></a>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:BoundField HeaderText="Description" DataField="ProductDescription" />
        <asp:BoundField HeaderText="Number of Packages" DataField="ProductQuantity" DataFormatString="{0:n0}"  />
        <asp:BoundField HeaderText="Package Price" DataField="ProductPrice" DataFormatString="{0:n2}"  />

        <asp:TemplateField ItemStyle-Wrap="false" ItemStyle-Width="20px">
            <ItemTemplate>
                <insite:IconLink runat="server" Name="pencil" ToolTip="Edit"
                    NavigateUrl='<%# GetEditUrl() %>' />
            </ItemTemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>