<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.Admin.Records.Scores.Controls.SearchResults" %>

<asp:Literal id="Instructions" runat="server" />

<insite:Grid runat="server" ID="Grid" DataKeyNames="GradebookIdentifier">
    <Columns>

        <asp:TemplateField HeaderText="Gradebook" ItemStyle-Wrap="False">
            <ItemTemplate>
                <a href='/ui/admin/records/gradebooks/outline?<%# Eval("GradebookIdentifier", "id={0}") %>'><%# Eval("Gradebook.GradebookTitle") %></a>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Grade Item" ItemStyle-Wrap="False">
            <ItemTemplate>
                <%# Eval("GradeItem.GradeItemName") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Type" ItemStyle-Wrap="False">
            <ItemTemplate>
                <%# Eval("GradeItem.GradeItemType") %>
            </ItemTemplate>
        </asp:TemplateField>

       <asp:TemplateField HeaderText="Format" ItemStyle-Wrap="False">
            <ItemTemplate>
                <%# Eval("GradeItem.GradeItemFormat") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Learner" ItemStyle-Wrap="False">
            <ItemTemplate>
                <a href='/ui/admin/contacts/people/edit?<%# Eval("UserIdentifier", "contact={0}") %>'><%# Eval("Learner.UserFullName") %></a>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Score" ItemStyle-Wrap="False" ItemStyle-CssClass="text-end" HeaderStyle-CssClass="text-end">
            <ItemTemplate>
                <%# GetScoreValue(Container.DataItem) %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Comment" ItemStyle-Wrap="True">
            <ItemTemplate>
                <%# Eval("ProgressComment") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Class Name">
            <ItemTemplate>
                <div runat="server" visible='<%# Eval("Gradebook.EventIdentifier") != null %>'>
                        <a href='/ui/admin/events/classes/outline?<%# Eval("Gradebook.EventIdentifier", "event={0}") %>'><%# Eval("Gradebook.Event.EventTitle") %></a>
                    </div>
                    <div class="form-text"><%# Eval("Gradebook.Event.EventDescription") %></div>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Class Date(s)" ItemStyle-Wrap="False">
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
                        NavigateUrl='<%# "/ui/admin/records/scores/change?gradebook=" + Eval("GradebookIdentifier") + "&item=" + Eval("GradeItemIdentifier") + "&student=" + Eval("UserIdentifier") %>'
                    />
                <insite:IconLink runat="server" ID="DeleteLink"
                        ToolTip="Delete" Name="trash-alt"
                        Visible='<%# !(bool)Eval("Gradebook.IsLocked") %>'
                        NavigateUrl='<%# "/admin/records/scores/delete?gradebook=" + Eval("GradebookIdentifier") + "&item=" + Eval("GradeItemIdentifier") + "&student=" + Eval("UserIdentifier") %>'
                    />
            </ItemTemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>