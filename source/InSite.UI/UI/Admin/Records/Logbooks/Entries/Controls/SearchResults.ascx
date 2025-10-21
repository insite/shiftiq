<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.UI.Admin.Records.Logbooks.Entries.Controls.SearchResults" %>

<asp:Literal id="Instructions" runat="server" />

<insite:Grid runat="server" ID="Grid" DataKeyNames="JournalIdentifier">
    <Columns>

        <asp:TemplateField HeaderText="Logbook" ItemStyle-Wrap="False">
            <ItemTemplate>
                <a runat="server" visible="<%# IsValidator %>" href='<%# Eval("JournalSetupIdentifier", "/ui/admin/records/logbooks/validators/outline?journalsetup={0}") %>'>
                    <%# Eval("JournalSetupName") %>
                </a>
                <a runat="server" visible="<%# !IsValidator %>" href='<%# Eval("JournalSetupIdentifier", "/ui/admin/records/logbooks/outline?journalsetup={0}") %>'>
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

        <asp:TemplateField HeaderText="Training Type">
            <ItemTemplate>
                <%# Eval("TrainingType") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField ItemStyle-Width="100" ItemStyle-HorizontalAlign="Right">
            <ItemTemplate>
                <insite:IconLink runat="server" Name='<%# Eval("ValidateButtonIcon") %>' ToolTip='<%# Eval("ValidateButtonHint") %>'
                    Visible="<%# IsValidator %>"
                    NavigateUrl='<%# Eval("ExperienceIdentifier", "/ui/admin/records/logbooks/validators/validate-experience?experience={0}") %>'
                />
                <insite:IconLink runat="server" Name="search" ToolTip="View Learner's Entry"
                    Visible="<%# IsValidator %>"
                    NavigateUrl='<%# string.Format("/ui/admin/records/logbooks/validators/entries/view-experience?experience={0}", Eval("ExperienceIdentifier")) %>'
                />

                <insite:IconLink runat="server" Name='<%# Eval("ValidateButtonIcon") %>' ToolTip='<%# Eval("ValidateButtonHint") %>'
                    Visible="<%# !IsValidator %>"
                    NavigateUrl='<%# Eval("ExperienceIdentifier", "/ui/admin/records/logbooks/validate-experience?experience={0}") %>'
                />
                <insite:IconLink runat="server" Name="search" ToolTip="View Learner's Entry"
                    Visible="<%# !IsValidator %>"
                    NavigateUrl='<%# string.Format("/ui/admin/records/logbooks/entries/view-experience?experience={0}", Eval("ExperienceIdentifier")) %>'
                />

                <insite:IconLink runat="server" Name="trash-alt" ToolTip="Delete" NavigateUrl='<%# Eval("DeleteUrl") %>' />
            </ItemTemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>
