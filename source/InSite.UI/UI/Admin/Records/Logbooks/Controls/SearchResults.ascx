<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.Admin.Records.Logbooks.Controls.SearchResults" %>

<asp:Literal id="Instructions" runat="server" />

<insite:Grid runat="server" ID="Grid" DataKeyNames="JournalSetupIdentifier">
    <Columns>

        <asp:TemplateField HeaderText="Logbook" ItemStyle-Wrap="False">
            <ItemTemplate>
                <a href='<%# Eval("JournalSetupIdentifier", OutlineURL + "?journalsetup={0}") %>'><%# Eval("JournalSetupName") %> <%# GetLockedStatusEval(Eval("JournalSetupLocked")) %></a>
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
                <%# GetLocalTime(Eval("JournalSetupCreated")) %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField ItemStyle-Width="65" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <insite:IconLink runat="server" Name="search" ToolTip="Outline"
                    NavigateUrl='<%# Eval("JournalSetupIdentifier", OutlineURL + "?journalsetup={0}") %>'
                />
                <insite:IconLink runat="server" Name="trash-alt" ToolTip="Delete"
                    Visible='<%# !IsValidator %>'
                    NavigateUrl='<%# string.Format("/ui/admin/records/logbooks/delete?journalsetup={0}", Eval("JournalSetupIdentifier")) %>'
                />
            </ItemTemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>