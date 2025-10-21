<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.UI.Admin.Records.Logbooks.Competencies.Controls.SearchResults" %>

<asp:Literal id="Instructions" runat="server" />

<insite:Grid runat="server" ID="Grid" DataKeyNames="JournalIdentifier">
    <Columns>

        <asp:TemplateField HeaderText="Logbook" ItemStyle-Wrap="False">
            <ItemTemplate>
                <a runat="server" visible='<%# IsValidator %>' href='<%# Eval("JournalSetupIdentifier", "/ui/admin/records/logbooks/validators/outline?journalsetup={0}") %>'>
                    <%# Eval("JournalSetupName") %>
                </a>
                <a runat="server" visible='<%# !IsValidator %>' href='<%# Eval("JournalSetupIdentifier", "/ui/admin/records/logbooks/outline?journalsetup={0}") %>'>
                    <%# Eval("JournalSetupName") %>
                </a>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Learner">
            <ItemTemplate>
                <a href='/ui/admin/contacts/people/edit?<%# Eval("UserIdentifier", "contact={0}") %>'><%# Eval("UserFullName") %></a>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Email">
            <ItemTemplate>
                <%# Eval("UserEmail", "<a href='mailto:{0}'>{0}</a>") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="#">
            <ItemTemplate>
                <%# Eval("Sequence") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Created">
            <ItemTemplate>
                <%# GetLocalTime(Eval("Created")) %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Status">
            <ItemTemplate>
                <%# Eval("Status") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Framework">
            <ItemTemplate>
                <a href='/ui/admin/standards/edit?<%# Eval("FrameworkIdentifier", "id={0}") %>'><%# Eval("FrameworkName") %></a>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Competency">
            <ItemTemplate>
                <a href='/ui/admin/standards/edit?<%# Eval("CompetencyIdentifier", "id={0}") %>'><%# Eval("CompetencyName") %></a>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Hours">
            <ItemTemplate>
                <%# Eval("Hours") != null ? Eval("Hours", "{0:n2}") : "None" %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Satisfaction Level">
            <ItemTemplate>
                 <%# Eval("SatisfactionLevel") %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Skill Rating">
            <ItemTemplate>
                <%# Eval("SkillRating") != null ? Eval("SkillRating", "{0:n0}") : "None" %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField ItemStyle-Width="65" ItemStyle-HorizontalAlign="Right">
            <ItemTemplate>
                <insite:IconLink runat="server" Name="search" ToolTip="View Log Competency" Visible="<%# IsValidator %>"
                    NavigateUrl='<%# string.Format("/ui/admin/records/logbooks/validators/competencies/view-experience?experience={0}&competency={1}", Eval("ExperienceIdentifier"), Eval("CompetencyIdentifier")) %>'
                />
                <insite:IconLink runat="server" Name="search" ToolTip="View Log Competency" Visible="<%# !IsValidator %>"
                    NavigateUrl='<%# string.Format("/ui/admin/records/logbooks/competencies/view-experience?experience={0}&competency={1}", Eval("ExperienceIdentifier"), Eval("CompetencyIdentifier")) %>'
                />

                <insite:IconLink runat="server" Name="trash-alt" ToolTip="Delete" Visible="<%# IsValidator %>"
                    NavigateUrl='<%# string.Format("/admin/records/logbooks/validators/competencies/delete?experience={0}&competency={1}", Eval("ExperienceIdentifier"), Eval("CompetencyIdentifier")) %>'
                />
                <insite:IconLink runat="server" Name="trash-alt" ToolTip="Delete" Visible="<%# !IsValidator %>"
                    NavigateUrl='<%# string.Format("/admin/records/logbooks/competencies/delete?experience={0}&competency={1}", Eval("ExperienceIdentifier"), Eval("CompetencyIdentifier")) %>'
                />
            </ItemTemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>
