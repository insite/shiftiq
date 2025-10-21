<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.Admin.Payments.Discounts.Controls.SearchResults" %>

<asp:Literal id="Instructions" runat="server" />

<insite:Grid runat="server" ID="Grid">
    <Columns>

        <asp:TemplateField HeaderText="Discount Code"> 
            <ItemTemplate>
                <a href='/ui/admin/sales/discounts/edit?<%# string.Format("code={0}", HttpUtility.UrlEncode((string)Eval("DiscountCode"))) %>'><%# Eval("DiscountCode") %></a>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Discount Percent"> 
            <ItemTemplate>
                <%# Eval("DiscountPercent", "{0:n2}") %>%
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Discount Description" ItemStyle-Wrap="False"> 
            <ItemTemplate>
                <%# SplitLines(Eval("DiscountDescription")) %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField ItemStyle-Wrap="False" ItemStyle-Width="20px">
            <ItemTemplate>
                <insite:IconLink runat="server" Name="pencil" ToolTip="Edit"
                    NavigateUrl='<%# string.Format("/ui/admin/sales/discounts/edit?code={0}", HttpUtility.UrlEncode((string)Eval("DiscountCode"))) %>' />
                <insite:IconLink runat="server" Name="trash-alt" ToolTip="Delete"
                    NavigateUrl='<%# string.Format("/admin/sales/discounts/delete?code={0}", HttpUtility.UrlEncode((string)Eval("DiscountCode"))) %>' />
            </ItemTemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>