<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.UI.Admin.Records.Rurbics.Controls.SearchResults" %>

<%@ Import Namespace="InSite.UI.Admin.Records.Rurbics" %>

<asp:Literal id="Instructions" runat="server" />

<insite:Grid runat="server" ID="Grid">
    <Columns>
        <asp:TemplateField ItemStyle-Width="50px" ItemStyle-Wrap="false">
            <ItemTemplate>
                <insite:IconLink runat="server" Name="search" ToolTip="Edit"
                    NavigateUrl='<%# Outline.GetNavigateUrl((Guid)Eval("RubricIdentifier")) %>' />
                <insite:IconLink runat="server" Name="trash-alt" ToolTip="Delete"
                    NavigateUrl='<%# Delete.GetNavigateUrl((Guid)Eval("RubricIdentifier")) %>' />
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Rubric Title">
            <ItemTemplate>
                <a href='<%# Outline.GetNavigateUrl((Guid)Eval("RubricIdentifier")) %>'><%# Eval("RubricTitle") %></a>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Total Rubric Points" ItemStyle-HorizontalAlign="Right" HeaderStyle-HorizontalAlign="Right">
            <ItemTemplate>
                <%# Eval("RubricPoints", "{0:n2}") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Number of Criteria" ItemStyle-HorizontalAlign="Right" HeaderStyle-HorizontalAlign="Right">
            <ItemTemplate>
                <%# Eval("CriteriaCount", "{0:n0}") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Status">
            <ItemTemplate>
                <%# GetStatus() %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Created" ItemStyle-Wrap="False">
            <ItemTemplate>
                <%# GetLocalTime(Eval("Created")) %>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</insite:Grid>