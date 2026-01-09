<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.Admin.Records.Outcomes.Controls.SearchResults" %>

<asp:Literal id="Instructions" runat="server" />

<insite:Grid runat="server" ID="Grid" DataKeyNames="GradebookIdentifier">
    <Columns>

        <asp:TemplateField HeaderText="Gradebook" ItemStyle-Wrap="False">
            <ItemTemplate>
                <a href='/ui/admin/records/gradebooks/outline?<%# Eval("GradebookIdentifier", "id={0}") %>'><%# Eval("Gradebook.GradebookTitle") %></a>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Student" ItemStyle-Wrap="False">
            <ItemTemplate>
                <a href='/ui/admin/contacts/people/edit?<%# Eval("UserIdentifier", "contact={0}") %>'><%# Eval("Student.UserFullName") %></a>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Competency" ItemStyle-Wrap="False">
            <ItemTemplate>
                <%# Eval("Standard.StandardTitle") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Points" ItemStyle-Wrap="False" ItemStyle-CssClass="text-end" HeaderStyle-CssClass="text-end">
            <ItemTemplate>
                <%# Eval("ValidationPoints", "{0:n2}") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Class">
            <ItemTemplate>
                <div runat="server" visible='<%# Eval("Gradebook.EventIdentifier") != null %>'>
                        <a href='/ui/admin/events/classes/outline?<%# Eval("Gradebook.EventIdentifier", "event={0}") %>'><%# Eval("Gradebook.Event.EventTitle") %></a>
                    </div>
                    <div class="form-text"><%# Eval("Gradebook.Event.EventDescription") %></div>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Scheduled" ItemStyle-Wrap="False">
            <ItemTemplate>
                <%# GetLocalTime(Eval("Gradebook.Event.EventScheduledStart")) %> - <%# GetLocalTime(Eval("Gradebook.Event.EventScheduledEnd")) %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Achievement">
            <ItemTemplate>
                <div runat="server" visible='<%# Eval("Gradebook.AchievementIdentifier") != null %>'>
                        <a href='/ui/admin/records/achievements/outline?<%# Eval("Gradebook.AchievementIdentifier", "id={0}") %>'><%# Eval("Gradebook.Achievement.AchievementTitle") %></a>
                    </div>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField ItemStyle-Wrap="False">
            <ItemTemplate>
                <insite:IconLink runat="server" ID="EditLink"
                        ToolTip="Edit" Name="pencil"
                        NavigateUrl='<%# "/ui/admin/records/outcomes/change?gradebook=" + Eval("GradebookIdentifier") + "&competency=" + Eval("CompetencyIdentifier") + "&user=" + Eval("UserIdentifier") %>'
                    />
            </ItemTemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>