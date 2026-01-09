<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.Cmds.Controls.Training.Achievements.AchievementSearchResults" %>

<asp:Literal id="Instructions" runat="server" />

<insite:Grid runat="server" ID="Grid">
    <Columns>

        <asp:BoundField HeaderText="Type" ItemStyle-Wrap="false" DataField="AchievementLabel" />

        <asp:TemplateField HeaderText="Title">
            <ItemTemplate>
                <a href='/ui/cmds/admin/achievements/edit?id=<%# Eval("AchievementIdentifier") %>'><%# Eval("AchievementTitle") %></a>
                <span ID="TimeSensitiveImage" runat="server" class="badge bg-danger" />
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Credentials">
            <ItemTemplate>
                <%# GetProgressionsCount((Guid)Eval("AchievementIdentifier")) %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Organizations">
            <ItemTemplate>
                <%# GetCompaniesCount((Guid)Eval("AchievementIdentifier")) %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Departments">
            <ItemTemplate>
                <%# GetDepartmentsCount((Guid)Eval("AchievementIdentifier")) %>
            </ItemTemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>
