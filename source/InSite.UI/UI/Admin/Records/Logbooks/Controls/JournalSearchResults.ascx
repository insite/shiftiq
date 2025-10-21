<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="JournalSearchResults.ascx.cs" Inherits="InSite.Admin.Records.Logbooks.Controls.JournalSearchResults" %>

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
                <a href='<%# Eval("LearnerUrl") %>'><%# Eval("UserFullName") %></a>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Email">
            <ItemTemplate>
                <%# Eval("UserEmail", "<a href='mailto:{0}'>{0}</a>") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Entries" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <%# Eval("EntryCount", "{0:n0}") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField ItemStyle-Width="65" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <insite:IconLink runat="server" Name="search" ToolTip="View Learner's Logbook" NavigateUrl='<%# Eval("JournalUrl") %>' />
                <insite:IconLink runat="server" Name="trash-alt" ToolTip="Delete" NavigateUrl='<%# Eval("DeleteUrl") %>' />
            </ItemTemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>