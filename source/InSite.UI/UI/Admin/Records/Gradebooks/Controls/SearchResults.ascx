<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.Admin.Records.Gradebooks.Controls.SearchResults" %>

<asp:Literal id="Instructions" runat="server" />

<insite:Grid runat="server" ID="Grid" DataKeyNames="GradebookIdentifier">
    <Columns>

        <asp:TemplateField HeaderText="Title" ItemStyle-Wrap="False">
            <ItemTemplate>
                <a href='/ui/admin/records/gradebooks/outline?<%# Eval("GradebookIdentifier", "id={0}") %>'><%# Eval("GradebookTitle") %></a>
                <i class="fas fa-lock" style="color: red;" runat="server" visible='<%# (bool)Eval("IsLocked") %>'></i>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Class">
            <ItemTemplate>
                <div runat="server" visible='<%# Eval("EventIdentifier") != null %>'>
                    <a href='/ui/admin/events/classes/outline?<%# Eval("EventIdentifier", "event={0}") %>'><%# Eval("Event.EventTitle") %></a>
                </div>
                <div class="form-text"><%# Eval("Event.EventDescription") %></div>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Course Name">
            <ItemTemplate>
                <asp:Repeater runat="server" ID="CourseRepeater">
                    <ItemTemplate>
                        <div class='<%# Container.ItemIndex != 0 ? "mt-1" : "" %>'>
                            <a href='/ui/admin/courses/manage?<%# Eval("CourseIdentifier", "course={0}") %>'><%# Eval("CourseName") %></a>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="# of Learners" ItemStyle-Wrap="False">
            <ItemTemplate>
               <%# Eval("Enrollments") != null ? ((IEnumerable)Eval("Enrollments")).Cast<object>().Count() : 0 %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Achievements Granted" ItemStyle-Wrap="False">
            <ItemTemplate>
               <%# GrantedAchievements((Guid?)Eval("GradebookIdentifier"),(Guid?)Eval("AchievementIdentifier")) %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Scheduled" ItemStyle-Wrap="False">
            <ItemTemplate>
                <%# GetLocalTime(Eval("Event.EventScheduledStart")) %> - <%# GetLocalTime(Eval("Event.EventScheduledEnd")) %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Achievement">
            <ItemTemplate>
                <div runat="server" visible='<%# Eval("AchievementIdentifier") != null %>'>
                    <a href='/ui/admin/records/achievements/outline?<%# Eval("AchievementIdentifier", "id={0}") %>'><%# Eval("Achievement.AchievementTitle") %></a>
                </div>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Created" ItemStyle-Wrap="False">
            <ItemTemplate>
                <%# DateToHtml(Eval("GradebookCreated")) %>
            </ItemTemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>