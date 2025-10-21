<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CompetencyGrid.ascx.cs" Inherits="InSite.Admin.Records.Logbooks.Controls.CompetencyGrid" %>

<div class="mb-3">
    <insite:Button runat="server" ID="AddCompetencies" Text="Add Competencies" Icon="fas fa-plus-circle" ButtonStyle="Default" />
</div>

<h3 class="mb-3">
    <asp:Literal runat="server" ID="FrameworkTitle" />
</h3>

<asp:Repeater runat="server" ID="AreaRepeater">
    <HeaderTemplate>
        <table class="table table-striped">
            <thead>
                <tr>
                    <th></th>
                    <th>Competency</th>
                    <th class="text-center">
                        <insite:ContentTextLiteral runat="server" ContentLabel="Number of Hours" /></th>
                    <th class="text-center">Number of Log Entries</th>
                    <th class="text-center">Skill Rating</th>
                </tr>
            </thead>
            <tbody>
    </HeaderTemplate>
    <FooterTemplate>
            </tbody>
        </table>
    </FooterTemplate>
    <ItemTemplate>
        <tr>
            <td class="text-nowrap" style="width: 40px;">
                <insite:IconLink runat="server" ToolTip="Change" Name="pencil"
                    NavigateUrl='<%# "/ui/admin/records/logbooks/change-area?journalsetup=" + JournalSetupIdentifier + "&area=" + Eval("Identifier") %>' />
            </td>
            <td>
                <b><a href='<%# "/ui/admin/standards/edit?id=" + Eval("Identifier") %>'><%# Eval("Name") %></a></b>
            </td>
            <td class="text-center"><%# Eval("Hours", "{0:n2}") %></td>
            <td></td>
            <td></td>
        </tr>

        <asp:Repeater runat="server" ID="CompetencyRepeater">
            <ItemTemplate>
                <tr>
                    <td style="width: 80px;">
                        <insite:IconLink runat="server" ToolTip="Change" Name="pencil"
                            NavigateUrl='<%# "/ui/admin/records/logbooks/change-competency?journalsetup=" + JournalSetupIdentifier + "&competency=" + Eval("Identifier") %>' />
                        <insite:IconLink runat="server" ToolTip="Delete" Name="trash-alt"
                            NavigateUrl='<%# "/admin/records/logbooks/delete-competency?journalsetup=" + JournalSetupIdentifier + "&competency=" + Eval("Identifier") %>' />
                    </td>
                    <td><a href='<%# "/ui/admin/standards/edit?id=" + Eval("Identifier") %>'><%# Eval("Name") %> </a></td>
                    <td class="text-center"><%# Eval("Hours") != null ? Eval("Hours", "{0:n2}") : "None" %></td>
                    <td class="text-center"><%# Eval("JournalItems") != null ? Eval("JournalItems", "{0:n0}") : "None" %></td>
                    <td class="text-center"><%# Eval("SkillRating") != null ? Eval("SkillRating", "{0:n0}") : "None" %></td>
                </tr>
            </ItemTemplate>
        </asp:Repeater>
    </ItemTemplate>
</asp:Repeater>
