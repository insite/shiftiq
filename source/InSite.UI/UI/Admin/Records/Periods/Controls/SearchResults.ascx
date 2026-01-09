<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.Admin.Records.Periods.Controls.SearchResults" %>

<asp:Literal id="Instructions" runat="server" />

<insite:Grid runat="server" ID="Grid">
    <Columns>

        <asp:TemplateField HeaderText="Period Name" ItemStyle-Wrap="False">
            <ItemTemplate>
                <a href='/ui/admin/records/periods/edit?<%# Eval("PeriodIdentifier", "period={0}") %>'><%# Eval("PeriodName") %></a>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Period Range" ItemStyle-Wrap="False">
            <ItemTemplate>
                <%# Eval("PeriodStart", "{0:MMM d, yyyy}") %> - <%# Eval("PeriodEnd", "{0:MMM d, yyyy}") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField ItemStyle-Width="20px" ItemStyle-Wrap="false">
            <ItemTemplate>
                <insite:IconLink runat="server" Name="pencil" ToolTip="Edit"
                    NavigateUrl='<%# Eval("PeriodIdentifier", "/ui/admin/records/periods/edit?period={0}") %>' />
                <insite:IconLink runat="server" Name="trash-alt" ToolTip="Delete"
                    NavigateUrl='<%# Eval("PeriodIdentifier", "/ui/admin/records/periods/delete?period={0}") %>' />
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</insite:Grid>